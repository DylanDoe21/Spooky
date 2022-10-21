using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Tree;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Chests;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.NPCs.Friendly;

namespace Spooky.Content.Generation
{
    public class SpookyForest : ModSystem
    {
        //default positions, edit based on worldsize below
        static int PositionX = Main.maxTilesX / 2;
        static int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

        static bool PlacedGrass = false;

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

            PositionX = Main.maxTilesX / 2;
            PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            //set to default values in case of non vanilla world sizes
            int Size = Main.maxTilesX / 12;
            int BiomeHeight = Main.maxTilesY / 6;

            //place the actual biome
            for (int Y = 0; Y < BiomeHeight; Y += 50)
            {
                SpookyWorldMethods.TileRunner(PositionX, PositionY + Y + 10, (double)Size + Y / 2, 1, ModContent.TileType<SpookyDirt>(), 
                ModContent.WallType<SpookyGrassWall>(), 0, true, 0f, 0f, true, true, true);
            }

            //dig crater to lead to the underground
            for (int CraterDepth = PositionY; CraterDepth <= (int)Main.worldSurface + 55; CraterDepth += 5)
            {
                TileRunner runner = new TileRunner(new Vector2(PositionX - Main.rand.Next(30, 55), CraterDepth), new Vector2(0, 5), new Point16(-5, 5), 
                new Point16(-5, 5), 15f, Main.rand.Next(5, 10), 0, false, true);
                runner.Start();    
            }
            
