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

using Spooky.Core;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.Minibiomes.Christmas;
using Spooky.Content.Tiles.Minibiomes.Christmas.Furniture;
using Spooky.Content.Tiles.Painting;

namespace Spooky.Content.Generation
{
	public class ChristmasDungeon : ModSystem
	{
		public static List<ushort> BlockTypes = new()
		{
			(ushort)ModContent.TileType<ChristmasBrickRed>(),
			(ushort)ModContent.TileType<ChristmasBrickBlue>(),
			(ushort)ModContent.TileType<ChristmasBrickGreen>(),
			(ushort)ModContent.TileType<ChristmasSlabRed>(),
			(ushort)ModContent.TileType<ChristmasSlabBlue>(),
			(ushort)ModContent.TileType<ChristmasSlabGreen>(),
			(ushort)ModContent.TileType<ChristmasWood>(),
			(ushort)ModContent.TileType<ChristmasCarpet>()
		};

		public static List<int> WallTypes = new()
		{
			ModContent.WallType<ChristmasBrickRedWall>(),
			ModContent.WallType<ChristmasBrickBlueWall>(),
			ModContent.WallType<ChristmasBrickGreenWall>(),
			ModContent.WallType<ChristmasWoodWall>(),
			ModContent.WallType<ChristmasWindow>()
		};

		public static List<int> FurnitureTypes = new()
		{
			ModContent.TileType<ChristmasChandelier>(),
			ModContent.TileType<ChristmasLantern>(),
			ModContent.TileType<ChristmasLamp>(),
			ModContent.TileType<ChristmasWorkBench>(),
			ModContent.TileType<ChristmasDresser>(),
			ModContent.TileType<ChristmasBed>(),
			ModContent.TileType<ChristmasSofa>(),
			ModContent.TileType<ChristmasChair>(),
			ModContent.TileType<ChristmasCandelabra>(),
			ModContent.TileType<ChristmasCandle>(),
			ModContent.TileType<ChristmasTable>(),
			ModContent.TileType<ChristmasBookcase>(),
			ModContent.TileType<ChristmasPiano>(),
			ModContent.TileType<ChristmasClock>(),
			ModContent.TileType<ChristmasBathtub>(),
			ModContent.TileType<ChristmasSink>(),
			ModContent.TileType<KrampusGiantWorkBench>(),
			ModContent.TileType<KrampusSawStation>(),
			ModContent.TileType<KrampusDrillStation>()
		};

		public static Vector2[] RoomChestList = new Vector2[] {};

