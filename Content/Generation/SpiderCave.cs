using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.SpiderCave.Tree;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Generation
{
    public class SpiderCave : ModSystem
    {
        static int initialStartPosX;
        static int startPosX;
        static int startPosY;

        private void PlaceSpiderCave(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpiderCave").Value;

            //biome position stuff
            initialStartPosX = GenVars.dungeonX;
            startPosX = GenVars.dungeonX;
            startPosY = Main.maxTilesY >= 1800 ? (Main.maxTilesY - (Main.maxTilesY / 3) - 30) : Main.maxTilesY / 2 + 100;

            //attempt to find a valid position for the biome to place in
            bool foundValidPosition = false;
            int attempts = 0;

            //the biomes initial position is the very center of the snow biome
            //this code basically looks for snow biome blocks, and if it finds any, keep moving the biome over until it is far enough away from the snow biome
            while (!foundValidPosition && attempts++ < 100000)
            {
                while (!CanPlaceBiome(startPosX, startPosY))
                {
                    startPosX += (initialStartPosX > (Main.maxTilesX / 2) ? -50 : 50);
                }
                if (CanPlaceBiome(startPosX, startPosY))
                {
                    foundValidPosition = true;
                }

                if (initialStartPosX < Main.maxTilesX / 2 && startPosX > Main.maxTilesX / 2)
                {
                    startPosX = Main.maxTilesX / 2;
                    foundValidPosition = true;
                }
                if (initialStartPosX > Main.maxTilesX / 2 && startPosX < Main.maxTilesX / 2)
                {
                    startPosX = Main.maxTilesX / 2;
                    foundValidPosition = true;
                }
            }

            int cavePerlinSeed = WorldGen.genRand.Next();

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int InitialSize = Main.maxTilesY >= 1800 ? 240 : 150;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

			//first place a bunch of spider caves as a barrier around the biome
			for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
			{
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
				{
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
					{
						float percent = dist / constant;
						float blurPercent = 0.99f;

						if (percent > blurPercent && Main.tile[X, Y - 20].WallType != WallID.SpiderUnsafe)
						{
							SpookyWorldMethods.PlaceCircle(X, Y, -1, WallID.SpiderUnsafe, WorldGen.genRand.Next(45, 65), false, false);
						}
					}
				}
			}

			//place dithering around the edge of the oval and blocks inside of the oval
			for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
			{
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
				{
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
					{
						float percent = dist / constant;
						float blurPercent = 0.99f;

						if (percent < blurPercent)
						{
							Main.tile[X, Y].ClearEverything();
							WorldGen.PlaceTile(X, Y, ModContent.TileType<DampSoil>());
							WorldGen.PlaceWall(X, Y, ModContent.WallType<DampSoilWall>());
						}
					}
				}
			}

            int XIncrement = Main.maxTilesX / 98;
            int YIncrement = Main.maxTilesY / 36;

			//dig out caves
			for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X += XIncrement)
			{
				int StartValue = origin.X - biomeSize - 2;
				int EndValue = origin.X + biomeSize + 2;
				progress.Set((X - StartValue) / (EndValue - StartValue));

				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y += YIncrement)
				{
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
					{
						float percent = dist / constant;
						float blurPercent = 0.99f;

						if (percent < blurPercent)
						{
							int NewX = X + WorldGen.genRand.Next(-20, 21);
							int NewY = Y + WorldGen.genRand.Next(-20, 21);

							CavePatch(NewX, NewY);
						}
					}
				}
			}

            //clean out small floating chunks of blocks
            CleanOutSmallClumps(true);
            CleanOutSmallClumps(false);

			//place dithering around the edge of the oval and blocks inside of the oval
			for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
			{
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
				{
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
					{
						float percent = dist / constant;
						float blurPercent = 0.99f;

						if (percent >= blurPercent)
						{
							if (WorldGen.genRand.NextBool())
							{
								if (Main.tile[X, Y].HasTile)
								{
									Main.tile[X, Y].TileType = (ushort)ModContent.TileType<DampSoil>();
								}

								if (Main.tile[X, Y].WallType > 0)
								{
									Main.tile[X, Y].WallType = (ushort)ModContent.WallType<DampSoilWall>();
								}
							}
						}
					}
				}
			}

			//place clumps of stone
			for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.98f;

                        if (percent < blurPercent)
                        {
                            //occasionally place large chunks of stone blocks
                            if (WorldGen.genRand.NextBool(1000) && Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>())
                            {
                                WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(20, 28), WorldGen.genRand.Next(20, 28), ModContent.TileType<DampStone>(), false, 0f, 0f, true, true);
                            }
                        }
                    }
                }
            }

			//place leaf walls around dirt blocks
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.98f;

                        if (percent < blurPercent)
                        {
                            if (Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<DampSoil>() &&
                            (!Main.tile[X - 1, Y].HasTile || !Main.tile[X + 1, Y].HasTile || !Main.tile[X, Y - 1].HasTile || !Main.tile[X, Y + 1].HasTile))
                            {
                                SpookyWorldMethods.PlaceCircle(X, Y, -1, ModContent.WallType<DampGrassWall>(), 3, false, true);
                            }
                        }
                    }
                }
            }
            //place dirt walls around stone blocks
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float percent = dist / constant;
                        float blurPercent = 0.98f;

                        if (percent < blurPercent)
                        {
                            if (Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<DampStone>() &&
                            (!Main.tile[X - 1, Y].HasTile || !Main.tile[X + 1, Y].HasTile || !Main.tile[X, Y - 1].HasTile || !Main.tile[X, Y + 1].HasTile))
                            {
                                SpookyWorldMethods.PlaceCircle(X, Y, -1, ModContent.WallType<DampSoilWall>(), 3, false, true);
                            }
                        }
                    }
                }
            }

            //noise dirt walls
            int Seed = WorldGen.genRand.Next();
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(X / 550f, Y / 550f, 5, Seed) * 0.5f;
                        float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(X / 550f, Y / 550f, 5, Seed) + 0.5f + horizontalOffsetNoise;
                        float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(X / 550f, Y / 550f, 5, Seed) + 0.5f;
                        float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

                        if (caveNoiseMap * caveNoiseMap < caveCreationThreshold)
						{
                            if (!Main.tile[X, Y].HasTile)
                            {
                                WorldGen.PlaceWall(X, Y, ModContent.WallType<WebBlockWall>());
                                Main.tile[X, Y].WallType = (ushort)ModContent.WallType<WebBlockWall>();
                            }
                        }
                    }
                }
            }

            //clean out small floating chunks of walls
            CleanOutSmallClumps(false);

            //some small last minute things, mainly clean up before ambient tiles are placed
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        //tile sloping
                        Tile.SmoothSlope(X, Y, true);

                        //remove any extra liquids that may still be in the biome
                        Main.tile[X, Y].LiquidAmount = 0;

                        //clean tiles that are sticking out (basically tiles only attached to one tile on one side)
                        bool OnlyRight = !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile;
                        bool OnlyLeft = !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X + 1, Y].HasTile;
                        bool OnlyDown = !Main.tile[X, Y - 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile;
                        bool OnlyUp = !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile;

                        if (OnlyRight || OnlyLeft || OnlyDown || OnlyUp)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //kill random single floating tiles
                        if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //kill one block thick surfaces
                        if (Main.tile[X, Y].HasTile && !Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        //kill random single floating walls
                        if (Main.tile[X, Y - 1].WallType <= 0 && Main.tile[X, Y + 1].WallType <= 0 && Main.tile[X - 1, Y].WallType <= 0 && Main.tile[X + 1, Y].WallType <= 0)
                        {
                            WorldGen.KillWall(X, Y);
                        }

                        //get rid of 1x2 tiles on the ground since it looks weird
                        if (Main.tile[X, Y].HasTile && Main.tile[X - 1, Y].HasTile && !Main.tile[X - 2, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                            WorldGen.KillTile(X - 1, Y);
                        }

                        //get rid of single tiles on the ground since it looks weird
                        if (Main.tile[X, Y].HasTile && !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }
					}
                }
            }

            //place clumps of vanilla ores
            ushort OppositeTier1Ore = WorldGen.SavedOreTiers.Copper == TileID.Copper ? TileID.Tin : TileID.Copper;
            ushort OppositeTier2Ore = WorldGen.SavedOreTiers.Iron == TileID.Iron ? TileID.Lead : TileID.Iron;
            ushort OppositeTier3Ore = WorldGen.SavedOreTiers.Silver == TileID.Silver ? TileID.Tungsten : TileID.Silver;
            ushort OppositeTier4Ore = WorldGen.SavedOreTiers.Gold == TileID.Gold ? TileID.Platinum : TileID.Gold;

            for (int copper = 0; copper < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 5E-05); copper++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<DampStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(8, 12), WorldGen.genRand.Next(8, 12), OppositeTier1Ore, false, 0f, 0f, false, true);
                }
            }

            for (int iron = 0; iron < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 4E-05); iron++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<DampStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(6, 10), WorldGen.genRand.Next(6, 10), OppositeTier2Ore, false, 0f, 0f, false, true);
                }
            }

            for (int silver = 0; silver < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 3E-05); silver++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<DampStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), OppositeTier3Ore, false, 0f, 0f, false, true);
                }
            }

            for (int gold = 0; gold < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 2E-05); gold++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<DampStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(4, 6), WorldGen.genRand.Next(4, 6), OppositeTier4Ore, false, 0f, 0f, false, true);
                }
            }

            //spread grass after placing structures, but before placing ambient tiles
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        Tile tile = Main.tile[X, Y];
                        Tile tileAbove = Main.tile[X, Y - 1];
                        Tile tileBelow = Main.tile[X, Y + 1];
                        Tile tileLeft = Main.tile[X - 1, Y];
                        Tile tileRight = Main.tile[X + 1, Y];

                        //spread grass onto the dirt blocks throughout the biome
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<DampSoil>(), ModContent.TileType<DampGrass>(), false);
                    }
                }
            }

            //place ambient tiles
            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
				for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
					if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
					{
						Tile tile = Main.tile[X, Y];
						Tile tileAbove = Main.tile[X, Y - 1];
						Tile tileBelow = Main.tile[X, Y + 1];
                        Tile tileLeft = Main.tile[X - 1, Y];
						Tile tileRight = Main.tile[X + 1, Y];

						//place ceiling webs
						if (tile.TileType == ModContent.TileType<WebBlock>())
						{
							WorldGen.PlaceObject(X, Y + 1, ModContent.TileType<HangingWeb>(), true, WorldGen.genRand.Next(0, 6));
						}

                        //place stalagmite and stalactite
						if (tile.TileType == ModContent.TileType<DampStone>())
						{
                            if (WorldGen.genRand.NextBool(5))
							{
							    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<DampStalagmite>(), true, WorldGen.genRand.Next(0, 6));
                            }

                            if (WorldGen.genRand.NextBool(5))
							{
							    WorldGen.PlaceObject(X, Y + 1, ModContent.TileType<DampStalactite>(), true, WorldGen.genRand.Next(0, 6));
                            }
						}

						//place ambient tiles that can spawn on stone and grass
						if (tile.TileType == ModContent.TileType<DampGrass>() || tile.TileType == ModContent.TileType<DampStone>())
						{
							//large hanging roots
							if (WorldGen.genRand.NextBool(5))
							{
								WorldGen.PlaceObject(X, Y + 1, ModContent.TileType<HangingRoots>(), true, WorldGen.genRand.Next(0, 2));
							}
						}

						//grass only ambient tiles
						if (tile.TileType == ModContent.TileType<DampGrass>())
						{
							if (WorldGen.genRand.NextBool(4) && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
							{
								GrowGiantRoot(X, Y, ModContent.TileType<GiantRoot>(), 3, 10);
							}

							//mushrooms
							if (WorldGen.genRand.NextBool(15))
							{
								ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<MushroomBlue>(), (ushort)ModContent.TileType<MushroomRedBrown>(),
                                (ushort)ModContent.TileType<MushroomBrown>(), (ushort)ModContent.TileType<MushroomYellow>(), 
                                (ushort)ModContent.TileType<MushroomGreen>(), (ushort)ModContent.TileType<MushroomPurple>(),
                                (ushort)ModContent.TileType<MushroomRed>(), (ushort)ModContent.TileType<MushroomTeal>(), 
                                (ushort)ModContent.TileType<MushroomWhite>() };

								WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Mushrooms), true, WorldGen.genRand.Next(0, 2));
							}

                            //dead trees
                            if (WorldGen.genRand.NextBool(12) && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock &&
                            !tileLeft.LeftSlope && !tileLeft.RightSlope && !tileLeft.IsHalfBlock && 
                            !tileRight.LeftSlope && !tileRight.RightSlope && !tileRight.IsHalfBlock)
							{
								GrowDeadTree(X, Y, 4, 10);
							}

							//grow weeds
							if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
							{
								WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SpiderCaveWeeds>());
								tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(36) * 18);
								WorldGen.SquareTileFrame(X, Y + 1, true);
								if (Main.netMode == NetmodeID.Server)
								{
									NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
								}
							}
						}

						if (Main.tile[X, Y].TileType == ModContent.TileType<DampGrass>() && !Main.tile[X, Y + 1].HasTile)
						{
							if (WorldGen.genRand.NextBool(5))
							{
								WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<DampVinesLight>());
							}
                            if (WorldGen.genRand.NextBool(2))
							{
								WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<DampVines>());
							}
						}

                        int[] ValidTiles = { ModContent.TileType<DampGrass>() };

						if (Main.tile[X, Y].TileType == ModContent.TileType<DampVines>())
						{
							SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<DampVines>(), ValidTiles);
						}
                        if (Main.tile[X, Y].TileType == ModContent.TileType<DampVinesLight>())
                        {
                            SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<DampVinesLight>(), ValidTiles);
                        }
					}
                }
            }
        }

        private void DeleteAnnoyingTraps(GenerationProgress progress, GameConfiguration configuration)
        {
            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int InitialSize = Main.maxTilesY >= 1800 ? 240 : 150;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        WorldGen.KillWire(X, Y);
                        Main.tile[X, Y].LiquidAmount = 0;

                        if (Main.tile[X, Y].TileType == TileID.PressurePlates || Main.tile[X, Y].TileType == TileID.GeyserTrap)
                        {
                            WorldGen.KillTile(X, Y);
                        }

                        if (Main.tile[X, Y].TileType == TileID.Traps)
                        {
                            Main.tile[X, Y].TileType = (ushort)ModContent.TileType<DampGrass>();
                        }
                    }
                }
            }
        }

        public static void CavePatch(int i, int j)
		{
			double SizeX = WorldGen.genRand.Next(75, 125);
			double SizeY = WorldGen.genRand.Next(20, 50);
			double CaveSize = (double)1.52380952381;

			SizeX *= CaveSize;
			SizeY *= CaveSize;
			double SizeYModified = SizeY - 1.0;
			double CaveDigThreshold = SizeX;
			Vector2D val = default(Vector2D);
			val.X = i;
			val.Y = (double)j - SizeY * 0.3;
			Vector2D val2 = default(Vector2D);
			val2.X = (double)WorldGen.genRand.Next(-100, 101) * 0.005;
			val2.Y = (double)WorldGen.genRand.Next(-200, -100) * 0.005;
			while (SizeX > 0.0 && SizeY > 0.0)
			{
				SizeX -= (double)WorldGen.genRand.Next(3);
				SizeY -= 1.0;
				int XMin = (int)(val.X - SizeX * 0.5);
				int XMax = (int)(val.X + SizeX * 0.5);
				int YMin = (int)(val.Y - SizeX * 0.5);
				int YMax = (int)(val.Y + SizeX * 0.5);
				if (XMin < 0)
				{
					XMin = 0;
				}
				if (XMax > Main.maxTilesX)
				{
					XMax = Main.maxTilesX;
				}
				if (YMin < 0)
				{
					YMin = 0;
				}
				if (YMax > Main.maxTilesY)
				{
					YMax = Main.maxTilesY;
				}

				CaveDigThreshold = SizeX * (double)WorldGen.genRand.Next(80, 120) * 0.01;

				for (int k = XMin; k < XMax; k++)
				{
					for (int l = YMin; l < YMax; l++)
					{
						double num10 = Math.Abs((double)k - val.X);
						double num11 = Math.Abs(((double)l - val.Y) * 2.3);
						double num12 = Math.Sqrt(num10 * num10 + num11 * num11);

						//dig out caves
						if (num12 < CaveDigThreshold * 0.2 && (double)l < val.Y)
						{
							if (Main.tile[k, l].HasTile && Main.tile[k, l].TileType == ModContent.TileType<DampSoil>())
							{
								Tile tile = Main.tile[k, l];

                                tile.HasTile = false;
								WorldGen.KillTile(k, l);

								if (Main.tile[k, l].WallType != WallID.SpiderUnsafe)
								{
									WorldGen.KillWall(k, l);
								}
							}
						}
					}
				}

				val += val2;
				val.X += val2.X;
				val2.X += (double)WorldGen.genRand.Next(-110, 110) * 0.005;
				val2.Y -= (double)WorldGen.genRand.Next(110) * 0.005;
				if (val2.X > -0.5 && val2.X < 0.5)
				{
					if (val2.X < 0.0)
					{
						val2.X = -0.5;
					}
					else
					{
						val2.X = 0.5;
					}
				}
				if (val2.X > 0.5)
				{
					val2.X = 0.5;
				}
				if (val2.X < -0.5)
				{
					val2.X = -0.5;
				}
				if (val2.Y > 0.5)
				{
					val2.Y = 0.5;
				}
				if (val2.Y < -0.5)
				{
					val2.Y = -0.5;
				}
				for (int m = 0; m < 2; m++)
				{
					int num13 = (int)val.X + WorldGen.genRand.Next(-20, 20);
					int num14 = (int)val.Y + WorldGen.genRand.Next(0, 20);
					while (!Main.tile[num13, num14].HasTile && Main.tile[num13, num14].TileType != ModContent.TileType<DampSoil>())
					{
						num13 = (int)val.X + WorldGen.genRand.Next(-20, 20);
						num14 = (int)val.Y + WorldGen.genRand.Next(0, 20);
					}
					int num15 = WorldGen.genRand.Next(10, 20);
					int steps = WorldGen.genRand.Next(10, 20);
					WorldGen.TileRunner(num13, num14, num15, steps, ModContent.TileType<DampSoil>(), addTile: false, 0.0, 2.0, noYChange: true, ignoreTileType: TileID.Stone);
				}
			}
		}

        //method to clean up small clumps of tiles
        public static void CleanOutSmallClumps(bool Tiles)
        {
            int cutoffLimit = 200;

            List<ushort> BlockTypes = new()
            {
                (ushort)ModContent.TileType<DampGrass>(),
                (ushort)ModContent.TileType<DampSoil>(),
                (ushort)ModContent.TileType<DampStone>(),
                (ushort)ModContent.TileType<WebBlock>(),
            };
            
            void getAttachedPoints(int x, int y, List<Point> points)
            {
                Tile t = Main.tile[x, y];
                Point p = new(x, y);

                if (!WorldGen.InWorld(x, y))
                {
                    t = new Tile();
                }
                
                if (!BlockTypes.Contains(t.TileType) || !t.HasTile || points.Count > cutoffLimit || points.Contains(p))
                {
                    return;
                }

                points.Add(p);

                getAttachedPoints(x + 1, y, points);
                getAttachedPoints(x - 1, y, points);
                getAttachedPoints(x, y + 1, points);
                getAttachedPoints(x, y - 1, points);
            }

            List<ushort> WallTypes = new()
            {
                (ushort)ModContent.WallType<DampSoilWall>(),
                (ushort)ModContent.WallType<DampGrassWall>(),
                (ushort)ModContent.WallType<WebBlockWall>()
            };

            void getAttachedWallPoints(int x, int y, List<Point> points)
			{
                if (!WorldGen.InWorld(x, y, 10))
                {
                    return;
                }

                Tile tile = Main.tile[x, y];
                Point point = new(x, y);

                if (!WallTypes.Contains(tile.WallType) || points.Count > cutoffLimit || points.Contains(point))
                {
                    return;
                }

                points.Add(point);

                getAttachedWallPoints(x + 1, y, points);
                getAttachedWallPoints(x - 1, y, points);
                getAttachedWallPoints(x, y + 1, points);
                getAttachedWallPoints(x, y - 1, points);
			}

            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int InitialSize = Main.maxTilesY >= 1800 ? 240 : 150;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            for (int X = origin.X - biomeSize - 2; X <= origin.X + biomeSize + 2; X++)
            {
                for (int Y = (int)(origin.Y - verticalRadius * 0.4f) - 3; Y <= origin.Y + verticalRadius + 3; Y++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        if (Tiles)
                        {
                            //clean up tiles
                            List<Point> chunkPoints = new();
                            getAttachedPoints(X, Y, chunkPoints);

                            if (WorldGen.InWorld(X, Y, 10) && chunkPoints.Count >= 1 && chunkPoints.Count < cutoffLimit)
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
                        else
                        {
                            //clean up walls
                            List<Point> WallPoints = new();
                            getAttachedWallPoints(X, Y, WallPoints);

                            if (WorldGen.InWorld(X, Y, 10) && WallPoints.Count >= 1 && WallPoints.Count <= cutoffLimit)
                            {
                                foreach (Point p in WallPoints)
                                {
                                    WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
                                    {
                                        new Actions.ClearWall(true)
                                    }));
                                }
                            }
                        }
                    }
                }
            }
        }

        //determine if theres no snow blocks nearby so the biome doesnt place in the snow biome
        public static bool CanPlaceBiome(int X, int Y)
        {
            Point origin = new Point(startPosX, startPosY);
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            //slightly bigger size than default to account for the area of spider cave walls around the biome
            int InitialSize = Main.maxTilesY >= 1800 ? 260 : 170;
            int biomeSize = InitialSize + (Main.maxTilesX / 180);
            float actualSize = biomeSize * 16f;
            float constant = actualSize * 2f / (float)Math.Sin(angle);

            float biomeSpacing = actualSize * (float)Math.Sin(otherAngle) / (float)Math.Sin(angle);
            int verticalRadius = (int)(constant / 16f);

            Vector2 biomeOffset = Vector2.UnitY * biomeSpacing;
            Vector2 biomeTop = center - biomeOffset;
            Vector2 biomeBottom = center + biomeOffset;

            for (int i = origin.X - biomeSize - 2; i <= origin.X + biomeSize + 2; i++)
            {
                for (int j = (int)(origin.Y - verticalRadius * 0.4f) - 3; j <= origin.Y + verticalRadius + 3; j++)
                {
                    if (CheckInsideOval(new Point(X, Y), biomeTop, biomeBottom, constant, center, out float dist))
                    {
                        int[] InvalidTiles = { TileID.SnowBlock, TileID.IceBlock, TileID.Sandstone, TileID.JungleGrass, TileID.LihzahrdBrick };

                        if (WorldGen.InWorld(i, j) && (InvalidTiles.Contains(Main.tile[i, j].TileType) || Main.tileDungeon[Main.tile[i, j].TileType]))
                        {
                            return false;
                        }
                    }
                }
            }
    
            return true;
        }

        //determine if a giant root can be grown on a set block
        public static bool GrowGiantRoot(int X, int Y, int tileType, int minSize, int maxSize)
        {
            //check a 10 by 10 square for other giant roots before placing
            for (int i = X - 2; i < X + 2; i++)
            {
                for (int j = Y - 2; j < Y + 2; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        return false;
                    }
                }
            }

            GiantRoot.Grow(X, Y + 1, minSize, maxSize);

            return true;
        }

        //determine if a giant root can be grown on a set block
        public static bool GrowDeadTree(int X, int Y, int minSize, int maxSize)
        {
            //check a 10 by 10 square for other giant roots before placing
            for (int i = X - 2; i < X + 2; i++)
            {
                for (int j = Y - 2; j < Y + 2; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<DeadTree1>() ||
                    Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<DeadTree2>() ||
                    Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<DeadTree3>())
                    {
                        return false;
                    }
                }
            }

            switch (WorldGen.genRand.Next(3))
            {
                case 0:
                {
                    DeadTree1.Grow(X, Y - 1, minSize, maxSize, ModContent.TileType<DeadTree1>());
                    break;
                }
                case 1:
                {
                    DeadTree2.Grow(X, Y - 1, minSize, maxSize, ModContent.TileType<DeadTree2>());
                    break;
                }
                case 2:
                {
                    DeadTree3.Grow(X, Y - 1, minSize, maxSize, ModContent.TileType<DeadTree3>());
                    break;
                }
            }

            return true;
        }

        //method to make sure things only generate in the biome's circle
        public static bool CheckInsideOval(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance)
        {
            Vector2 point = tile.ToWorldCoordinates();
            float distY = center.Y - point.Y;

            //squish the circle vertically to create an oval shape
            point.Y -= distY * 2.5f;

            float distance1 = Vector2.Distance(point, focus1);
            float distance2 = Vector2.Distance(point, focus2);
            distance = distance1 + distance2;
            
            return distance <= distanceConstant;
        }

        //worldgenning tasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {  
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Hellforge"));
            if (GenIndex1 == -1) 
            {
                return;
            }

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Spider Grotto", PlaceSpiderCave));

            int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Water Plants"));
            if (GenIndex2 == -1)
            {
                return;
            }

            tasks.Insert(GenIndex2 + 1, new PassLegacy("Spider Grotto Trap Removal", DeleteAnnoyingTraps));
        }

        public override void PostWorldGen()
		{
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

					int[] MainItems = new int[] { ItemID.BandofRegeneration, ItemID.AnkletoftheWind, ItemID.HermesBoots, ItemID.CloudinaBottle, ItemID.Aglet, ItemID.LuckyHorseshoe };

					if (chestTile.TileFrameX == 5 * 36 && MainItems.Contains(chest.item[0].type))
					{
						//potions
						int[] Potions1 = new int[] { ItemID.BattlePotion, ItemID.CratePotion, ItemID.EndurancePotion };

						//more potions
						int[] Potions2 = new int[] { ItemID.LuckPotion, ItemID.ShinePotion, ItemID.LifeforcePotion };

						//recorvery potions
						int[] RecoveryPotions = new int[] { ItemID.HealingPotion, ItemID.ManaPotion };

						//bars
						int[] Bars = new int[] { ItemID.GoldBar, ItemID.PlatinumBar };

						//bars
						chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
						chest.item[1].stack = WorldGen.genRand.Next(5, 16);
                        //torches
                        chest.item[2].SetDefaults(ModContent.ItemType<SpiderBiomeTorchItem>());
						chest.item[2].stack = WorldGen.genRand.Next(3, 8);
						//potions
						chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions1));
						chest.item[3].stack = WorldGen.genRand.Next(1, 3);
						//even more potions
						chest.item[4].SetDefaults(WorldGen.genRand.Next(Potions2));
						chest.item[4].stack = WorldGen.genRand.Next(1, 3);
						//recovery potions
						chest.item[5].SetDefaults(WorldGen.genRand.Next(RecoveryPotions));
						chest.item[5].stack = WorldGen.genRand.Next(3, 7);
						//goodie bags
						chest.item[6].SetDefaults(ItemID.GoodieBag);
						chest.item[6].stack = WorldGen.genRand.Next(1, 3);
						//gold coins
						chest.item[7].SetDefaults(ItemID.GoldCoin);
						chest.item[7].stack = WorldGen.genRand.Next(1, 3);
					}
				}
            }
        }
    }
}