            //place clumps of stone in the underground
            for (int stone = 0; stone < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 12E-05); stone++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface + 10, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(35, 45), WorldGen.genRand.Next(35, 45), 
                        ModContent.TileType<SpookyStone>(), true, 0f, 0f, true, true);
                    }
                }
            }

            //place clumps of green grass
            for (int moss = 0; moss < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 15E-05); moss++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY);
                
                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    //surface clumps
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(15, 20), WorldGen.genRand.Next(15, 20), 
                        ModContent.TileType<SpookyDirt2>(), false, 0f, 0f, false, true);
                    }

                    //bigger clumps underground
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(15, 20), WorldGen.genRand.Next(15, 20), 
                        ModContent.TileType<SpookyDirt2>(), false, 0f, 0f, false, true);
                    }
                }
            }

            //place clumps of vanilla ores
            for (int copper = 0; copper < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 10E-05); copper++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(3, 10), WorldGen.genRand.Next(6, 10), TileID.Copper, false, 0f, 0f, false, true);
                }
            }

            for (int iron = 0; iron < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 7E-05); iron++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(2, 8), WorldGen.genRand.Next(5, 8), TileID.Iron, false, 0f, 0f, false, true);
                }
            }

            for (int silver = 0; silver < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 6E-05); silver++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(2, 8), WorldGen.genRand.Next(3, 6), TileID.Silver, false, 0f, 0f, false, true);
                }
            }

            //place custom caves
            for (int caves = 0; caves < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 7E-05); caves++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    TileRunner runner = new TileRunner(new Vector2(X, Y), new Vector2(0, 5), new Point16(-35, 35), 
                    new Point16(-12, 12), 15f, Main.rand.Next(25, 50), 0, false, true);
                    runner.Start();
                }
            }
        }

        private void SpreadSpookyGrass(GenerationProgress progress, GameConfiguration configuration)
        {
            //spread grass on all spooky dirt tiles
            for (int X = PositionX - 500; X <= PositionX + 500; X++)
			{
                for (int Y = PositionY - 100; Y <= Main.maxTilesY - 100; Y++)
				{ 
                    Tile up = Main.tile[X, Y - 1];
                    Tile down = Main.tile[X, Y + 1];
                    Tile left = Main.tile[X - 1, Y];
                    Tile right = Main.tile[X + 1, Y];

                    if (Main.tile[X, Y].HasTile)
                    {
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>() &&
                        ((!up.HasTile || up.TileType == TileID.Trees) || !down.HasTile || !left.HasTile || !right.HasTile))
                        {
                            Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyGrass>();
                        }

                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt2>() &&
                        ((!up.HasTile || up.TileType == TileID.Trees) || !down.HasTile || !left.HasTile || !right.HasTile))
                        {
                            Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
                        }
                    }

                    if (X == PositionX + 500 && Y == Main.maxTilesY - 100)
                    {
                        PlacedGrass = true;
                    }
                }
            }
        }

        private void GrowSpookyTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            //grow trees
            for (int X = PositionX - 300; X <= PositionX + 300; X++)
			{
                for (int Y = 0; Y < (int)Main.worldSurface - 50; Y++)
				{  
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyDirt>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyDirt2>())
                    {
                        WorldGen.GrowTree(X, Y - 1);
                    }
                }
            }
        }

        private void SpookyForestAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            if (PlacedGrass)
            {
                //place ambient objects
                for (int X = PositionX - 300; X < PositionX + 300; X++)
                {
                    for (int Y = PositionY - 100; Y < Main.maxTilesY - 100; Y++)
                    {  
                        //kill any single floating tiles so things dont look ugly
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>() ||
                        Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>() ||
                        Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                        {
                            if (!Main.tile[X, Y - 1].HasTile && !Main.tile[X, Y + 1].HasTile &&
                            !Main.tile[X - 1, Y].HasTile && !Main.tile[X + 1, Y].HasTile)
                            {
                                WorldGen.KillTile(X, Y);
                            }
                        }

                        //orange spooky vines
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>() && !Main.tile[X, Y + 1].HasTile)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyVines>());
                            }
                        }

                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyVines>())
                        {
                            SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<SpookyVines>());
                        }

                        //green spooky vines
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>() && !Main.tile[X, Y + 1].HasTile)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyVinesGreen>());
                            }
                        }

                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyVinesGreen>())
                        {
                            SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<SpookyVinesGreen>());
                        }

                        //place surface objects
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>() ||
                        Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>())
                        {
                            //jack o lanterns
                            if (WorldGen.genRand.Next(15) == 0)
                            {
                                WorldGen.PlaceObject(X, Y - 1, 35, true, WorldGen.genRand.Next(0, 8));                        
                            }

                            //pumpkins
                            if (WorldGen.genRand.Next(7) == 0)
                            {
                                ushort[] Pumpkins = new ushort[] { (ushort)ModContent.TileType<SpookyPumpkin1>(), 
                                (ushort)ModContent.TileType<SpookyPumpkin2>(), (ushort)ModContent.TileType<SpookyPumpkin3>() };

                                WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Pumpkins));    
                            }
                        }

                        //place orange grass only on orange grass
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>())
                        {
                            if (Main.rand.Next(3) == 0) 
                            {
                                ushort[] TallWeed = new ushort[] { (ushort)ModContent.TileType<SpookyWeedsTallOrange1>(), 
                                (ushort)ModContent.TileType<SpookyWeedsTallOrange2>(),(ushort)ModContent.TileType<SpookyWeedsTallOrange3>() };

                                WorldGen.PlaceObject(X, Y - 1, Main.rand.Next(TallWeed));
                            }
                        }

                        //place green grass only on green grass
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>())
                        {
                            if (Main.rand.Next(3) == 0) 
                            {
                                ushort[] TallWeed = new ushort[] { (ushort)ModContent.TileType<SpookyWeedsTallGreen1>(), 
                                (ushort)ModContent.TileType<SpookyWeedsTallGreen2>(),(ushort)ModContent.TileType<SpookyWeedsTallGreen3>() };

                                WorldGen.PlaceObject(X, Y - 1, Main.rand.Next(TallWeed));
                            }
                        }
                    }
                }

                //place stuff underground
                for (int X = PositionX - 300; X < PositionX + 300; X++)
                {
                    for (int Y = (int)Main.worldSurface; Y < (int)Main.worldSurface + 250; Y++)
                    { 
                        if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>() ||
                        Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                        {   
                            if (Main.rand.Next(8) == 0) 
                            {
                                ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<SpookyMushroomTall1>(), (ushort)ModContent.TileType<SpookyMushroomTall2>() };

                                WorldGen.PlaceObject(X, Y - 1, Main.rand.Next(Mushrooms));
                            }

                            //candles
                            if (WorldGen.genRand.Next(20) == 0)
                            {
                                WorldGen.PlaceObject(X, Y - 1, (ushort)ModContent.TileType<Candle>());
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
                                tile.ClearEverything();
                                break;
                            }
                            //old wood
                            case 2:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyWood>());
                                break;
                            }
                            //wood platform
                            case 3:
                            {
                                tile.ClearEverything();
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.Platforms);
                                break;
                            }
                            //dynasty shingles
                            case 4:
                            {
                                tile.ClearEverything();
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.RedDynastyShingles);
                                break;
                            }
                            //old wood wall
                            case 5:
                            {
                                tile.ClearEverything();
					            WorldGen.PlaceWall(StructureX, StructureY, ModContent.WallType<SpookyWoodWall>());
                                break;
                            }
                            //spooky dirt
                            case 6:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyDirt>());
                                break;
                            }
                            //living wood
                            case 7:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceTile(StructureX, StructureY, TileID.LivingWood);
                                break;
                            }
                            //living wood wall
                            case 8:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceWall(StructureX, StructureY, WallID.LivingWood);
                                break;
                            }
                            //living wood and wall (for the tunnels)
                            case 9:
                            {
                                tile.ClearEverything();
                                WorldGen.PlaceTile(StructureX, StructureY, TileID.LivingWood);
                                WorldGen.PlaceWall(StructureX, StructureY, WallID.LivingWood);
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
                            //wooden beam
                            case 1:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, TileID.WoodenBeam);
                                break;
                            }
                            //pumpkin chandelier
                            case 2:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, TileID.Chandeliers, true, 6);
                                break;
                            }
                            //little bone
                            case 3:
                            {
                                tile.ClearTile();
                                NPC.NewNPC(null, StructureX * 16, StructureY * 16, ModContent.NPCType<LittleBoneSleeping>(), 0, 0f, 0f, 0f, 0f, 255);
                                break;
                            }
                            //candles
                            case 4:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, ModContent.TileType<Candle>());
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void GenerateStarterHouse(GenerationProgress progress, GameConfiguration configuration)
        {
            //tiles
            //0 = dont touch
            //1 = clear everything
            //2 = old wood
            //3 = wood platform
            //4 = dynasty shingles
            //5 = old wood wall
            //6 = spooky dirt
            //7 = living wood
            //8 = living wood wall
            //9 = living wood and wall

            //objects
            //0 = dont touch
            //1 = wooden beams
            //2 = pumpkin chandelier
            //3 = little bone
            //4 = candles

            int [,] StarterHouse = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,4,4,4,4,4,4,2,2,2,2,2,2,2,4,4,4,4,4,4,0,0,0,0,0,0},
                {0,0,0,0,0,0,4,4,4,2,2,2,2,2,1,1,1,5,5,2,2,2,2,2,4,4,4,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,2,2,5,1,1,5,1,1,1,1,5,5,1,5,1,2,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,2,0,5,1,1,5,5,1,1,1,5,5,1,5,1,5,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,2,0,5,1,1,1,5,1,1,1,5,1,1,5,1,5,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,2,5,5,5,1,1,5,1,1,5,5,1,1,5,1,5,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,2,5,1,5,5,1,5,1,1,5,1,1,1,5,5,5,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,2,2,2,2,1,5,5,5,5,5,5,1,1,1,5,2,2,2,2,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,2,2,2,2,2,2,3,3,3,3,3,2,2,2,2,2,2,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,2,2,2,5,1,1,5,5,5,1,1,5,2,2,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,2,2,5,5,1,1,5,5,5,1,1,5,5,2,2,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,2,5,1,1,5,5,5,5,1,5,5,5,2,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,5,1,1,5,1,5,5,5,5,1,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,5,1,1,5,1,5,5,5,1,1,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,5,1,1,5,1,1,5,5,1,1,5,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,6,6,6,6,6,6,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,6,6,6,6,0,0,0,0},
                {6,6,6,6,6,6,6,6,6,6,2,2,2,2,2,2,2,2,2,2,2,2,2,6,6,6,6,6,6,6,6,6},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0},
                {0,6,6,6,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0},
                {0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,0,0,0,0,6,6,6,6,0,6,6,0,0,0,0,0,0},
            };

            int [,] StarterHouseObjects = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,3,1,0,0,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                //place starter house
                int HouseX = PositionX + ((Main.maxTilesX / 12) / 5); //get the biomes size, then divide that more and get the distance from the center
                int HouseY = PositionY; //start here to not touch floating islands

                while (!WorldGen.SolidTile(HouseX, HouseY) && HouseY <= Main.worldSurface)
				{
					HouseY++;
				}

                if (Main.tile[HouseX, HouseY].TileType != ModContent.TileType<SpookyDirt>() ||
                Main.tile[HouseX, HouseY].TileType != ModContent.TileType<SpookyDirt2>() ||
                Main.tile[HouseX, HouseY].TileType != ModContent.TileType<SpookyGrass>() ||
                Main.tile[HouseX, HouseY].TileType != ModContent.TileType<SpookyGrassGreen>())
				{
					PlaceStructures(HouseX, HouseY - 18, StarterHouse, StarterHouseObjects);
                    placed = true;
				}
            }
        }

        /*
        public void GenerateGiantTree(GenerationProgress progress, GameConfiguration configuration)
        {
            //tiles
            //0 = dont touch
            //1 = clear everything
            //2 = old wood
            //3 = wood platform
            //4 = dynasty shingles
            //5 = old wood wall
            //6 = spooky dirt
            //7 = living wood
            //8 = living wood wall
            //9 = living wood and wall

            //objects
            //0 = dont touch
            //1 = wooden beams
            //2 = pumpkin chandelier
            //3 = little bone
            //4 = candles

            int [,] GiantTree = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,7,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,7,7,0,0,0,0,7,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,7,0,0,0,0,0,0,0,0,0,7,7,0,0,0,7,7,7,0,0,0,0,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,1,7,7,0,0,0,0,0,0,0,0,7,7,7,0,7,7,7,0,0,0,0,0,7,7,1,1,1,0,1,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,1,1,7,7,7,7,0,0,0,0,0,0,7,7,7,7,7,7,0,0,0,0,0,7,7,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,1,1,1,1,1,1,1,1,7,7,7,7,7,7,0,0,7,7,7,7,7,7,0,0,0,7,7,7,7,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,7,7,7,7,7,7,7,0,7,7,7,7,7,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,7,7,7,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0},
                {0,0,0,1,1,1,1,1,1,1,1,1,1,1,7,7,1,1,7,7,7,7,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
                {0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,7,7,1,1,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
                {0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,7,1,1,1,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,8,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,7,8,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,8,8,7,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,7,7,8,8,8,7,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,1,1,1,1,1,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,7,7,7,8,8,8,1,1,1,1,7,1,1,1,1,7,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,1,1,1,1,1,1,1,7,7,7,1,1,1,7,1,1,1,1,1,1,7,7,8,8,8,8,1,1,1,1,7,7,1,1,7,7,7,1,1,1,1,1,1,1,1,1,1,0},
                {0,1,1,1,1,1,1,7,7,7,7,7,1,7,7,7,1,1,1,1,1,7,8,8,8,8,8,1,1,1,7,7,7,1,7,7,7,7,1,1,1,1,1,1,1,1,1,1,0},
                {0,1,1,1,1,1,1,7,7,1,7,7,7,7,7,7,7,1,1,1,1,1,8,8,8,8,8,1,1,7,7,7,7,7,7,7,7,7,7,1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,7,7,1,1,1,7,7,7,7,7,7,1,1,1,1,1,8,8,8,8,8,7,7,7,7,7,7,7,7,7,1,7,7,1,1,1,1,1,1,1,1,0,0},
                {1,1,1,1,6,6,7,7,1,1,1,7,7,7,1,7,7,1,1,1,1,1,8,8,8,8,8,7,7,7,7,1,7,7,7,1,1,7,7,1,1,1,1,1,1,1,1,0,0},
                {0,1,1,6,6,7,7,6,6,1,1,1,7,7,1,7,7,7,1,1,1,1,8,8,8,8,8,7,7,7,7,1,7,7,6,6,1,7,7,1,1,1,1,1,1,1,1,0,0},
                {0,6,6,6,7,7,6,6,6,6,6,6,7,7,1,7,7,7,1,7,7,7,8,8,8,8,8,7,7,7,7,6,7,7,7,6,6,6,7,7,6,1,1,1,1,1,1,0,0},
                {0,6,6,6,6,6,6,6,6,6,6,7,7,7,6,7,7,7,7,7,7,7,8,8,8,8,8,7,7,7,7,6,7,7,7,6,6,6,7,7,6,6,6,1,1,1,1,0,0},
                {0,6,6,6,6,6,6,6,6,6,6,7,7,7,6,7,7,7,7,7,7,7,8,8,8,8,8,7,7,7,7,6,6,7,7,6,6,6,6,7,7,6,6,6,1,1,0,0,0},
                {6,6,6,6,6,6,6,6,6,6,7,7,7,6,6,7,7,7,7,7,7,7,8,8,8,8,7,7,7,7,7,7,6,7,7,7,6,6,6,6,6,6,6,6,6,6,0,0,0},
                {6,6,6,6,6,6,6,6,6,7,7,7,7,6,6,6,7,7,7,7,7,7,8,8,8,8,7,7,7,7,7,7,6,6,7,7,6,6,6,6,6,6,6,6,6,6,6,6,0},
                {6,6,6,6,6,6,6,6,6,7,7,6,6,6,6,6,7,7,7,7,7,8,8,8,8,8,7,7,7,6,7,7,6,6,6,7,7,6,6,6,6,6,6,6,6,6,6,6,0},
                {0,0,6,6,6,6,6,6,6,7,7,6,6,6,6,6,7,7,6,7,7,8,8,8,8,8,7,7,7,6,7,7,6,6,6,7,7,6,6,6,6,6,6,6,6,6,6,6,0},
                {0,0,0,6,6,6,6,6,6,6,7,7,6,6,6,6,7,6,6,7,7,8,8,8,8,8,8,7,7,6,7,7,6,6,7,7,6,6,6,6,6,6,6,6,6,6,6,6,0},
                {0,0,6,6,6,6,6,6,6,6,6,7,6,6,6,6,7,6,6,7,8,8,8,8,8,8,8,7,7,6,6,7,6,6,7,6,6,6,6,6,6,6,6,6,6,6,6,0,0},
                {0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,7,8,8,8,8,8,8,8,8,7,6,6,7,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,8,6,8,8,8,6,8,8,7,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0},
                {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,8,6,6,6,8,6,6,8,7,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,8,6,6,6,8,6,6,8,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6},
                {0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0},
                {0,0,0,0,0,0,0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0},
            };

            int [,] GiantTreeObjects = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int [,] TreeTunnel1 = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,9,9,9,8,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,9,9,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,9,9,8,8,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,9,8,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,8,9,9,9,9,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,8,8,9,9,8,8,8,8,8,8,8,8,9,9,9,9,9,0,9,9,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,8,9,8,8,8,8,8,8,8,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,9,9,9,9,9,8,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,9,9,8,8,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int [,] TreeTunnel2 = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,9,9,9,9,9,9,9,8,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,8,8,9,9,9,9,9,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,8,8,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,9,9,9,9,8,8,8,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,9,9,9,8,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,8,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,8,8,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,8,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,8,8,9,9,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int [,] TreeTunnelLeft = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,0,0,0,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,9,9,9,0,0,9,9,0,0,0,0},
                {0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,9,9,0},
                {0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,9,9,9,9,9,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
                {0,0,9,9,9,9,9,9,9,9,9,9,9,9,8,8,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,8,8,9,9,9,9,9,9,9,9},
                {0,9,9,9,9,9,9,8,8,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,8,8,8,8,9,9,9,9,9,9},
                {9,9,9,9,9,9,8,8,8,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
                {9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9},
                {9,9,9,9,9,9,8,8,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,8,8,8,8,9,9,8,8,9,9,9,9,9,9,9},
                {0,9,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,8,8,9,9,9,9,9,9,9,9,9,9,9,9,9},
                {0,0,9,9,9,9,9,9,9,9,9,9,8,9,9,8,8,8,8,9,9,9,9,9,9,8,8,8,9,9,8,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0},
                {0,0,0,0,9,9,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,9,0,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,0,0,9,9,9,0,0,0,0,0,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int [,] TreeTunnelRight = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,0,0,0,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,9,9,9,0,0,9,9,0,0,0,0},
                {0,0,9,9,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,9,9,0},
                {9,9,9,9,0,9,9,9,9,9,9,9,9,8,8,9,9,9,9,9,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9},
                {9,9,9,9,9,9,9,9,9,9,9,9,9,9,8,8,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,8,8,9,9,9,9,9,9,9,9},
                {9,9,9,9,9,9,9,8,8,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,8,8,8,8,9,9,9,9,9,9},
                {8,8,8,9,9,9,8,8,8,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9},
                {9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9},
                {9,9,9,9,9,9,8,8,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,8,8,8,8,9,9,8,8,9,9,9,9,9,9,0},
                {9,9,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,8,8,9,9,9,9,9,9,9,9,9,9,9,0,0},
                {0,0,9,9,9,9,9,9,9,9,9,9,8,9,9,8,8,8,8,9,9,9,9,9,9,8,8,8,9,9,8,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0},
                {0,0,0,9,9,9,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,9,0,9,0,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,9,9,9,0,0,9,9,9,0,0,0,0,0,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int [,] TreeTunnelConnector = new int [,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,9,9,9,9,9,9,8,9,9,9,9,9,9,9,9,8,8,8,8,8,9,9,9,9,8,8,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0,0},
                {0,0,0,0,0,9,9,9,9,9,9,9,9,8,8,9,9,9,9,9,8,8,8,8,8,8,8,8,8,9,9,9,8,8,9,9,9,9,9,9,9,9,9,9,9,9,9,0,0},
                {0,0,0,9,9,9,9,9,9,9,9,9,9,9,8,8,9,8,8,8,8,8,8,8,8,8,8,8,8,8,9,8,8,8,9,9,9,9,9,8,8,9,9,9,9,9,9,9,0},
                {0,9,9,9,9,9,9,8,8,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,8,8,8,8,9,9,9,9,9,9},
                {9,9,9,9,9,9,8,8,8,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9},
                {9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9},
                {9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9},
                {9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9},
                {9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,8,8,9,9,9,9,9,9,0},
                {0,9,9,0,0,9,9,9,9,9,9,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,9,0,0},
                {0,0,0,0,0,0,0,9,9,9,9,9,8,8,9,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,9,9,9,9,0,0,0},
                {0,0,0,0,0,0,0,0,9,9,9,9,9,8,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,9,9,9,8,8,9,9,9,9,8,8,8,8,8,8,8,8,9,9,9,9,8,8,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,9,9,9,8,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,8,8,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,9,9,9,9,9,9,9,9,9,9,8,8,8,8,8,8,8,9,9,9,9,9,9,9,9,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int [,] TreeTunnelObjects = new int [,]
            {
                {0},
            };

            bool placed = false;
            int attempts = 0;

            int TreeX = PositionX - ((Main.maxTilesX / 12) / 3);
            int TreeY = PositionY;

            while (!placed && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(TreeX, TreeY) && TreeY <= Main.worldSurface)
				{
					TreeY++;
				}

                if (Main.tile[TreeX, TreeY].TileType != ModContent.TileType<SpookyDirt>() ||
                Main.tile[TreeX, TreeY].TileType != ModContent.TileType<SpookyDirt2>() ||
                Main.tile[TreeX, TreeY].TileType != ModContent.TileType<SpookyGrass>() ||
                Main.tile[TreeX, TreeY].TileType != ModContent.TileType<SpookyGrassGreen>())
				{
					PlaceStructures(TreeX, TreeY - 30, GiantTree, GiantTreeObjects);
                    placed = true;
				}
            }

            //place the tree tunnel
            for (int Y = TreeY; Y <= (int)Main.worldSurface; Y += 20)
            {
                if (Y == TreeY + 40)
                {
                    PlaceStructures(TreeX,  Y + 5, TreeTunnelConnector, TreeTunnelObjects);
                    PlaceStructures(TreeX - 47, Y + 5, TreeTunnelLeft, TreeTunnelObjects);
                }
                else if (Y == TreeY + 80)
                {
                    PlaceStructures(TreeX,  Y + 5, TreeTunnelConnector, TreeTunnelObjects);
                    PlaceStructures(TreeX + 47,  Y + 5, TreeTunnelRight, TreeTunnelObjects);
                }
                else
                {
                    switch (WorldGen.genRand.Next(2))
                    {
                        case 0:
                        {
                            PlaceStructures(TreeX + WorldGen.genRand.Next(-1, 1),  Y + 5, TreeTunnel1, TreeTunnelObjects);
                            break;
                        }
                        case 1:
                        {
                            PlaceStructures(TreeX + WorldGen.genRand.Next(-1, 1),  Y + 5, TreeTunnel2, TreeTunnelObjects);
                            break;
                        }
                    }
                }
            }

            //dig caves at the bottom of the tree tunnel
            TileRunner runner = new TileRunner(new Vector2((TreeX + 20) + WorldGen.genRand.Next(-1, 1), (int)Main.worldSurface + 20), 
            new Vector2(0, 5), new Point16(-12, 12), new Point16(-12, 12), 15f, Main.rand.Next(25, 50), 0, false, true);
            runner.Start();
        }
        */

        private void PlaceLootStructures(int X, int Y, int[,] BlocksArray, int[,] ObjectArray)
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
                            //old wood
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
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.Platforms);
                                break;
                            }
                            //old wood wall
                            case 4:
                            {
                                tile.ClearTile();
                                WorldGen.KillWall(StructureX, StructureY);
					            WorldGen.PlaceWall(StructureX, StructureY, ModContent.WallType<SpookyWoodWall>());
                                break;
                            }
                            //moss stone
                            case 5:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyStone>());
                                tile.HasTile.Equals(true);
                                break;
                            }
                            //spooky dirt
                            case 6:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceTile(StructureX, StructureY, ModContent.TileType<SpookyDirt2>());
                                tile.HasTile.Equals(true);
                                break;
                            }
                            //water
                            case 7:
                            {
                                tile.ClearTile();
                                tile.LiquidType = LiquidID.Water;
					            tile.LiquidAmount = 255;
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
                            //wood beams
                            case 1:
                            {
                                tile.ClearTile();
					            WorldGen.PlaceTile(StructureX, StructureY, TileID.WoodenBeam);
                                break;
                            }
                            //candles
                            case 2:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, ModContent.TileType<Candle>());
                                break;
                            }
                            //lanterns
                            case 3:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceObject(StructureX, StructureY, 42, true, 3);
                                break;
                            }
                            //chest
                            case 4:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest>(), false, 0);
                                break;
                            }
                            //chest
                            case 5:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest2>(), false, 0);
                                break;
                            }
                            //chest
                            case 6:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest3>(), false, 0);
                                break;
                            }
                            //chest
                            case 7:
                            {
                                tile.ClearTile();
                                WorldGen.PlaceChest(StructureX, StructureY, (ushort)ModContent.TileType<HalloweenChest4>(), false, 0);
                                break;
                            }
                            //chest
                            case 8:
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

        public void GenerateLootStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            //tiles
            //0 = dont touch
            //1 = clear everything
            //2 = old wood
            //3 = wooden platform
            //4 = old wood wall
            //5 = moss stone
            //6 = spooky dirt
            //7 = water

            //objects
            //0 = dont touch
            //1 = wood beams
            //2 = candles
            //3 = lanterns
            //4-8 = halloween chest
            
            //first loot room
            int[,] LootRoom1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,2,2,2,2,2,2,2,0,0},
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,0},
                {0,0,0,1,1,1,1,1,1,1,1,1,1,2,2,4,1,1,1,1,1,4,1,4,2,2},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,4,1,4,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,4,4,4,4,4,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,4,1,1,4,4,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,4,1,1,1,4,1,1,1,4,1,1},
                {1,1,1,1,6,6,6,1,1,1,1,1,1,1,1,4,1,1,1,4,1,1,1,2,2,2},
                {0,1,1,6,6,6,6,6,6,3,3,3,3,3,2,2,2,2,3,3,3,2,2,2,2,0},
                {0,0,6,6,6,6,6,6,6,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,0,6,6,6,6,6,6,6,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,0,6,6,6,6,6,6,6,6,6,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,6,6,6,6,6,6,6,6,6,6,1,1,1,1,1,1,1,1,1,1,1,5,5,5,0},
                {0,6,6,6,6,6,6,6,6,6,6,6,1,1,1,1,1,5,5,5,1,5,5,5,5,5},
                {0,6,6,6,6,6,6,6,6,6,6,6,1,1,1,1,5,5,5,5,5,5,5,5,5,5},
                {0,6,6,6,6,6,6,6,6,6,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
                {0,0,0,6,6,6,6,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
                {0,0,0,0,0,6,6,6,6,5,5,5,5,5,5,5,0,0,5,5,5,5,5,5,0,0},
            };

            int[,] LootRoomObjects1 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,1,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,1,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //second loot room
            int[,] LootRoom2 = new int[,]
            {
                {0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,1,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0},
                {0,0,0,0,0,1,1,1,1,1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,0},
                {0,0,0,1,1,1,1,5,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,0,0,0},
                {0,0,1,1,1,1,1,5,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,1,1,1,1,5,1,1,1,1,1,1,1},
                {0,0,1,1,1,1,1,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,5,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,5,5,2,2,2,3,3,2,2,2,3,1,1,1,1,1,1,1,1,1,5,5,1,1,1,1,1,1},
                {1,1,1,1,1,5,1,5,5,1,1,5,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,5,5,1,5,1,1,1,1},
                {1,1,1,1,1,5,1,5,5,1,5,5,1,1,1,4,1,1,1,1,1,1,1,1,1,1,1,5,5,1,5,1,1,1,1},
                {1,1,1,1,5,5,1,5,5,5,5,5,1,1,1,4,4,4,1,1,3,2,2,2,3,3,5,5,5,1,5,1,1,1,1},
                {0,1,1,1,5,5,1,5,5,5,5,5,4,4,4,4,1,4,4,4,1,1,4,1,1,1,5,5,5,5,5,1,1,1,0},
                {0,0,1,1,5,5,5,5,5,5,5,5,5,1,1,4,1,1,1,4,4,4,4,4,4,4,5,5,5,5,5,5,1,0,0},
                {0,0,0,1,5,5,5,5,5,5,5,5,5,1,1,4,1,1,1,1,1,1,4,1,1,5,5,5,5,5,5,5,1,0,0},
                {0,0,0,5,5,5,5,5,5,5,5,5,5,1,1,4,1,1,1,1,1,1,4,1,1,5,5,5,5,5,5,5,5,0,0},
                {0,0,0,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,2,2,2,2,2,2,5,5,5,5,5,5,5,5,0,0},
                {0,0,0,0,5,5,5,5,5,5,5,5,5,5,2,2,2,2,2,2,2,2,2,2,5,5,5,5,5,5,5,5,5,0,0},
                {0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0,0,0,0,0,0},
            };

            int[,] LootRoomObjects2 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //third loot room
            int[,] LootRoom3 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,0,0,0,1,1,1,0,0,0,0,1,1,1,1,0,0,0},
                {0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0},
                {0,0,1,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,5,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,5,5,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,5,1,1,1,1},
                {0,1,1,1,5,5,1,5,5,5,3,3,3,2,2,2,3,3,3,3,3,3,2,2,2,3,3,3,5,5,5,5,1,1,0},
                {0,0,0,1,5,5,5,5,5,5,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,5,5,5,5,1,1,0},
                {0,0,0,5,5,5,5,5,5,5,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,0,0},
                {0,0,0,5,5,5,5,5,5,5,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,0,0},
                {0,0,0,5,5,5,5,5,5,5,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,5,0,0},
                {0,0,0,0,5,5,5,5,5,5,5,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,5,0,0},
                {0,0,0,0,5,5,5,5,5,5,5,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,5,5,0,0},
                {0,0,0,0,0,5,5,5,5,5,5,5,7,7,7,7,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,5,0,0,0},
                {0,0,0,0,0,0,0,5,5,5,5,5,5,5,7,7,7,7,7,7,7,7,7,7,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,7,7,7,7,7,7,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,5,5,5,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,5,5,0,0,5,5,5,5,0,0,5,5,5,0,0,0,0,0,0,0,0,0},
            };

            int[,] LootRoomObjects3 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,0,0,0,6,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //fourth loot room
            int[,] LootRoom4 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,5,5,0,0,0,5,5,5,5,5,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0,0,0,0},
                {0,0,0,0,0,1,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,5,5,0},
                {0,0,0,1,1,1,1,1,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5},
                {0,0,1,1,1,1,1,1,5,5,2,2,2,5,5,5,5,2,2,2,5,5,5,5,5,5,5,5},
                {0,0,1,1,1,1,1,1,1,2,2,4,2,2,2,2,2,2,4,2,2,5,5,5,5,5,5,5},
                {1,0,1,1,1,1,1,1,1,1,1,4,4,1,4,1,4,4,4,1,2,2,5,5,5,5,5,0},
                {1,1,1,1,1,1,1,1,1,1,1,4,4,4,4,1,4,1,4,1,2,2,5,5,5,5,5,0},
                {1,1,1,1,1,1,1,1,1,1,4,4,1,1,4,4,4,1,4,1,2,2,5,5,5,5,5,0},
                {1,1,1,1,1,1,1,1,1,1,4,1,1,1,1,1,4,1,4,1,2,2,5,5,5,5,0,0},
                {0,1,1,1,1,1,1,1,1,1,4,1,2,2,2,2,2,2,2,2,2,2,5,5,5,5,0,0},
                {0,1,6,6,1,1,1,1,1,1,2,2,2,2,2,2,2,2,2,2,2,5,5,5,5,5,5,0},
                {0,6,6,6,6,6,2,2,2,2,2,2,2,6,6,6,6,5,5,5,5,5,5,5,5,5,5,0},
                {0,6,6,6,6,6,6,2,2,2,2,6,6,6,6,6,6,6,6,5,5,5,5,5,5,5,0,0},
                {0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,5,5,5,0,0,0,0},
                {0,0,0,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,6,6,6,6,6,6,6,6,6,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            int[,] LootRoomObjects4 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,3,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,7,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //fifth loot room
            int[,] LootRoom5 = new int[,]
            {
                {0,0,0,0,0,5,5,5,0,0,5,5,5,5,5,0,0,0,0,5,5,5,5,5,0,0,0,0},
                {0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0},
                {0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0},
                {0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,0,1,1,5,5,1,1,1,1,1,5,5,5,5,1,1,1,1,5,5,5,1,1,1,0,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {0,1,1,1,1,1,1,3,2,2,2,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0},
                {1,1,1,1,1,1,1,1,2,2,2,2,3,3,3,2,2,2,2,2,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {0,0,1,1,5,5,5,5,2,2,2,5,5,5,5,5,5,2,2,2,5,5,5,5,1,1,1,0},
                {0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0,0},
                {0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0},
                {0,0,0,0,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,0,0},
                {0,0,0,0,0,0,5,5,0,0,5,5,5,5,5,0,0,0,5,5,0,0,5,5,0,0,0,0},
            };

            int[,] LootRoomObjects5 = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,3,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,8,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,3,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,2,0,0,0,1,0,0,0,0,0,0,0,0,1,0,0,2,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
            };

            //how much distance should be inbetween each loot room
            int ChestDistance = (Main.maxTilesX / 82);

            //depth of each loot room
            int InitialDepth = (int)Main.worldSurface + (Main.maxTilesY / 30);
            int ChestDepth = (Main.maxTilesY / 15) / 2;

            //actual loot room positions
            int ChestX = PositionX;
            int ChestY = InitialDepth + (ChestDepth + 35);

            //reset ChestY each time so each room is at a different position
            ChestY = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            PlaceLootStructures(ChestX - (ChestDistance * 2), ChestY, LootRoom4, LootRoomObjects4);

            ChestY = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            PlaceLootStructures((ChestX - ChestDistance) - 8, ChestY, LootRoom2, LootRoomObjects2);

            ChestY = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            PlaceLootStructures(ChestX, ChestY + 15, LootRoom1, LootRoomObjects1);

            ChestY = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            PlaceLootStructures(ChestX + ChestDistance, ChestY, LootRoom3, LootRoomObjects3);

            ChestY = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            PlaceLootStructures(ChestX + (ChestDistance * 2), ChestY, LootRoom5, LootRoomObjects5);
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
            //generate biome
			int SpookyForestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Pyramids"));
			if (SpookyForestIndex == -1)
			{
				return;
			}

            tasks.Insert(SpookyForestIndex + 1, new PassLegacy("SpookyForest", GenerateSpookyForest));
            tasks.Insert(SpookyForestIndex + 2, new PassLegacy("SpookyLoot", GenerateLootStructures));
            tasks.Insert(SpookyForestIndex + 3, new PassLegacy("SpookyHouse", GenerateStarterHouse));

            //place objects and grow trees
            int SpookyAmbientIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Grass Wall"));
			if (SpookyAmbientIndex == -1)
			{
                return;
            }

            tasks.Insert(SpookyAmbientIndex + 1, new PassLegacy("SpookyTrees", GrowSpookyTrees));
            tasks.Insert(SpookyAmbientIndex + 2, new PassLegacy("SpookyTrees", GrowSpookyTrees));
            tasks.Insert(SpookyAmbientIndex + 3, new PassLegacy("SpookyTrees", GrowSpookyTrees));
            tasks.Insert(SpookyAmbientIndex + 4, new PassLegacy("SpookyTrees", GrowSpookyTrees));
            tasks.Insert(SpookyAmbientIndex + 5, new PassLegacy("SpookyTrees", GrowSpookyTrees));
            tasks.Insert(SpookyAmbientIndex + 6, new PassLegacy("SpookyGrass", SpreadSpookyGrass));
            tasks.Insert(SpookyAmbientIndex + 7, new PassLegacy("SpookyAmbience", SpookyForestAmbience));
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
                                chest.item[0].SetDefaults(ModContent.ItemType<CandyBag>());
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest2>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<LeafBlower>());
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest3>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<ToiletPaper>());
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest4>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<CreepyCandle>());
                                chest.item[0].stack = 1;
                            }
                            if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest5>())
                            {
                                chest.item[0].SetDefaults(ModContent.ItemType<NecromancyTome>());
                                chest.item[0].stack = 1;
                            }

                            //iron or lead bars
							chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
							chest.item[1].stack = WorldGen.genRand.Next(3, 8);
                            //light sources
                            chest.item[2].SetDefaults(WorldGen.genRand.Next(LightSources));
							chest.item[2].stack = WorldGen.genRand.Next(10, 35);
                            //potions
							chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions));
							chest.item[3].stack = WorldGen.genRand.Next(2, 3);
                            //goodie bags
							chest.item[4].SetDefaults(ItemID.GoodieBag);
							chest.item[4].stack = WorldGen.genRand.Next(1, 2);
                            //pumpkin seeds or cobwebs
							chest.item[5].SetDefaults(WorldGen.genRand.Next(Misc));
							chest.item[5].stack = WorldGen.genRand.Next(2, 8);
                            //coins
                            chest.item[6].SetDefaults(ItemID.SilverCoin);
							chest.item[6].stack = WorldGen.genRand.Next(2, 25);
						}
					}
                }
            }
        }
    }
}