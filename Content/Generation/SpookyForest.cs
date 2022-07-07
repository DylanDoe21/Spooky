using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.NPCs.Friendly;

namespace Spooky.Content.Generation
{
    public class SpookyForest : ModSystem
    {
        static int PositionX = Main.maxTilesX / 2;
        static int PositionY = (int)Main.worldSurface - 150; //start here to not touch floating islands

        private void GenerateSpookyForest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Generating spooky forest";

            //decide whether or not to use the alt background
            if (Main.rand.Next(2) == 0)
            {
                Flags.SpookyBackgroundAlt = false;
            }
            else
            {
                Flags.SpookyBackgroundAlt = true;
            }

            //set to default values in case of non vanilla world sizes
            int Size = 285;
            int BiomeHeight = 165;
            
            //change biome size based on world size
            if (Main.maxTilesX == 4200) //small worlds
            {
                Size = 250;
                BiomeHeight = 165;
            }
            else if (Main.maxTilesX == 6400) //medium worlds
            {
                Size = 285;
                BiomeHeight = 200;
            }
            else if (Main.maxTilesX == 8400) //large worlds
            {
                Size = 350;
                BiomeHeight = 275;
            }

            //place the actual biome
            for (int Y = 0; Y < BiomeHeight; Y++)
            {
                if (WorldGen.genRand.Next(2) == 0)
                {
                    SpookyWorldMethods.TileRunner(PositionX, PositionY + Y + 10, (double)Size + Y / 2, 1, ModContent.TileType<SpookyGrass>(), 
                    WallID.DirtUnsafe3, ModContent.WallType<SpookyStoneWall>(), true, 0f, 0f, true, true, true);
                }
            }

            //dig crater to lead to the underground
            for (int CraterDepth = (int)Main.worldSurface - 150; CraterDepth < (int)Main.worldSurface + 55; CraterDepth++)
            {
                if (WorldGen.genRand.Next(2) == 0)
                {
                    //use CraterDepth here since it needs to place continuously
                    SpookyWorldMethods.Circle(PositionX - Main.rand.Next(30, 45), CraterDepth, WorldGen.genRand.Next(3, 6), 0, true);
                }
            }
            
