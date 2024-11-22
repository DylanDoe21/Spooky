using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.Minibiomes;

namespace Spooky.Content.Generation.Minibiomes
{
	public class ChristmasDungeon : ModSystem
	{
		public static List<int> WallTypes = new()
		{
			ModContent.WallType<ChristmasBrickWall>(),
			ModContent.WallType<ChristmasWoodWall>(),
			ModContent.WallType<ChristmasWallpaperRed>(),
			ModContent.WallType<ChristmasWallpaperGreen>(),
			ModContent.WallType<ChristmasWallpaperBlue>(),
		};

		private void PlaceChristmasDungeon(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.ChrismtasDungeon").Value;

			int DungeonWidth = Main.maxTilesX / 32;
			int DungeonHeight = Main.maxTilesY / 11;

			int maxDungeonSegmentSize = 25;
			int minDungeonSegmentSize = 24;
			int MaxRoomSize = 20;
			int MinRoomSize = 10;

			/*
			//create a box around the area of the biome for debugging
			for (int i = PositionX - 15 - (DungeonWidth / 2); i <= PositionX + 15 + (DungeonWidth / 2); i++)
			{
				for (int j = PositionY - 15 - (DungeonHeight / 2); j <= PositionY + 15 + (DungeonHeight / 2); j++)
				{
					WorldGen.KillTile(i, j);
					WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrick>());
					WorldGen.KillWall(i, j);
					WorldGen.PlaceWall(i, j, ModContent.WallType<ChristmasBrickWall>());
				}
			}
			*/

			bool JungleOnLeftSide = GenVars.JungleX < (Main.maxTilesX / 2);

			int Start = (Main.maxTilesX / 2);
			int End = JungleOnLeftSide ? Main.maxTilesX - 200 : 200;

			int Increment = JungleOnLeftSide ? 20 : -20;

			//find a valid position in the jungle away from other structures
			for (int X = Start; JungleOnLeftSide ? X <= End : X >= End; X += Increment)
			{
				for (int Y = Main.maxTilesY / 2; Y <= Main.maxTilesY - 300; Y += 20)
				{
					if (CanPlaceBiome(X, Y, DungeonWidth, DungeonHeight))
					{
						ProceduralRoomsGenerator Generator = new ProceduralRoomsGenerator(X, Y, DungeonWidth, DungeonHeight, maxDungeonSegmentSize, minDungeonSegmentSize, MaxRoomSize, MinRoomSize, ModContent.WallType<ChristmasBrickWall>());
						Generator.GenerateDungeon();
						CleanUpDungeon(X, Y, DungeonWidth, DungeonHeight);

						//get rid of annoying liquid inside the dungeon
						for (int i = X - 15 - (DungeonWidth / 2); i <= X + 15 + (DungeonWidth / 2); i++)
						{
							for (int j = Y - 15 - (DungeonHeight / 2); j <= Y + 15 + (DungeonHeight / 2); j++)
							{
								Main.tile[i, j].LiquidAmount = 0;
							}
						}

						return;
					}
				}
			}
		}

