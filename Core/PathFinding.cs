using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Spooky.Content.Tiles.Minibiomes.Ocean;
using Terraria.ModLoader;

namespace Spooky.Core
{
	//credit to gabehaswon for the pathfinding code, from: https://github.com/Path-of-Terraria/PathOfTerraria/blob/develop/Common/NPCs/Pathfinding/Pathfinder.cs
	internal class PathFinding(int refreshTime)
	{
		public enum Direction : byte
		{
			None,
			Above,
			Below,
			Left,
			Right
		}

		public readonly record struct FoundPoint(Point16 Position, Direction Direction);
		public readonly record struct WorkingPoint(Direction Direction, float Weight);

		public bool HasPath { get; private set; }

		public List<FoundPoint> Path { get; private set; } = [];

		public readonly int RefreshTime = refreshTime;

		public int RefreshTimer = 0;

		private readonly Dictionary<Point16, WorkingPoint> found = [];
		private readonly PriorityQueue<Point16, float> frontier = new();
		private (Point16 start, Point16 end) cachedLocations = new();
		private Rectangle checkingRectangle = default;
		private bool cached = false;
		private Vector2 objectSize = default;
		private Vector2 posOffset = default;

		public bool CheckDrawPath(Point16 start, Point16 end, int MaxIterations, Vector2 objSizeInTiles = default, Vector2? positionOffset = null)
		{
			RefreshTimer--;

			if (start == end)
			{
				return true;
			}

			checkingRectangle = GetCheckArea(start, end, new Point16(200, 180));
			objectSize = objSizeInTiles;
			posOffset = positionOffset ?? Vector2.Zero;

			if (RefreshTimer == 0)
			{
				ResetState();
			}

			if (!cached && RefreshTimer > 0)
			{
				return false;
			}

			if (!cached)
			{
				Path.Clear();
				found.Clear();
				frontier.Clear();

				AddPoint(end, start, Direction.None, 0);
				RefreshTimer = RefreshTime;
			}
			else
			{
				start = cachedLocations.start;
				end = cachedLocations.end;
			}

			int iterations = 0;

			while (frontier.Count > 0)
			{
				iterations++;

				if (iterations > MaxIterations)
				{
					cached = true;
					cachedLocations = (start, end);
					return false;
				}

				Point16 pos = frontier.Dequeue();

				if (pos.Equals(end))
				{
					BuildPath(start, end);
					ResetState();

					HasPath = true;
					RefreshTimer = RefreshTime;
					return true;
				}

				AddSurrounds(pos, end, found[pos].Weight);
			}

			ResetState();
			return false;
		}

		private void ResetState()
		{
			cached = false;
			found.Clear();
		}

		private void BuildPath(Point16 start, Point16 end)
		{
			Point16 pos = end;

			while (pos != start)
			{
				Direction dir = found[pos].Direction;
				Path.Add(new FoundPoint(pos, dir));

				Point16 direction = ToVector(dir);
				pos = new Point16(pos.X + direction.X, pos.Y + direction.Y);
			}

			Path = new(Path);
		}

		public static Point16 ToVector(Direction direction)
		{
			return direction switch
			{
				Direction.Left => new Point16(-1, 0),
				Direction.Right => new Point16(1, 0),
				Direction.Below => new Point16(0, 1),
				Direction.Above => new Point16(0, -1),
				_ => Point16.Zero
			};
		}

		public static Vector2 ToVector2(Direction direction)
		{
			return direction switch
			{
				Direction.Left => new Vector2(-1, 0),
				Direction.Right => new Vector2(1, 0),
				Direction.Below => new Vector2(0, 1),
				Direction.Above => new Vector2(0, -1),
				_ => Vector2.Zero
			};
		}

		public bool InvalidTile(Point16 position)
		{
			return !checkingRectangle.Contains(position.ToPoint())  || SolidBig(position, objectSize, posOffset) || found.ContainsKey(position);
		}

		private static bool SolidBig(Point16 position, Vector2 objectSize, Vector2 positionOffset)
		{
			Vector2 pos = position.ToWorldCoordinates() + positionOffset;

			int[] BlockTypes = new int[]
			{
				ModContent.TileType<OceanSand>(),
				ModContent.TileType<OceanBiomass>(),
				ModContent.TileType<OceanMeat>(),
				ModContent.TileType<LabMetalPipe>(),
				ModContent.TileType<LabMetalPlate>()
			};

			return SolidCollisionWithSpecificTiles(pos, (int)(objectSize.X * 16), (int)(objectSize.Y * 16), BlockTypes);
		}

		//custom copied version of vanilla SolidCollision but with a list of specific tiles
		//used for the dunkelosteus so that it only checks for specific tiles when pathfinding since it is meant to destroy any tiles that arent a part of the zombie ocean biome
		public static bool SolidCollisionWithSpecificTiles(Vector2 Position, int Width, int Height, int[] TileTypes)
		{
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector = default(Vector2);
			for (int i = num; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile == null || !tile.HasTile || !TileTypes.Contains(tile.TileType))
					{
						continue;
					}
					bool flag = Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];
					if (flag)
					{
						vector.X = i * 16;
						vector.Y = j * 16;
						int num2 = 16;
						if (tile.IsHalfBlock)
						{
							vector.Y += 8f;
							num2 -= 8;
						}
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num2)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void AddSurrounds(Point16 pos, Point16 end, float currentWeight)
		{
			if (!InvalidTile(new Point16(pos.X, pos.Y + 1)))
			{
				AddPoint(end, new Point16(pos.X, pos.Y + 1), Direction.Above, currentWeight);
			}

			if (!InvalidTile(new Point16(pos.X, pos.Y - 1)))
			{
				AddPoint(end, new Point16(pos.X, pos.Y - 1), Direction.Below, currentWeight);
			}

			if (!InvalidTile(new Point16(pos.X + 1, pos.Y)))
			{
				AddPoint(end, new Point16(pos.X + 1, pos.Y), Direction.Left, currentWeight);
			}

			if (!InvalidTile(new Point16(pos.X - 1, pos.Y)))
			{
				AddPoint(end, new Point16(pos.X - 1, pos.Y), Direction.Right, currentWeight);
			}
		}

		private void AddPoint(Point16 end, Point16 position, Direction direction, float currentWeight)
		{
			if (!WorldGen.InWorld(position.X, position.Y, 40))
			{
				return;
			}

			found.Add(position, new WorkingPoint(direction, currentWeight));
			frontier.Enqueue(position, currentWeight + SquaredDistance(end, position));
		}

		private static float SquaredDistance(Point16 start, Point16 position)
		{
			return MathF.Pow(position.Y - start.Y, 2) + MathF.Pow(position.X - start.X, 2);
		}

		public static Rectangle GetCheckArea(Point16 start, Point16 end, Point16 inflationSize)
		{
			int minX = Math.Min(start.X, end.X);
			int minY = Math.Min(start.X, end.Y);
			int maxX = Math.Max(start.X, end.X);
			int maxY = Math.Max(start.X, end.Y);

			Rectangle rect = new(minX, minY, maxX - minX, maxY - minY);
			rect.Inflate(inflationSize.X, inflationSize.Y);
			return rect;
		}
	}
}