            //place clumps of stone in the underground
            for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 15E-05); k++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface + 10, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(45, 55), WorldGen.genRand.Next(45, 55), 
                        ModContent.TileType<SpookyStone>(), false, 0f, 0f, false, true);
                    }
                }
            }

            //place clumps of green moss in the entire biome
            for (int l = 0; l < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 15E-05); l++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY);
                
                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    //surface clumps
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(15, 20), WorldGen.genRand.Next(15, 20), 
                        ModContent.TileType<SpookyGrassGreen>(), false, 0f, 0f, false, true);
                    }

                    //bigger clumps underground
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(10, 15), 
                        ModContent.TileType<SpookyGrassGreen>(), false, 0f, 0f, false, true);
                    }
                }
            }

            //place clumps of vanilla ores
            for (int l = 0; l < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 10E-05); l++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(3, 10), WorldGen.genRand.Next(3, 10), TileID.Copper, false, 0f, 0f, false, true);
                    }
                }
            }

            for (int l = 0; l < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 7E-05); l++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(2, 8), WorldGen.genRand.Next(2, 8), TileID.Iron, false, 0f, 0f, false, true);
                    }
                }
            }
        }

        private void GrowSpookytrees(GenerationProgress progress, GameConfiguration configuration)
        {
            //grow trees
            for (int X = PositionX - 250; X < PositionX + 250; X++)
			{
                for (int Y = (int)Main.worldSurface - 150; Y < (int)Main.worldSurface - 50; Y++)
				{  
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrass>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrassGreen>())
                    {
                        if (WorldGen.genRand.Next(2) == 0)
                        {
                            WorldGen.GrowTree(X, Y - 1);
                        }
                    }
                }
            }
        }

        private void SpookyForestAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            //place ambient objects
            for (int X = PositionX - 250; X < PositionX + 250; X++)
			{
                for (int Y = PositionY - 100; Y < PositionY + 250; Y++)
				{  
                    //kill any single floating tiles so things dont look ugly
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile &&
                        !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                        {
                            WorldGen.KillTile(X, Y);
                        }
                    }

                    //place surface objects
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>())
                    {
                        //jack o lanterns
                        if (WorldGen.genRand.Next(20) == 0)
                        {
                            WorldGen.PlaceObject(X, Y - 1, 35, true, WorldGen.genRand.Next(0, 8));                        
                        }

                        //large weeds
                        if (WorldGen.genRand.Next(10) == 0)
                        {
                            ushort[] Weeds = new ushort[] { (ushort)ModContent.TileType<SpookyWeedBig1>(), 
                            (ushort)ModContent.TileType<SpookyWeedBig2>(), (ushort)ModContent.TileType<SpookyWeedBig3>(),
                            (ushort)ModContent.TileType<SpookyWeedTall1>(), (ushort)ModContent.TileType<SpookyWeedTall2>(), 
                            (ushort)ModContent.TileType<SpookyWeedTall3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Weeds));    
                        }

                        //pumpkins
                        if (WorldGen.genRand.Next(15) == 0)
                        {
                            ushort[] Pumpkins = new ushort[] { (ushort)ModContent.TileType<SpookyPumpkin1>(), 
                            (ushort)ModContent.TileType<SpookyPumpkin2>(), (ushort)ModContent.TileType<SpookyPumpkin3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Pumpkins));    
                        }
                    }

                    //place stuff on rock
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        //spooky gravestones
                        if (WorldGen.genRand.Next(10) == 0)
                        {    
                            ushort[] Gravestones = new ushort[] { (ushort)ModContent.TileType<SpookyGravestone1>(), 
                            (ushort)ModContent.TileType<SpookyGravestone2>(), (ushort)ModContent.TileType<SpookyGravestone3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Gravestones));              
                        }

                        //mossy rock piles
                        if (WorldGen.genRand.Next(12) == 0)
                        {    
                            ushort[] Rocks = new ushort[] { (ushort)ModContent.TileType<SpookyRock1>(), 
                            (ushort)ModContent.TileType<SpookyRock2>(), (ushort)ModContent.TileType<SpookyRock3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Rocks));           
                        }
                    }
                }
            }

            //place stuff underground
            for (int X = PositionX - 180; X < PositionX + 180; X++)
			{
                for (int Y = (int)Main.worldSurface; Y < (int)Main.worldSurface + 250; Y++)
				{ 
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        //mossy rock spikes
                        if (WorldGen.genRand.Next(7) == 0)
                        {    
                            ushort[] Spikes = new ushort[] { (ushort)ModContent.TileType<MossSpike1>(), 
                            (ushort)ModContent.TileType<MossSpike2>(), (ushort)ModContent.TileType<MossSpike3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Spikes));           
                        }

                        //candles
                        if (WorldGen.genRand.Next(25) == 0)
                        {
                            WorldGen.PlaceObject(X, Y - 1, (ushort)ModContent.TileType<Candle>());
                        }
                        
                        //mossy rock piles
                        if (WorldGen.genRand.Next(10) == 0)
                        {    
                            ushort[] Rocks = new ushort[] { (ushort)ModContent.TileType<SpookyRock1>(), 
                            (ushort)ModContent.TileType<SpookyRock2>(), (ushort)ModContent.TileType<SpookyRock3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Rocks));           
                        }

                        //hanging glow vines
                        if (WorldGen.genRand.Next(8) == 0)
                        {    
                            ushort[] Vines = new ushort[] { (ushort)ModContent.TileType<HangingVine1>(), 
                            (ushort)ModContent.TileType<HangingVine2>(), (ushort)ModContent.TileType<HangingVine3>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(Vines));           
                        }
                    }
                }
            }
        }

        private void PlaceRuinedHouse(int X, int Y, int[,] BlocksArray, int[,] ObjectArray)
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
                            //spooky forest wood
                            case 1:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyWood>());
                                break;
                            }
                            //wood platform
                            case 2:
                            {
                                tile.ClearEverything();
					            WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyPlatform>());
                                break;
                            }
                            //hay bale blocks
                            case 3:
                            {
                                tile.ClearEverything();
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.HayBlock);
                                break;
                            }
                            //spooky forest wood wall
                            case 4:
                            {
                                tile.ClearEverything();
					            WorldGen.PlaceWall(StructureX, StructureY, ModContent.WallType<SpookyWoodWall>());
                                break;
                            }
                            //spooky moss stone
                            case 5:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyStone>());
                                break;
                            }
                            //spooky grass
                            case 6:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyGrass>());
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
                            //cobwebs
                            case 1:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, TileID.Cobweb);
                                break;
                            }
                            //wooden beam
                            case 2:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, TileID.WoodenBeam);
                                break;
                            }
                            //potted skull
                            case 3:
                            {
                                tile.ClearTile();
                                NPC.NewNPC(null, StructureX * 16, StructureY * 16, ModContent.NPCType<LittleBoneSleeping>(), 0, 0f, 0f, 0f, 0f, 255);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void GenRuinedHouse(GenerationProgress progress, GameConfiguration configuration)
        {
            //tiles
            //0 = dont touch
            //1 = spooky forest wood
            //2 = wood platform
            //3 = hay bale blocks
            //4 = spooky forest wood wall
            //5 = spooky moss stone
            //6 = spooky grass

            //objects
            //0 = dont touch
            //1 = cobwebs
            //2 = wooden beams
            //3 = little bone npc

            //first house
            int[,] RuinedHouse1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,3,3,3,3,3,3,3,0,0,0,0,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,3,3,1,1,1,3,3,3,0,0,3,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,3,3,3,1,4,1,1,3,3,3,0,3,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,3,3,3,1,1,4,4,1,1,3,3,3,3,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,3,3,1,1,4,4,4,4,1,1,3,1,1,1,3,3,3,0,0,0,0,0},
                {0,0,0,0,0,0,1,4,4,4,4,4,4,1,1,1,4,1,1,3,3,0,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,1,1,3,3,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,4,1,3,3,0,0,0,0},
                {0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,1,1,1,2,2,2,1,1,4,4,4,4,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,4,4,4,4,4,4,4,4,4,4,4,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,4,4,4,4,4,4,4,4,4,4,4,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0},
                {0,6,5,5,5,5,1,5,1,1,1,1,1,1,1,5,1,1,1,1,5,5,0,5,5,0},
                {6,5,5,5,5,5,5,5,5,5,5,1,1,1,1,1,1,5,5,1,1,5,5,5,5,5},
                {6,6,5,6,6,5,5,6,5,5,6,6,5,5,6,6,5,5,6,6,5,5,6,6,5,5},
                {0,6,6,6,5,5,6,6,6,5,5,5,5,5,5,6,6,6,6,5,5,6,6,6,6,0},
                {0,0,6,6,6,6,6,6,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,0,0,0},
                {0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,0,6,6,6,6,6,0,0,0,0,0},
            };

            int[,] RuinedHouseObjects1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,2,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,1,2,0,1,1,0,0,0,1,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,1,1,2,0,1,0,1,0,1,1,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,0,1,2,1,0,0,2,0,1,0,2,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,2,1,1,0,2,0,0,0,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,1,1,0,0,2,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,0,1,0,0,2,1,0,0,2,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,2,0,0,1,2,0,0,1,2,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,2,0,0,1,2,0,0,1,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,2,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //second house
            int[,] RuinedHouse2 = new int[,]
            {
                {0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,3,3,3,0,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,3,3,3,3,3,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,3,3,3,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,0,0,3,3,3,1,1,1,3,3,3,0,0,0,0,0,0},
                {0,0,0,0,0,3,3,3,1,1,4,1,1,3,3,3,0,0,0,0,0},
                {0,0,0,0,0,3,3,1,1,4,4,4,1,1,3,3,3,0,0,0,0},
                {0,0,0,0,3,3,1,1,4,4,4,4,4,1,1,3,3,0,0,0,0},
                {0,0,0,0,3,3,1,4,4,4,4,4,4,4,1,1,3,3,0,0,0},
                {0,0,0,0,0,0,1,4,4,4,4,4,4,4,4,1,3,3,0,0,0},
                {0,0,0,0,0,0,1,4,4,4,4,4,4,4,4,1,0,0,0,0,0},
                {0,0,0,0,0,0,1,4,4,1,1,2,2,2,1,1,0,0,0,0,0},
                {0,0,0,0,0,0,1,4,4,4,4,4,4,4,4,1,0,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0},
                {0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0},
                {0,6,6,5,5,6,6,6,6,5,5,6,6,6,6,5,5,5,6,6,0},
                {6,6,5,5,5,5,5,5,6,6,6,6,5,5,6,6,6,5,5,6,6},
                {6,5,5,6,6,5,5,6,6,5,5,6,6,5,6,5,5,5,5,5,6},
                {0,6,5,5,6,6,6,6,5,5,5,5,5,5,6,6,5,5,5,5,0},
                {0,6,6,6,6,6,6,6,6,6,5,5,5,5,5,6,6,6,5,0,0},
                {0,0,6,6,6,6,0,0,6,6,6,6,6,6,6,6,6,0,0,0,0},
            };

            int[,] RuinedHouseObjects2 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,2,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,1,0,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,1,0,2,1,0,1,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,0,2,0,0,0,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,2,0,0,1,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,2,0,0,0,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,2,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,2,1,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //third house
            int [,] RuinedHouse3 = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,3,3,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,3,3,3,0,3,3,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,3,3,3,1,3,3,3,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,3,3,1,1,1,3,3,3,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,3,3,1,4,1,1,3,3,0,3,3,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,0,3,1,1,4,4,1,1,3,3,3,3,0,0,0,0,0},
                {0,0,0,0,0,0,0,3,3,3,3,1,4,4,4,4,1,3,3,3,3,3,0,0,0,0},
                {0,0,0,0,0,0,3,3,3,3,3,1,4,4,4,4,1,1,3,3,3,3,0,0,0,0},
                {0,0,0,0,0,0,3,3,3,3,1,1,4,4,4,4,4,1,1,1,3,3,0,0,0,0},
                {0,0,0,0,0,3,3,3,1,1,1,4,4,4,4,4,4,4,4,1,1,3,3,0,0,0},
                {0,0,0,0,0,3,3,1,1,4,4,4,4,4,4,4,4,4,4,4,1,3,3,0,0,0},
                {0,0,0,0,0,0,0,1,4,4,4,4,4,4,4,4,4,4,4,4,1,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,4,4,4,4,4,4,4,4,4,4,4,1,1,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,1,4,4,4,4,4,4,1,1,1,1,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,4,4,1,2,2,2,1,4,4,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0,0},
                {0,5,5,0,5,5,1,1,1,1,5,1,1,1,1,1,1,1,5,1,5,5,5,5,6,0},
                {5,5,5,5,5,1,1,5,5,1,1,1,1,1,1,5,5,5,5,5,5,5,5,5,5,6},
                {5,5,6,6,5,5,6,6,5,5,6,6,5,5,6,6,5,5,6,5,5,6,6,5,6,6},
                {0,6,6,6,6,5,5,6,6,6,6,5,5,5,5,5,5,6,6,6,5,5,6,6,6,0},
                {0,0,0,6,6,6,6,6,6,6,6,6,6,5,5,5,5,5,6,6,6,6,6,6,0,0},
                {0,0,0,0,0,6,6,6,6,6,0,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0},
            };

            int [,] RuinedHouseObjects3 = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,2,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,2,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,2,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,2,1,0,0,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,1,2,1,0,0,1,0,1,1,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,1,1,2,0,0,0,2,0,0,0,1,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,1,1,0,2,0,0,0,2,0,3,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,2,1,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,2,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            bool placed = false;
            while (!placed)
            {
                //first house
                int House1X = PositionX - WorldGen.genRand.Next(77, 82);
                int House1Y = (int)Main.worldSurface - 150; //start here to not touch floating islands

                while (!WorldGen.SolidTile(House1X, House1Y) && House1Y <= Main.worldSurface)
				{
					House1Y++;
				}
                if (House1Y > Main.worldSurface)
				{
					PlaceRuinedHouse(House1X - 12, (int)Main.worldSurface - 45, RuinedHouse1, RuinedHouseObjects1);
				}
                if (Main.tile[House1X, House1Y].TileType != ModContent.TileType<SpookyGrass>() &&
                Main.tile[House1X, House1Y].TileType != ModContent.TileType<SpookyGrassGreen>())
				{
					continue;
				}

                //second house
                int House2X = PositionX + WorldGen.genRand.Next(85, 100);
                int House2Y = (int)Main.worldSurface - 150; //start here to not touch floating islands

                while (!WorldGen.SolidTile(House2X, House2Y) && House2Y <= Main.worldSurface)
				{
					House2Y++;
				}
                if (House2Y > Main.worldSurface)
				{
					PlaceRuinedHouse(House1X - 12, (int)Main.worldSurface - 45, RuinedHouse1, RuinedHouseObjects1);
				}
                if (Main.tile[House2X, House2Y].TileType != ModContent.TileType<SpookyGrass>() &&
                Main.tile[House2X, House2Y].TileType != ModContent.TileType<SpookyGrassGreen>())
				{
					continue;
				}

                //third house
                int House3X = PositionX + 25;
                int House3Y = (int)Main.worldSurface - 150; //start here to not touch floating islands

                while (!WorldGen.SolidTile(House3X, House3Y) && House3Y <= Main.worldSurface)
				{
					House3Y++;
				}
                if (House3Y > Main.worldSurface)
				{
					PlaceRuinedHouse(House1X - 12, (int)Main.worldSurface - 45, RuinedHouse1, RuinedHouseObjects1);
				}
                if (Main.tile[House3X, House3Y].TileType != ModContent.TileType<SpookyGrass>() &&
                Main.tile[House3X, House3Y].TileType != ModContent.TileType<SpookyGrassGreen>())
				{
					continue;
				}

                PlaceRuinedHouse(House1X - 12, House1Y - 18, RuinedHouse1, RuinedHouseObjects1);
                PlaceRuinedHouse(House2X - 12, House2Y - 16, RuinedHouse2, RuinedHouseObjects2);
                PlaceRuinedHouse(House3X - 12, House3Y - 22, RuinedHouse3, RuinedHouseObjects3);

                placed = true;
            }
        }

        private void PlaceChestHouses(int X, int Y, int[,] BlocksArray, int[,] ObjectArray)
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
                            //clear tile, dont touch walls
                            case 1:
                            {
                                tile.ClearTile();
                                break;
                            }
                            //spooky forest wood
                            case 2:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyWood>());
                                tile.HasTile.Equals(true);
                                break;
                            }
                            //wooden platform
                            case 3:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.Platforms, mute: true);
                                break;
                            }
                            //wood beams
                            case 4:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.WoodenBeam);
                                break;
                            }
                            //spooky forest wood wall
                            case 5:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
					            WorldGen.PlaceWall(StructureX, StructureY, ModContent.WallType<SpookyWoodWall>());
                                break;
                            }
                            //moss stone
                            case 6:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyStone>());
                                tile.HasTile.Equals(true);
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
                            //place cobwebs
                            case 1:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, TileID.Cobweb);
                                break;
                            }
                            //place lanterns
                            case 2:
                            {
                                Framing.GetTileSafely(StructureX, StructureY).ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, 42, true, 3);
                                break;
                            }
                            //place chest
                            case 3:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest>(), false, 0);
                                break;
                            }
                            //place chest
                            case 4:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest2>(), false, 0);
                                break;
                            }
                            //place chest
                            case 5:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest3>(), false, 0);
                                break;
                            }
                            //place chest
                            case 6:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest4>(), false, 0);
                                break;
                            }
                            //place chest
                            case 7:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest5>(), false, 0);
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void GenerateChestHouses(GenerationProgress progress, GameConfiguration configuration)
        {
            //tiles
            //0 = dont touch
            //1 = clear everything
            //2 = spooky forest wood
            //3 = wooden platform
            //4 = wooden beam
            //5 = spooky forest wood wall
            //6 = moss stone

            //objects
            //0 = dont touch
            //1 = cobwebs
            //2 = lanterns
            //3-7 = halloween chest
            
            //first chest room
            int[,] ChestHouseShape1 = new int[,]
            {
                {0,6,6,2,3,3,3,2,2,6,6,6,6,0},
                {0,6,5,5,5,1,5,5,1,1,1,5,2,0},
                {0,2,1,1,5,5,5,1,1,1,5,5,2,0},
                {0,0,1,5,5,5,5,5,5,5,5,1,0,0},
                {0,0,1,5,1,5,1,1,5,5,5,1,0,0},
                {0,0,1,5,1,5,5,1,1,5,5,1,0,0},
                {0,2,2,2,6,6,2,3,3,3,2,2,2,0},
                {0,0,4,1,1,4,1,1,1,1,1,4,0,0},
                {0,0,4,1,1,4,1,1,1,1,1,4,0,0},
                {0,0,4,6,6,6,1,1,1,1,1,4,0,0},
                {0,6,6,6,6,6,6,1,1,1,6,6,6,0},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,0,6,6,6,6,6,6,6,6,6,6,6,0},
                {0,0,0,0,0,6,6,6,6,6,6,0,0,0},
            };

            int[,] ChestHouseObjects1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,1,1,0,0,0,1,1,2,1,1,0,0},
                {0,0,1,1,0,0,0,0,1,0,1,1,0,0},
                {0,0,0,1,0,0,0,0,0,0,1,1,1,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,1,0},
                {0,0,0,0,0,3,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,1,1,0,1,0,0,0,1,0,1,0},
                {0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //second chest room
            int[,] ChestHouseShape2 = new int[,]
            {
                {0,2,2,6,6,6,3,3,3,2,2,6,6,0},
                {0,2,5,1,1,5,1,1,1,5,1,5,6,0},
                {0,2,5,5,1,5,1,1,1,5,1,5,0,0},
                {0,0,1,5,5,5,1,1,1,5,5,5,0,0},
                {0,0,1,1,5,5,1,1,5,5,5,5,0,0},
                {0,0,1,5,5,1,1,1,5,1,5,5,0,0},
                {0,6,6,2,2,2,2,2,6,2,2,2,6,0},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,6,6,6,6,6,6,0,0,0,6,6,6,0},
                {0,0,0,6,6,6,0,0,0,0,0,0,0,0},
            };

            int[,] ChestHouseObjects2 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,1,1,2,1,0,0,0,1,1,1,0,0},
                {0,0,1,1,0,1,0,0,0,0,1,1,1,0},
                {0,1,1,1,0,0,0,0,0,0,1,0,1,0},
                {0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,1,0,0,0,0,4,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] ChestHouseObjects2Alt = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,1,1,2,1,0,0,0,1,1,1,0,0},
                {0,0,1,1,0,1,0,0,0,0,1,1,1,0},
                {0,1,1,1,0,0,0,0,0,0,1,0,1,0},
                {0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,1,0,0,0,0,5,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //third chest room
            int[,] ChestHouseShape3 = new int[,]
            {
                {0,0,0,0,0,6,6,6,0,0,0,0,0,0,0,0,0,0},
                {0,0,6,6,6,6,6,6,6,0,6,6,6,0,0,6,0,0},
                {0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0},
                {0,6,6,6,6,6,6,6,2,2,2,6,6,6,6,6,6,0},
                {0,0,6,6,6,6,2,2,2,2,6,6,2,2,6,6,0,0},
                {0,0,0,4,1,2,2,5,5,1,1,4,6,2,2,2,0,0},
                {0,0,1,4,1,2,1,5,5,5,1,4,1,5,5,2,0,0},
                {0,0,1,4,1,1,1,5,5,5,1,4,1,1,5,1,0,0},
                {0,0,1,4,1,1,5,5,1,5,1,4,1,1,5,1,0,0},
                {0,0,0,4,1,1,5,5,1,5,1,4,1,1,5,1,0,0},
                {0,6,2,2,6,2,2,2,2,2,6,6,6,2,2,2,6,0},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,6,6,0,0,0,0,6,6,6,0,6,6,6,6,6,6,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,6,6,6,0,0},
            };

            int[,] ChestHouseObjects3 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,1,1,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,1,1,0,0,0,1,1,1,0,0,0},
                {0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,6,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] ChestHouseObjects3Alt = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,1,1,1,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,1,1,0,0,0,1,1,1,0,0,0},
                {0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int ChestX = PositionX;
            int Chest1Y = WorldGen.genRand.Next((int)Main.worldSurface + 35, (int)Main.worldSurface + 75);
            int Chest2Y = WorldGen.genRand.Next((int)Main.worldSurface + 35, (int)Main.worldSurface + 75);
            int Chest3Y = WorldGen.genRand.Next((int)Main.worldSurface + 35, (int)Main.worldSurface + 75);
            int Chest4Y = WorldGen.genRand.Next((int)Main.worldSurface + 35, (int)Main.worldSurface + 75);
            int Chest5Y = WorldGen.genRand.Next((int)Main.worldSurface + 35, (int)Main.worldSurface + 75);

            SpookyWorldMethods.Circle(ChestX - 95, Chest1Y, 12, 0, true);
            SpookyWorldMethods.Circle(ChestX - 45, Chest2Y, 12, 0, true);
            SpookyWorldMethods.Circle(ChestX, Chest3Y, 12, 0, true);
            SpookyWorldMethods.Circle(ChestX + 45, Chest4Y, 12, 0, true);
            SpookyWorldMethods.Circle(ChestX + 95, Chest5Y, 12, 0, true);
            
            PlaceChestHouses(ChestX - 100, Chest1Y, ChestHouseShape1, ChestHouseObjects1);
            PlaceChestHouses(ChestX - 50, Chest2Y, ChestHouseShape2, ChestHouseObjects2);
            PlaceChestHouses(ChestX - 8, Chest3Y, ChestHouseShape2, ChestHouseObjects2Alt);
            PlaceChestHouses(ChestX + 40, Chest4Y, ChestHouseShape3, ChestHouseObjects3);
            PlaceChestHouses(ChestX + 90, Chest5Y, ChestHouseShape3, ChestHouseObjects3Alt);
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
            //generate biome
			int SpookyIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Pyramids"));
			if (SpookyIndex == -1)
			{
				return;
			}

            tasks.Insert(SpookyIndex + 1, new PassLegacy("SpookyForest", GenerateSpookyForest));
            tasks.Insert(SpookyIndex + 2, new PassLegacy("SpookyHouse", GenRuinedHouse));
            tasks.Insert(SpookyIndex + 3, new PassLegacy("SpookyChest", GenerateChestHouses));

            //grow extra trees and ambience after
            int SpookyTreeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Planting Trees"));
			if (SpookyTreeIndex == -1)
			{
                return;
            }

            tasks.Insert(SpookyTreeIndex + 1, new PassLegacy("SpookyTrees", GrowSpookytrees));
            tasks.Insert(SpookyTreeIndex + 2, new PassLegacy("SpookyTrees", GrowSpookytrees));
            tasks.Insert(SpookyTreeIndex + 3, new PassLegacy("SpookyTrees", GrowSpookytrees));
            tasks.Insert(SpookyTreeIndex + 4, new PassLegacy("SpookyTrees", GrowSpookytrees));
            tasks.Insert(SpookyTreeIndex + 5, new PassLegacy("SpookyTrees", GrowSpookytrees));
            tasks.Insert(SpookyTreeIndex + 6, new PassLegacy("SpookyAmbience", SpookyForestAmbience));
        }

        //post worldgen to place items in the spooky biome chests
        public override void PostWorldGen()
		{
            int[] Bars = new int[] { ItemID.CopperBar, ItemID.TinBar, ItemID.IronBar, ItemID.LeadBar };
            int[] LightSources = new int[] { ItemID.OrangeTorch, ModContent.ItemType<CandleItem>() };
            int[] Potions = new int[] { ItemID.LesserHealingPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };
            int[] Misc = new int[] { ItemID.PumpkinSeed, ItemID.Cobweb };

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex]; 

				if (chest != null && (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest>() ||
                Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest2>() || Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest3>() ||
                Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest4>() || Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest5>()))
                {
                    for (int inventoryIndex = 0; inventoryIndex < 5; inventoryIndex++) 
                    {
						if (chest.item[inventoryIndex].type == ItemID.None) 
                        {
                            //the actual main item
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<CandyBag>(), false);
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest2>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<LeafBlower>(), false);
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest3>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<ToiletPaper>(), false);
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest4>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<CreepyCandle>(), false);
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest5>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<NecromancyTome>(), false);
                                chest.item[0].stack = 1;
                            }

                            //iron or lead bars
							chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars), false);
							chest.item[1].stack = WorldGen.genRand.Next(3, 8);
                            //light sources
                            chest.item[2].SetDefaults(WorldGen.genRand.Next(LightSources), false);
							chest.item[2].stack = WorldGen.genRand.Next(2, 5);
                            //potions
							chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions));
							chest.item[3].stack = WorldGen.genRand.Next(2, 3);
                            //goodie bags
							chest.item[4].SetDefaults(ItemID.GoodieBag, false);
							chest.item[4].stack = WorldGen.genRand.Next(1, 2);
                            //pumpkin seeds or cobwebs
							chest.item[5].SetDefaults(WorldGen.genRand.Next(Misc), false);
							chest.item[5].stack = WorldGen.genRand.Next(2, 8);
                            //coins
                            chest.item[6].SetDefaults(ItemID.SilverCoin, false);
							chest.item[6].stack = WorldGen.genRand.Next(2, 25);
						}
					}
                }
            }
        }
    }
}