		public void CleanUpDungeon(int PositionX, int PositionY, int Width, int Height)
		{
			List<ushort> BlockTypes = new()
			{
				(ushort)ModContent.TileType<ChristmasBrick>(),
				(ushort)ModContent.TileType<ChristmasWoodPlanks>(),
				(ushort)ModContent.TileType<ChristmasCarpet>(),
			};

			void getAttachedPoints(int x, int y, List<Point> points)
			{
				Tile tile = Main.tile[x, y];
				Point point = new(x, y);

				if (!WorldGen.InWorld(x, y))
				{
					tile = new Tile();
				}

				if (!BlockTypes.Contains(tile.TileType) || !tile.HasTile || points.Count > 200 || points.Contains(point))
				{
					return;
				}

				points.Add(point);

				getAttachedPoints(x + 1, y, points);
				getAttachedPoints(x - 1, y, points);
				getAttachedPoints(x, y + 1, points);
				getAttachedPoints(x, y - 1, points);
			}

			//clean up the dungeon by removing floating clumps of tiles or small tiles sticking out
			for (int i = PositionX - 25 - (Width / 2); i <= PositionX + 25 + (Width / 2); i++)
			{
				for (int j = PositionY - 25 - (Height / 2); j <= PositionY + 25 + (Height / 2); j++)
				{
					List<Point> chunkPoints = new();
					getAttachedPoints(i, j, chunkPoints);

					int cutoffLimit = 200;
					if (chunkPoints.Count >= 1 && chunkPoints.Count < cutoffLimit)
					{
						foreach (Point p in chunkPoints)
						{
							WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
							{
								new Actions.ClearTile(true)
							}));
						}
					}

					//clean tiles that are sticking out (basically tiles only attached to one tile on one side)
					bool OnlyRight = !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile;
					bool OnlyLeft = !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile && !Main.tile[i + 1, j].HasTile;
					bool OnlyDown = !Main.tile[i, j - 1].HasTile && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile;
					bool OnlyUp = !Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile;

					if (OnlyRight || OnlyLeft || OnlyDown || OnlyUp)
					{
						if (BlockTypes.Contains(Main.tile[i, j].TileType))
						{
							WorldGen.KillTile(i, j);
						}
					}

					if (!Main.tile[i, j].HasTile && Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasBrick>() && Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasBrick>() &&
					Main.tile[i, j - 1].TileType == ModContent.TileType<ChristmasBrick>() && Main.tile[i, j + 1].TileType == ModContent.TileType<ChristmasBrick>())
					{
						WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrick>());
					}

					//get rid single 1x1 strips of tiles
					bool NoUpOrDown = !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile;
					bool NoLeftOrRight = !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile;

					if (NoUpOrDown || NoLeftOrRight)
					{
						if (BlockTypes.Contains(Main.tile[i, j].TileType))
						{
							WorldGen.KillTile(i, j);
						}
					}

					//get rid of 1x2 tiles on the ground since it looks weird
                    if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrick>() && Main.tile[i - 1, j].HasTile && !Main.tile[i - 2, j].HasTile && !Main.tile[i + 1, j].HasTile)
                    {
                        WorldGen.KillTile(i, j);
                        WorldGen.KillTile(i - 1, j);
                    }

