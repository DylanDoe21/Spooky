using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.SpookyHell.Ambient;
using Spooky.Content.Tiles.SpookyHell.Chests;
using Spooky.Content.Tiles.SpookyHell.Furniture;
using Spooky.Content.Tiles.SpookyHell.Tree;
using Spooky.Content.NPCs.Friendly;

namespace Spooky.Content.Generation
{
    public class SpookyHell : ModSystem
    {
        static int StartPosition = (Main.maxTilesX / 2) + 650;
        static int BiomeEdge = Main.maxTilesX - (Main.maxTilesX / 7);

        //clear area for the biome to generate in
        private void ClearArea(GenerationProgress progress, GameConfiguration configuration)
        {
            StartPosition = (Main.maxTilesX / 2) + 650;
            BiomeEdge = Main.maxTilesX - (Main.maxTilesX / 7);

            //clear all blocks and lava in the area
            for (int X = StartPosition; X <= BiomeEdge; X++)
            {
                for (int Y = Main.maxTilesY - 200; Y < Main.maxTilesY - 2; Y++)
                {
                    Tile newTile = Main.tile[X, Y];

                    newTile.ClearEverything();
                    WorldGen.KillWall(X, Y);
                }
            }
        }

        private void GenerateSpookyHell(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating the eye valley";

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

            for (int X = StartPosition; X <= BiomeEdge; X++)
            {
                double BiomeHeight = peakheight / rand1 * Math.Sin((float)X / flatness * rand1 + rand1);
                BiomeHeight += peakheight / rand2 * Math.Sin((float)X / flatness * rand2 + rand2);
                BiomeHeight += peakheight / rand3 * Math.Sin((float)X / flatness * rand3 + rand3);

                BiomeHeight += offset;

                terrainContour[X] = (int)BiomeHeight;
            }

            for (int X = StartPosition; X <= BiomeEdge; X++)
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
            for (int X = StartPosition - 50; X <= StartPosition; X++)
            {
                for (int Y = Main.maxTilesY - 135; Y < Main.maxTilesY - 2; Y++)
                {
                    if (WorldGen.genRand.Next(30) == 0)
                    {
                        SpookyWorldMethods.Circle(X, Y, WorldGen.genRand.Next(5, 8), (ushort)ModContent.TileType<SpookyMush>(), false);
                    }
                }
            }
			for (int X = BiomeEdge; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 135; Y < Main.maxTilesY - 2; Y++)
                {
                    if (WorldGen.genRand.Next(30) == 0)
                    {
                        SpookyWorldMethods.Circle(X, Y, WorldGen.genRand.Next(5, 8), (ushort)ModContent.TileType<SpookyMush>(), false);
                    }
                }
            }

            //place ceiling across the top of the biome
            for (int X = StartPosition; X <= BiomeEdge; X++)
            {
                for (int Y = Main.maxTilesY - 215; Y <= Main.maxTilesY - 192; Y++)
                {
                    if (WorldGen.genRand.Next(50) == 0)
                    {
                        SpookyWorldMethods.Circle(X, Y, WorldGen.genRand.Next(5, 10), (ushort)ModContent.TileType<SpookyMush>(), false);
                    }
                }
            }

            //generate caves
            for (int X = StartPosition + 50; X <= BiomeEdge - 50; X++)
            {
                for (int Y = Main.maxTilesY - 120; Y <= Main.maxTilesY - 40; Y++)
                {
                    if (WorldGen.genRand.Next(900) == 0)
                    {
                        TileRunner runner = new TileRunner(new Vector2(X, Y), new Vector2(0, 5), new Point16(-35, 35), 
                        new Point16(-35, 35), 15f, Main.rand.Next(100, 200), 0, false, true);
                        runner.Start();
                    }
                }
            }

