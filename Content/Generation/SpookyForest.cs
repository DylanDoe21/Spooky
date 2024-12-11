using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Gourds;
using Spooky.Content.Tiles.SpookyBiome.Mushrooms;
using Spooky.Content.Tiles.SpookyBiome.Tree;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class SpookyForest : ModSystem
    {
        //default positions, edit based on worldsize below
        static int PositionX = Main.maxTilesX / 2;
		static int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

		private void GenerateSpookyForest(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpookyForest").Value;

			//decide whether or not to use the alt background
			if (WorldGen.genRand.NextBool(2))
			{
				Flags.SpookyBackgroundAlt = true;
			}
			else
			{
				Flags.SpookyBackgroundAlt = false;
			}

			//if config is enabled, place it at spawn
			if (ModContent.GetInstance<SpookyConfig>().SpookyForestSpawn)
			{
				PositionX = Main.maxTilesX / 2;
			}
			//otherwise place it in front of the dungeon
			else
			{
				PositionX = (GenVars.snowOriginLeft + GenVars.snowOriginRight) / 2;

				//attempt to find a valid position for the biome to place in
				bool foundValidPosition = false;
				int attempts = 0;

				//the biomes initial position is the very center of the snow biome
				//this code basically looks for snow biome blocks, and if it finds any, keep moving the biome over until it is far enough away from the snow biome
				while (!foundValidPosition && attempts++ < 100000)
				{
					while (!NoSnowBiomeNearby(PositionX, PositionY))
					{
						PositionX += (PositionX > (Main.maxTilesX / 2) ? 100 : -100);
					}
					if (NoSnowBiomeNearby(PositionX, PositionY))
					{
						foundValidPosition = true;
					}
				}
			}

			//set y position again so it is always correct before placing
			PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

			//place the initial ellipse
			int SizeX = Main.maxTilesX / 5;
			int SizeY = Main.maxTilesY / 3;

			PlaceSpookyForestEllipse(PositionX, PositionY, SizeX / 6, SizeY);

			//place mushroom minibiomes in the underground
			int SizeXInt = 80;
			int SizeYInt = 50;
			int MushroomSizeX = Main.maxTilesX / SizeXInt;
			int MushroomSizeY = Main.maxTilesY / SizeYInt;

			SpookyWorldMethods.PlaceOval(PositionX, (int)Main.worldSurface + (Main.maxTilesY / 7), ModContent.TileType<MushroomMoss>(), 0, MushroomSizeX, MushroomSizeY, 2f, false);

			//place clumps of green grass using a temporary dirt tile clone that will be replaced later in generation
			for (int moss = 0; moss < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 15E-05); moss++)
			{
				int X = WorldGen.genRand.Next(0, Main.maxTilesX);
				int Y = WorldGen.genRand.Next(0, Main.maxTilesY);

				if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
				{
					//surface clumps
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>())
					{
						WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(15, 28), WorldGen.genRand.Next(15, 28),
						ModContent.TileType<SpookyDirt2>(), false, 0f, 0f, false, true);
					}

					//smaller clumps underground
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
					{
						WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(5, 12), WorldGen.genRand.Next(5, 12),
						ModContent.TileType<SpookyDirt2>(), false, 0f, 0f, false, true);
					}
				}
			}

			//place clumps of stone throughout the surface
			for (int Rocks = 0; Rocks < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 15E-05); Rocks++)
			{
				int X = WorldGen.genRand.Next(0, Main.maxTilesX);
				int Y = WorldGen.genRand.Next(0, (int)Main.worldSurface);

				if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
				{
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>() && WorldGen.genRand.NextBool(3))
					{
						WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(12, 22), WorldGen.genRand.Next(12, 22),
						ModContent.TileType<SpookyStone>(), false, 0f, 0f, false, true);
					}
				}
			}

			//dig out noise caves in the biome
			int cavePerlinSeed = WorldGen.genRand.Next();

			//dig out caves specifically on the surface of the biome and only in moss stone to create some surface caves
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = 100; Y < (int)Main.worldSurface + 10; Y++)
				{
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 350f, Y / 250f, 5, unchecked(cavePerlinSeed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 350f, Y / 250f, 5, cavePerlinSeed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 350f, Y / 250f, 5, unchecked(cavePerlinSeed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//kill or place tiles depending on the noise map
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							WorldGen.KillTile(X, Y);

							for (int i = X - 1; i <= X + 1; i++)
							{
								for (int j = Y - 1; j <= Y + 1; j++)
								{
									if (Main.tile[i, j].WallType > 0)
									{
										Main.tile[i, j].WallType = (ushort)ModContent.WallType<SpookyStoneWall>();
									}
								}
							}
						}
					}
				}
			}

			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = (int)Main.worldSurface + 10; Y < Main.maxTilesY - 200; Y++)
				{
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 450f, Y / 300f, 5, unchecked(cavePerlinSeed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 450f, Y / 300f, 5, cavePerlinSeed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 450f, Y / 300f, 5, unchecked(cavePerlinSeed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//kill or place tiles depending on the noise map
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							WorldGen.KillTile(X, Y);
						}
					}

					if (Main.tile[X, Y].TileType == ModContent.TileType<MushroomMoss>())
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 2000f, Y / 300f, 5, unchecked(cavePerlinSeed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 2000f, Y / 300f, 5, cavePerlinSeed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 2000f, Y / 300f, 5, unchecked(cavePerlinSeed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//kill or place tiles depending on the noise map
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							WorldGen.KillTile(X, Y);
						}
					}

					if (Main.tile[X, Y].WallType == ModContent.WallType<SpookyStoneWall>())
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 200f, Y / 1000f, 5, unchecked(cavePerlinSeed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 200f, Y / 1000f, 5, cavePerlinSeed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 200f, Y / 1000f, 5, unchecked(cavePerlinSeed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//kill or place tiles depending on the noise map
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							WorldGen.KillWall(X, Y);
						}
					}
				}
			}

			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = (int)Main.worldSurface + 10; Y < Main.maxTilesY - 200; Y++)
				{
					if (Main.tile[X, Y].TileType == ModContent.TileType<MushroomMoss>())
					{
						if (Main.tile[X - 1, Y].HasTile && Main.tile[X + 1, Y].HasTile && Main.tile[X, Y - 1].HasTile && Main.tile[X, Y + 1].HasTile)
						{
							Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyStone>();
						}
					}
				}
			}

			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = 100; Y < (int)Main.worldSurface + 10; Y++)
				{
					if (Main.tile[X, Y].HasTile && Main.tile[X, Y].WallType == ModContent.WallType<SpookyStoneWall>())
					{
						Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyStone>();
						Main.tile[X - 1, Y].TileType = (ushort)ModContent.TileType<SpookyStone>();
						Main.tile[X + 1, Y].TileType = (ushort)ModContent.TileType<SpookyStone>();
						Main.tile[X, Y - 1].TileType = (ushort)ModContent.TileType<SpookyStone>();
						Main.tile[X, Y + 1].TileType = (ushort)ModContent.TileType<SpookyStone>();
					}
				}
			}

			bool PlacedMineshaft = false;
			int MineshaftAttempts = 0;
			while (!PlacedMineshaft && MineshaftAttempts++ < 100000)
			{
				//place starter house
				int x = PositionX > (Main.maxTilesX / 2) ? PositionX + ((Main.maxTilesX / 12) / 6) : PositionX - ((Main.maxTilesX / 12) / 6);
				int y = PositionY; //start here to not touch floating islands

				while ((!WorldGen.SolidTile(x, y) || !Cemetery.NoFloatingIsland(x, y)) && y <= Main.worldSurface)
				{
					y++;
				}
				if (WorldGen.SolidTile(x, y) && Cemetery.NoFloatingIsland(x, y))
				{
					PlaceMineshaft(x, y);

					Vector2 MineshaftOrigin = new Vector2(x - 26, y - 20);
					Generator.GenerateStructure("Content/Structures/SpookyBiome/MineshaftEntrance", MineshaftOrigin.ToPoint16(), Mod);

					PlacedMineshaft = true;
				}
			}

			CleanOutSmallClumps();

			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = 100; Y < Main.maxTilesY - 200; Y++)
				{
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>() && !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile)
					{
						WorldGen.KillTile(X, Y);
					}
				}
			}

			//place wall dithering at the entrance to the underground for a nicer transition
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = (int)Main.worldSurface; Y < (int)Main.worldSurface + 10; Y++)
				{
					if (Main.tile[X, Y].WallType == ModContent.WallType<SpookyStoneWall>())
					{
						for (int newY = Y; newY <= Y + 5; newY++)
						{
							if (WorldGen.genRand.NextBool(3))
							{
								WorldGen.PlaceWall(X, newY, ModContent.WallType<SpookyStoneWall>());
							}
						}
					}
				}
			}
		}

		private void SpreadSpookyGrass(GenerationProgress progress, GameConfiguration configuration)
        {
			//spread grass on all spooky dirt tiles
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = PositionY - 100; Y <= Main.maxTilesY - 100; Y++)
				{ 
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];
                    Tile tileLeft = Main.tile[X - 1, Y];
                    Tile tileRight = Main.tile[X + 1, Y];

                    if (tile.TileType == ModContent.TileType<SpookyDirt>() && (!tileAbove.HasTile || !tileBelow.HasTile || !tileLeft.HasTile || !tileRight.HasTile))
                    {
                        tile.TileType = (ushort)ModContent.TileType<SpookyGrass>();
                    }

                    if (tile.TileType == ModContent.TileType<SpookyDirt2>() && (!tileAbove.HasTile || !tileBelow.HasTile || !tileLeft.HasTile || !tileRight.HasTile))
                    {
                        tile.TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
                    }

                    if (tile.TileType == ModContent.TileType<SpookyDirt>() && (tileAbove.TileType == ModContent.TileType<SpookyGrass>() || tileBelow.TileType == ModContent.TileType<SpookyGrass>() || 
                    tileLeft.TileType == ModContent.TileType<SpookyGrass>() || tileRight.TileType == ModContent.TileType<SpookyGrass>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyDirt>(), ModContent.TileType<SpookyGrass>(), false);
                    }

                    if (tile.TileType == ModContent.TileType<SpookyDirt2>() && (tileAbove.TileType == ModContent.TileType<SpookyGrassGreen>() || tileBelow.TileType == ModContent.TileType<SpookyGrassGreen>() || 
                    tileLeft.TileType == ModContent.TileType<SpookyGrassGreen>() || tileRight.TileType == ModContent.TileType<SpookyGrassGreen>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyDirt2>(), ModContent.TileType<SpookyGrassGreen>(), false);
                    }
                }
            }
        }

        private void GrowSpookyTrees(GenerationProgress progress, GameConfiguration configuration)
        {
			//grow trees
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = 0; Y < (int)Main.worldSurface - 50; Y++)
                {
					//regular surface trees
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrass>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrassGreen>())
                    {
                        WorldGen.GrowTree(X, Y - 1);
                    }
                }

                //grow giant mushrooms
                for (int Y = (int)Main.worldSurface + 25; Y < (Main.maxTilesY / 2); Y++)
                {
                    if ((Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrassGreen>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyStone>()) &&
                    !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                    {
                        if (WorldGen.genRand.NextBool(18))
                        {
                            GrowGiantMushroom(X, Y, ModContent.TileType<GiantShroom>(), 5, 8);
                        }
                    }

                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<MushroomMoss>() && !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                    {
                        if (WorldGen.genRand.NextBool(5))
                        {
                            GrowGiantMushroom(X, Y, ModContent.TileType<GiantShroom>(), 6, 10);
                        }
                    }
                }
            }
        }

		private void SpookyForestAmbience(GenerationProgress progress, GameConfiguration configuration)
		{
			//place ambient objects
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = 100; Y < Main.maxTilesY - 200; Y++)
				{
					Tile tile = Main.tile[X, Y];
					Tile tileAbove = Main.tile[X, Y - 1];
					Tile tileBelow = Main.tile[X, Y + 1];
					Tile tileLeft = Main.tile[X - 1, Y];
					Tile tileRight = Main.tile[X + 1, Y];

					//kill any single floating tiles so things dont look ugly
					if (tile.TileType == ModContent.TileType<SpookyGrass>() || tile.TileType == ModContent.TileType<SpookyGrassGreen>() || tile.TileType == ModContent.TileType<SpookyStone>())
					{
						if (!tileAbove.HasTile && !tileBelow.HasTile && !tileLeft.HasTile && !tileRight.HasTile)
						{
							WorldGen.KillTile(X, Y);
						}
					}

					//convert leftover green grass dirt back into regular dirt
					if (tile.TileType == ModContent.TileType<SpookyDirt2>())
					{
						tile.TileType = (ushort)ModContent.TileType<SpookyDirt>();
					}

					//orange spooky vines
					if (tile.TileType == ModContent.TileType<SpookyGrass>() && !Main.tile[X, Y + 1].HasTile)
					{
						if (WorldGen.genRand.NextBool(2))
						{
							WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyVines>());
						}
					}
					if (tile.TileType == ModContent.TileType<SpookyVines>())
					{
						int[] ValidTiles = { ModContent.TileType<SpookyGrass>() };

						SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<SpookyVines>(), ValidTiles);
					}

					//green spooky vines
					if (tile.TileType == ModContent.TileType<SpookyGrassGreen>() && !Main.tile[X, Y + 1].HasTile)
					{
						if (WorldGen.genRand.NextBool(2))
						{
							WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyVinesGreen>());
						}
					}
					if (tile.TileType == ModContent.TileType<SpookyVinesGreen>())
					{
						int[] ValidTiles = { ModContent.TileType<SpookyGrassGreen>() };

						SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<SpookyVinesGreen>(), ValidTiles);
					}

					//spooky fungus vines
					if (tile.TileType == ModContent.TileType<MushroomMoss>() && !Main.tile[X, Y + 1].HasTile)
					{
						if (WorldGen.genRand.NextBool(2))
						{
							WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyFungusVines>());
						}
					}
					if (tile.TileType == ModContent.TileType<SpookyFungusVines>())
					{
						int[] ValidTiles = { ModContent.TileType<MushroomMoss>() };

						SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<SpookyFungusVines>(), ValidTiles);
					}

					//place gourds and weeds
					if (tile.TileType == (ushort)ModContent.TileType<SpookyGrass>())
					{
						//gourds
						if (WorldGen.genRand.NextBool(3) && CanGrowGourd(X, Y))
						{
							ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(),
							(ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(),
							(ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>() };

							WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Gourds), true, WorldGen.genRand.Next(0, 2));
						}

						//grow weeds
						if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
						{
							WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpookyWeedsOrange>());
							tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
							WorldGen.SquareTileFrame(X, Y + 1, true);
							if (Main.netMode == NetmodeID.Server)
							{
								NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
							}
						}
					}
					if (tile.TileType == (ushort)ModContent.TileType<SpookyGrassGreen>())
					{
						//gourds
						if (WorldGen.genRand.NextBool(3) && CanGrowGourd(X, Y))
						{
							ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(),
							(ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(),
							(ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>() };

							WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Gourds), true, WorldGen.genRand.Next(0, 2));
						}

						//grow weeds
						if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
						{
							WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpookyWeedsGreen>());
							tileAbove.TileFrameY = 0;
							tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
							WorldGen.SquareTileFrame(X, Y + 1, true);
							if (Main.netMode == NetmodeID.Server)
							{
								NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
							}
						}
					}
				}
			}

			//place stuff underground
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = (int)Main.worldSurface; Y < Main.maxTilesY - 200; Y++)
				{
					//mossy stone objects
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
					{
						if (WorldGen.genRand.NextBool(4))
						{
							ushort[] Vines = new ushort[] { (ushort)ModContent.TileType<HangingVine1>(), (ushort)ModContent.TileType<HangingVine2>(), (ushort)ModContent.TileType<HangingVine3>() };

							WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(Vines));
						}

						if (WorldGen.genRand.NextBool(3))
						{
							WorldGen.PlaceObject(X, Y - 1, (ushort)ModContent.TileType<MossyRock>());
						}
					}

					//place stuff on mushroom moss
					if (Main.tile[X, Y].TileType == ModContent.TileType<MushroomMoss>())
					{
						//grow big mushrooms
						if (WorldGen.genRand.NextBool(20))
						{
							ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroom1>(), (ushort)ModContent.TileType<GiantShroom2>(),
							(ushort)ModContent.TileType<GiantShroom3>(), (ushort)ModContent.TileType<GiantShroom4>() };

							WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Shrooms));
						}

						//grow big yellow mushrooms
						if (WorldGen.genRand.NextBool(20))
						{
							ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroomYellow1>(), (ushort)ModContent.TileType<GiantShroomYellow2>(),
							(ushort)ModContent.TileType<GiantShroomYellow3>(), (ushort)ModContent.TileType<GiantShroomYellow4>() };

							WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Shrooms));
						}

						//place mushroom rock piles
						if (WorldGen.genRand.NextBool(7))
						{
							ushort[] RockPiles = new ushort[] { (ushort)ModContent.TileType<MushroomRockGiant>(),
							(ushort)ModContent.TileType<MushroomRockBig>(), (ushort)ModContent.TileType<MushroomRockSmall>() };

							WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(RockPiles));
						}

						//grow weeds
						if (WorldGen.genRand.NextBool(3) && !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
						{
							WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpookyMushroom>());
							Main.tile[X, Y - 1].TileFrameX = (short)(WorldGen.genRand.Next(4) * 18);
						}
					}
				}
			}

			//place wooden beams in the mineshaft
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = 100; Y < (int)Main.worldSurface + 10; Y++)
				{
					PlaceMineshaftBeam(X, Y);
				}
			}

			//place furniture in underground old wood cabins
			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = (int)Main.worldSurface + 10; Y < Main.maxTilesY - 300; Y++)
				{
					//table
					if (WorldGen.genRand.NextBool(5) && IsFlatSurface(X, Y, 5))
					{
						WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<OldWoodTable>());
						WorldGen.PlaceObject(X - 2, Y - 1, ModContent.TileType<OldWoodChair>(), direction: 1);
						WorldGen.PlaceObject(X + 2, Y - 1, ModContent.TileType<OldWoodChair>(), direction: -1);
					}

					//bookcases
					if (WorldGen.genRand.NextBool(7) && IsFlatSurface(X, Y, 3))
					{
						WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<OldWoodBookcase>());
					}

					//organ
					if (WorldGen.genRand.NextBool(10) && IsFlatSurface(X, Y, 3))
					{
						WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<OldWoodOrgan>());
					}

					//work benches
					if (WorldGen.genRand.NextBool(3) && IsFlatSurface(X, Y, 2))
					{
						WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<OldWoodWorkBench>());
					}
				}
			}
		}

		public void ClearStuffAroundMushroomMoss(GenerationProgress progress, GameConfiguration configuration)
        {
			//statues and traps are annoying, so clear out everything from the mushroom area in the spooky forest
			for (int mushroomX = PositionX - Main.maxTilesX / 12; mushroomX <= PositionX + Main.maxTilesX / 12; mushroomX++)
			{
				for (int mushroomY = PositionY - 100; mushroomY < Main.maxTilesY / 2 - 50; mushroomY++)
                {
                    //whitelist so tiles meant to be on mushroom moss dont get cleared
                    int[] ClearWhitelist = { ModContent.TileType<MushroomMoss>(), ModContent.TileType<SpookyMushroom>(), 
                    ModContent.TileType<GiantShroom>(), ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(),
                    ModContent.TileType<SpookyDirt>(), ModContent.TileType<SpookyStone>(), ModContent.TileType<MushroomRockGiant>(), 
                    ModContent.TileType<MushroomRockBig>(), ModContent.TileType<MushroomRockSmall>(), ModContent.TileType<GiantShroom1>(),
                    ModContent.TileType<GiantShroom2>(), ModContent.TileType<GiantShroom3>(), ModContent.TileType<GiantShroom4>(),
                    ModContent.TileType<GiantShroomYellow1>(), ModContent.TileType<GiantShroomYellow2>(), ModContent.TileType<GiantShroomYellow3>(), 
                    ModContent.TileType<GiantShroomYellow4>() };

                    if (Main.tile[mushroomX, mushroomY].TileType == ModContent.TileType<MushroomMoss>() && !ClearWhitelist.Contains(Main.tile[mushroomX, mushroomY - 1].TileType))
                    {
                        WorldGen.KillTile(mushroomX, mushroomY - 1);
                    }

                    //also get rid of any liquids
                    if (Main.tile[mushroomX, mushroomY].TileType == ModContent.TileType<MushroomMoss>() && Main.tile[mushroomX, mushroomY - 1].LiquidAmount > 0)
                    {
                        for (int checkY = mushroomY; checkY >= mushroomY - 12; checkY--)
                        {
                            Main.tile[mushroomX, checkY].LiquidAmount = 0;
                        }
                    }
                }
            }
        }

        public void GenerateStarterHouse(GenerationProgress progress, GameConfiguration configuration)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                //place starter house
                int x = PositionX <= (Main.maxTilesX / 2) ? PositionX + ((Main.maxTilesX / 12) / 6) : PositionX - ((Main.maxTilesX / 12) / 6);
                int y = PositionY; //start here to not touch floating islands

                while ((!WorldGen.SolidTile(x, y) || !Cemetery.NoFloatingIsland(x, y)) && y <= Main.worldSurface)
				{
					y++;
				}
                if (WorldGen.SolidTile(x, y) && Cemetery.NoFloatingIsland(x, y))
				{
                    Vector2 origin = new Vector2(x - 10, y - 25);

                    //clear trees around the house since it is placed after them
                    for (int i = (int)origin.X - 15; i <= (int)origin.X + 15; i++)
                    {
                        for (int j = (int)origin.Y - 50; j <= (int)origin.Y + 50; j++)
                        {
                            if (Main.tile[i, j].TileType == 5)
                            {
                                WorldGen.KillTile(i, j);
                            }
                        }
                    }

                    //place starter house
                    Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestHouse", origin.ToPoint16(), Mod);

                    //place little bone in the house
                    NPC.NewNPC(null, (x + 1) * 16, (y - 9) * 16, ModContent.NPCType<LittleBoneSleeping>());

                    placed = true;
				}
            }
        }

		public void GenerateCabins(GenerationProgress progress, GameConfiguration configuration)
        {
			for (int X = PositionX - Main.maxTilesX / 45; X <= PositionX + Main.maxTilesX / 45; X++)
			{
				for (int Y = (int)Main.worldSurface + 35; Y <= Main.maxTilesY / 2 + 50; Y++)
				{
					if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
					{
						if (CanPlaceLootCabin(X, Y) && WorldGen.genRand.NextBool(45))
						{
							Vector2 CabinOrigin = new Vector2(X - 12, Y - 6);
							Generator.GenerateStructure("Content/Structures/SpookyBiome/SpookyForestCabin" + WorldGen.genRand.Next(1, 7), CabinOrigin.ToPoint16(), Mod);
						}
					}
				}
			}
        }

        //determine if theres no snow blocks nearby so the biome doesnt place in the snow biome
        public static bool NoSnowBiomeNearby(int X, int Y)
        {
            for (int i = X - 300; i < X + 300; i++)
            {
                for (int j = Y; j < Y + 300; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.SnowBlock || Main.tile[i, j].TileType == TileID.IceBlock))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

		public bool CanPlaceMushroomBiome(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
			{
				for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
				{
					if (Main.tile[i, j].TileType != ModContent.TileType<SpookyStone>())
					{
						return false;
					}
				}
			}

			for (int i = PositionX - SizeX * 2; i < PositionX + SizeX * 2; i++)
			{
				for (int j = PositionY - SizeY * 2; j < PositionY + SizeY * 2; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<MushroomMoss>())
					{
						return false;
					}
				}
			}

			return true;
		}

		public bool CanPlaceLootCabin(int PositionX, int PositionY)
		{
			for (int i = PositionX - 80; i < PositionX + 80; i++)
			{
				for (int j = PositionY - 80; j < PositionY + 80; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWood>() || Main.tile[i, j].TileType == ModContent.TileType<OldWoodChest>() || Main.tileDungeon[Main.tile[i, j].TileType])
					{
						return false;
					}
				}
			}

			return true;
		}

		//check for a flat surface in the dungeon that also has no tiles above the entire flat space
		//use to check for a specific width to place individual pieces of furniture, or in other cases multiple pieces of furniture (such as tables with chairs next to them)
		public bool IsFlatSurface(int PositionX, int PositionY, int Width)
		{
			bool AtLeastHasOneWall = false;

			for (int x = PositionX - (Width / 2); x <= PositionX + (Width / 2); x++)
			{
				if (Main.tile[x, PositionY - 1].WallType == ModContent.WallType<SpookyWoodWall>())
				{
					AtLeastHasOneWall = true;
				}

				//check specifically for christmas carpet since the entire floor will be made out of that
				if ((Main.tile[x, PositionY].TileType == ModContent.TileType<SpookyWood>() || Main.tile[x, PositionY].TileType == ModContent.TileType<OldWoodPlatform>()) && !Main.tile[x, PositionY - 1].HasTile)
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			return AtLeastHasOneWall;
		}

		public void PlaceMineshaft(int x, int y)
		{
			//initial pit
			int CurrentY = y;
			int IncrementY = y;
			int CurrentX = x;
			int IncrementX = x;

			for (int i = 0; i <= 100; i++)
			{
				//save the X-position so the next tunnel starts from where the last horizontal tunnel ended
				CurrentX = IncrementX;

				//place vertical tunnel downward
				for (IncrementY = CurrentY; IncrementY < CurrentY + 25; IncrementY++)
				{
					MineshaftCircle(CurrentX, IncrementY);
					WorldGen.digTunnel(CurrentX, IncrementY, 0, 0, 1, 3, false);
				}

				//save the Y-position so the next tunnel starts from the bottom of the previous one
				CurrentY = IncrementY;

				//place horizontal tunnel that randomly decides to go left or right
				bool Left = WorldGen.genRand.NextBool();

				if (Left)
				{
					for (IncrementX = CurrentX; IncrementX > CurrentX - 25; IncrementX--)
					{
						MineshaftCircle(IncrementX, CurrentY);
						WorldGen.digTunnel(IncrementX, CurrentY, 0, 0, 1, 3, false);
					}
				}
				else
				{
					for (IncrementX = CurrentX; IncrementX < CurrentX + 25; IncrementX++)
					{
						MineshaftCircle(IncrementX, CurrentY);
						WorldGen.digTunnel(IncrementX, CurrentY, 0, 0, 1, 3, false);
					}
				}

				//stop placing the tunnels once it reaches the underground layer
				if (CurrentY > Main.worldSurface + 20)
				{
					break;
				}
			}
		}

		private static void MineshaftCircle(int i, int j)
		{
			const int BaseRadius = 5;
			int radius = BaseRadius;

			for (int y = j - radius; y <= j + radius; y++)
			{
				for (int x = i - radius; x <= i + radius + 1; x++)
				{
					if ((int)Vector2.Distance(new Vector2(x, y), new Vector2(i, j)) <= radius)
					{
						if (Main.tile[x, y].HasTile)
						{
							Main.tile[x, y].TileType = (ushort)ModContent.TileType<SpookyWood>();
						}

						if (Main.tile[x, y].WallType > 0 && Main.tile[x, y].WallType != ModContent.WallType<SpookyStoneWall>())
						{
							Main.tile[x, y].WallType = (ushort)ModContent.WallType<SpookyWoodWall>();
						}
					}
				}

				radius = BaseRadius - WorldGen.genRand.Next(-1, 2);
			}
		}

		public void PlaceMineshaftBeam(int PositionX, int PositionY)
		{
			int AdjacentWoodCount = 0;
			
			bool CanPlaceBeam = false;

			if (Main.tile[PositionX, PositionY].TileType == ModContent.TileType<SpookyWood>() && Main.tile[PositionX, PositionY + 1].WallType == ModContent.WallType<SpookyWoodWall>() && !Main.tile[PositionX, PositionY + 1].HasTile)
			{
				for (int j = PositionY; j <= PositionY + 30; j++)
				{
					for (int i = PositionX - 3; i <= PositionX + 3; i++)
					{
						//dont place the beam if there isnt enough room (less than 6 tiles downward)
						if (j >= PositionY + 6 && Main.tile[PositionX, j + 1].HasTile)
						{
							CanPlaceBeam = true;
							break;
						}
						
						//if a wooden beam is too close, dont place another beam
						if (Main.tile[i, j].TileType == TileID.WoodenBeam)
						{
							return;
						}

						//if a tile is floating in the air and doesnt have a tile above it, dont place a beam under it
						if (j == PositionY && !Main.tile[PositionX, j - 1].HasTile)
						{
							return;
						}

						//dont place the beam if there isnt enough room (less than 6 tiles downward)
						if (j < PositionY + 6 && Main.tile[PositionX, j + 1].HasTile)
						{
							return;
						}

						//dont place the beam if theres too much room
						if (j == PositionY + 12 && !Main.tile[PositionX, j + 1].HasTile)
						{
							return;
						}
					}

					if (CanPlaceBeam)
					{
						break;
					}
				}

				if (CanPlaceBeam)
				{
					for (int j = PositionY + 1; j <= PositionY + 20; j++)
					{
						if (Main.tile[PositionX, j].HasTile)
						{
							return;
						}

						WorldGen.PlaceTile(PositionX, j, TileID.WoodenBeam);
						Tile tile = Main.tile[PositionX, j + 1];
						tile.Slope = 0;
						tile.IsHalfBlock = false;
					}
				}
			}
		}

		public static void PlaceSpookyForestEllipse(int X, int Y, int radius, int radiusY)
		{
			float scale = radiusY / (float)radius;
			float invertScale = (float)radius / radiusY;
			for (int x = -radius; x <= radius; x++)
			{
				for (float y = -radius; y <= radius; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(2.5f, 4.5f) * 2f;

					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);

						int heightLimit = (int)Main.worldSurface - (Main.maxTilesY / 10) - 20;

						if (PositionY > heightLimit)
						{
							if (PositionY <= Main.worldSurface)
							{
								if (Math.Sqrt(x * x + y * y) >= radius - radialMod)
								{
									if (WorldGen.genRand.NextBool() && tile.HasTile)
									{
										tile.TileType = (ushort)ModContent.TileType<SpookyDirt>();
									}
								}
								else
								{
									if (tile.HasTile || tile.WallType > 0)
									{
										WorldGen.KillTile(PositionX, PositionY);
										tile.TileType = (ushort)ModContent.TileType<SpookyDirt>();
										tile.HasTile = true;
										Main.tile[PositionX, PositionY + 2].WallType = (ushort)ModContent.WallType<SpookyGrassWall>();
										tile.LiquidAmount = 0;
									}
								}
							}
							else
							{
								if (Math.Sqrt(x * x + y * y) >= radius - radialMod)
								{
									if (WorldGen.genRand.NextBool() && tile.HasTile)
									{
										tile.TileType = (ushort)ModContent.TileType<SpookyStone>();
									}
								}
								else
								{
									//add a bit of noise inbetween the underground and surface
									for (int newY = PositionY - 8; newY <= PositionY; newY++)
									{
										if (WorldGen.genRand.NextBool(3))
										{
											Main.tile[PositionX, newY].TileType = (ushort)ModContent.TileType<SpookyStone>();
											Main.tile[PositionX, newY].WallType = (ushort)ModContent.WallType<SpookyStoneWall>();
										}
									}

									WorldGen.KillTile(PositionX, PositionY);
									tile.TileType = (ushort)ModContent.TileType<SpookyStone>();
									tile.HasTile = true;
									tile.WallType = (ushort)ModContent.WallType<SpookyStoneWall>();
									tile.LiquidAmount = 0;
								}
							}
						}
					}
				}
			}
		}

		//method to clean up small clumps of tiles
		public static void CleanOutSmallClumps()
		{
			List<ushort> blockTileTypes = new()
			{
				(ushort)ModContent.TileType<SpookyStone>(),
				(ushort)ModContent.TileType<SpookyDirt>(),
				(ushort)ModContent.TileType<SpookyDirt2>(),
				(ushort)ModContent.TileType<SpookyWood>(),
				(ushort)ModContent.TileType<MushroomMoss>(),
			};

			int MaxPoints = 250;

			void getAttachedPoints(int x, int y, List<Point> points)
			{
				Tile tile = Main.tile[x, y];
				Point point = new(x, y);

				if (!WorldGen.InWorld(x, y))
				{
					tile = new Tile();
				}

				if (!blockTileTypes.Contains(tile.TileType) || !tile.HasTile || points.Count > MaxPoints || points.Contains(point))
				{
					return;
				}

				points.Add(point);

				getAttachedPoints(x + 1, y, points);
				getAttachedPoints(x - 1, y, points);
				getAttachedPoints(x, y + 1, points);
				getAttachedPoints(x, y - 1, points);
			}

			for (int X = PositionX - Main.maxTilesX / 12; X <= PositionX + Main.maxTilesX / 12; X++)
			{
				for (int Y = 100; Y < Main.maxTilesY - 200; Y++)
				{
					List<Point> chunkPoints = new();
					getAttachedPoints(X, Y, chunkPoints);

					if (chunkPoints.Count <= MaxPoints)
					{
						foreach (Point p in chunkPoints)
						{
							WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
							{
								new Actions.ClearTile(true)
							}));
						}
					}
				}
			}
		}

		//grow giant glowshrooms
		public static bool GrowGiantMushroom(int X, int Y, int tileType, int minSize, int maxSize)
		{
			int canPlace = 0;

			//do not allow giant mushrooms to place if another one is too close
			for (int i = X - 5; i < X + 5; i++)
			{
				for (int j = Y - 5; j < Y + 5; j++)
				{
					if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
					{
						canPlace++;
						if (canPlace > 0)
						{
							return false;
						}
					}
				}
			}

			//make sure the area is large enough for it to place in both horizontally and vertically
			for (int i = X - 2; i < X + 2; i++)
			{
				for (int j = Y - 12; j < Y - 2; j++)
				{
					//only check for solid blocks, ambient objects dont matter
					if (Main.tile[i, j].HasTile && Main.tileSolid[Main.tile[i, j].TileType])
					{
						canPlace++;
						if (canPlace > 0)
						{
							return false;
						}
					}
				}
			}

			GiantShroom.Grow(X, Y - 1, minSize, maxSize, false);

			return true;
		}

		//make sure normal gourds cant place close to each other
		public static bool CanGrowGourd(int X, int Y)
		{
			ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(),
			(ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(),
			(ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>(),
			(ushort)ModContent.TileType<SpookySapling>(), (ushort)ModContent.TileType<SpookySaplingGreen>() };

			for (int i = X - 12; i < X + 12; i++)
			{
				for (int j = Y - 7; j < Y + 7; j++)
				{
					if (Main.tile[i, j].HasTile && Gourds.Contains(Main.tile[i, j].TileType))
					{
						return false;
					}
				}
			}

			return true;
		}

		//make sure rotten gourds cannot place too close to each other
		public static bool CanGrowRottenGourd(int X, int Y)
		{
			for (int i = X - 25; i < X + 25; i++)
			{
				for (int j = Y - 25; j < Y + 25; j++)
				{
					if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<GourdRotten>())
					{
						return false;
					}
				}
			}

			return true;
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
            //generate biome
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spooky Forest", GenerateSpookyForest));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("Little Bone House", GenerateStarterHouse));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("Spooky Forest Grass", SpreadSpookyGrass));

			//place house again because stupid ahh walls
			int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (GenIndex2 == -1)
			{
                return;
            }

            tasks.Insert(GenIndex2 + 1, new PassLegacy("Spooky Forest Cabins", GenerateCabins));
            tasks.Insert(GenIndex2 + 2, new PassLegacy("Spooky Forest Grass Again", SpreadSpookyGrass));
            tasks.Insert(GenIndex2 + 3, new PassLegacy("Glowshroom Cleanup", ClearStuffAroundMushroomMoss));
            tasks.Insert(GenIndex2 + 4, new PassLegacy("Spooky Forest Trees", GrowSpookyTrees));
            tasks.Insert(GenIndex2 + 5, new PassLegacy("Spooky Forest Ambient Tiles", SpookyForestAmbience));
        }

        //post worldgen to place items in the spooky biome chests
        public override void PostWorldGen()
		{
			List<int> MainItem = new List<int>
			{
				ModContent.ItemType<ToiletPaper>(), ModContent.ItemType<LeafBlower>(), ModContent.ItemType<NecromancyTome>(), 
				ModContent.ItemType<AutumnLeaf>(), ModContent.ItemType<EggCarton>(),
				ModContent.ItemType<CreepyCandle>(), ModContent.ItemType<CandyBag>()
			};

			List<int> ActualMainItem = new List<int>(MainItem);

			for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex];

				if (chest == null) 
                {
					continue;
				}

				if (WorldGen.InWorld(chest.x, chest.y))
				{
					Tile chestTile = Main.tile[chest.x, chest.y];

					if (chestTile.TileType == ModContent.TileType<OldWoodChest>())
					{
						if (ActualMainItem.Count == 0)
						{
							ActualMainItem = new List<int>(MainItem);
						}

						int ItemToPutInChest = WorldGen.genRand.Next(ActualMainItem.Count);

						int[] Bars = new int[] { ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar };
						int[] LightSources = new int[] { ModContent.ItemType<SpookyBiomeTorchItem>(), ModContent.ItemType<CandleItem>() };
						int[] Potions = new int[] { ItemID.LesserHealingPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };

						//main items
						chest.item[0].SetDefaults(ActualMainItem[ItemToPutInChest]);
						chest.item[0].stack = 1;
						ActualMainItem.RemoveAt(ItemToPutInChest);
						//iron or lead bars
						chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
						chest.item[1].stack = WorldGen.genRand.Next(5, 10);
						//light sources
						chest.item[2].SetDefaults(WorldGen.genRand.Next(LightSources));
						chest.item[2].stack = WorldGen.genRand.Next(3, 8);
						//potions
						chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions));
						chest.item[3].stack = WorldGen.genRand.Next(2, 3);
						//goodie bags
						chest.item[4].SetDefaults(ItemID.GoodieBag);
						chest.item[4].stack = WorldGen.genRand.Next(1, 2);
						//coins
						chest.item[5].SetDefaults(ItemID.GoldCoin);
						chest.item[5].stack = WorldGen.genRand.Next(1, 2);
					}
				}
            }
        }
    }
}