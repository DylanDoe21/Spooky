using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.NPCs.Friendly;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell.Ambient;
using Spooky.Content.Tiles.SpookyHell.Furniture;
using Spooky.Content.Tiles.SpookyHell.Tree;

using StructureHelper;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Generation
{
    public class SpookyHell : ModSystem
    {
        static int StartPosition = (GenVars.JungleX < Main.maxTilesX / 2) ? 70 : Main.maxTilesX - (Main.maxTilesX / 5) - 80;
        static int BiomeEdge = StartPosition + (Main.maxTilesX / 5);

        private void GenerateSpookyHell(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.EyeValley").Value;

            //set these to their intended values again just to be safe
            StartPosition = (GenVars.JungleX < Main.maxTilesX / 2) ? 70 : Main.maxTilesX - (Main.maxTilesX / 5) - 80;
            BiomeEdge = StartPosition + (Main.maxTilesX / 5);

            //extra clear width depending on the side of the world its on
            int extraClearStart = (GenVars.JungleX < Main.maxTilesX / 2) ? 50 : 0;
            int extraClearEnd = (GenVars.JungleX > Main.maxTilesX / 2) ? 50 : 0;

            //clear everything in the area the biome generates in to prevent unwanted collisions with obsidian houses or lava flooding
            for (int X = StartPosition - extraClearStart; X <= BiomeEdge + extraClearEnd; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y < Main.maxTilesY - 15; Y++)
                {
                    Tile tile = Main.tile[X, Y];

                    tile.ClearEverything();
                    WorldGen.KillWall(X, Y);
                }
            }

            //generate the surface
            int width = BiomeEdge;
            int height = Main.maxTilesY - 150;

            int[] terrainContour = new int[width * height];

            double rand1 = WorldGen.genRand.NextDouble() + 1;
            double rand2 = WorldGen.genRand.NextDouble() + 2;
            double rand3 = WorldGen.genRand.NextDouble() + 3;

            float peakheight = 10;
            float flatness = 50;
            int offset = Main.maxTilesY - 130;

            //genrate a random wave
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                double BiomeHeight = peakheight / rand1 * Math.Sin((float)X / flatness * rand1 + rand1);
                BiomeHeight += peakheight / rand2 * Math.Sin((float)X / flatness * rand2 + rand2);
                BiomeHeight += peakheight / rand3 * Math.Sin((float)X / flatness * rand3 + rand3);

                BiomeHeight += offset;

                terrainContour[X] = (int)BiomeHeight;
            }

            //place the randomized wave of blocks, with walls behind them
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY - 6; Y++)
                {
                    if (Y > terrainContour[X] && WorldGen.InWorld(X, Y))
                    {
                        WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<SpookyMush>());
                        Main.tile[X, Y + 5].WallType = (ushort)ModContent.WallType<SpookyMushWall>();
                    }
                }
            }

            //place clumps of blocks along the edges of the biome so it transitions nicely
            for (int X = StartPosition - 50; X <= StartPosition; X++)
            {
                for (int Y = Main.maxTilesY - 110; Y < Main.maxTilesY - 20; Y++)
                {
                    if (WorldGen.genRand.NextBool(30))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), 0, WorldGen.genRand.Next(5, 20), true, true);
                    }
                }
            }
            for (int X = BiomeEdge; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 110; Y < Main.maxTilesY - 20; Y++)
                {
                    if (WorldGen.genRand.NextBool(30))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), 0, WorldGen.genRand.Next(5, 20), true, true);
                    }
                }
            }

            //place ceiling of blocks across the top of the biome
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 215; Y <= Main.maxTilesY - 192; Y++)
                {
                    if (WorldGen.genRand.NextBool(15))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), 0, WorldGen.genRand.Next(5, 7), true, true);
                    }
                }
            }

            //place clumps of eye blocks throughout the biome
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 1E-05); i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface + 100, Main.maxTilesY - 15);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Y >= Main.maxTilesY - 160)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMush>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(45, 65), WorldGen.genRand.Next(45, 65), 
                        ModContent.TileType<EyeBlock>(), false, 0f, 0f, false, true);
                    }
                }
            }
        }

        private void RoughenUpTerrain(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int roughPass = 0; roughPass < 2; roughPass++)
            {
                int lastMaxTileX = 0;
                int lastXRadius = 0;

                for (int i = StartPosition - 50; i <= BiomeEdge + 50; i++)
                {
                    // flat ellipses
                    if (WorldGen.genRand.Next(0, 40) == 0 && i > lastMaxTileX + lastXRadius)
                    {
                        int roughingPosition = 0;
                        // Look for a Y position to put ellipses
                        for (int lookupY = Main.maxTilesY - 150; lookupY <= Main.maxTilesY - 6; lookupY++)
                        {
                            if (Framing.GetTileSafely(i, lookupY).HasTile)
                            {
                                roughingPosition = lookupY;
                                break;
                            }
                        }
                        // Create random-sized boxes in which our ellipses will be carved
                        int radiusX;
                        int radiusY;
                        switch (roughPass)
                        {
                            // This is for the big, ellipses
                            case 0:
                                radiusX = WorldGen.genRand.Next(8, 18);
                                radiusY = WorldGen.genRand.Next(4, 10);
                                break;
                            case 1:
                                radiusX = WorldGen.genRand.Next(8, 15);
                                radiusY = WorldGen.genRand.Next(2, 4);
                                break;
                            default:
                                radiusX = 0;
                                radiusY = 0;
                                break;
                        }

                        int minTileX = i - radiusX;
                        int maxTileX = i + radiusX;
                        int minTileY = roughingPosition - radiusY;
                        int maxTileY = roughingPosition + radiusY;

                        // Calculate diameter and center of ellipse based on the boundaries specified
                        int diameterX = Math.Abs(minTileX - maxTileX);
                        int diameterY = Math.Abs(minTileY - maxTileY);
                        float centerX = (minTileX + maxTileX - 1) / 2f;
                        float centerY = (minTileY + maxTileY - 1) / 2f;

                        // Make the ellipse
                        for (int ellipseTileX = minTileX; ellipseTileX < maxTileX; ellipseTileX++)
                        {
                            for (int ellipseTileY = minTileY; ellipseTileY < maxTileY; ellipseTileY++)
                            {
                                // This is the equation for the unit ellipse; we're dividing by squares of the diameters to scale along the axes
                                if
                                (
                                    (
                                        Math.Pow(ellipseTileX - centerX, 2) / Math.Pow(diameterX / 2, 2))
                                        + (Math.Pow(ellipseTileY - centerY, 2) / Math.Pow(diameterY / 2, 2)
                                    ) <= 1
                                )
                                {
                                    if (ellipseTileX < Main.maxTilesX && ellipseTileY < Main.maxTilesY && ellipseTileX >= 0 && ellipseTileY >= 0)
                                    {
                                        Main.tile[ellipseTileX, ellipseTileY].ClearEverything();
                                        WorldGen.KillWall(ellipseTileX + 1, ellipseTileY + 1);
                                        WorldGen.KillWall(ellipseTileX - 1, ellipseTileY + 1);
                                        WorldGen.KillWall(ellipseTileX, ellipseTileY + 1);
                                        WorldGen.KillWall(ellipseTileX, ellipseTileY - 1);
                                    }
                                }
                            }
                            for (int ellipseTileY = minTileY - 20; ellipseTileY < maxTileY - diameterY / 2; ellipseTileY++)
                            {
                                if (ellipseTileX < Main.maxTilesX && ellipseTileY < Main.maxTilesY && ellipseTileX >= 0 && ellipseTileY >= 0)
                                {
                                    Main.tile[ellipseTileX, ellipseTileY].ClearEverything();
                                    WorldGen.KillWall(ellipseTileX + 1, ellipseTileY + 1);
                                    WorldGen.KillWall(ellipseTileX - 1, ellipseTileY + 1);
                                    WorldGen.KillWall(ellipseTileX, ellipseTileY + 1);
                                    WorldGen.KillWall(ellipseTileX, ellipseTileY - 1);
                                }
                            }
                        }
                        lastMaxTileX = maxTileX;
                        lastXRadius = diameterX / 2;
                    }
                }
            }
        }

        private void ErodeEdges(GenerationProgress progress, GameConfiguration configuration)
        {
            int numPasses = 2; // clear edges twice
            for (int clearPass = 0; clearPass < numPasses; clearPass++)
            {
                List<Point> list = new();
                for (int tileX = StartPosition - 50; tileX <= BiomeEdge + 50; tileX++)
                {
                    for (int tileY = Main.maxTilesY - 200; tileY <= Main.maxTilesY - 6; tileY++)
                    {
                        // check if there's a tile, so it won't check all the surrounding tiles for nothing
                        if (Main.tile[tileX, tileY].HasTile)
                        {
                            // if there are some tiles below..
                            bool tilesBelow = Main.tile[tileX, tileY + 1].HasTile &&
                                              Main.tile[tileX, tileY + 2].HasTile;

                            // ..and no tiles above
                            bool tilesAbove = Main.tile[tileX - 1, tileY - 1].HasTile &&
                                              Main.tile[tileX, tileY - 1].HasTile &&
                                              Main.tile[tileX + 1, tileY - 1].HasTile;

                            // check if there are tiles on the right and right-down
                            bool tilesRight = Main.tile[tileX + 1, tileY].HasTile &&
                                              Main.tile[tileX + 1, tileY + 1].HasTile;

                            // check if there are tiles on the left and left-down
                            bool tilesLeft = Main.tile[tileX - 1, tileY].HasTile &&
                                             Main.tile[tileX - 1, tileY + 1].HasTile;

                            // add the tile to a list if there are tiles on one side but not the other (XOR)
                            if (tilesBelow && !tilesAbove && (tilesRight ^ tilesLeft))
                                list.Add(new Point(tileX, tileY));

                            break; // so it won't find a tile somewhere below the surface
                        }
                    }
                }

                foreach (Point p in list)
                {
                    // clear the found tile 
                    Main.tile[p.X, p.Y].ClearEverything();
                    WorldGen.KillWall(p.X + 1, p.Y);
                    WorldGen.KillWall(p.X - 1, p.Y);
                    WorldGen.KillWall(p.X, p.Y + 1);
                    WorldGen.KillWall(p.X, p.Y - 1);
                }
            }
        }

        private void CreateWallPillars(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition + 50; X <= BiomeEdge - 50; X++)
            {
                if (WorldGen.genRand.NextBool(75))
                {
                    PlaceWallPillar(X);

                    X += 20;
                }
            }
        }

        public static void PlaceWallPillar(int StartPosition)
        {
            bool HasPlacedPlatform = false;

            for (int Y = Main.maxTilesY - 190; Y <= Main.maxTilesY - 6; Y += 5)
            {
                //place platforms sometimes
                if (Y > Main.maxTilesY - 160 && Y < Main.maxTilesY - 20 && WorldGen.SolidTile(StartPosition, Y) && !HasPlacedPlatform)
                {
                    PlacePlatform(StartPosition + WorldGen.genRand.Next(-5, 5), Y - WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(28, 36), 17, ModContent.TileType<SpookyMush>());
                    HasPlacedPlatform = true;
                }

                if (WorldGen.SolidTile(StartPosition, Y))
                {
                    SpookyWorldMethods.PlaceCircle(StartPosition + 10, Y, -1, ModContent.WallType<SpookyMushWall>(), 15, false, false);
                    SpookyWorldMethods.PlaceCircle(StartPosition - 10, Y, -1, ModContent.WallType<SpookyMushWall>(), 15, false, false);
                }

                SpookyWorldMethods.PlaceCircle(StartPosition + WorldGen.genRand.Next(-5, 6), Y, -1, ModContent.WallType<SpookyMushWall>(), 15, false, false);
            }
        }

        public static void PlacePlatform(int X, int Y, int Width, int Height, int TileType)
		{
			bool HasSmoothed = false;

			int smoothedSidesForBottomRowsLeft = 0;
			int smoothedSidesForBottomRowsRight = 0;

			int topLeftX = X - Width / 2;
			int height = Height;

			for (int j = 0; j < height; j++)
			{
				int y = Y + j;

				//Top two rows are fully placed
				if (j >= 2)
				{
					HasSmoothed = true;
					//The more rows the more tiles are "smoothed" away
					smoothedSidesForBottomRowsLeft += WorldGen.genRand.Next(0, 2);
					smoothedSidesForBottomRowsRight += WorldGen.genRand.Next(0, 2);
				}

				for (int i = 0; i < Width; i++)
				{
					int x = topLeftX + i;

					//Don't generate dirt if the range forbids it (that way it smooths the bottom corners)
					if (!HasSmoothed || (i > smoothedSidesForBottomRowsLeft && i < Width - smoothedSidesForBottomRowsRight))
					{
						WorldGen.PlaceTile(x, y, TileType, mute: true, forced: true);
					}

                    if (!HasSmoothed)
                    {
                        SpookyWorldMethods.PlaceCircle(x, y, ModContent.TileType<SpookyMush>(), 0, WorldGen.genRand.Next(1, 3), false, false);
                    }
				}
			}
        }

        private void SpreadSpookyHellGrass(GenerationProgress progress, GameConfiguration configuration)
        {
            //spread grass on all mush tiles
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 250; Y < Main.maxTilesY - 15; Y++)
                {
                    Tile up = Main.tile[X, Y - 1];
                    Tile down = Main.tile[X, Y + 1];
                    Tile left = Main.tile[X - 1, Y];
                    Tile right = Main.tile[X + 1, Y];

                    if (Main.tile[X, Y].HasTile)
                    {
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMush>() &&
                        ((!up.HasTile || up.TileType == TileID.Trees) || !down.HasTile || !left.HasTile || !right.HasTile))
                        {
                            Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyMushGrass>();
                        }
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMush>() &&
                    (up.TileType == ModContent.TileType<SpookyMushGrass>() || down.TileType == ModContent.TileType<SpookyMushGrass>() || 
                    left.TileType == ModContent.TileType<SpookyMushGrass>() || right.TileType == ModContent.TileType<SpookyMushGrass>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyMush>(), ModContent.TileType<SpookyMushGrass>(), false);
                    }
                }
            }
        }

        public static void SpookyHellTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 155; Y < Main.maxTilesY - 120; Y++)
                {
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() && CanPlaceTree(X, Y))
                    {
                        if (WorldGen.genRand.NextBool(20) && (Main.tile[X, Y - 1].WallType <= 0 || Main.tile[X, Y - 1].WallType == ModContent.WallType<SpookyMushWall>()) && 
                        !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                        {
                            EyeTree.Grow(X, Y - 1, 12, 35, false);
                        }
                    }

                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>() && CanPlaceTree(X, Y))
                    {
                        if (WorldGen.genRand.NextBool(10) && (Main.tile[X, Y - 1].WallType <= 0 || Main.tile[X, Y - 1].WallType == ModContent.WallType<SpookyMushWall>()) && 
                        !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                        {
                            EyeTree.Grow(X, Y - 1, 12, 35, false);
                        }
                    }
                }
            }
        }

        public static bool CanPlaceTree(int X, int Y)
        {
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<EyeTree>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void SpookyHellAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X < BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y < Main.maxTilesY - 15; Y++)
                {
                    //eye vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMushGrass>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(8))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<EyeVine>());
                        }
                    }
                    if (Main.tile[X, Y].TileType == ModContent.TileType<EyeVine>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<EyeVine>());
                    }

                    //plants that can grow on both blocks
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() || Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        //eye stalks
                        if (WorldGen.genRand.NextBool(30))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkThin>(), (ushort)ModContent.TileType<EyeStalkThinTall>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(35))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkSmall1>(), (ushort)ModContent.TileType<EyeStalkSmall2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(40))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkMedium1>(), (ushort)ModContent.TileType<EyeStalkMedium2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(45))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkBig1>(), (ushort)ModContent.TileType<EyeStalkBig2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                        if (WorldGen.genRand.NextBool(50))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkGiant1>(), (ushort)ModContent.TileType<EyeStalkGiant2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }

                        //bones
                        if (WorldGen.genRand.NextBool(40))
                        {
                            ushort[] Bones = new ushort[] { (ushort)ModContent.TileType<Bone1>(), (ushort)ModContent.TileType<Bone2>(), (ushort)ModContent.TileType<Bone3>(), (ushort)ModContent.TileType<Bone4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Bones), true);
                        }

                        //arteries
                        if (WorldGen.genRand.NextBool(12))
                        {
                            ushort[] Arteries = new ushort[] { (ushort)ModContent.TileType<Artery1>(), (ushort)ModContent.TileType<Artery2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Arteries), true);
                        }
                        if (WorldGen.genRand.NextBool(15))
                        {
                            ushort[] Arteries = new ushort[] { (ushort)ModContent.TileType<ArteryHanging1>(), (ushort)ModContent.TileType<ArteryHanging2>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(Arteries), true);
                        }
                    }

                    //mush grass plants
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>())
                    {
                        //purple stalks
                        if (WorldGen.genRand.NextBool(25))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<StalkRed1>(), (ushort)ModContent.TileType<StalkRed2>(), (ushort)ModContent.TileType<StalkRed3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }

                        //ambient manhole teeth
                        if (WorldGen.genRand.NextBool(20))
                        {
                            WorldGen.PlaceObject(X, Y - 1, (ushort)ModContent.TileType<Tooth>(), true);
                        }
                    }

                    //eye block plants
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        //red stalks
                        if (WorldGen.genRand.NextBool(25))
                        {
                            ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<StalkPurple1>(), (ushort)ModContent.TileType<StalkPurple2>(), (ushort)ModContent.TileType<StalkPurple3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Stalks), true);
                        }
                    }
                }
            }
        }

        private void SpookyHellPolish(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X < BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 230; Y < Main.maxTilesY - 15; Y++)
                {
                    //get rid of any other left over lava
                    if (Main.tile[X, Y].LiquidType == LiquidID.Lava && !Main.tile[X, Y].HasTile)
                    {
                        Tile newTile = Main.tile[X, Y];
                        newTile.ClearEverything();
                    }

                    //get rid of random floating singular tiles
                    if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile &&
                    !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                    {
                        WorldGen.KillTile(X, Y);
                    }

                    //slope tiles
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<ValleyStone>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<LivingFlesh>() ||
                    Main.tile[X, Y].TileType == TileID.ObsidianBrick)
                    {
                        Tile.SmoothSlope(X, Y);
                    }
                }
            }
        }

        public void GenerateStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            /*
            //define the center of the biome
            int XMiddle = (StartPosition + BiomeEdge) / 2;

            int StartPosY = Main.maxTilesY - 10;

            //place first flesh pillar
            GenerateStructure(StartPosition + 135, StartPosY, "FleshPillar-1", 16, 32);

            ///place little eye's house
            int HouseX = (GenVars.JungleX > Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 : (XMiddle + BiomeEdge) / 2;
            GenerateStructure(HouseX, StartPosY, "LittleEyeHouse", 23, 18);

            //place second flesh pillar
            GenerateStructure(XMiddle - 135, StartPosY, "FleshPillar-2", 15, 38);

            //place orroboro nest
            GenerateStructure(XMiddle, StartPosY, "OrroboroNest", 6, 16);

            //place third flesh pillar
            GenerateStructure(XMiddle + 135, StartPosY, "FleshPillar-3", 15, 29);

            //anything bigger than small worlds
            if (Main.maxTilesX >= 6400)
            {
                //place moco shrine
                int ShrineX = (GenVars.JungleX < Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 - 45 : (XMiddle + BiomeEdge) / 2 - 45;
                GenerateStructure(ShrineX, StartPosY, "MocoShrine", 19, 18);

                //place blood lake
                int LakeX = (GenVars.JungleX < Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 + 75 : (XMiddle + BiomeEdge) / 2 + 75;
                GenerateStructure(LakeX, StartPosY, "BloodLake", 47, 25);
            }
            //small worlds
            else
            {
                //place moco shrine
                int ShrineX = (GenVars.JungleX < Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 - 15 : (XMiddle + BiomeEdge) / 2 - 5;
                GenerateStructure(ShrineX, StartPosY, "MocoShrine", 19, 18);

                //place blood lake
                int LakeX = (GenVars.JungleX < Main.maxTilesX / 2) ? XMiddle - 360 : XMiddle + 360;
                GenerateStructure(LakeX, StartPosY, "BloodLake", 47, 25);
            }

            //place fourth flesh pillar
            GenerateStructure(BiomeEdge - 135, StartPosY, "FleshPillar-4", 15, 39);

            //lock all monster chests
            for (int X = StartPosition - 50; X < BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 180; Y < Main.maxTilesY - 15; Y++)
                {
                    //check for the top left frame of the chest
                    if (Main.tile[X, Y].TileType == ModContent.TileType<EyeChest>() && //top left
                    Main.tile[X + 1, Y].TileType == ModContent.TileType<EyeChest>() && //top right
                    Main.tile[X, Y + 1].TileType == ModContent.TileType<EyeChest>() && //bottom left
                    Main.tile[X + 1, Y + 1].TileType == ModContent.TileType<EyeChest>()) //bottom right
                    {
                        //top left
                        Main.tile[X, Y].TileFrameX = 36;
                        Main.tile[X, Y].TileFrameY = 0;

                        //top right
                        Main.tile[X + 1, Y].TileFrameX = 18 + 36;
                        Main.tile[X + 1, Y].TileFrameY = 0;

                        //bottom left
                        Main.tile[X, Y + 1].TileFrameX = 36;
                        Main.tile[X, Y + 1].TileFrameY = 18;

                        //bottom right
                        Main.tile[X + 1, Y + 1].TileFrameX = 18 + 36;
                        Main.tile[X + 1, Y + 1].TileFrameY = 18;
                    }
                }
            }
            */
        }

        //method for finding a valid surface and placing the structure on it
        public void GenerateStructure(int startX, int startY, string StructureFile, int offsetX, int offsetY)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                while (WorldGen.SolidTile(startX, startY) && startY >= Main.maxTilesY - 200)
				{
					startY--;
				}
                if (WorldGen.SolidTile(startX, startY))
                {
					continue;
                }

                Vector2 origin = new Vector2(startX - offsetX, startY - offsetY);
                Generator.GenerateStructure("Content/Structures/SpookyHell/" + StructureFile, origin.ToPoint16(), Mod);

                if (StructureFile == "LittleEyeHouse")
                {
                    NPC.NewNPC(null, (startX - 1) * 16, (startY - 5) * 16, ModContent.NPCType<LittleEyeSleeping>(), 0, 0f, 0f, 0f, 0f, 255);
                }

                placed = true;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Eye Valley", GenerateSpookyHell));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("Eye Valley", RoughenUpTerrain));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("Eye Valley", ErodeEdges));
            tasks.Insert(GenIndex1 + 4, new PassLegacy("Eye Valley Wall Pillars", CreateWallPillars));
            tasks.Insert(GenIndex1 + 5, new PassLegacy("Eye Valley Structures", GenerateStructures));
            tasks.Insert(GenIndex1 + 6, new PassLegacy("Eye Valley Grass", SpreadSpookyHellGrass));
            tasks.Insert(GenIndex1 + 7, new PassLegacy("Eye Valley Trees", SpookyHellTrees));
            tasks.Insert(GenIndex1 + 8, new PassLegacy("Eye Valley Polish", SpookyHellPolish));
            tasks.Insert(GenIndex1 + 9, new PassLegacy("Eye Valley Ambient Tiles", SpookyHellAmbience));
        }
    }
}