            //place clumps of eye blocks
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 1E-05); i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface + 100, Main.maxTilesY - 2);

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

        private void SpreadSpookyHellGrass(GenerationProgress progress, GameConfiguration configuration)
        {
            //spread grass on all mush tiles
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 250; Y < Main.maxTilesY - 2; Y++)
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
            progress.Message = "Growing eye stalks";

            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 175; Y < Main.maxTilesY - 120; Y++)
                {
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMush>())
                    {
                        bool doPlace = true;

                        if (!WorldGen.TileEmpty(X, Y - 2))
                        {
                            doPlace = false;
                        }

                        //this looks fucking awful but i dont really care, might fix it later
                        if (Main.tile[X + 3, Y].TileType != ModContent.TileType<EyeTree>() && Main.tile[X - 3, Y].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X + 2, Y].TileType != ModContent.TileType<EyeTree>() && Main.tile[X - 2, Y].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X + 1, Y].TileType != ModContent.TileType<EyeTree>() && Main.tile[X - 1, Y].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X, Y + 3].TileType != ModContent.TileType<EyeTree>() && Main.tile[X, Y - 3].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X, Y + 2].TileType != ModContent.TileType<EyeTree>() && Main.tile[X, Y - 2].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X, Y + 1].TileType != ModContent.TileType<EyeTree>() && Main.tile[X, Y - 1].TileType != ModContent.TileType<EyeTree>())
                        {
                            if (doPlace && WorldGen.genRand.Next(20) == 0)
                            {
                                EyeTree.Spawn(X, Y - 1, -1, WorldGen.genRand, 12, 35, false, -1, false);
                            }
                        }
                    }

                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        bool doPlace = true;

                        if (!WorldGen.TileEmpty(X, Y - 2))
                        {
                            doPlace = false;
                        }

                        //this looks fucking awful but i dont really care, might fix it later
                        if (Main.tile[X + 3, Y].TileType != ModContent.TileType<EyeTree>() && Main.tile[X - 3, Y].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X + 2, Y].TileType != ModContent.TileType<EyeTree>() && Main.tile[X - 2, Y].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X + 1, Y].TileType != ModContent.TileType<EyeTree>() && Main.tile[X - 1, Y].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X, Y + 3].TileType != ModContent.TileType<EyeTree>() && Main.tile[X, Y - 3].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X, Y + 2].TileType != ModContent.TileType<EyeTree>() && Main.tile[X, Y - 2].TileType != ModContent.TileType<EyeTree>() &&
                        Main.tile[X, Y + 1].TileType != ModContent.TileType<EyeTree>() && Main.tile[X, Y - 1].TileType != ModContent.TileType<EyeTree>())
                        {
                            if (doPlace && WorldGen.genRand.Next(10) == 0)
                            {
                                EyeTree.Spawn(X, Y - 1, -1, WorldGen.genRand, 12, 35, false, -1, false);
                            }
                        }
                    }
                }
            }
        }

        private void SpookyHellAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X < BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 250; Y < Main.maxTilesY - 2; Y++)
                {
                    //follicle vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyMushGrass>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.Next(8) == 0)
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<TendrilVine>());
                        }
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<TendrilVine>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<TendrilVine>());
                    }

                    //eye vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<EyeBlock>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.Next(8) == 0)
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<EyeVine>());
                        }
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<EyeVine>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<EyeVine>());
                    }

                    //plants that can grow on both blocks
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        //eye bushes
                        if (WorldGen.genRand.Next(8) == 0)
                        {
                            ushort[] EyeBushes = new ushort[] { (ushort)ModContent.TileType<EyeBush1>(), (ushort)ModContent.TileType<EyeBush2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeBushes));    
                        }

                        //tall eye stalks
                        if (WorldGen.genRand.Next(10) == 0)
                        {
                            ushort[] EyeStalks = new ushort[] { (ushort)ModContent.TileType<TallEyeStalk1>(), 
                            (ushort)ModContent.TileType<TallEyeStalk2>(), (ushort)ModContent.TileType<TallEyeStalk2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeStalks)); 
                        }
                    }

                    //mush grass plants
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>())
                    {
                        //eye flowers
                        if (WorldGen.genRand.Next(10) == 0)
                        {
                            ushort[] EyeFlowers = new ushort[] { (ushort)ModContent.TileType<EyeFlower1>(), 
                            (ushort)ModContent.TileType<EyeFlower2>(), (ushort)ModContent.TileType<EyeFlower3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeFlowers)); 
                        }

                        //tendrils
                        if (WorldGen.genRand.Next(10) == 0)
                        {
                            ushort[] EyeFlowers = new ushort[] { (ushort)ModContent.TileType<Tentacle1>(), (ushort)ModContent.TileType<Tentacle2>(),
                            (ushort)ModContent.TileType<Tentacle3>(), (ushort)ModContent.TileType<Tentacle4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeFlowers)); 
                        }

                        //eye piles
                        if (WorldGen.genRand.Next(15) == 0)
                        {
                            ushort[] EyePiles = new ushort[] { (ushort)ModContent.TileType<EyePile1>(), (ushort)ModContent.TileType<EyePile2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyePiles));   
                        }

                        //fingers
                        if (WorldGen.genRand.Next(8) == 0)
                        {
                            ushort[] Fingers = new ushort[] { (ushort)ModContent.TileType<Finger1>(), (ushort)ModContent.TileType<Finger2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Fingers));    
                        }

                        //hanging fingers
                        if (WorldGen.genRand.Next(8) == 0)
                        {
                            ushort[] HangingFinger = new ushort[] { (ushort)ModContent.TileType<FingerHanging1>(), (ushort)ModContent.TileType<FingerHanging2>() };

                            //cannot be bothered to check which one actually works
                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(HangingFinger));
                            WorldGen.PlaceObject(X, Y + 2, WorldGen.genRand.Next(HangingFinger));    
                            WorldGen.PlaceObject(X, Y + 3, WorldGen.genRand.Next(HangingFinger));
                            WorldGen.PlaceObject(X, Y + 4, WorldGen.genRand.Next(HangingFinger));
                        }
                    }

                    //eye block plants
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        //arteries
                        if (WorldGen.genRand.Next(4) == 0)
                        {
                            ushort[] Arteries = new ushort[] { (ushort)ModContent.TileType<Artery1>(), 
                            (ushort)ModContent.TileType<Artery2>(), (ushort)ModContent.TileType<Artery3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Arteries));    
                        }

                        //bone piles
                        if (WorldGen.genRand.Next(4) == 0)
                        {
                            ushort[] BonePiles = new ushort[] { (ushort)ModContent.TileType<BonePile1>(), (ushort)ModContent.TileType<BonePile2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(BonePiles));    
                        }
                    }
                }
            }
        }

        private void SpookyHellPolish(GenerationProgress progress, GameConfiguration configuration)
        {
            for (int X = StartPosition - 50; X < BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 180; Y < Main.maxTilesY - 2; Y++)
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
                            //clear everything
                            case 1:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
                                break;
                            }
                            //spooky mush
                            case 2:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyMush>());
                                break;
                            }
                            //eye blocks
                            case 3:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<EyeBlock>());
                                break;
                            }
                            //valley stone
                            case 4:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<ValleyStone>());
                                break;
                            }
                            //living flesh
                            case 5:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<LivingFlesh>());
                                break;
                            }
                            //living flesh walls
                            case 6:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
					            WorldGen.PlaceWall(StructureX, StructureY, ModContent.WallType<LivingFleshWall>());
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
                            //orroboro egg
                            case 1:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, ModContent.TileType<OrroboroEgg>(), true);
                                break;
                            }
                            //nose shrine
                            case 2:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, (ushort)ModContent.TileType<NoseShrine>(), true);
                                break;
                            }
                            //cauldron
                            case 3:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, (ushort)ModContent.TileType<Cauldron>(), true);
                                break;
                            }
                            //chest
                            case 4:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest>(), true, 1);
                                break;
                            }
                            //chest
                            case 5:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest2>(), true, 1);
                                break;
                            }
                            //chest
                            case 6:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest3>(), true, 1);
                                break;
                            }
                            //chest
                            case 7:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<EyeChest4>(), true, 1);
                                break;
                            }
                            //little eye
                            case 8:
                            {
                                tile.ClearTile();
                                NPC.NewNPC(null, StructureX * 16, StructureY * 16, ModContent.NPCType<LittleEye>(), 0, 0f, 0f, 0f, 0f, 255);
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
            //1 = clear everything
            //2 = spooky mush
            //3 = eye blocks
            //4 = valley stone
            //5 = living flesh
            //6 = living flesh walls

            //objects
            //0 = dont touch
            //1 = orroboro egg
            //2 = nose shrine
            //3 = cauldron
            //4-7 = flesh chest

            int[,] OrroboroNest = new int[,]
            { 
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,4,4,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,4,4,1,1,0,0,0,0,0,0,1,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,4,4,4,1,1,1,0,0,0,0,1,1,1,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,0,0,0,0,0,4,4,4,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,0,0,0,4,4,4,4,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3,0,4,4,4,4,4,6,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,0,0,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,2,2,2,0,0,0,0,3,3,3,3,3,3,4,4,4,4,4,4,4,6,6,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,0,3,3,3,3,3,3,0,0,0,0,2,2,2,2,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,0,0,3,3,3,3,3,4,4,4,4,4,4,4,4,6,6,6,6,6,1,1,1,1,1,1,1,1,1,1,4,4,4,4,3,3,3,3,3,3,3,0,0,2,2,2,2,2,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,2,3,3,3,3,3,4,4,4,4,4,4,4,4,1,1,6,6,6,6,6,6,1,1,1,1,1,1,1,1,4,4,4,4,4,3,3,3,3,3,3,3,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,3,3,3,3,3,4,4,4,4,4,4,4,4,4,1,1,1,1,1,1,6,6,6,1,1,1,1,1,1,1,4,4,4,4,4,4,3,3,3,3,3,3,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,3,3,3,3,3,4,3,3,4,4,4,4,4,1,1,1,1,1,4,1,1,6,6,1,6,6,1,1,1,1,6,4,4,4,4,4,4,3,3,3,3,3,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,3,3,3,3,3,3,3,4,4,4,4,4,1,1,1,1,4,4,4,1,1,6,6,6,6,6,6,1,1,1,6,4,4,4,4,4,4,4,3,3,3,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0},
                {0,2,2,2,0,2,2,2,2,2,2,3,3,3,3,3,3,4,4,4,4,1,1,1,1,4,4,4,1,1,1,1,6,6,6,6,6,6,1,6,6,4,4,4,3,3,4,4,3,3,3,2,2,2,2,2,2,2,2,0,0,0,2,2,2,0,0},
                {2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,4,4,4,3,3,1,1,1,4,4,4,1,1,1,1,1,6,6,6,6,6,6,6,6,6,4,4,4,3,3,4,3,3,3,2,2,2,2,2,2,2,2,2,0,2,2,2,2,2,0},
                {2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,4,4,3,3,3,1,1,4,4,4,1,1,1,1,1,1,6,6,6,1,6,6,6,6,1,4,4,4,4,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,1,1,4,4,4,1,1,1,1,1,1,6,6,1,1,1,6,6,1,1,3,4,4,4,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,1,4,4,4,4,6,1,1,1,1,6,6,6,1,1,1,1,1,1,1,3,4,4,4,4,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,4,4,4,4,6,6,6,1,1,1,6,6,6,1,1,1,1,1,1,1,3,3,4,4,4,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,4,4,4,4,4,6,6,6,6,6,6,6,6,1,1,1,1,1,1,1,1,1,3,3,4,4,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,4,4,4,4,1,1,6,6,6,6,6,6,6,1,1,1,4,1,1,1,1,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,4,4,4,4,4,1,1,1,1,6,6,6,6,1,1,1,1,4,4,4,1,1,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,4,4,4,4,4,4,1,1,1,1,1,1,6,6,1,1,1,1,1,4,4,1,1,3,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,4,4,4,4,4,1,1,1,1,1,1,6,6,6,1,1,1,1,1,4,4,4,1,1,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,4,4,3,4,4,4,1,1,1,1,1,6,6,6,6,6,1,1,1,1,4,4,4,4,1,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,4,4,3,1,1,1,6,6,6,6,6,6,6,6,1,1,1,4,4,4,4,4,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,4,4,3,3,1,1,6,6,6,6,1,1,6,6,6,6,1,1,1,4,4,4,4,4,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,6,6,6,1,1,1,1,1,1,6,6,6,1,1,4,4,4,4,4,4,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,6,6,1,1,1,1,1,1,1,1,6,6,1,1,1,4,4,4,4,4,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,0,0,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,6,1,1,1,1,1,1,1,1,6,6,6,1,1,4,4,3,4,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,1,1,1,1,1,1,6,6,6,6,3,3,4,4,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,4,4,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,0,0,2,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,3,3,3,3,3,3,3,2,2,2,2,2,3,3,3,3,2,2,2,2,3,3,3,3,3,3,3,2,2,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0,0},
                {0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,0,0},
                {0,0,0,0,0,0,0,2,2,2,2,2,2,0,0,0,2,2,2,2,2,2,2,0,0,0,0,0,2,2,2,2,0,0,0,0,2,2,2,2,2,2,2,0,0,2,2,2,2,0,0,0,2,2,2,2,2,2,2,2,2,0,0,0,0,0,0},
            };

            int[,] OrroboroNestObjects = new int[,]
            { 
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] MocoShrine = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,0,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,1,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,4,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,1,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,4,4,0,0,0},
                {0,0,4,0,0,0,0,0,0,0,0,6,6,0,6,6,1,1,1,6,6,6,1,6,6,6,0,0,0,0,0,0,0,4,4,0,0,0,0},
                {0,0,4,4,4,0,0,0,0,0,6,6,6,6,6,6,1,1,1,6,6,6,1,6,6,6,0,0,0,0,0,0,4,4,4,0,0,0,0},
                {0,0,0,4,4,4,0,0,0,0,6,6,6,6,6,6,1,1,1,6,1,1,1,6,6,6,0,0,0,0,0,4,4,4,0,0,0,0,0},
                {0,0,0,4,4,4,4,0,0,6,6,6,1,6,6,1,1,1,1,1,1,1,1,6,6,6,0,0,0,0,4,4,4,4,0,0,0,0,0},
                {0,0,0,4,4,4,4,0,0,6,6,6,1,1,1,1,1,1,1,1,1,1,1,1,6,6,0,0,0,4,4,4,4,4,0,0,0,0,0},
                {0,0,0,0,4,4,4,4,0,6,6,6,1,1,1,1,1,1,1,1,1,1,1,1,6,6,6,0,0,4,4,4,4,0,0,0,0,0,0},
                {0,0,0,0,4,4,4,4,4,6,6,6,1,1,1,1,1,1,1,1,1,1,1,1,6,6,6,0,4,4,4,4,4,0,0,0,0,0,0},
                {0,0,0,0,0,4,4,4,4,4,6,6,1,1,1,1,1,1,1,1,1,1,1,1,6,6,6,4,4,4,4,4,4,3,0,0,0,0,0},
                {0,0,0,0,3,3,4,4,4,4,6,3,3,1,1,1,1,1,1,1,1,1,3,3,3,6,3,4,4,4,4,4,3,3,0,0,0,0,0},
                {0,0,0,3,3,3,4,4,3,4,3,3,3,3,1,1,1,1,1,1,1,3,3,3,3,3,3,4,4,3,4,4,3,3,3,0,0,0,0},
                {0,0,0,3,3,3,3,4,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,4,3,3,3,3,0,0,0,0},
                {0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0},
                {0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0},
                {0,0,0,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,0,0,0},
                {0,0,0,3,3,3,3,3,3,2,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,2,2,2,3,3,3,3,3,0,0,0},
                {0,0,0,0,2,3,3,3,3,2,2,3,3,3,3,3,2,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,3,3,3,0,0,0,0},
                {0,0,0,2,2,2,2,2,2,2,2,3,3,3,3,3,2,3,3,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,0,0,0},
                {0,0,0,2,2,2,2,2,2,2,2,3,3,3,3,2,2,2,3,3,3,3,3,3,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,0,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0},
                {0,0,0,0,0,0,0,2,2,2,2,2,2,2,0,0,0,0,0,0,0,2,2,2,0,0,0,2,2,2,2,2,0,0,0,0,0,0,0},
            };

            int[,] MocoShrineObjects = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] FleshPillar1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,6,6,0,0,0,0,0,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,5,5,6,6,6,0,0,0,6,6,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,5,5,6,6,6,6,0,0,6,6,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,5,5,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,5,5,5,6,6,1,6,6,6,6,6,0,0,0,0,0,0,4,0},
                {0,0,0,0,0,0,0,5,5,6,6,1,1,6,6,6,6,0,0,0,0,4,4,4,4,0},
                {0,0,0,0,0,0,0,5,5,6,6,1,1,1,6,6,6,0,4,4,4,4,4,4,0,0},
                {0,0,0,0,0,0,5,5,5,6,6,1,1,1,6,4,4,4,4,4,4,4,0,0,0,0},
                {0,0,0,0,0,0,5,5,6,6,1,1,1,4,4,4,4,4,4,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,5,5,6,6,1,1,4,4,4,4,4,5,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,6,6,6,1,1,1,1,6,5,5,5,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,6,6,1,1,1,1,1,6,6,5,5,5,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,6,6,1,1,1,1,6,6,6,6,5,5,0,0,0,0,0,0,0},
                {0,0,0,4,0,0,0,6,6,1,1,1,1,6,6,6,6,5,5,5,0,0,0,0,0,0},
                {0,0,0,4,4,4,4,4,6,1,1,1,6,6,6,6,6,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,4,4,4,4,4,4,4,4,6,6,1,1,6,6,5,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,4,4,4,4,4,1,1,6,6,6,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,5,4,4,4,4,4,1,1,6,6,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,5,5,5,6,6,1,1,1,6,6,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,6,6,1,1,1,6,5,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,6,6,1,1,1,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,6,6,6,1,1,1,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,6,6,6,1,1,1,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,6,6,1,1,1,6,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,6,6,1,1,1,6,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,0},
                {0,0,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,5,5,5,5,5,5,5,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,0,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0},
            };

            int[,] FleshPillar1Objects = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] FleshPillar2 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,6,6,0,5,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,1,6,6,5,5,5,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,1,1,6,6,6,5,5,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,6,1,1,1,6,6,5,5,5,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,6,6,1,1,1,6,6,5,5,5,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,6,6,6,1,1,6,6,6,5,5,0,0,0,0,0,0,0},
                {0,0,4,0,0,0,0,0,6,6,6,6,1,1,1,6,6,5,5,5,0,0,0,0,0,0},
                {0,0,4,4,4,4,4,4,6,6,6,6,6,6,1,6,6,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,4,4,4,4,4,4,4,4,6,6,6,6,6,6,5,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,4,4,4,4,4,4,4,6,6,6,6,6,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,5,4,4,4,4,4,6,1,6,6,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,5,5,5,6,6,6,6,1,6,6,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,6,6,6,1,1,6,5,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,6,6,6,1,1,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,6,6,6,1,1,6,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,6,6,6,1,1,6,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,6,6,1,1,6,6,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,6,6,1,1,6,6,6,0,0,0,0,0,0,0,0},
                {0,0,0,0,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,0},
                {0,0,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,5,5,5,5,5,5,5,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {2,2,2,2,2,2,2,2,2,2,0,0,0,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,0},
            };

            int[,] FleshPillar2Objects = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,8,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] FleshPillar3 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,0,6,6,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,5,5,5,6,6,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,5,5,6,6,6,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,5,5,5,6,6,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,5,5,6,6,6,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,5,6,6,6,6,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,6,6,6,6,6,6,0,0,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,5,5,5,6,6,6,1,6,6,5,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,6,6,6,1,1,6,6,5,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,6,6,6,1,1,1,1,6,5,5,0,0,0,0,0,4},
                {0,0,0,0,0,0,0,0,0,6,6,6,1,1,1,1,1,6,5,5,4,4,4,4,4,4},
                {0,0,0,0,0,0,0,0,5,5,6,6,1,1,1,1,1,6,4,4,4,4,4,4,0,0},
                {0,0,0,0,0,0,0,0,5,5,6,1,1,1,4,4,4,4,4,4,4,4,0,0,0,0},
                {0,0,0,0,0,0,0,5,5,5,6,1,1,4,4,4,4,4,4,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,5,5,6,6,1,1,6,6,5,5,5,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,5,5,6,6,1,6,6,6,5,5,5,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,5,5,0,0,0,0,0,0,0,0},
                {0,4,0,0,0,0,0,0,6,6,6,6,6,1,6,6,5,5,5,0,0,0,0,0,0,0},
                {0,4,4,4,4,0,0,0,0,6,6,6,6,1,1,6,6,5,5,0,0,0,0,0,0,0},
                {0,0,4,4,4,4,4,4,0,6,6,6,1,1,1,6,6,5,5,5,0,0,0,0,0,0},
                {0,0,0,0,4,4,4,4,4,4,4,6,1,1,1,6,6,5,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,4,4,4,4,1,1,1,6,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,5,4,4,4,4,4,1,1,6,6,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,5,5,5,6,6,1,1,1,6,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,5,5,5,6,6,1,1,1,1,1,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,5,5,6,6,6,6,1,1,1,1,6,6,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,5,5,5,6,6,6,6,6,1,1,1,6,6,0,0,0,0,4,0,0},
                {0,0,0,0,0,0,5,5,6,6,6,1,6,6,6,1,1,4,4,4,4,4,4,4,0,0},
                {0,0,0,0,0,5,5,5,6,6,1,1,1,6,4,4,4,4,4,4,4,4,0,0,0,0},
                {0,0,0,0,0,5,5,6,6,6,1,1,4,4,4,4,4,4,4,4,0,0,0,0,0,0},
                {0,0,0,0,0,5,5,6,6,1,1,4,4,4,4,4,5,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,5,5,6,6,1,1,1,6,6,5,5,5,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,5,5,5,6,1,1,1,6,6,5,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,5,5,6,1,1,1,6,6,5,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,5,5,6,1,1,1,6,6,6,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,6,6,1,1,1,6,6,6,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,6,6,6,1,1,1,6,6,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,6,6,6,1,1,1,6,6,0,0,0,0,0,0,0,0,0,0,0},
                {0,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,0,0,0,0},
                {2,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,0,0},
                {2,2,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,2,0},
                {2,2,2,2,2,2,2,2,2,5,5,5,5,5,5,5,2,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2},
                {0,0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0,0},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,0},
                {2,2,2,2,2,2,2,2,2,2,2,2,2,0,0,0,2,2,2,2,2,2,2,2,2,2},
                {0,2,2,2,2,2,2,2,2,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,0},
            };

            int[,] FleshPillar3Objects = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int XMiddle = (StartPosition + BiomeEdge) / 2;

            //place first flesh pillar
            bool placedPillar1 = false;
            int pillar1Attempts = 0;
            while (!placedPillar1 && pillar1Attempts++ < 100000)
            {
                int PillarX = StartPosition + 150;
                int PillarY = Main.maxTilesY - 160;

                while (!WorldGen.SolidTile(PillarX, PillarY) && PillarY <= Main.maxTilesY)
                {
                    PillarY++;
                }

                if (Main.tile[PillarX, PillarY].HasTile || Main.tile[PillarX, PillarY].WallType == ModContent.WallType<SpookyMushWall>())
				{
					PlaceStructures(PillarX - 15, PillarY - 26, FleshPillar1, FleshPillar1Objects);
                    placedPillar1 = true;
				}
            }

            //place second flesh pillar
            bool placedPillar2 = false;
            int pillar2Attempts = 0;
            while (!placedPillar2 && pillar2Attempts++ < 100000)
            {
                int PillarX = XMiddle - 150;
                int PillarY = Main.maxTilesY - 160;

                while (!WorldGen.SolidTile(PillarX, PillarY) && PillarY <= Main.maxTilesY)
                {
                    PillarY++;
                }

                if (Main.tile[PillarX, PillarY].HasTile || Main.tile[PillarX, PillarY].WallType == ModContent.WallType<SpookyMushWall>())
				{
					PlaceStructures(PillarX - 15, PillarY - 20, FleshPillar2, FleshPillar2Objects);
                    placedPillar2 = true;
				}
            }

            //place orroboro nest
            bool placedNest = false;
            int nestAttempts = 0;
            while (!placedNest && nestAttempts++ < 100000)
            {
                int NestX = XMiddle;
                int NestY = Main.maxTilesY - 160;

                while (!WorldGen.SolidTile(NestX, NestY) && NestY <= Main.maxTilesY)
                {
                    NestY++;
                }

                if (Main.tile[NestX, NestY].HasTile || Main.tile[NestX, NestY].WallType == ModContent.WallType<SpookyMushWall>())
				{
					PlaceStructures(NestX - 15, NestY - 23, OrroboroNest, OrroboroNestObjects);
                    placedNest = true;
				}
            }

            //place third flesh pillar
            bool placedPillar3 = false;
            int pillar3Attempts = 0;
            while (!placedPillar3 && pillar3Attempts++ < 100000)
            {
                int PillarX = XMiddle + 150;
                int PillarY = Main.maxTilesY - 160;

                while (!WorldGen.SolidTile(PillarX, PillarY) && PillarY <= Main.maxTilesY)
                {
                    PillarY++;
                }

                if (Main.tile[PillarX, PillarY].HasTile || Main.tile[PillarX, PillarY].WallType == ModContent.WallType<SpookyMushWall>())
				{
					PlaceStructures(PillarX - 15, PillarY - 40, FleshPillar3, FleshPillar3Objects);
                    placedPillar3 = true;
				}
            }

            //place moco shrine
            bool placedShrine = false;
            int shrineAttempts = 0;
            while (!placedShrine && shrineAttempts++ < 100000)
            {
                int ShrineX = BiomeEdge - 150;
                int ShrineY = Main.maxTilesY - 160;

                while (!WorldGen.SolidTile(ShrineX, ShrineY) && ShrineY <= Main.maxTilesY)
                {
                    ShrineY++;
                }

                if (Main.tile[ShrineX, ShrineY].HasTile || Main.tile[ShrineX, ShrineY].WallType == ModContent.WallType<SpookyMushWall>())
				{
					PlaceStructures(ShrineX - 15, ShrineY - 20, MocoShrine, MocoShrineObjects);
                    placedShrine = true;
				}
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("ClearArea", ClearArea));

            int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
			if (GenIndex2 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex2 + 1, new PassLegacy("SpookyHell", GenerateSpookyHell));
            tasks.Insert(GenIndex2 + 2, new PassLegacy("SpookyHellStructures", GenerateStructures));
            tasks.Insert(GenIndex2 + 3, new PassLegacy("SpookyHellGrass", SpreadSpookyHellGrass));
            tasks.Insert(GenIndex2 + 4, new PassLegacy("SpookyHellTrees", SpookyHellTrees));
            tasks.Insert(GenIndex2 + 5, new PassLegacy("SpookyHellPolish", SpookyHellPolish));
            tasks.Insert(GenIndex2 + 6, new PassLegacy("SpookyHellAmbience", SpookyHellAmbience));
		}

        /*
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
        */
    }
}