                    //get rid of single tiles on the ground since it looks weird
                    if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrick>() && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile)
                    {
                        WorldGen.KillTile(i, j);
                    }

					//get rid of 1x2 tiles on the ground since it looks weird
                    if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrick>() && Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasBrick>() && !Main.tile[i - 2, j].HasTile && !Main.tile[i + 1, j].HasTile)
                    {
                        WorldGen.KillTile(i, j);
                        WorldGen.KillTile(i - 1, j);
                    }

					//get rid of surfaces in the dungeon that arent thick enough to place planks flooring
                    if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrick>())
                    {
						if ((!Main.tile[i, j - 1].HasTile && !Main.tile[i, j - 2].HasTile && !Main.tile[i, j - 3].HasTile) && !CanPlacePlankFlooring(i, j))
						{
                        	WorldGen.KillTile(i, j);
						}
                    }
				}
			}

			int WallpaperType = WorldGen.genRand.Next(WallTypes);

			//place the colored wallpaper inside of the dungeon and clean up walls
			for (int i = PositionX - 25 - (Width / 2); i <= PositionX + 25 + (Width / 2); i++)
			{
				for (int j = PositionY - 25 - (Height / 2); j <= PositionY + 25 + (Height / 2); j++)
				{
					//destroy walls on the outside of the dungeon so that they dont stick out of the edges of it
					bool NoTileUp = Main.tile[i, j - 1].TileType != ModContent.TileType<ChristmasBrick>() && !WallTypes.Contains(Main.tile[i, j - 1].WallType);
					bool NoTileDown = Main.tile[i, j + 1].TileType != ModContent.TileType<ChristmasBrick>() && !WallTypes.Contains(Main.tile[i, j + 1].WallType);
					bool NoTileLeft = Main.tile[i - 1, j].TileType != ModContent.TileType<ChristmasBrick>() && !WallTypes.Contains(Main.tile[i - 1, j].WallType);
					bool NoTileRight = Main.tile[i + 1, j].TileType != ModContent.TileType<ChristmasBrick>() && !WallTypes.Contains(Main.tile[i + 2, j].WallType);

					if (NoTileUp || NoTileDown || NoTileLeft || NoTileRight)
					{
						WorldGen.KillWall(i, j);
					}

					/*
					//place colored wallpapers
					if ((Main.tile[i, j].WallType == ModContent.WallType<ChristmasBrickWall>() || WallTypes.Contains(Main.tile[i, j].WallType)) && CanPlaceWallpaper(i, j))
					{
						for (int newX = i - 3; newX <= i + 3; newX++)
						{
							for (int newY = j - 4; newY <= j + 4; newY++)
							{
								Main.tile[newX, newY].WallType = (ushort)WallpaperType;
							}
						}
					}
					*/

					//place planks along the floors and ceiling of the rooms
					//specifically check to make sure the wall type of 2 blocks higher is a dungeon wall, this prevents the planks from being placed outside of the dungeon
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrick>() && WallTypes.Contains(Main.tile[i, j - 2].WallType))
					{
						if ((!Main.tile[i, j - 1].HasTile || !Main.tile[i, j - 2].HasTile) && CanPlacePlankFlooring(i, j))
						{
							Main.tile[i, j].TileType = (ushort)ModContent.TileType<ChristmasWoodPlanks>();
						}
					}
				}
			}

			//finally place carpets and any last minute cleaning up
			for (int i = PositionX - 25 - (Width / 2); i <= PositionX + 25 + (Width / 2); i++)
			{
				for (int j = PositionY - 25 - (Height / 2); j <= PositionY + 25 + (Height / 2); j++)
				{	
					//place carpets on the plank flooring
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasWoodPlanks>() && !Main.tile[i, j - 1].HasTile && Main.tile[i - 1, j].HasTile && Main.tile[i + 1, j].HasTile &&
					(Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasWoodPlanks>() || Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasWoodPlanks>() ||
					Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasCarpet>() || Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasCarpet>()))
					{
						Main.tile[i, j].TileType = (ushort)ModContent.TileType<ChristmasCarpet>();
						Main.tile[i, j + 1].TileType = (ushort)ModContent.TileType<ChristmasWoodPlanks>();
						Main.tile[i - 1, j + 1].TileType = (ushort)ModContent.TileType<ChristmasWoodPlanks>();
						Main.tile[i + 1, j + 1].TileType = (ushort)ModContent.TileType<ChristmasWoodPlanks>();
					}

					//slope tiles
					if (BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Tile.SmoothSlope(i, j);
					}

					bool NoSolidTile = !Main.tile[i, j].HasTile || Main.tile[i, j].BottomSlope || Main.tile[i, j].TopSlope || Main.tile[i, j].LeftSlope || Main.tile[i, j].RightSlope;
					bool AnySurroundingTile = Main.tile[i - 1, j].HasTile || Main.tile[i + 1, j].HasTile || Main.tile[i, j - 1].HasTile || Main.tile[i, j + 1].HasTile;

					if (Main.tile[i, j].WallType == ModContent.WallType<ChristmasBrickWall>() && NoSolidTile && AnySurroundingTile)
					{
						Main.tile[i, j].WallType = (ushort)ModContent.WallType<ChristmasWoodWall>();
					}
				}
			}
		}

		public bool CanPlacePlankFlooring(int PositionX, int PositionY)
		{
			for (int y = PositionY; y <= PositionY + 3; y++)
			{
				if (!Main.tile[PositionX, y].HasTile)
				{
					return false;
				}
			}

			return true;
		}

		public bool CanPlaceWallpaper(int PositionX, int PositionY)
		{
			for (int i = PositionX - 5; i <= PositionX + 5; i++)
			{
				for (int j = PositionY - 2; j <= PositionY + 2; j++)
				{
					if (Main.tile[i, j].HasTile)
					{
						return false;
					}
				}
			}

			return true;
		}

		//place the biome if no important structures are nearby and theres enough snow biome blocks
		public bool CanPlaceBiome(int PositionX, int PositionY, int Width, int Height)
		{
			int numSnowTiles = 0;

			for (int i = PositionX - 15 - (Width / 2); i <= PositionX + 15 + (Width / 2); i++)
			{
				for (int j = PositionY - 15 - (Height / 2); j <= PositionY + 15 + (Height / 2); j++)
				{
					int[] ValidTiles = { TileID.SnowBlock, TileID.IceBlock, TileID.Slush, TileID.BreakableIce };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && ValidTiles.Contains(Main.tile[i, j].TileType))
					{
						numSnowTiles++;
					}

					int[] InvalidTiles = { TileID.Dirt, TileID.Stone };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && (Main.tileDungeon[Main.tile[i, j].TileType] || InvalidTiles.Contains(Main.tile[i, j].TileType)))
					{
						return false;
					}
				}
			}

			float AmountOfTilesNeeded = (Width * Height) / 1.2f;

			if (numSnowTiles > AmountOfTilesNeeded)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps"));
			if (GenIndex1 == -1)
			{
				return;
			}

			tasks.Insert(GenIndex1 + 1, new PassLegacy("Christmas Mansion", PlaceChristmasDungeon));
		}
	}

	//code referenced/modified from: https://github.com/Fixtone/DungeonCarver/blob/master/Assets/Scripts/Maps/MapGenerators/BSPTreeMapGenerator.cs
	public class ProceduralRoomsGenerator
	{
		private readonly int _positionX;
		private readonly int _positionY;
		private readonly int _width;
		private readonly int _height;
		private readonly int _maxDungeonSegmentSize;
		private readonly int _minDungeonSegmentSize;
		private readonly int _MaxRoomSize;
		private readonly int _MinRoomSize;
		private readonly int _wallType;

		private List<DungeonSegment> DungeonSegments = new List<DungeonSegment>();

		public ProceduralRoomsGenerator(int PositionX, int PositionY, int width, int height, int maxDungeonSegmentSize, int minDungeonSegmentSize, int MaxRoomSize, int MinRoomSize, int WallType)
		{
			_positionX = PositionX;
			_positionY = PositionY;
			_width = width;
			_height = height;
			_maxDungeonSegmentSize = maxDungeonSegmentSize;
			_minDungeonSegmentSize = minDungeonSegmentSize;
			_MaxRoomSize = MaxRoomSize;
			_MinRoomSize = MinRoomSize;
			_wallType = WallType;
		}

		public void GenerateDungeon()
		{
			DungeonSegment rootDungeonSegment = new DungeonSegment(_positionX - (_width / 2), _positionY - (_height / 2), _width, _height);
			DungeonSegments.Add(rootDungeonSegment);

			bool SplitDungeon = true;

			//loop through all leaves until they can no longer split successfully
			while (SplitDungeon)
			{
				SplitDungeon = false;

				for (int i = 0; i < DungeonSegments.Count; i++)
				{
					if (DungeonSegments[i].childDungeonSegmentLeft == null && DungeonSegments[i].childDungeonSegmentRight == null)
					{
						if ((DungeonSegments[i].DungeonSegmentWidth > _maxDungeonSegmentSize) || (DungeonSegments[i].DungeonSegmentHeight > _maxDungeonSegmentSize))
						{
							//try to split the dungeon segments
							if (DungeonSegments[i].SplitDungeonSegment(_minDungeonSegmentSize))
							{
								DungeonSegments.Add(DungeonSegments[i].childDungeonSegmentLeft);
								DungeonSegments.Add(DungeonSegments[i].childDungeonSegmentRight);
								SplitDungeon = true;
							}
						}
					}
				}
			}

			rootDungeonSegment.PlaceRooms(this, _maxDungeonSegmentSize, _MaxRoomSize, _MinRoomSize);
		}

		public void PlaceRoom(Rectangle room)
		{
			//subtract half of the width and height from the position so the rooms are properly centered when placed
			room.X = room.X - room.Width / 2;
			room.Y = room.Y - room.Height / 2;

			//place larger square of blocks
			for (int i = room.X - (room.Width / 2) - 8; i <= room.X + (room.Width / 2) + 8; i++)
			{
				for (int j = room.Y - (room.Height / 2) - 8; j <= room.Y + (room.Height / 2) + 8; j++)
				{
					if (Main.tile[i, j].TileType != ModContent.TileType<ChristmasBrick>() && Main.tile[i, j].WallType != _wallType)
					{
						Main.tile[i, j].ClearEverything();
						WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrick>());
						WorldGen.PlaceWall(i, j, _wallType);
					}
				}
			}

			//dig out smaller box inside the main one
			for (int i = room.X - (room.Width / 2); i <= room.X + (room.Width / 2); i++)
			{
				for (int j = room.Y - (room.Height / 2); j <= room.Y + (room.Height / 2); j++)
				{
					WorldGen.KillTile(i, j);
					WorldGen.KillTile(i - 1, j);
					WorldGen.KillTile(i + 1, j);
					WorldGen.KillTile(i, j - 1);
					WorldGen.KillTile(i, j + 1);
				}
			}
		}

		public void CreatePathways(Rectangle room1, Rectangle room2)
		{
			Vector2 room1Center = new Vector2(room1.X - (room1.Width / 2), room1.Y - (room1.Height / 2));
			Vector2 room2Center = new Vector2(room2.X - (room2.Width / 2), room2.Y - (room2.Height / 2));

			if (WorldGen.genRand.NextBool(3))
			{
				HorizontalPathway((int)room1Center.X, (int)room2Center.X, (int)room1Center.Y);
				VerticalPathway((int)room1Center.Y, (int)room2Center.Y, (int)room2Center.X);
			}
			else
			{
				VerticalPathway((int)room1Center.Y, (int)room2Center.Y, (int)room1Center.X);
				HorizontalPathway((int)room1Center.X, (int)room2Center.X, (int)room2Center.Y);
			}
		}

		private void HorizontalPathway(int xStart, int xEnd, int PositionY)
		{
			//place additional tiles if theres no dungeon blocks where the pathway is
			for (int x = Math.Min(xStart, xEnd) - 5; x <= Math.Max(xStart, xEnd) + 5; x++)
			{
				for (int j = PositionY - 10; j <= PositionY + 9; j++)
				{
					if (Main.tile[x, j].TileType != ModContent.TileType<ChristmasBrick>() && Main.tile[x, j].WallType != _wallType)
					{
						Main.tile[x, j].ClearEverything();
						WorldGen.PlaceTile(x, j, ModContent.TileType<ChristmasBrick>());
						WorldGen.PlaceWall(x, j, _wallType);
					}
				}
			}

			for (int x = Math.Min(xStart, xEnd); x <= Math.Max(xStart, xEnd); x++)
			{
				for (int j = PositionY - 2; j <= PositionY + 1; j++)
				{
					WorldGen.KillTile(x, j);
				}
			}
		}

		private void VerticalPathway(int yStart, int yEnd, int PositionX)
		{
			//place additional tiles if theres no dungeon blocks where the pathway is
			for (int y = Math.Min(yStart, yEnd) - 5; y <= Math.Max(yStart, yEnd) + 5; y++)
			{
				for (int i = PositionX - 10; i <= PositionX + 10; i++)
				{
					if (Main.tile[i, y].TileType != ModContent.TileType<ChristmasBrick>() && Main.tile[i, y].WallType != _wallType)
					{
						Main.tile[i, y].ClearEverything();
						WorldGen.PlaceTile(i, y, ModContent.TileType<ChristmasBrick>());
						WorldGen.PlaceWall(i, y, _wallType);
					}
				}
			}

			for (int y = Math.Min(yStart, yEnd); y <= Math.Max(yStart, yEnd); y++)
			{
				for (int i = PositionX - 1; i <= PositionX + 1; i++)
				{
					WorldGen.KillTile(i, y);
				}
			}
		}
	}

	//code referenced/modified from: https://github.com/Fixtone/DungeonCarver/blob/master/Assets/Scripts/Maps/Leaf.cs
	public class DungeonSegment
	{
		public int DungeonSegmentWidth
		{
			get; private set;
		}
		public int DungeonSegmentHeight
		{
			get; private set;
		}
		public DungeonSegment childDungeonSegmentLeft
		{
			get; private set;
		}
		public DungeonSegment childDungeonSegmentRight
		{
			get; private set;
		}

		private readonly int _x;
		private readonly int _y;
		private Rectangle RoomRectangle;
		private Rectangle RoomRectangle1;
		private Rectangle RoomRectangle2;

		public DungeonSegment(int x, int y, int DungeonSegmentWidth, int DungeonSegmentHeight)
		{
			this.DungeonSegmentWidth = DungeonSegmentWidth;
			this.DungeonSegmentHeight = DungeonSegmentHeight;
			_x = x;
			_y = y;
		}

		public bool SplitDungeonSegment(int minDungeonSegmentSize)
		{
			if (childDungeonSegmentLeft != null || childDungeonSegmentRight != null)
			{
				return false;
			}

			bool splitHorizontally = WorldGen.genRand.NextBool();

			float hotizontalFactor = (float)DungeonSegmentWidth / DungeonSegmentHeight;
			float verticalFactor = (float)DungeonSegmentHeight / DungeonSegmentWidth;

			if (hotizontalFactor >= 1.25)
			{
				splitHorizontally = false;
			}
			else if (verticalFactor >= 1.25)
			{
				splitHorizontally = true;
			}

			int max = 0;
			if (splitHorizontally)
			{
				max = DungeonSegmentHeight - minDungeonSegmentSize;
			}
			else
			{
				max = DungeonSegmentWidth - minDungeonSegmentSize;
			}

			if (max <= minDungeonSegmentSize)
			{
				return false;
			}

			int split = WorldGen.genRand.Next(minDungeonSegmentSize, max);

			if (splitHorizontally)
			{
				childDungeonSegmentLeft = new DungeonSegment(_x, _y, DungeonSegmentWidth, split);
				childDungeonSegmentRight = new DungeonSegment(_x, _y + split, DungeonSegmentWidth, DungeonSegmentHeight - split);
			}
			else
			{
				childDungeonSegmentLeft = new DungeonSegment(_x, _y, split, DungeonSegmentHeight);
				childDungeonSegmentRight = new DungeonSegment(_x + split, _y, DungeonSegmentWidth - split, DungeonSegmentHeight);
			}

			return true;
		}

		public void PlaceRooms(ProceduralRoomsGenerator mapGenerator, int maxDungeonSegmentSize, int MaxRoomSize, int MinRoomSize)
		{
			if (childDungeonSegmentLeft != null || childDungeonSegmentRight != null)
			{
				//recursively search for children until you hit the end of the branch
				if (childDungeonSegmentLeft != null)
				{
					childDungeonSegmentLeft.PlaceRooms(mapGenerator, maxDungeonSegmentSize, MaxRoomSize, MinRoomSize);
				}

				if (childDungeonSegmentRight != null)
				{
					childDungeonSegmentRight.PlaceRooms(mapGenerator, maxDungeonSegmentSize, MaxRoomSize, MinRoomSize);
				}

				if (childDungeonSegmentLeft != null && childDungeonSegmentRight != null)
				{
					mapGenerator.CreatePathways(childDungeonSegmentLeft.GetRoom(), childDungeonSegmentRight.GetRoom());
				}
			}
			else
			{
				int roomWidth = WorldGen.genRand.Next(MinRoomSize, MaxRoomSize);
				int roomHeight = WorldGen.genRand.Next(MinRoomSize, MaxRoomSize);
				int RoomX = WorldGen.genRand.Next(_x, _x + (DungeonSegmentWidth - 1) + roomWidth);
				int RoomY = WorldGen.genRand.Next(_y, _y + (DungeonSegmentHeight - 1) + roomHeight);

				RoomRectangle = new Rectangle(RoomX, RoomY, roomWidth, roomHeight);

				mapGenerator.PlaceRoom(RoomRectangle);
			}
		}

		private Rectangle GetRoom()
		{
			if (RoomRectangle != new Rectangle(0, 0, 0, 0))
			{
				return RoomRectangle;
			}
			else
			{
				if (childDungeonSegmentLeft != null)
				{
					RoomRectangle1 = childDungeonSegmentLeft.GetRoom();
				}

				if (childDungeonSegmentRight != null)
				{
					RoomRectangle2 = childDungeonSegmentRight.GetRoom();
				}
			}

			if (childDungeonSegmentLeft == null && childDungeonSegmentRight == null)
			{
				return new Rectangle(0, 0, 0, 0);
			}
			else if (RoomRectangle2 == new Rectangle(0, 0, 0, 0))
			{
				return RoomRectangle1;
			}
			else if (RoomRectangle1 == new Rectangle(0, 0, 0, 0))
			{
				return RoomRectangle2;
			}
			else if (WorldGen.genRand.NextBool())
			{
				return RoomRectangle1;
			}
			else
			{
				return RoomRectangle2;
			}
		}
	}
}