		private void PlaceChristmasDungeon(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.ChrismtasDungeon").Value;

			int DungeonWidth = Main.maxTilesX / 30;
			int DungeonHeight = Main.maxTilesY / 10;

			int maxDungeonSegmentSize = 25;
			int minDungeonSegmentSize = 24;
			int MaxRoomSize = 20;
			int MinRoomSize = 10;

			bool JungleOnLeftSide = GenVars.JungleX < (Main.maxTilesX / 2);

			int Start = (Main.maxTilesX / 2);
			int End = JungleOnLeftSide ? Main.maxTilesX - 200 : 200;

			int Increment = JungleOnLeftSide ? 20 : -20;

			//find a valid position in the underground snow biome
			for (int Y = Main.maxTilesY - 300; Y >= Main.worldSurface + 90; Y -= 5)
			{
				for (int X = Start; JungleOnLeftSide ? X <= End : X >= End; X += Increment)
				{
					if (CanPlaceBiome(X, Y, DungeonWidth, DungeonHeight))
					{
						ProceduralRoomsGenerator Generator = new ProceduralRoomsGenerator(X, Y, DungeonWidth, DungeonHeight, maxDungeonSegmentSize, minDungeonSegmentSize, MaxRoomSize, MinRoomSize);
						Generator.GenerateDungeon();
						FillEmptySpaceInbetweenRooms(X, Y, DungeonWidth, DungeonHeight);
						for (double i = 0; i < 0.5; i += 0.00001)
						{
							progress.Set(i);
						}

						PlaceKrampusRoom(X, Y, 35, 22);
						DungeonCleanup(X, Y, DungeonWidth, DungeonHeight);
						for (double i = 0.5; i < 0.75; i += 0.00001)
						{
							progress.Set(i);
						}

						DungeonAmbienceAndDetails(X, Y, DungeonWidth, DungeonHeight);
						for (double i = 0.75; i < 1; i += 0.00001)
						{
							progress.Set(i);
						}

						//get rid of annoying liquid inside the dungeon
						for (int i = X - (DungeonWidth / 2) - 15; i <= X + (DungeonWidth / 2) + 15; i++)
						{
							for (int j = Y - (DungeonHeight / 2) - 15; j <= Y + (DungeonHeight / 2) + 15; j++)
							{
								Main.tile[i, j].LiquidAmount = 0;
							}
						}

						//change the dungeon to the actual colors once everything else is done generating
						int Brick = ModContent.TileType<ChristmasBrickRed>();
						int BrickWall = ModContent.WallType<ChristmasBrickRedWall>();
						int Slab = ModContent.TileType<ChristmasSlabRed>();

						//0 = red, 1 = blue, 2 = green (applies to the main brick color)
						int BrickColor = WorldGen.genRand.Next(3);

						//randomize the colors for the slab and walls
						bool FirstColor = WorldGen.genRand.NextBool();

						//determine the color of what each brick (regular brick, slab, and brick wall) colors should be
						//uses FirstColor to ensure that none of the 3 different bricks (regular brick, slab, and brick wall) are ever the same color
						switch (BrickColor)
						{
							case 0:
							{
								Brick = ModContent.TileType<ChristmasBrickRed>();
							 	BrickWall = FirstColor ? ModContent.WallType<ChristmasBrickBlueWall>() : ModContent.WallType<ChristmasBrickGreenWall>();
								Slab = !FirstColor ? ModContent.TileType<ChristmasSlabBlue>() : ModContent.TileType<ChristmasSlabGreen>();
								break;
							}
							case 1:
							{
								Brick = ModContent.TileType<ChristmasBrickBlue>();
							 	BrickWall = FirstColor ? ModContent.WallType<ChristmasBrickGreenWall>() : ModContent.WallType<ChristmasBrickRedWall>();
								Slab = !FirstColor ? ModContent.TileType<ChristmasSlabGreen>() : ModContent.TileType<ChristmasSlabRed>();
								break;
							}
							case 2:
							{
								Brick = ModContent.TileType<ChristmasBrickGreen>();
							 	BrickWall = FirstColor ? ModContent.WallType<ChristmasBrickRedWall>() : ModContent.WallType<ChristmasBrickBlueWall>();
								Slab = !FirstColor ? ModContent.TileType<ChristmasSlabRed>() : ModContent.TileType<ChristmasSlabBlue>();
								break;
							}
						}

						for (int i = X - (DungeonWidth / 2) - 50; i <= X + (DungeonWidth / 2) + 50; i++)
						{
							for (int j = Y - (DungeonHeight / 2) - 50; j <= Y + (DungeonHeight / 2) + 50; j++)
							{
								if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>())
								{
									Main.tile[i, j].TileType = (ushort)Brick;
								}

								if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasSlabRed>())
								{
									Main.tile[i, j].TileType = (ushort)Slab;
								}

								if (Main.tile[i, j].WallType == ModContent.WallType<ChristmasBrickRedWall>())
								{
									Main.tile[i, j].WallType = (ushort)BrickWall;
								}
							}
						}

						return;
					}
				}
			}
		}

		public void FillEmptySpaceInbetweenRooms(int PositionX, int PositionY, int Width, int Height)
		{
			//first, fill in general space toward the center of the dungeon
			for (int i = PositionX - (Width / 2) + 25; i <= PositionX + (Width / 2) - 25; i++)
			{
				for (int j = PositionY - (Height / 2) + 25; j <= PositionY + (Height / 2) - 25; j++)
				{
					if (!BlockTypes.Contains(Main.tile[i, j].TileType) && !WallTypes.Contains(Main.tile[i, j].WallType))
					{
						Main.tile[i, j].ClearEverything();
						WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrickRed>());
						WorldGen.PlaceWall(i, j, ModContent.WallType<ChristmasBrickRedWall>());
					}
				}
			}

			//afterward fill in smaller leftover clusters surrounded by the dungeon to prevent ugly areas of snow/ice/caves just existing inbetween the dungeon rooms/halls
			int MaxPoints = 1000;

			void getAttachedPoints(int x, int y, List<Point> points)
			{
				Tile tile = Main.tile[x, y];
				Point point = new(x, y);

				if (!WorldGen.InWorld(x, y))
				{
					tile = new Tile();
				}

				if (BlockTypes.Contains(tile.TileType) || WallTypes.Contains(tile.WallType) || points.Count > MaxPoints || points.Contains(point))
				{
					return;
				}

				//do not attempt to add to the point list if the point is outside of the box where the dungeon has generated
				if (x >= PositionX - (Width / 2) && x <= PositionX + (Width / 2) && y >= PositionY - (Height / 2) && y <= PositionY + (Height / 2))
				{
					points.Add(point);

					getAttachedPoints(x + 1, y, points);
					getAttachedPoints(x - 1, y, points);
					getAttachedPoints(x, y + 1, points);
					getAttachedPoints(x, y - 1, points);
				}
			}

			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					List<Point> chunkPoints = new();

					getAttachedPoints(i, j, chunkPoints);

					if (chunkPoints.Count <= MaxPoints)
					{
						foreach (Point p in chunkPoints)
						{
							WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
							{
								new Actions.ClearTile(true), new Actions.SetLiquid(0, 0),
								new Actions.PlaceTile((ushort)ModContent.TileType<ChristmasBrickRed>()),
								new Actions.PlaceWall((ushort)ModContent.WallType<ChristmasBrickRedWall>())
							}));
						}
					}
				}
			}
		}

		public void PlaceKrampusRoom(int PositionX, int PositionY, int Width, int Height)
		{
			//places the floor without any tile check so that krampus always has a solid floor he can spawn on
			for (int i = PositionX - (Width / 3) - 5; i <= PositionX + (Width / 3) + 5; i++)
			{
				for (int j = PositionY + (Height / 2) + 2; j <= PositionY + (Height / 2) + 8; j++)
				{
					Main.tile[i, j].ClearEverything();
					WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrickRed>());
					WorldGen.PlaceWall(i, j, ModContent.WallType<ChristmasBrickRedWall>());
				}
			}

			/*
			//places the ceiling without any tile check so that krampus always has a solid floor he can spawn on
			for (int i = PositionX - (Width / 3) - 8; i <= PositionX + (Width / 3) + 8; i++)
			{
				for (int j = PositionY - (Height / 2) - 8; j <= PositionY - (Height / 2) - 2; j++)
				{
					Main.tile[i, j].ClearEverything();
					WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrickRed>());
					WorldGen.PlaceWall(i, j, ModContent.WallType<ChristmasBrickRedWall>());
				}
			}
			*/
			
			//dig out smaller box inside the main one
			for (int i = PositionX - (Width / 2); i <= PositionX + (Width / 2); i++)
			{
				for (int j = PositionY - (Height / 2); j <= PositionY + (Height / 2); j++)
				{
					WorldGen.KillTile(i, j);
					WorldGen.KillTile(i - 1, j);
					WorldGen.KillTile(i + 1, j);
					WorldGen.KillTile(i, j - 1);
					WorldGen.KillTile(i, j + 1);
				}
			}

			//spawn krampus
			Flags.KrampusPosition = new Vector2((PositionX + 1) * 16, (PositionY + 12) * 16);
			NPC.NewNPC(null, (int)Flags.KrampusPosition.X, (int)Flags.KrampusPosition.Y, ModContent.NPCType<Krampus>());
		}

		public void DungeonCleanup(int PositionX, int PositionY, int Width, int Height)
		{
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

			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					if (!Main.tile[i, j].HasTile && Main.tile[i, j].WallType == ModContent.WallType<ChristmasBrickRedWall>() && Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasBrickRed>())
					{
						bool CanDestroyX = false;
						for (int CheckX = i + 1; CheckX < i + 10; CheckX++)
						{
							if (Main.tile[CheckX, j].TileType != ModContent.TileType<ChristmasBrickRed>() && Main.tile[CheckX, j].WallType == ModContent.WallType<ChristmasBrickRedWall>())
							{
								CanDestroyX = true;
								break;
							}
						}
						if (CanDestroyX)
						{
							for (int CheckX = i + 1; CheckX < i + 10; CheckX++)
							{
								WorldGen.KillTile(CheckX, j);
							}
						}
					}
				}
			}

			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					//clean up floating clumps of tiles in the dungeon
					List<Point> chunkPoints = new();
					getAttachedPoints(i, j, chunkPoints);

					int cutoffLimit = 200;
					if (chunkPoints.Count > 0 && chunkPoints.Count < cutoffLimit)
					{
						foreach (Point p in chunkPoints)
						{
							WorldGen.KillTile(p.X, p.Y);
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

					if (!Main.tile[i, j].HasTile && Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasBrickRed>() && Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasBrickRed>() &&
					Main.tile[i, j - 1].TileType == ModContent.TileType<ChristmasBrickRed>() && Main.tile[i, j + 1].TileType == ModContent.TileType<ChristmasBrickRed>())
					{
						WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrickRed>());
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

					//get rid of single tiles on the ground since it looks weird
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile)
					{
						WorldGen.KillTile(i, j);
					}

					//get rid of 1x2 tiles on the ground since it looks weird
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasBrickRed>() && !Main.tile[i - 2, j].HasTile && !Main.tile[i + 1, j].HasTile)
					{
						WorldGen.KillTile(i, j);
						WorldGen.KillTile(i - 1, j);
					}

					//get rid of 1x3 tiles on the ground since it looks weird
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasBrickRed>() && 
					Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasBrickRed>() && !Main.tile[i - 2, j].HasTile && !Main.tile[i + 2, j].HasTile)
					{
						WorldGen.KillTile(i, j);
						WorldGen.KillTile(i - 1, j);
						WorldGen.KillTile(i + 1, j);
					}

					//get rid of surfaces in the dungeon that arent thick enough for wooden planks flooring to place
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>())
					{
						if ((!Main.tile[i, j - 1].HasTile && !Main.tile[i, j - 2].HasTile && !Main.tile[i, j - 3].HasTile) && !CanPlacePlankFlooring(i, j, false))
						{
							WorldGen.KillTile(i, j);
						}
					}
				}
			}

			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					//fill in horizontal spaces that are too thin
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && !Main.tile[i + 1, j].HasTile)
					{
						int tileCountX = 0;
						bool CanFillInX = false;
						for (int CheckX = i + 1; CheckX < i + 7; CheckX++)
						{
							tileCountX++;

							if (Main.tile[CheckX, j].TileType == ModContent.TileType<ChristmasBrickRed>())
							{
								CanFillInX = true;
								break;
							}
						}
						if (CanFillInX)
						{
							for (int CheckX = i + 1; CheckX < i + tileCountX; CheckX++)
							{
								WorldGen.PlaceTile(CheckX, j, ModContent.TileType<ChristmasBrickRed>());
							}
						}
					}

					//fill in vertical spaces that are too thin
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && !Main.tile[i, j + 1].HasTile)
					{
						int tileCountY = 0;
						bool CanFillInY = false;
						for (int CheckY = j + 1; CheckY < j + 7; CheckY++)
						{
							tileCountY++;

							if (Main.tile[i, CheckY].TileType == ModContent.TileType<ChristmasBrickRed>())
							{
								CanFillInY = true;
								break;
							}
						}
						if (CanFillInY)
						{
							for (int CheckY = j + 1; CheckY < j + tileCountY; CheckY++)
							{
								WorldGen.PlaceTile(i, CheckY, ModContent.TileType<ChristmasBrickRed>());
							}
						}
					}
				}
			}

			//destroy walls on the outside of the dungeon so that they dont stick out of the edges of it, and place secret rooms inside of walls
			for (int i = PositionX - 25 - (Width / 2); i <= PositionX + 25 + (Width / 2); i++)
			{
				for (int j = PositionY - 25 - (Height / 2); j <= PositionY + 25 + (Height / 2); j++)
				{
					bool NoTileUp = Main.tile[i, j - 1].TileType != ModContent.TileType<ChristmasBrickRed>() && !WallTypes.Contains(Main.tile[i, j - 1].WallType);
					bool NoTileDown = Main.tile[i, j + 1].TileType != ModContent.TileType<ChristmasBrickRed>() && !WallTypes.Contains(Main.tile[i, j + 1].WallType);
					bool NoTileLeft = Main.tile[i - 1, j].TileType != ModContent.TileType<ChristmasBrickRed>() && !WallTypes.Contains(Main.tile[i - 1, j].WallType);
					bool NoTileRight = Main.tile[i + 1, j].TileType != ModContent.TileType<ChristmasBrickRed>() && !WallTypes.Contains(Main.tile[i + 2, j].WallType);

					if ((NoTileUp || NoTileDown || NoTileLeft || NoTileRight) && WallTypes.Contains(Main.tile[i, j].WallType))
					{
						WorldGen.KillWall(i, j);
					}

					//place hidden rooms inside of walls if theres enough room
					if (CanPlaceHiddenRoom(i, j))
					{
						//dig out square
						for (int x = i - 4; x <= i + 5; x++)
						{
							for (int y = j - 4; y <= j + 4; y++)
							{
								WorldGen.KillTile(x, y);
								WorldGen.KillTile(x - 1, y);
								WorldGen.KillTile(x + 1, y);
								WorldGen.KillTile(x, y - 1);
								WorldGen.KillTile(x, y + 1);
							}
						}

						RoomChestList = RoomChestList.Append(new Vector2(i + WorldGen.genRand.Next(-3, 5), j + 5)).ToArray();
					}
				}
			}	
		}

		public void DungeonAmbienceAndDetails(int PositionX, int PositionY, int Width, int Height)
		{
			//place entrances along the left side of the dungeon
			for (int i = PositionX - (Width / 2) - 25; i <= PositionX - 20; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					//left side door entrance
					if (CanPlaceEntrance(i, j, false, false) && Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && !Main.tile[i - 1, j].HasTile && Main.tile[i, j].WallType == 0)
					{
						bool CanPlace = false;

						for (int xCheck = i; xCheck <= i + 7; xCheck++)
						{
							if (!Main.tile[xCheck, j].HasTile && !Main.tile[xCheck, j - 1].HasTile && !Main.tile[xCheck, j - 2].HasTile && 
							!Main.tile[xCheck, j + 1].HasTile && !Main.tile[xCheck, j + 2].HasTile && WallTypes.Contains(Main.tile[xCheck, j].WallType))
							{
								CanPlace = true;
								break;
							}
						}

						if (CanPlace)
						{
							Vector2 EntranceOrigin = new Vector2(i - 2, j - 3);
							StructureHelper.API.Generator.GenerateStructure("Content/Structures/ChristmasDungeon/EntranceLeft.shstruct", EntranceOrigin.ToPoint16(), Mod);
						}
					}
				}
			}

			//place entrances along the right side of the dungeon
			for (int i = PositionX + 20; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					//right side door entrance
					if (CanPlaceEntrance(i, j, true, false) && Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && !Main.tile[i + 1, j].HasTile && Main.tile[i, j].WallType == 0)
					{
						bool CanPlace = false;

						for (int xCheck = i; xCheck >= i - 7; xCheck--)
						{
							if (!Main.tile[xCheck, j].HasTile && !Main.tile[xCheck, j - 1].HasTile && !Main.tile[xCheck, j - 2].HasTile && 
							!Main.tile[xCheck, j + 1].HasTile && !Main.tile[xCheck, j + 2].HasTile && WallTypes.Contains(Main.tile[xCheck, j].WallType))
							{
								CanPlace = true;
								break;
							}
						}

						if (CanPlace)
						{
							Vector2 EntranceOrigin = new Vector2(i - 6, j - 3);
							StructureHelper.API.Generator.GenerateStructure("Content/Structures/ChristmasDungeon/EntranceRight.shstruct", EntranceOrigin.ToPoint16(), Mod);
						}
					}
				}
			}

			//place trapdoor entrances on the top of the dungeon
			for (int i = PositionX - (Width / 2); i <= PositionX + (Width / 2); i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY - 20; j++)
				{
					//trapdoor entrance
					if (CanPlaceEntrance(i, j, false, true) && Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() && 
					Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasBrickRed>() && Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasBrickRed>() && 
					!Main.tile[i, j - 1].HasTile && !Main.tile[i - 1, j - 1].HasTile && Main.tile[i, j].WallType == 0)
					{
						bool CanPlace = false;

						for (int yCheck = j; yCheck <= j + 7; yCheck++)
						{
							if (!Main.tile[i, yCheck].HasTile && !Main.tile[i - 1, yCheck].HasTile && !Main.tile[i - 2, yCheck].HasTile && 
							!Main.tile[i + 1, yCheck].HasTile && !Main.tile[i + 2, yCheck].HasTile && WallTypes.Contains(Main.tile[i, yCheck].WallType))
							{
								CanPlace = true;
								break;
							}
						}

						if (CanPlace)
						{
							Vector2 EntranceOrigin = new Vector2(i - 3, j);
							StructureHelper.API.Generator.GenerateStructure("Content/Structures/ChristmasDungeon/EntranceTop.shstruct", EntranceOrigin.ToPoint16(), Mod);
						}
					}
				}
			}

			//place slab lining around the inside of the biome and slope tiles
			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					//place slab bricks around open surfaces inside of the dungeon
					bool AnySurroundingAir = !Main.tile[i - 1, j].HasTile || !Main.tile[i + 1, j].HasTile || !Main.tile[i, j - 1].HasTile || !Main.tile[i, j + 1].HasTile;
					
					bool NoWallsAround = WallTypes.Contains(Main.tile[i - 1, j].WallType) || WallTypes.Contains(Main.tile[i + 1, j].WallType) || 
					WallTypes.Contains(Main.tile[i, j - 1].WallType) || WallTypes.Contains(Main.tile[i, j + 1].WallType);

					if ((Main.tile[i, j].TileType == ModContent.TileType<ChristmasBrickRed>() || Main.tile[i, j].TileType == ModContent.TileType<ChristmasSlabRed>()) && 
					WallTypes.Contains(Main.tile[i, j].WallType) && AnySurroundingAir) //&& !NoWallsAround)
					{
						for (int x = i - 1; x <= i + 1; x++)
						{
							for (int y = j - 1; y <= j + 1; y++)
							{
								if (Main.tile[x, y].TileType == ModContent.TileType<ChristmasBrickRed>() && WallTypes.Contains(Main.tile[x, y].WallType))
								{
									Main.tile[x, y].TileType = (ushort)ModContent.TileType<ChristmasSlabRed>();
								}
							}
						}
					}

					//slope tiles
					if (BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Tile.SmoothSlope(i, j);
					}
				}
			}

			//place wooden plank walls, place wooden planks on the floor, and place platforms on the sides of walls
			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					//place wooden plank walls
					if (Main.tile[i, j].WallType == ModContent.WallType<ChristmasBrickRedWall>() && CanPlaceWoodWall(i, j))
					{
						Main.tile[i, j].WallType = (ushort)ModContent.WallType<ChristmasWoodWall>();
					}

					//specifically check to make sure the wall type 2 blocks above is a dungeon wall, this prevents the planks from being placed outside of the dungeon
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasSlabRed>() && WallTypes.Contains(Main.tile[i, j - 2].WallType))
					{
						if (!Main.tile[i, j - 1].HasTile && CanPlacePlankFlooring(i, j, true))
						{
							Main.tile[i, j].TileType = (ushort)ModContent.TileType<ChristmasWood>();
							Main.tile[i, j + 1].TileType = (ushort)ModContent.TileType<ChristmasWood>();
							Main.tile[i, j + 2].TileType = (ushort)ModContent.TileType<ChristmasWood>();
						}
					}

					//place wooden platform shelves
					if (WorldGen.genRand.NextBool(20) && BlockTypes.Contains(Main.tile[i, j].TileType) && WallTypes.Contains(Main.tile[i, j].WallType))
					{
						int Length = WorldGen.genRand.Next(3, 5);

						if (!Main.tile[i + 1, j].HasTile && Main.tile[i, j - 1].HasTile && Main.tile[i, j + 1].HasTile)
						{
							if (CanPlacePlatform(i, j, Length, false))
							{
								for (int x = i + 1; x <= i + Length; x++)
								{
									WorldGen.PlaceTile(x, j, ModContent.TileType<ChristmasPlatform>());

									if (WorldGen.genRand.NextBool(5))
									{
										WorldGen.PlaceObject(x, j - 1, TileID.ClayPot);
										WorldGen.PlaceObject(x, j - 2, TileID.BloomingHerbs, true, 6);
									}
									else if (WorldGen.genRand.NextBool(6))
									{	
										WorldGen.PlaceObject(x, j - 1, TileID.Presents, true, WorldGen.genRand.Next(0, 8));
									}
									else
									{	
										WorldGen.PlaceObject(x, j - 1, TileID.Books, true, WorldGen.genRand.Next(0, 5));
									}
								}
							}
						}
					}

					if (WorldGen.genRand.NextBool(20) && BlockTypes.Contains(Main.tile[i, j].TileType) && WallTypes.Contains(Main.tile[i, j].WallType))
					{
						int Length = WorldGen.genRand.Next(3, 5);

						if (!Main.tile[i - 1, j].HasTile && Main.tile[i, j - 1].HasTile && Main.tile[i, j + 1].HasTile)
						{
							if (CanPlacePlatform(i, j, Length, true))
							{
								for (int x = i - Length; x <= i - 1; x++)
								{
									WorldGen.PlaceTile(x, j, ModContent.TileType<ChristmasPlatform>());

									if (WorldGen.genRand.NextBool(5))
									{
										WorldGen.PlaceObject(x, j - 1, TileID.ClayPot);
										WorldGen.PlaceObject(x, j - 2, TileID.BloomingHerbs, true, 6);
									}
									else if (WorldGen.genRand.NextBool(6))
									{	
										WorldGen.PlaceObject(x, j - 1, TileID.Presents, true, WorldGen.genRand.Next(0, 8));
									}
									else
									{	
										WorldGen.PlaceObject(x, j - 1, TileID.Books, true, WorldGen.genRand.Next(0, 5));
									}
								}
							}
						}
					}
				}
			}

			//place windows, place ropes, place christmas carpet on the floor, and clean up wood floors too
			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					//get rid of wood flooring chunks if they are too small so there isnt any super thin layers of wood anywhere on the floor
					if ((Main.tile[i, j].TileType == ModContent.TileType<ChristmasWood>() || Main.tile[i, j].TileType == ModContent.TileType<ChristmasCarpet>()) && 
					Main.tile[i - 1, j].TileType != ModContent.TileType<ChristmasWood>() && Main.tile[i - 1, j].TileType != ModContent.TileType<ChristmasCarpet>() && !Main.tile[i, j - 1].HasTile)
					{
						int plankCount = 0;
						bool RemovePlankFlooring = false;
						for (int CheckX = i + 1; CheckX < i + 5; CheckX++)
						{
							plankCount++;

							if (Main.tile[CheckX, j].TileType != ModContent.TileType<ChristmasWood>() && Main.tile[CheckX, j].TileType != ModContent.TileType<ChristmasCarpet>())
							{
								RemovePlankFlooring = true;
								break;
							}
						}

						if (RemovePlankFlooring)
						{
							for (int CheckX = i; CheckX < i + plankCount; CheckX++)
							{
								Main.tile[CheckX, j].TileType = (ushort)ModContent.TileType<ChristmasSlabRed>();
								Main.tile[CheckX, j + 1].TileType = (ushort)ModContent.TileType<ChristmasSlabRed>();

								if (Main.tile[CheckX, j + 3].TileType == ModContent.TileType<ChristmasSlabRed>())
								{
									Main.tile[CheckX, j + 2].TileType = (ushort)ModContent.TileType<ChristmasSlabRed>();
								}
								else
								{
									Main.tile[CheckX, j + 2].TileType = (ushort)ModContent.TileType<ChristmasBrickRed>();
								}
							}
						}
					}

					int WindowSizeX = WorldGen.genRand.Next(6, 10);
					int WindowSizeY = WorldGen.genRand.Next(6, 10);
					if (WorldGen.genRand.NextBool(7) && !Main.tile[i, j].HasTile && Main.tile[i, j].WallType == ModContent.WallType<ChristmasWoodWall>())
					{
						if (CanPlaceWindow(i, j, WindowSizeX, WindowSizeY))
						{
							//place the actual window itself
							for (int x = i - (WindowSizeX / 2); x <= i + (WindowSizeX / 2); x++)
							{
								for (int y = j - (WindowSizeY / 2); y <= j + (WindowSizeY / 2); y++)
								{
									//place a border of christmas wood around the window
									if (x == i - (WindowSizeX / 2) || x == i + (WindowSizeX / 2) || y == j - (WindowSizeY / 2) || y == j + (WindowSizeY / 2))
									{
										Main.tile[x, y].WallType = (ushort)ModContent.WallType<ChristmasBrickRedWall>();
									}
									//place the actual window wall
									else
									{
										Main.tile[x, y].WallType = (ushort)ModContent.WallType<ChristmasWindow>();
									}
								}
							}
						}
					}
					
					//place ropes
					if (BlockTypes.Contains(Main.tile[i, j].TileType) && WallTypes.Contains(Main.tile[i, j].WallType) && !Main.tile[i, j + 1].HasTile)
					{
						PlaceChainRope(i, j);
					}

					//place carpets on the plank flooring
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasWood>() && (!Main.tile[i, j - 1].HasTile || Main.tile[i, j - 1].TileType == ModContent.TileType<ChristmasChain>()) && 
					Main.tile[i - 1, j].HasTile && Main.tile[i + 1, j].HasTile && (Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasWood>() || Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasWood>() ||
					Main.tile[i - 1, j].TileType == ModContent.TileType<ChristmasCarpet>() || Main.tile[i + 1, j].TileType == ModContent.TileType<ChristmasCarpet>()))
					{
						Main.tile[i, j].TileType = (ushort)ModContent.TileType<ChristmasCarpet>();
					}
				}
			}

			//place chests in rooms by going through the list of placed rooms
			foreach (Vector2 pos in RoomChestList)
			{
				WorldGen.PlaceChest((int)pos.X, (int)pos.Y, (ushort)ModContent.TileType<ChristmasChest>());
			}

			//place furniture randomly in the dungeon
			for (int i = PositionX - 25 - (Width / 2); i <= PositionX + 25 + (Width / 2); i++)
			{
				for (int j = PositionY - 25 - (Height / 2); j <= PositionY + 25 + (Height / 2); j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasChain>() && !Main.tile[i, j - 1].HasTile)
					{
						WorldGen.KillTile(i, j);
					}

					//tables and chairs with candles/candelabras on them
					if (WorldGen.genRand.NextBool(3) && CanPlaceFurniture(i, j, 12))
					{
						switch (WorldGen.genRand.Next(6))
						{
							//two tables with chairs and sometimes candles/candelabras on the tables
							case 0:
							{
								WorldGen.PlaceObject(i - 3, j - 1, ModContent.TileType<ChristmasTable>());
								if (WorldGen.genRand.NextBool())
								{
									int Type = WorldGen.genRand.NextBool() ? ModContent.TileType<ChristmasCandelabra>() : ModContent.TileType<ChristmasCandle>();
									WorldGen.PlaceObject(i - 3, j - 3, Type);
								}
								WorldGen.PlaceObject(i - 5, j - 1, ModContent.TileType<ChristmasChair>(), direction: 1);
								WorldGen.PlaceObject(i - 1, j - 1, ModContent.TileType<ChristmasChair>(), direction: -1);

								WorldGen.PlaceObject(i + 3, j - 1, ModContent.TileType<ChristmasTable>());
								if (WorldGen.genRand.NextBool())
								{
									int Type = WorldGen.genRand.NextBool() ? ModContent.TileType<ChristmasCandelabra>() : ModContent.TileType<ChristmasCandle>();
									WorldGen.PlaceObject(i + 3, j - 3, Type);
								}
								WorldGen.PlaceObject(i + 5, j - 1, ModContent.TileType<ChristmasChair>(), direction: -1);
								WorldGen.PlaceObject(i + 1, j - 1, ModContent.TileType<ChristmasChair>(), direction: 1);
								break;
							}
							//bookcases with lamps on either side of them
							case 1:
							{
								WorldGen.PlaceObject(i - 6, j - 1, ModContent.TileType<ChristmasLamp>());
								WorldGen.PlaceObject(i - 3, j - 1, ModContent.TileType<ChristmasBookcase>());
								WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ChristmasBookcase>());
								WorldGen.PlaceObject(i + 3, j - 1, ModContent.TileType<ChristmasBookcase>());
								WorldGen.PlaceObject(i + 6, j - 1, ModContent.TileType<ChristmasLamp>());
								break;
							}
							//bed with some dressers and candle
							case 2:
							{
								WorldGen.PlaceObject(i - 5, j - 1, (ushort)ModContent.TileType<ChristmasBed>(), direction: 1);
								WorldGen.PlaceChest(i, j - 1, (ushort)ModContent.TileType<ChristmasDresser>());
								WorldGen.PlaceObject(i, j - 3, (ushort)ModContent.TileType<ChristmasCandle>());
								WorldGen.PlaceChest(i + 3, j - 1, (ushort)ModContent.TileType<ChristmasDresser>());
								WorldGen.PlaceObject(i + 5, j - 1, (ushort)ModContent.TileType<ChristmasChair>());
								break;
							}
							//chair, piano, and dresser with stuff on top of it
							case 3:
							{
								WorldGen.PlaceObject(i - 3, j - 1, ModContent.TileType<ChristmasChair>(), direction: 1);
								WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ChristmasPiano>());
								WorldGen.PlaceObject(i + 3, j - 1, ModContent.TileType<ChristmasTable>());

								for (int tableLength = i + 2; tableLength <= i + 4; tableLength++)
								{
									if (WorldGen.genRand.NextBool(6))
									{
										WorldGen.PlaceObject(tableLength, j - 3, TileID.ClayPot);
										WorldGen.PlaceObject(tableLength, j - 4, TileID.BloomingHerbs, true, 6);
									}
									else
									{
										WorldGen.PlaceObject(tableLength, j - 3, TileID.Books, true, WorldGen.genRand.Next(0, 5));
									}
								}
								break;
							}
							//bathtub and sink
							case 4:
							{
								WorldGen.PlaceObject(i - 3, j - 1, ModContent.TileType<ChristmasTable>());
								WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ChristmasClock>());
								WorldGen.PlaceObject(i + 4, j - 1, ModContent.TileType<ChristmasTable>());
								break;
							}
							case 5:
							{
								WorldGen.PlaceObject(i + 1, j - 1, ModContent.TileType<ChristmasBathtub>());
								WorldGen.PlaceObject(i - 1, j - 1, ModContent.TileType<ChristmasSink>());
								break;
							}
						}
					}

					//random krampus contraptions
					if (WorldGen.genRand.NextBool(20) && CanPlaceFurniture(i, j, 3))
					{
						switch (WorldGen.genRand.Next(3))
						{
							case 0:
							{
								WorldGen.PlaceObject(i, j - 1, ModContent.TileType<KrampusGiantWorkBench>());
								break;
							}
							case 1:
							{
								WorldGen.PlaceObject(i, j - 1, ModContent.TileType<KrampusDrillStation>());
								break;
							}
							case 2:
							{
								WorldGen.PlaceObject(i, j - 1, ModContent.TileType<KrampusSawStation>());
								break;
							}
						}
					}

					//lamps
					if (WorldGen.genRand.NextBool(25) && CanPlaceFurniture(i, j, 3))
					{
						WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ChristmasLamp>());
					}

					//lanterns
					if (WorldGen.genRand.NextBool(18) && IsFlatCeiling(i, j, 2) && CanPlaceFurniture(i, j, 4, true))
					{
						WorldGen.PlaceObject(i, j + 1, ModContent.TileType<ChristmasLantern>());
					}

					//chandeliers
					if (WorldGen.genRand.NextBool(25) && IsFlatCeiling(i, j, 4) && CanPlaceFurniture(i, j, 4, true))
					{
						WorldGen.PlaceObject(i, j + 1, ModContent.TileType<ChristmasChandelier>());
					}
				}
			}

			//place paintings
			for (int i = PositionX - (Width / 2) - 25; i <= PositionX + (Width / 2) + 25; i++)
			{
				for (int j = PositionY - (Height / 2) - 25; j <= PositionY + (Height / 2) + 25; j++)
				{
					if (WorldGen.genRand.NextBool(4) && !Main.tile[i, j].HasTile && Main.tile[i, j].WallType == ModContent.WallType<ChristmasWoodWall>())
					{
						List<ushort> Paintings = new()
						{
							(ushort)ModContent.TileType<XmasAbstractPainting1>(),
							(ushort)ModContent.TileType<XmasAbstractPainting2>(),
							(ushort)ModContent.TileType<XmasAbstractPainting3>(),
							(ushort)ModContent.TileType<XmasAbstractPainting4>(),
							(ushort)ModContent.TileType<XmasForestPainting>(),
							(ushort)ModContent.TileType<XmasNorthStarPainting>(),
							(ushort)ModContent.TileType<XmasSnowPillarPainting>(),
							(ushort)ModContent.TileType<XmasSnowTownPainting>(),
							(ushort)ModContent.TileType<XmasSpiritPainting>(),
							(ushort)ModContent.TileType<XmasTiltedHousePainting>(),
							(ushort)ModContent.TileType<XmasTrainPainting>(),
						};

						if (CanPlacePainting(i, j, Paintings))
						{
							WorldGen.PlaceObject(i, j, WorldGen.genRand.Next(Paintings));
						}
					}
				}
			}
		}

		//check for a flat surface in the dungeon that also has no tiles above the entire flat space
		//use to check for a specific width to place individual pieces of furniture, or in other cases multiple pieces of furniture (such as tables with chairs next to them)
		public bool CanPlaceFurniture(int PositionX, int PositionY, int Width, bool Ceiling = false)
		{
			if (!Ceiling)
			{
				for (int x = PositionX - (Width / 2); x <= PositionX + (Width / 2); x++)
				{
					//check specifically for christmas carpet since the entire floor will be made out of that
					if (Main.tile[x, PositionY].TileType == ModContent.TileType<ChristmasCarpet>() && !Main.tile[x, PositionY - 1].HasTile && !FurnitureTypes.Contains(Main.tile[x, PositionY - 1].TileType))
					{
						continue;
					}
					else
					{
						return false;
					}
				}
			}

			for (int x = PositionX - Width; x <= PositionX + Width; x++)
			{
				for (int y = PositionY - 5; y <= PositionY + 5; y++)
				{
					if (FurnitureTypes.Contains(Main.tile[x, y].TileType))
					{
						return false;
					}
				}
			}

			return true;
		}

		//same as above but for ceilings instead of the floor for hanging lights
		public bool IsFlatCeiling(int PositionX, int PositionY, int Width)
		{
			for (int x = PositionX - (Width / 2); x <= PositionX + (Width / 2); x++)
			{
				if (Main.tile[x, PositionY].TileType == ModContent.TileType<ChristmasSlabRed>() && !FurnitureTypes.Contains(Main.tile[x, PositionY + 1].TileType) && !Main.tile[x, PositionY + 1].HasTile && 
				!Main.tile[x, PositionY + 2].HasTile && !Main.tile[x, PositionY + 3].HasTile && !Main.tile[x, PositionY + 4].HasTile && !Main.tile[x, PositionY + 5].HasTile)
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public bool CanPlaceEntrance(int PositionX, int PositionY, bool Right, bool Trapdoor)
		{
			//check for doors and trapdoors to prevent entrances from generating too close to each other
			for (int x = PositionX - 20; x <= PositionX + 20; x++)
			{
				for (int y = PositionY - 20; y <= PositionY + 20; y++)
				{
					if (Main.tile[x, y].TileType == ModContent.TileType<ChristmasDoorClosed>() || Main.tile[x, y].TileType == 387)
					{
						return false;
					}
				}
			}

			//dont run any of the horizontal checking for trapdoor entrances
			if (!Trapdoor)
			{
				//preform another check to make sure that theres enough open space where the entrance will place, used for entrances on the side of the walls
				if (!Right)
				{
					for (int x = PositionX - 1; x >= PositionX - 8; x--)
					{
						if (Main.tile[x, PositionY].HasTile || Main.tile[x, PositionY].LiquidAmount > 0)
						{
							return false;
						}
					}
				}
				else
				{
					for (int x = PositionX + 1; x <= PositionX + 8; x++)
					{
						if (Main.tile[x, PositionY].HasTile || Main.tile[x, PositionY].LiquidAmount > 0)
						{
							return false;
						}
					}
				}
			}
			//preform an upward check for trapdoor entrances to make sure theres enough room above it
			else
			{
				for (int y = PositionY - 1; y >= PositionY - 8; y--)
				{
					if (Main.tile[PositionX, y].HasTile || Main.tile[PositionX, y].LiquidAmount > 0)
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanPlacePlankFlooring(int PositionX, int PositionY, bool ActuallyPlacing)
		{
			if (ActuallyPlacing)
			{
				//check for a thick enough surface where wood plank flooring can be placed
				for (int x = PositionX - 1; x <= PositionX + 1; x++)
				{
					for (int y = PositionY; y <= PositionY + 4; y++)
					{
						if (!BlockTypes.Contains(Main.tile[x, y].TileType))
						{
							return false;
						}
					}
				}
			}

			for (int y = PositionY; y <= PositionY + 4; y++)
			{
				if (!Main.tile[PositionX, y].HasTile)
				{
					return false;
				}
			}

			return true;
		}

		public bool CanPlaceHiddenRoom(int PositionX, int PositionY)
		{
			//check for a thick enough surface where wood plank flooring can be placed
			for (int x = PositionX - 15; x <= PositionX + 15; x++)
			{
				for (int y = PositionY - 15; y <= PositionY + 15; y++)
				{
					if (Main.tile[x, y].TileType != ModContent.TileType<ChristmasBrickRed>())
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanPlaceWoodWall(int PositionX, int PositionY)
		{
			//check for a thick enough surface where wood plank flooring can be placed
			for (int x = PositionX - 1; x <= PositionX + 1; x++)
			{
				for (int y = PositionY - 1; y <= PositionY + 1; y++)
				{
					if (WorldGen.SolidTile(x, y) && Main.tile[x, y].TileType != ModContent.TileType<ChristmasPlatform>())
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanPlacePainting(int PositionX, int PositionY, List<ushort> PaintingsToCheckFor)
		{
			//first check to make sure no other paintings are nearby
			for (int i = PositionX - 6; i <= PositionX + 6; i++)
			{
				for (int j = PositionY - 6; j <= PositionY + 6; j++)
				{
					if (PaintingsToCheckFor.Contains(Main.tile[i, j].TileType) || Main.tile[i, j].WallType != ModContent.WallType<ChristmasWoodWall>())
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanPlaceWindow(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			//first check to make sure no other windows are nearby
			for (int i = PositionX - (SizeX / 2) - 10; i <= PositionX + (SizeX / 2) + 10; i++)
			{
				for (int j = PositionY - (SizeY / 2) - 10; j <= PositionY + (SizeY / 2) + 10; j++)
				{
					if (Main.tile[i, j].WallType == ModContent.WallType<ChristmasWindow>())
					{
						return false;
					}
				}
			}

			//then check if the entire area where the window should generate is all brick walls and has no christmas wood walls
			for (int i = PositionX - (SizeX / 2) - 2; i <= PositionX + (SizeX / 2) + 2; i++)
			{
				for (int j = PositionY - (SizeY / 2) - 2; j <= PositionY + (SizeY / 2) + 2; j++)
				{
					if (Main.tile[i, j].HasTile || Main.tile[i, j].WallType != ModContent.WallType<ChristmasWoodWall>())
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanPlacePlatform(int PositionX, int PositionY, int Length, bool LeftSide)
		{
			if (LeftSide)
			{
				for (int x = PositionX - Length; x <= PositionX - 1; x++)
				{
					for (int y = PositionY - 6; y <= PositionY + 6; y++)
					{
						if (Main.tile[x, y].HasTile || !WallTypes.Contains(Main.tile[x, y].WallType))
						{
							return false;
						}
					}
				}
			}
			else
			{
				for (int x = PositionX + 1; x <= PositionX + Length; x++)
				{
					for (int y = PositionY - 6; y <= PositionY + 6; y++)
					{
						if (Main.tile[x, y].HasTile || !WallTypes.Contains(Main.tile[x, y].WallType))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		public void PlaceChainRope(int PositionX, int PositionY)
		{
			//first preform a downward check to make sure theres enough vertical room to place the rope
			//check down for 55 tiles at minimum and if theres that many tiles or more downward, allow the actual rope to be placed
			for (int j = PositionY + 2; j <= PositionY + 55; j++)
			{
				if (Main.tile[PositionX, j].TileType == ModContent.TileType<ChristmasPlatform>() || Main.tile[PositionX - 1, j].TileType == ModContent.TileType<ChristmasPlatform>() || Main.tile[PositionX + 1, j].TileType == ModContent.TileType<ChristmasPlatform>())
				{
					continue;
				}

				//if theres a tile too close to where the rope will place, dont allow it to place
				if ((WorldGen.SolidTile(PositionX, j) && !BlockTypes.Contains(Main.tile[PositionX, j].TileType)) || WorldGen.SolidTile(PositionX - 1, j) || WorldGen.SolidTile(PositionX + 1, j))
				{
					return;
				}

				//check 15 tiles left and right around where the rope will place
				//if another rope is too close, dont allow a new rope to place
				for (int i = PositionX - 15; i <= PositionX + 15; i++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<ChristmasChain>())
					{
						return;
					}
				}
			}

			//if the above checking loop is successful, then place the actual rope
			//use an arbitrary maximum of 300 tiles since the rope (probably) wont ever go that far down
			for (int j = PositionY + 1; j <= PositionY + 300; j++)
			{
				//stop placing the rope if theres a tile on floor below it
				if (WorldGen.SolidTile(PositionX, j) && Main.tile[PositionX, j].TileType != ModContent.TileType<ChristmasPlatform>())
				{
					return;
				}

				if (Main.tile[PositionX, j].TileType == ModContent.TileType<ChristmasPlatform>())
				{
					continue;
				}

				WorldGen.PlaceTile(PositionX, j, ModContent.TileType<ChristmasChain>());

				if (!WorldGen.SolidTile(PositionX, j) && Main.tile[PositionX, j].TileType != ModContent.TileType<ChristmasPlatform>())
				{
					Main.tile[PositionX, j].TileType = (ushort)ModContent.TileType<ChristmasChain>();
				}
			}
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

			float AmountOfTilesNeeded = (Width * Height) / 1.5f;

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

			tasks.Insert(GenIndex1 + 1, new PassLegacy("Christmas Dungeon", PlaceChristmasDungeon));
		}

		//post worldgen to place items in the spooky biome chests
        public override void PostWorldGen()
		{
			for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex];

				if (chest == null) 
                {
					continue;
				}

				//placeholder loot for now
				if (WorldGen.InWorld(chest.x, chest.y))
				{
					Tile chestTile = Main.tile[chest.x, chest.y];

					if (chestTile.TileType == ModContent.TileType<ChristmasChest>())
					{
						//goodie bags
						chest.item[0].SetDefaults(ItemID.GoodieBag);
						chest.item[0].stack = WorldGen.genRand.Next(1, 3);
					}
				}
            }
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

		private List<DungeonSegment> DungeonSegments = new List<DungeonSegment>();

		public ProceduralRoomsGenerator(int PositionX, int PositionY, int width, int height, int maxDungeonSegmentSize, int minDungeonSegmentSize, int MaxRoomSize, int MinRoomSize)
		{
			_positionX = PositionX;
			_positionY = PositionY;
			_width = width;
			_height = height;
			_maxDungeonSegmentSize = maxDungeonSegmentSize;
			_minDungeonSegmentSize = minDungeonSegmentSize;
			_MaxRoomSize = MaxRoomSize;
			_MinRoomSize = MinRoomSize;
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
					if (Main.tile[i, j].TileType != ModContent.TileType<ChristmasBrickRed>() && Main.tile[i, j].WallType != ModContent.WallType<ChristmasBrickRedWall>())
					{
						Main.tile[i, j].ClearEverything();
						WorldGen.PlaceTile(i, j, ModContent.TileType<ChristmasBrickRed>());
						WorldGen.PlaceWall(i, j, ModContent.WallType<ChristmasBrickRedWall>());
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

			ChristmasDungeon.RoomChestList = ChristmasDungeon.RoomChestList.Append(new Vector2(room.X + WorldGen.genRand.Next(-8, 9), room.Y + (room.Height / 2) + 1)).ToArray();
		}

		public void CreatePathways(Rectangle room1, Rectangle room2)
		{
			Vector2 room1Center = new Vector2(room1.X - (room1.Width / 2), room1.Y - (room1.Height / 2));
			Vector2 room2Center = new Vector2(room2.X - (room2.Width / 2), room2.Y - (room2.Height / 2));

			if (WorldGen.genRand.NextBool())
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
					if (Main.tile[x, j].TileType != ModContent.TileType<ChristmasBrickRed>() && Main.tile[x, j].WallType != ModContent.WallType<ChristmasBrickRedWall>())
					{
						Main.tile[x, j].ClearEverything();
						WorldGen.PlaceTile(x, j, ModContent.TileType<ChristmasBrickRed>());
						WorldGen.PlaceWall(x, j, ModContent.WallType<ChristmasBrickRedWall>());
					}
				}
			}

			for (int x = Math.Min(xStart, xEnd) - 2; x <= Math.Max(xStart, xEnd) + 2; x++)
			{
				for (int j = PositionY - 3; j <= PositionY + 3; j++)
				{
					WorldGen.KillTile(x, j);
				}
			}
		}

		private void VerticalPathway(int yStart, int yEnd, int PositionX)
		{
			//place additional tiles if theres no dungeon blocks where the pathway is
			for (int y = Math.Min(yStart, yEnd) - 10; y <= Math.Max(yStart, yEnd) + 9; y++)
			{
				for (int i = PositionX - 10; i <= PositionX + 10; i++)
				{
					if (Main.tile[i, y].TileType != ModContent.TileType<ChristmasBrickRed>() && Main.tile[i, y].WallType != ModContent.WallType<ChristmasBrickRedWall>())
					{
						Main.tile[i, y].ClearEverything();
						WorldGen.PlaceTile(i, y, ModContent.TileType<ChristmasBrickRed>());
						WorldGen.PlaceWall(i, y, ModContent.WallType<ChristmasBrickRedWall>());
					}
				}
			}

			for (int y = Math.Min(yStart, yEnd) - 2; y <= Math.Max(yStart, yEnd) + 2; y++)
			{
				for (int i = PositionX - 3; i <= PositionX + 3; i++)
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

		private List<Rectangle> rooms = new List<Rectangle>();

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

				Rectangle RoomRectangleCheck = new Rectangle(RoomX - 25, RoomY - 25, roomWidth + 25, roomHeight + 25);

				bool newRoomIntersects = false;
				foreach (Rectangle existingRoom in rooms)
                {
                    if (RoomRectangleCheck.Intersects(existingRoom))
                    {
                        newRoomIntersects = true;
                        break;
                    }
                }

                if (!newRoomIntersects)
                {
                    rooms.Add(RoomRectangle);
					mapGenerator.PlaceRoom(RoomRectangle);
                }
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