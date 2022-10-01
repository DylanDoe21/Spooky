using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.IO;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell.Ambient;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Generation
{
    public class SpookyHell : ModSystem
    {
        static int StartPosition = 0;

        //clear area for the biome to generate in
        private void ClearArea(GenerationProgress progress, GameConfiguration configuration)
        {
            //choose x-coordinate start position of the biome based on the same side of the dungeon
            if (WorldGen.dungeonSide == 1)
			{
                StartPosition = Main.maxTilesX - 750;
			}
			else
			{
                StartPosition = 0;
			}

            int XStart = StartPosition;
            int XEdge = XStart + 750;

            //clear all blocks and lava in the area
            for (int X = XStart; X <= XEdge; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY; Y++)
                {
                    Tile newTile = Main.tile[X, Y];
                    
                    if (Main.tile[X, Y].HasTile || Main.tile[X, Y].LiquidType == LiquidID.Lava) 
                    {
                        newTile.ClearEverything();
                        WorldGen.KillWall(X, Y);
                    }
                }
            }
        }

        private void GenerateSpookyHell(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating the living hell";

            int XStart = StartPosition;
            int XEdge = XStart + 750;

            //generate the surface
            int width = XEdge;
            int height = Main.maxTilesY - 150;

            int[] terrainContour = new int[width * height];

            double rand1 = WorldGen.genRand.NextDouble() + 1;
            double rand2 = WorldGen.genRand.NextDouble() + 2;
            double rand3 = WorldGen.genRand.NextDouble() + 3;

            float peakheight = 10;
            float flatness = 50;
            int offset = Main.maxTilesY - 140;

            for (int X = StartPosition; X <= XEdge; X++)
            {
                double BiomeHeight = peakheight / rand1 * Math.Sin((float)X / flatness * rand1 + rand1);
                BiomeHeight += peakheight / rand2 * Math.Sin((float)X / flatness * rand2 + rand2);
                BiomeHeight += peakheight / rand3 * Math.Sin((float)X / flatness * rand3 + rand3);

                BiomeHeight += offset;

                terrainContour[X] = (int)BiomeHeight;
            }

            for (int X = StartPosition; X <= XEdge; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y <= Main.maxTilesY - 6; Y++)
                {
                    if (Y > terrainContour[X])
                    {
                        WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<SpookyMush>());
                        Main.tile[X, Y + 5].WallType = (ushort)ModContent.WallType<SpookyMushWall>();
                    }
                }
            }

            //place clumps of blocks along the edge of the biome so it doesnt look weird
            if (Terraria.Main.dungeonX > Main.maxTilesX / 2)
			{
                for (int X = XStart - 50; X <= XStart; X++)
                {
                    for (int Y = Main.maxTilesY - 135; Y <= Main.maxTilesY; Y++)
                    {
                        if (WorldGen.genRand.Next(30) == 0)
                        {
                            SpookyWorldMethods.Circle(X, Y, WorldGen.genRand.Next(5, 8), (ushort)ModContent.TileType<SpookyMush>(), false);
                        }
                    }
                }
			}
			else
			{
                for (int X = XEdge; X <= XEdge + 50; X++)
                {
                    for (int Y = Main.maxTilesY - 135; Y <= Main.maxTilesY; Y++)
                    {
                        if (WorldGen.genRand.Next(30) == 0)
                        {
                            SpookyWorldMethods.Circle(X, Y, WorldGen.genRand.Next(5, 8), (ushort)ModContent.TileType<SpookyMush>(), false);
                        }
                    }
                }
			}

            //place ceiling across the top of the biome
            for (int X = XStart; X <= XEdge; X++)
            {
                for (int Y = Main.maxTilesY - 215; Y <= Main.maxTilesY - 185; Y++)
                {
                    if (WorldGen.genRand.Next(50) == 0)
                    {
                        SpookyWorldMethods.Circle(X, Y, WorldGen.genRand.Next(5, 10), (ushort)ModContent.TileType<SpookyMush>(), false);
                    }
                }
            }

            //generate caves
            for (int X = XStart + 50; X <= XEdge - 50; X++)
            {
                for (int Y = Main.maxTilesY - 120; Y <= Main.maxTilesY - 40; Y++)
                {
                    if (WorldGen.genRand.Next(300) == 0)
                    {
                        SpookyWorldMethods.Circle(X, Y, WorldGen.genRand.Next(5, 10), 0, true);
                    }
                }
            }

            //place clumps of eye blocks
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 1E-05); i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface + 100, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMush>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(45, 65), WorldGen.genRand.Next(45, 65), 
                        ModContent.TileType<EyeBlock>(), false, 0f, 0f, false, true);
                    }
                }
            }
        }

        public static void SpookyHellTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Growing eye stalks";

            int XStart = StartPosition;
            int XEdge = XStart + 750;

            for (int X = XStart + 20; X < XEdge - 20; X++)
            {
                for (int Y = Main.maxTilesY - 220; Y <= Main.maxTilesY; Y++)
                {
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        if (WorldGen.genRand.Next(5) == 0)
                        {
                            WorldGen.GrowTree(X, Y - 1);
                        }
                    }
                }
            }
        }

        private void SpookyHellAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = StartPosition;
            int XEdge = XStart + 750;

            if (Terraria.Main.dungeonX > Main.maxTilesX / 2)
			{
                XStart = StartPosition - 50;
                XEdge = XStart + 750;
			}
			else
			{
                XStart = StartPosition;
                XEdge = XStart + 800;
			}

            for (int X = XStart + 20; X < XEdge - 20; X++)
            {
                for (int Y = Main.maxTilesY - 250; Y <= Main.maxTilesY; Y++)
                {
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>())
                    {
                        //weeds
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            ushort[] Weeds = new ushort[] { (ushort)ModContent.TileType<LivingWeed1>(), (ushort)ModContent.TileType<LivingWeed2>(), 
                            (ushort)ModContent.TileType<LivingWeed3>(), (ushort)ModContent.TileType<LivingWeed4>(), (ushort)ModContent.TileType<LivingWeed5>(), 
                            (ushort)ModContent.TileType<LivingWeed6>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Weeds));    
                        }

                        //teeth plants
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            ushort[] Teeth = new ushort[] { (ushort)ModContent.TileType<Tooth1>(), (ushort)ModContent.TileType<Tooth2>(), 
                            (ushort)ModContent.TileType<Tooth3>(), (ushort)ModContent.TileType<Tooth4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Teeth));    
                        }

                        //hair follicles
                        if (WorldGen.genRand.Next(6) == 0)
                        {
                            ushort[] Follicles = new ushort[] { (ushort)ModContent.TileType<Follicle1>(), (ushort)ModContent.TileType<Follicle2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Follicles));    
                        }

                        //hanging follicles
                        if (WorldGen.genRand.Next(6) == 0)
                        {
                            ushort[] HangingFollicle = new ushort[] { (ushort)ModContent.TileType<FollicleHanging1>(), (ushort)ModContent.TileType<FollicleHanging2>() };

                            WorldGen.PlaceObject(X, Y + 3, WorldGen.genRand.Next(HangingFollicle));    
                        }

                        //hanging follicles
                        if (WorldGen.genRand.Next(12) == 0)
                        {
                            WorldGen.PlaceObject(X, Y + 4, ModContent.TileType<HangingTongue>());    
                        }
                    }

                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        //eye bushes
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            ushort[] EyeBushes = new ushort[] { (ushort)ModContent.TileType<EyeBush1>(), (ushort)ModContent.TileType<EyeBush2>(), 
                            (ushort)ModContent.TileType<EyeBush3>(), (ushort)ModContent.TileType<EyeBush4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeBushes));    
                        }

                        //exposed nerves
                        if (WorldGen.genRand.Next(3) == 0)
                        {
                            ushort[] Nerves = new ushort[] { (ushort)ModContent.TileType<ExposedNerve1>(), (ushort)ModContent.TileType<ExposedNerve2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Nerves));    
                        }

                        //fingers
                        if (WorldGen.genRand.Next(5) == 0)
                        {
                            ushort[] Fingers = new ushort[] { (ushort)ModContent.TileType<Finger1>(), (ushort)ModContent.TileType<Finger2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Fingers));    
                        }

                        //hanging fingers
                        if (WorldGen.genRand.Next(5) == 0)
                        {
                            ushort[] HangingFinger = new ushort[] { (ushort)ModContent.TileType<FingerHanging1>(), (ushort)ModContent.TileType<FingerHanging2>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(HangingFinger));    
                        }
                    }
                }
            }
        }

        private void SpookyHellPolish(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = StartPosition;
            int XEdge = XStart + 750;

            for (int X = XStart + 5; X < XEdge - 5; X++)
            {
                for (int Y = Main.maxTilesY - 180; Y < Main.maxTilesY; Y++)
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
                        if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>())
                        {
                            WorldGen.KillTile(X, Y);
                        }
                    }

                    //slope tiles
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<Carapace>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>() ||
                    Main.tile[X, Y].TileType == TileID.IridescentBrick)
                    {
                        Tile.SmoothSlope(X, Y);
                    }
                }
            }
        }

        private void PlaceStructures(int X, int Y, int[,] BlocksArray, int[,] ObjectArray)
        {
            for (int PlaceX = 0; PlaceX < BlocksArray.GetLength(1); PlaceX++)
            {
                for (int PlaceY = 0; PlaceY < BlocksArray.GetLength(0); PlaceY++)
                {
                    int StructureX = X + PlaceX;
                    int StructureY = Y + PlaceY;

                    if (WorldGen.InWorld(StructureX, StructureY, 30))
                    {
                        Tile tile = Framing.GetTileSafely(StructureX, StructureY);
                        switch (BlocksArray[PlaceY, PlaceX])
                        {
                            //dont touch
                            case 0:
                            {
                                break;
                            }
                            //clear only tiles
                            case 1:
                            {
                                tile.ClearTile();
                                break;
                            }
                            //clear everything
                            case 2:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
                                break;
                            }
                            //spooky mush
                            case 3:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyMush>());
                                break;
                            }
                            //carapace blocks
                            case 4:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<Carapace>());
                                break;
                            }
                            //iridescent bricks
                            case 5:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.IridescentBrick);
                                break;
                            }
                            //eye blocks
                            case 6:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<EyeBlock>());
                                break;
                            }
                            //carapace block but it destroys walls too (specifically for the egg nest)
                            case 7:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<Carapace>());
                                break;
                            }
                        }
                    }
                }
            }

            for (int PlaceX = 0; PlaceX < ObjectArray.GetLength(1); PlaceX++)
            {
                for (int PlaceY = 0; PlaceY < ObjectArray.GetLength(0); PlaceY++)
                {
                    int StructureX = X + PlaceX;
                    int StructureY = Y + PlaceY;

                    if (WorldGen.InWorld(StructureX, StructureY, 30))
                    {
                        Tile tile = Framing.GetTileSafely(StructureX, StructureY);
                        switch (ObjectArray[PlaceY, PlaceX])
                        {
                            //dont touch
                            case 0:
                            {
                                break;
                            }
                            //big egg
                            case 1:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, ModContent.TileType<OrroboroEgg>(), true);
                                break;
                            }
                            //necromancy banner
                            case 2:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, TileID.Banners, true, 11);
                                break;
                            }
                            //alchemy lantern
                            case 3:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, TileID.HangingLanterns, true, 4);
                                break;
                            }
                            //bone campfire
                            case 4:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, TileID.Campfire, true, 7);
                                break;
                            }
                            //chest
                            case 5:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest>(), true, 1);
                                break;
                            }
                            //chest
                            case 6:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest2>(), true, 1);
                                break;
                            }
                            //chest
                            case 7:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest3>(), true, 1);
                                break;
                            }
                            //chest
                            case 8:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest4>(), true, 1);
                                break;
                            }
                            //nose shrine
                            case 9:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, (ushort)ModContent.TileType<NoseShrine>(), true);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void GenerateStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            //tiles
            //0 = dont touch
            //1 = clear only tiles
            //2 = clear everything
            //3 = spooky mush
            //4 = carapace block
            //5 = iridescent bricks
            //6 = eye blocks
            //carapace block but also destroys walls (specifically for the egg nest)

            //objects
            //0 = dont touch
            //1 = orroboro egg
            //2 = necromancy banner
            //3 = alchemy lantern
            //4 = bone campfire
            //5-8 = chest
            //9 = nose shrine

            int[,] BoneNestShape = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,0,0,0,0,0,0,0,0,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,0,2,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,2,2,2,0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,0},
                {0,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,2,2,2,2,2,2,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,0},
                {0,0,0,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,7,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,2,2,2,2,2,2,2,2,2,2,0,0,0},
                {0,0,2,2,2,2,2,2,2,2,7,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,2,2,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,7,2,2,2,2,2,2,2,2,2,2,2,2,2,7,2,2,2,2,2,2,2,2,2,7,2,2,2,7,7,7,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,7,7,7,2,7,7,7,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,3,7,7,7,7,2,2,2,7,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,2,2,2,2,2,2,2,7,7,7,7,7,7,2,2,2,2,2,2,0,0},
                {0,0,2,3,2,2,3,7,7,7,7,2,2,7,7,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,7,7,7,7,7,7,7,3,3,3,2,2,0,0},
                {0,0,2,3,3,2,3,3,7,7,7,2,7,7,7,2,2,2,2,2,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,7,2,2,2,2,7,7,7,7,7,7,3,3,3,3,3,0,0},
                {0,3,3,3,3,3,3,3,7,7,7,7,7,7,2,2,2,2,2,2,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,2,2,7,2,2,2,2,7,7,7,7,7,7,7,3,3,3,3,0,0},
                {0,3,3,3,3,3,3,3,7,7,7,7,7,7,2,7,2,7,2,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,2,7,7,2,7,2,7,7,7,7,7,3,7,3,3,3,3,0,0},
                {0,3,3,3,3,3,3,7,7,7,7,7,7,7,7,7,2,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,3,3,3,3,0,0},
                {0,0,3,3,3,3,3,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,3,3,3,3,0,0},
                {0,0,3,3,3,3,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,3,3,0,0,0},
                {0,0,3,3,3,3,7,7,7,7,7,7,3,7,7,7,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,7,7,7,7,7,7,7,7,7,7,3,7,3,3,3,3,0,0,0},
                {0,0,3,3,3,3,7,3,3,7,7,7,3,3,7,7,7,7,7,3,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,3,3,3,3,3,0,0,0},
                {0,0,3,3,3,3,3,3,3,7,7,3,3,3,7,7,7,3,3,3,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,7,7,7,7,7,7,3,7,7,7,7,3,3,3,3,3,3,0,0,0,0},
                {0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,7,7,3,3,3,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,2,2,7,7,7,7,3,7,7,7,7,3,3,3,7,3,7,7,3,3,3,3,3,0,0,0,0},
                {0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,7,3,3,3,7,7,7,7,7,7,7,2,2,2,2,2,2,2,2,2,2,7,7,7,7,7,3,3,3,7,7,7,3,3,3,3,3,7,3,3,3,3,0,0,0,0,0},
                {0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,7,7,7,3,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,7,3,3,3,3,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,7,7,3,7,7,7,7,7,7,7,7,7,7,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,3,3,3,7,7,3,7,7,7,7,3,7,7,7,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,7,3,3,7,7,7,7,3,3,7,3,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,3,3,7,7,7,3,7,3,3,3,3,3,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,7,7,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,7,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,3,3,3,3,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,3,3,3,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,0,0,3,3,3,3,3,0,0,0,0,0,0,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] BoneNestObjects = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int [,] MocoShrine = new int[,]
            {
                {0,0,0,0,0,2,2,2,2,2,2,2,2,0,0,0,0,0,0},
                {0,0,0,0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,3,3,2,2,2,2,2,2,2,2,2,2,3,3,3,0,0},
                {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0},
                {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
                {0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
                {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3},
                {3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0},
                {0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0},
                {0,0,0,0,0,3,3,3,3,3,3,3,0,3,3,3,3,0,0},
            };

            int [,] MocoShrineObjects = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,9,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //first alchemy room
            int[,] AlchemyCamp1 = new int[,]
            {
                {0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,3,3,3,3,0,3,0,3,3,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,5,5,3,3,3,3,3,3,3,3,3,3,0,0,3,3,0,3,3,0},
                {0,0,0,0,5,5,5,5,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0},
                {0,0,0,0,5,5,5,5,4,4,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3},
                {0,0,0,0,1,1,5,5,4,4,4,4,5,5,5,5,4,3,3,4,4,3,5,5,5},
                {0,0,0,1,1,1,1,4,4,4,5,5,5,5,5,5,4,4,4,4,4,5,5,5,5},
                {0,0,1,1,1,1,1,4,4,1,5,5,5,1,1,1,1,4,4,4,4,5,5,1,1},
                {0,5,5,1,1,1,4,4,4,1,1,1,1,1,1,1,1,4,4,4,1,1,1,1,1},
                {3,5,5,5,5,4,4,4,1,1,1,1,1,1,1,1,1,1,4,4,1,1,1,1,1},
                {3,3,5,4,4,4,4,1,1,1,1,1,1,1,1,1,1,1,4,4,1,1,1,1,1},
                {3,3,3,4,4,4,4,5,1,1,1,1,1,1,1,1,1,1,4,4,1,1,1,1,0},
                {0,3,3,3,3,4,4,5,1,1,1,1,1,1,1,1,1,1,4,4,1,1,1,1,0},
                {0,3,3,3,3,3,5,5,5,1,1,1,1,1,1,1,1,1,4,4,1,1,1,1,0},
                {0,0,0,3,3,3,5,5,5,5,1,1,1,1,1,5,5,4,4,4,4,5,1,1,0},
                {0,0,0,3,3,3,3,5,5,5,5,5,5,5,5,5,5,4,4,4,4,5,5,5,0},
                {0,0,0,3,3,3,3,3,3,5,5,5,5,5,5,5,4,4,3,4,4,5,5,5,0},
                {0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,4,4,3,3,3,3,0,0,0},
                {0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0},
                {0,0,0,0,0,3,3,3,3,3,3,0,0,3,3,3,3,3,3,3,3,3,0,0,0},
                {0,0,0,0,0,3,3,3,3,0,0,0,0,0,0,3,3,0,0,3,3,3,0,0,0},
            };

            int[,] AlchemyCampObjects1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0,0,0,0,2,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //second alchemy room
            int[,] AlchemyCamp2 = new int[,]
            {
                {0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,3,3,0,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0},
                {0,3,3,3,3,3,3,3,3,3,3,3,3,0,3,3,3,0,3,3,3,3,3,0,3,3,0,0},
                {0,3,3,3,3,4,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0},
                {3,3,3,4,4,4,4,5,5,5,5,5,3,3,3,5,5,5,5,5,5,3,3,3,3,3,3,0},
                {3,3,3,4,4,4,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,3,3,3,3,3,3},
                {0,3,3,3,4,4,1,1,1,1,1,5,5,5,5,5,1,1,1,1,1,1,3,3,3,3,3,3},
                {0,3,3,1,4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,1,3,3,1},
                {0,1,1,1,4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,4,4,4,1,1,1,1,1,1,1,1,1,1,1,1,1,5,5,1,1,1,1,1,0},
                {0,0,1,1,1,4,4,1,5,5,1,1,1,1,1,1,1,1,1,1,5,5,5,1,1,1,1,0},
                {0,0,1,1,1,4,4,4,5,5,5,1,1,1,1,1,1,1,1,1,5,5,5,3,3,1,1,0},
                {0,0,1,1,4,4,4,4,5,5,5,5,5,5,5,3,3,3,5,5,5,5,5,3,3,3,3,0},
                {0,0,3,3,4,4,3,4,3,5,5,5,5,3,3,3,3,5,5,5,5,5,3,3,3,3,3,0},
                {0,0,3,3,3,3,3,3,3,3,3,5,5,3,3,5,5,5,5,3,5,3,3,3,3,3,3,0},
                {0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0},
                {0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0},
                {0,0,0,3,3,3,3,0,3,3,3,3,0,3,3,0,0,3,3,3,3,0,0,3,3,3,0,0},
                {0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] AlchemyCampObjects2 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,6,0,0,0,4,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //third alchemy room
            int[,] AlchemyCamp3 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,0,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,0,3,3,0,0,0},
                {0,0,0,0,0,0,1,1,1,0,1,1,3,3,3,3,3,5,5,5,3,3,3,3,3,3,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,1,1,3,3,3,3,5,5,5,5,5,3,3,3,3,3,3,0,0},
                {0,0,0,0,5,5,1,1,1,1,1,1,1,1,3,3,5,5,5,5,5,5,5,3,3,3,3,3,0},
                {0,0,0,0,5,5,1,1,1,1,1,1,1,1,1,1,1,1,5,5,5,5,5,5,5,5,3,3,0},
                {0,0,0,5,5,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,5,5,5,5,3,3},
                {0,0,5,5,5,5,5,5,5,5,5,5,1,1,1,1,1,1,1,1,1,1,1,5,5,5,5,3,3},
                {0,0,5,5,5,5,5,5,5,5,5,5,5,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
                {0,3,3,5,5,5,5,5,5,5,5,5,5,5,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
                {3,3,3,3,3,3,3,5,5,5,5,1,1,5,5,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
                {0,3,3,3,3,3,3,3,3,1,1,1,1,1,5,5,1,1,1,1,1,1,1,1,1,0,0,0,0},
                {0,3,3,3,3,3,3,3,1,1,1,1,1,1,1,5,5,1,1,1,1,1,1,1,1,0,0,0,0},
                {0,0,0,3,3,3,3,1,1,1,1,1,1,1,1,1,5,5,1,1,1,5,5,1,0,0,0,0,0},
                {0,0,3,3,3,3,3,1,1,1,1,1,5,5,5,5,5,5,5,5,5,5,5,3,0,0,0,0,0},
                {0,0,3,3,3,3,3,5,1,1,1,1,5,5,5,5,5,5,5,5,5,5,5,3,0,0,0,0,0},
                {0,0,3,3,3,3,3,5,5,5,5,5,5,3,3,3,3,5,5,5,5,3,3,3,0,0,0,0,0},
                {0,0,0,0,3,3,3,3,3,5,5,5,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0},
                {0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,0,0,3,3,3,3,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,3,3,3,0,3,3,3,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0},
            };

            int[,] AlchemyCampObjects3 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //fourth alchemy room
            int[,] AlchemyCamp4 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,3,3,3,3,0,0,3,3,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,0,3,3,3,3,3,3,3,3,3,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,3,3,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0},
                {0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,3,3,5,5,5,5,5,3,3,3,3,3,0,0},
                {0,0,0,0,0,0,0,0,0,3,3,3,3,3,5,5,3,3,5,5,5,5,5,5,5,5,3,3,3,3,0},
                {0,0,0,0,0,0,0,0,3,3,3,3,5,5,5,5,5,5,5,5,5,1,1,5,5,5,5,5,3,3,3},
                {0,0,0,0,0,0,0,3,3,3,3,5,5,5,5,5,5,5,1,1,1,1,1,1,1,1,5,5,3,3,3},
                {0,0,0,0,0,0,0,3,3,3,3,5,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,0},
                {0,0,0,0,0,0,0,0,3,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,5,5,1,1,1,1,1,1,1,1,5,5,1,1,0,0},
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,5,5,5,1,1,1,1,1,5,5,5,5,3,1,0,0},
                {0,0,0,0,0,0,0,0,1,0,1,1,1,1,1,5,5,5,5,5,5,5,5,5,5,5,5,3,3,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,1,1,1,5,5,5,5,5,5,5,5,5,5,5,5,3,3,3,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,1,1,3,5,5,5,5,3,3,5,5,5,3,3,3,3,3,3,3,0},
                {0,0,0,0,0,0,0,1,0,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0},
                {0,0,0,0,0,0,0,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0},
                {0,0,0,0,0,0,0,1,1,1,1,1,1,1,3,3,3,3,3,3,3,3,3,3,0,0,3,3,0,0,0},
                {0,0,0,0,0,1,0,1,1,1,1,1,1,1,3,3,3,3,3,0,0,3,3,0,0,0,0,0,0,0,0},
                {0,0,1,0,1,1,1,1,1,1,1,1,1,1,1,1,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,5,5,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,1,1,5,5,1,1,1,1,1,1,1,5,5,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,1,1,5,5,1,1,1,1,1,1,1,5,5,5,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,3,5,5,5,5,1,1,1,1,1,5,5,5,5,5,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {3,3,5,5,5,5,5,5,5,5,5,5,5,5,5,5,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {3,3,3,3,5,5,5,5,5,5,5,5,5,5,5,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {3,3,3,3,3,3,3,5,5,5,5,5,5,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,3,3,0,3,3,3,3,3,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,3,3,3,0,0,0,0,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] AlchemyCampObjects4 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] BoneSpike1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0},
                {0,0,2,0,0,0,0,2,2,2,0,0,0,0,0,0},
                {0,2,2,2,2,0,2,2,2,2,2,0,0,0,0,0},
                {0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {2,2,2,2,2,2,4,2,2,2,2,2,0,0,0,0},
                {2,2,2,2,2,2,4,4,2,2,2,2,0,0,0,0},
                {2,2,2,2,2,2,4,4,2,2,2,2,2,0,0,0},
                {0,2,2,2,2,2,4,4,4,2,2,2,2,0,0,0},
                {0,2,2,2,2,2,2,4,4,2,2,2,2,0,0,0},
                {0,0,2,2,2,2,2,4,4,2,2,2,2,2,2,0},
                {0,0,2,2,2,2,2,4,4,2,2,2,2,2,2,0},
                {0,0,2,2,2,2,2,4,4,4,2,2,2,2,2,2},
                {0,0,2,2,2,2,2,4,4,4,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,4,4,4,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,4,4,4,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,4,4,4,4,2,2,2,2,0},
                {0,0,2,2,2,2,2,4,4,4,4,2,2,2,2,0},
                {0,0,0,2,2,2,2,4,4,4,4,6,6,2,0,0},
                {0,0,0,2,2,6,6,4,4,4,4,6,6,6,6,0},
                {0,0,0,6,6,6,6,4,4,4,4,6,6,6,6,0},
                {0,0,0,6,6,6,6,4,4,6,6,6,6,6,0,0},
                {0,0,0,0,6,6,6,6,6,6,6,6,6,6,0,0},
                {0,0,0,0,0,6,6,6,6,6,6,6,0,0,0,0},
                {0,0,0,0,6,6,6,6,6,6,0,0,0,0,0,0},
                {0,0,0,0,6,6,6,6,0,0,0,0,0,0,0,0},
            };

            int[,] BoneSpike2 = new int[,]
            {
                {0,0,0,0,0,0,0,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,2,2,0,0,0,0},
                {0,0,0,2,2,2,2,2,2,2,2,2,0,0,2,2},
                {0,0,2,2,2,2,2,2,4,2,2,2,2,2,2,2},
                {0,0,2,2,2,2,4,4,4,2,2,2,2,2,2,2},
                {0,0,2,2,2,2,4,4,2,2,2,2,2,2,2,2},
                {0,0,0,2,2,2,4,4,4,4,4,2,2,2,2,2},
                {0,2,0,2,2,2,2,4,4,4,4,4,2,2,2,2},
                {0,2,2,2,2,2,2,2,2,4,4,4,2,2,2,2},
                {2,2,2,2,2,2,2,4,4,4,4,2,2,2,2,0},
                {2,2,2,2,2,2,4,4,4,4,2,2,2,2,2,0},
                {2,2,2,2,2,4,4,4,4,2,2,2,2,2,2,0},
                {2,2,2,2,4,4,4,4,4,2,2,2,2,2,2,2},
                {2,2,2,2,4,4,4,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,4,4,4,4,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,4,4,4,2,2,2,2,2,0,0,0},
                {0,2,2,2,2,4,4,4,4,2,2,2,2,0,0,0},
                {0,2,2,2,2,2,4,4,4,4,2,2,2,2,0,0},
                {0,0,2,2,2,2,4,4,4,4,2,2,2,2,2,0},
                {0,0,2,2,2,2,2,4,4,4,4,6,2,2,2,0},
                {0,0,0,2,2,6,6,4,4,4,4,6,6,6,2,0},
                {0,0,0,2,6,6,6,4,4,6,4,6,6,6,0,0},
                {0,0,0,6,6,6,6,6,6,6,6,6,6,6,0,0},
                {0,0,0,6,6,6,6,6,6,6,6,6,6,6,0,0},
                {0,0,0,0,0,6,6,6,6,6,6,6,6,6,0,0},
                {0,0,0,0,6,6,6,6,6,6,6,6,6,0,0,0},
                {0,0,0,0,6,6,6,0,0,6,6,0,0,0,0,0},
            };

            int[,] BoneSpike3 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,0,0,2,2,2,0,0,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,4,2,2,2,2,2,0,0,0,0},
                {0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,4,4,4,2,2,2,2,0,2,2},
                {2,2,2,0,2,2,2,2,2,2,2,2,2,2,2,2,4,4,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,4,4,4,4,4,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,2,2,2,2,4,4,4,4,4,2,2,2,2,2,2,2,0},
                {2,2,2,2,2,2,2,2,2,2,2,2,4,4,4,2,2,2,2,2,2,2,2,2,0},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,4,4,4,4,2,2,2,2,2,2,0,0},
                {2,2,2,2,4,2,2,2,2,2,2,2,2,2,4,4,4,4,2,2,2,2,2,0,0},
                {0,2,2,2,4,4,4,4,2,2,2,2,2,2,2,4,4,4,4,2,2,2,2,2,0},
                {0,2,2,2,2,4,4,4,4,4,2,2,2,2,2,4,4,4,4,4,2,2,2,2,0},
                {0,0,2,2,2,2,4,4,4,4,2,2,2,2,2,2,2,4,4,4,2,2,2,2,0},
                {0,0,2,2,2,2,2,4,4,4,2,2,2,2,2,2,4,4,4,4,2,2,2,2,0},
                {0,2,2,2,2,2,2,4,4,4,2,2,2,2,2,2,4,4,4,2,2,2,2,0,0},
                {0,2,2,2,2,2,4,4,4,4,2,2,2,2,2,4,4,4,4,2,2,2,2,0,0},
                {2,2,2,2,2,2,4,4,4,2,2,2,2,2,4,4,4,4,2,2,2,2,2,2,0},
                {2,2,2,2,2,4,4,4,4,2,2,2,2,2,4,4,4,4,2,2,2,2,2,2,0},
                {0,2,2,2,2,4,4,4,4,2,2,2,6,4,4,4,4,2,2,2,2,2,0,0,0},
                {2,2,2,2,6,4,4,4,4,4,6,6,6,4,4,4,4,6,6,2,2,2,2,0,0},
                {2,2,6,6,6,4,4,6,4,4,6,6,6,4,6,4,4,6,6,6,2,2,2,0,0},
                {0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0},
                {0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0},
                {0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0},
                {0,0,0,6,6,6,6,6,6,0,0,6,6,6,6,6,6,6,6,6,0,0,0,0,0},
                {0,0,0,6,6,6,6,0,0,0,0,0,0,6,6,0,0,6,6,6,0,0,0,0,0},
            };

            //blank array since bone spikes have no objects
            int[,] BoneSpikeObjects = new int[,]
            {
                {},
            };

            int XStart = StartPosition;
            int XMiddle = XStart + 375;
            int XEdge = XStart + 750;

            bool placed = false;
            while (!placed)
            {
                //loop to place the nest
                int NestX = XMiddle;
                int NestY = Main.maxTilesY - 160;

                while (!WorldGen.SolidTile(NestX, NestY) && NestY <= Main.maxTilesY)
                {
                    NestY++;
                }
                if (!Main.tile[NestX, NestY].HasTile)
                {
                    continue;
                }

                PlaceStructures(NestX - 20, NestY - 15, BoneNestShape, BoneNestObjects);

                //dig crater that leads to each alchemy room
                for (int CraterDepth = Main.maxTilesY - 160; CraterDepth <= Main.maxTilesY - 100; CraterDepth++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        SpookyWorldMethods.Circle(XMiddle - 200, CraterDepth, WorldGen.genRand.Next(3, 4), 0, true);
                        SpookyWorldMethods.Circle(XMiddle - 100, CraterDepth, WorldGen.genRand.Next(3, 4), 0, true);
                        SpookyWorldMethods.Circle(XMiddle + 100, CraterDepth, WorldGen.genRand.Next(3, 4), 0, true);
                        SpookyWorldMethods.Circle(XMiddle + 200, CraterDepth, WorldGen.genRand.Next(3, 4), 0, true);
                    }
                }

                //dig giant hole around each alchemy room
                SpookyWorldMethods.Circle(XMiddle - 200, Main.maxTilesY - 100, 15, 0, true);
                SpookyWorldMethods.Circle(XMiddle - 100, Main.maxTilesY - 100, 15, 0, true);
                SpookyWorldMethods.Circle(XMiddle + 100, Main.maxTilesY - 100, 15, 0, true);
                SpookyWorldMethods.Circle(XMiddle + 200, Main.maxTilesY - 100, 15, 0, true);

                //place alchemy rooms
                PlaceStructures(XMiddle - 215, Main.maxTilesY - 100, AlchemyCamp1, AlchemyCampObjects1);
                PlaceStructures(XMiddle - 115, Main.maxTilesY - 100, AlchemyCamp2, AlchemyCampObjects2);
                PlaceStructures(XMiddle + 85, Main.maxTilesY - 100, AlchemyCamp3, AlchemyCampObjects3);
                PlaceStructures(XMiddle + 185, Main.maxTilesY - 100, AlchemyCamp4, AlchemyCampObjects4);

                //place bone spikes, theres probably a better way to do this but i need each one to be placed perfectly on the ground
                //first spike
                int BoneSpikeX1 = XMiddle - 300;
                int BoneSpikeY1 = Main.maxTilesY - 150;

                while (!WorldGen.SolidTile(BoneSpikeX1, BoneSpikeY1) && BoneSpikeY1 <= Main.maxTilesY)
                {
                    BoneSpikeY1++;
                }
                if (Main.tile[BoneSpikeX1, BoneSpikeY1].TileType != ModContent.TileType<SpookyMush>() &&
                Main.tile[BoneSpikeX1, BoneSpikeY1].TileType != ModContent.TileType<EyeBlock>())
				{
                    continue;
                }

                //second spike
                int BoneSpikeX2 = XMiddle - 250;
                int BoneSpikeY2 = Main.maxTilesY - 150;

                while (!WorldGen.SolidTile(BoneSpikeX2, BoneSpikeY2) && BoneSpikeY2 <= Main.maxTilesY)
                {
                    BoneSpikeY2++;
                }
                if (Main.tile[BoneSpikeX2, BoneSpikeY2].TileType != ModContent.TileType<SpookyMush>() &&
                Main.tile[BoneSpikeX2, BoneSpikeY2].TileType != ModContent.TileType<EyeBlock>())
				{
                    continue;
                }

                //third spike
                int BoneSpikeX3 = XMiddle - 125;
                int BoneSpikeY3 = Main.maxTilesY - 150;

                while (!WorldGen.SolidTile(BoneSpikeX3, BoneSpikeY3) && BoneSpikeY3 <= Main.maxTilesY)
                {
                    BoneSpikeY3++;
                }
                if (Main.tile[BoneSpikeX3, BoneSpikeY3].TileType != ModContent.TileType<SpookyMush>() &&
                Main.tile[BoneSpikeX3, BoneSpikeY3].TileType != ModContent.TileType<EyeBlock>())
				{
                    continue;
                }

                //fourth spike
                int BoneSpikeX4 = XMiddle + 125;
                int BoneSpikeY4 = Main.maxTilesY - 150;

                while (!WorldGen.SolidTile(BoneSpikeX4, BoneSpikeY4) && BoneSpikeY4 <= Main.maxTilesY)
                {
                    BoneSpikeY4++;
                }
                if (Main.tile[BoneSpikeX4, BoneSpikeY4].TileType != ModContent.TileType<SpookyMush>() &&
                Main.tile[BoneSpikeX4, BoneSpikeY4].TileType != ModContent.TileType<EyeBlock>())
				{
                    continue;
                }

                //fifth bone spike
                int BoneSpikeX5 = XMiddle + 250;
                int BoneSpikeY5 = Main.maxTilesY - 150;

                while (!WorldGen.SolidTile(BoneSpikeX5, BoneSpikeY5) && BoneSpikeY5 <= Main.maxTilesY)
                {
                    BoneSpikeY5++;
                }
                if (Main.tile[BoneSpikeX5, BoneSpikeY5].TileType != ModContent.TileType<SpookyMush>() &&
                Main.tile[BoneSpikeX5, BoneSpikeY5].TileType != ModContent.TileType<EyeBlock>())
				{
                    continue;
                }

                //sixth spike
                int BoneSpikeX6 = XMiddle + 300;
                int BoneSpikeY6 = Main.maxTilesY - 150;

                while (!WorldGen.SolidTile(BoneSpikeX6, BoneSpikeY6) && BoneSpikeY6 <= Main.maxTilesY)
                {
                    BoneSpikeY6++;
                }
                if (Main.tile[BoneSpikeX6, BoneSpikeY6].TileType != ModContent.TileType<SpookyMush>() &&
                Main.tile[BoneSpikeX6, BoneSpikeY6].TileType != ModContent.TileType<EyeBlock>())
				{
					continue;
				}

                //place the bone spokes
                PlaceStructures(BoneSpikeX1, BoneSpikeY1 - 20, BoneSpike1, BoneSpikeObjects);
                PlaceStructures(BoneSpikeX2, BoneSpikeY2 - 20, BoneSpike2, BoneSpikeObjects);
                PlaceStructures(BoneSpikeX3, BoneSpikeY3 - 20, BoneSpike3, BoneSpikeObjects);
                PlaceStructures(BoneSpikeX4, BoneSpikeY4 - 20, BoneSpike3, BoneSpikeObjects);
                PlaceStructures(BoneSpikeX5, BoneSpikeY5 - 20, BoneSpike2, BoneSpikeObjects);
                PlaceStructures(BoneSpikeX6, BoneSpikeY6 - 20, BoneSpike1, BoneSpikeObjects);

                if (Terraria.Main.dungeonX > Main.maxTilesX / 2)
                {
                    StartPosition = Main.maxTilesX - 750;
                }
                else
                {
                    StartPosition = 0;
                }

                int MocoShrineX = Terraria.Main.dungeonX > Main.maxTilesX / 2 ? XMiddle + 205 : XMiddle - 190;
                int MocoShrineY = Main.maxTilesY - 150;

                while (!WorldGen.SolidTile(MocoShrineX, MocoShrineY) && MocoShrineY <= Main.maxTilesY)
                {
                    MocoShrineY++;
                }
                if (Main.tile[MocoShrineX, MocoShrineY].TileType != ModContent.TileType<SpookyMush>() &&
                Main.tile[MocoShrineX, MocoShrineY].TileType != ModContent.TileType<EyeBlock>())
				{
                    continue;
                }

                PlaceStructures(MocoShrineX, MocoShrineY - 7, MocoShrine, MocoShrineObjects);

                placed = true;
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
            int SpookyHellClearIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Larva"));
			if (SpookyHellClearIndex == -1)
			{
				return;
			}

            tasks.Insert(SpookyHellClearIndex + 1, new PassLegacy("ClearArea", ClearArea));

            int SpookyHellIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (SpookyHellIndex == -1)
			{
				return;
			}

            tasks.Insert(SpookyHellIndex + 1, new PassLegacy("SpookyHell", GenerateSpookyHell));
            tasks.Insert(SpookyHellIndex + 2, new PassLegacy("SpookyHellStructures", GenerateStructures));
            tasks.Insert(SpookyHellIndex + 3, new PassLegacy("SpookyHellTrees", SpookyHellTrees));
            tasks.Insert(SpookyHellIndex + 4, new PassLegacy("SpookyHellPolish", SpookyHellPolish));
            tasks.Insert(SpookyHellIndex + 5, new PassLegacy("SpookyHellAmbience", SpookyHellAmbience));
		}

        //post worldgen to place items in the spooky biome chests
        public override void PostWorldGen()
		{
            int[] Potions1 = new int[] { ItemID.IronskinPotion, ItemID.WrathPotion, ItemID.MagicPowerPotion, ItemID.ThornsPotion };
            int[] Potions2 = new int[] { ItemID.PotionOfReturn, ItemID.TeleportationPotion, ItemID.StrangeBrew };

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex]; 

				if (chest != null && (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest>() || Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest2>() || 
                Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest3>() || Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest4>()))
                {
                    for (int inventoryIndex = 0; inventoryIndex < 5; inventoryIndex++) 
                    {
						if (chest.item[inventoryIndex].type == ItemID.None) 
                        {
                            //the actual main item
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<MonsterBloodVial>());
                                chest.item[0].stack = 1;
                                chest.item[1].SetDefaults(ModContent.ItemType<Flask1>());
                                chest.item[1].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest2>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<NerveWhip>());
                                chest.item[0].stack = 1;
                                chest.item[1].SetDefaults(ModContent.ItemType<Flask2>());
                                chest.item[1].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest3>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<TortumorStaff>());
                                chest.item[0].stack = 1;
                                chest.item[1].SetDefaults(ModContent.ItemType<Flask3>());
                                chest.item[1].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<EyeChest4>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<MonsterHeart>());
                                chest.item[0].stack = 1;
                                chest.item[1].SetDefaults(ModContent.ItemType<Flask4>());
                                chest.item[1].stack = 1;
                            }

                            //first potions
							chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions1));
							chest.item[2].stack = WorldGen.genRand.Next(3, 6);
                            //second potions
							chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions2));
							chest.item[3].stack = WorldGen.genRand.Next(3, 6);
                            //coins
							chest.item[4].SetDefaults(ItemID.GoldCoin);
							chest.item[4].stack = WorldGen.genRand.Next(1, 5);
						}
					}
                }
            }
        }
    }
}