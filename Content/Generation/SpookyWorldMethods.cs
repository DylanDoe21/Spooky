using Terraria;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Spooky.Content.Generation
{
	public class SpookyWorldMethods
	{
		public static void PlaceMound(int X, int Y, int tileType, int halfWidth, int height)
		{
			ShapeData mound = new ShapeData();
			GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Mound(halfWidth, height), Actions.Chain(new GenAction[]
			{
				blotchMod.Output(mound)
			}));

			WorldUtils.Gen(new Point(X, Y), new ModShapes.All(mound), Actions.Chain(new GenAction[]
			{
				new Actions.ClearTile(), new Actions.PlaceTile((ushort)tileType)
			}));
		}

		public static void PlaceOval(int X, int Y, int tileType, int wallType, int radius, int radiusY, float thickMult, bool ReplaceOnly, bool DestroyOnly)
		{
			float scale = radiusY / (float)radius;
			float invertScale = (float)radius / radiusY;
			for (int x = -radius; x <= radius; x++)
			{
				for (float y = -radius; y <= radius; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(2.5f, 4.5f) * thickMult;
					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);

						if (!ReplaceOnly)
						{
							if (tileType > -1)
							{
								WorldGen.KillTile(PositionX, PositionY);
								tile.TileType = (ushort)tileType;
								tile.HasTile = true;
							}

							if (wallType > 0)
							{
								tile.WallType = (ushort)wallType;
								tile.LiquidAmount = 0;
							}

							if (tileType == -1 && wallType == 0 && DestroyOnly)
							{
								WorldGen.KillTile(PositionX, PositionY);
								tile.LiquidAmount = 0;
							}
						}
						else if (ReplaceOnly && tile.HasTile)
						{
							if (tileType > -1)
							{
								tile.TileType = (ushort)tileType;
							}
							else
							{
								WorldGen.KillTile(PositionX, PositionY);
							}

							if (wallType > 0 && tile.WallType > 0)
							{
								tile.WallType = (ushort)wallType;
								tile.LiquidAmount = 0;
							}
						}

						//if (Math.Sqrt(x * x + y * y) >= radius - radialMod)
						//{
						//}
					}
				}
			}
		}

		public static void PlaceCircle(int X, int Y, int tileType, int wallType, int radius, bool clearTiles, bool clearWalls)
		{
			ShapeData circle = new ShapeData();
			GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
			{
				blotchMod.Output(circle)
			}));

			//clear tiles
			if (clearTiles)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
				{
					new Actions.ClearTile(), new Actions.SetLiquid(0, 0)
				}));
			}

			//place tiles for the circle
			if (tileType > -1)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
				{
					new Actions.PlaceTile((ushort)tileType)
				}));
			}

			//wall placing stuff
			ShapeData wallCircle = new ShapeData();
			GenAction wallBlotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius - 1), Actions.Chain(new GenAction[]
			{
				wallBlotchMod.Output(wallCircle)
			}));

			//clear walls
			if (clearWalls)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(wallCircle), Actions.Chain(new GenAction[]
				{
					new Actions.ClearWall()
				}));
			}

			//dont place walls if it is not set to place any
			if (wallType > 0)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(wallCircle), Actions.Chain(new GenAction[]
				{
					new Actions.PlaceWall((ushort)wallType)
				}));
			}
		}

		public static void PlaceVines(int X, int Y, int vineType, int[] ValidTiles)
		{
			Tile tileBelow = Framing.GetTileSafely(X, Y + 1);
			if (WorldGen.genRand.NextBool() && !tileBelow.HasTile && tileBelow.LiquidAmount <= 0)
			{
				bool PlaceVine = false;
				int Test = Y;
				while (Test > Y - 10)
				{
					Tile testTile = Framing.GetTileSafely(X, Test);
					if (testTile.BottomSlope)
					{
						break;
					}
					else if (!testTile.HasTile || !ValidTiles.Contains(testTile.TileType))
					{
						Test--;
						continue;
					}
					PlaceVine = true;
					break;
				}

				if (PlaceVine)
				{
					tileBelow.TileType = (ushort)vineType;
					tileBelow.HasTile = true;
					WorldGen.SquareTileFrame(X, Y + 1, true);
				}
			}
		}

		internal static readonly List<Vector2> Directions = new List<Vector2>()
		{
			new Vector2(-1f, -1f),
			new Vector2(1f, -1f),
			new Vector2(-1f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, -1f),
			new Vector2(-1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 0f),
		};

		public static float PerlinNoise2D(float x, float y, int octaves, int seed)
		{
			float SmoothFunction(float n) => 3f * n * n - 2f * n * n * n;
			
			float NoiseGradient(int s, int noiseX, int noiseY, float xd, float yd)
			{
				int hash = s;
				hash ^= 1619 * noiseX;
				hash ^= 31337 * noiseY;

				hash = hash * hash * hash * 60493;
				hash = (hash >> 13) ^ hash;

				Vector2 g = Directions[hash & 7];

				return xd * g.X + yd * g.Y;
			}

			int frequency = (int)Math.Pow(2D, octaves);
			x *= frequency;
			y *= frequency;

			int flooredX = (int)x;
			int flooredY = (int)y;
			int ceilingX = flooredX + 1;
			int ceilingY = flooredY + 1;
			float interpolatedX = x - flooredX;
			float interpolatedY = y - flooredY;
			float interpolatedX2 = interpolatedX - 1;
			float interpolatedY2 = interpolatedY - 1;

			float fadeX = SmoothFunction(interpolatedX);
			float fadeY = SmoothFunction(interpolatedY);

			float smoothX = MathHelper.Lerp(NoiseGradient(seed, flooredX, flooredY, interpolatedX, interpolatedY), NoiseGradient(seed, ceilingX, flooredY, interpolatedX2, interpolatedY), fadeX);
			float smoothY = MathHelper.Lerp(NoiseGradient(seed, flooredX, ceilingY, interpolatedX, interpolatedY2), NoiseGradient(seed, ceilingX, ceilingY, interpolatedX2, interpolatedY2), fadeX);

			return MathHelper.Lerp(smoothX, smoothY, fadeY);
		}
	}
}