using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyBiome.Tree;
using Spooky.Content.NPCs.Friendly;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class SpookyForest : ModSystem
    {
        //default positions, edit based on worldsize below
        static int PositionX = Main.maxTilesX / 2;
        static int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

        Vector2 SaveHousePosition;

        private void GenerateSpookyForest(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.SpookyForest").Value;

            //decide whether or not to use the alt background
            if (Main.rand.NextBool(2))
            {
                Flags.SpookyBackgroundAlt = false;
            }
            else
            {
                Flags.SpookyBackgroundAlt = true;
            }

            //if config is enabled, place it at spawn
            if (ModContent.GetInstance<SpookyConfig>().SpookyForestSpawn)
            {
                PositionX = Main.maxTilesX / 2;
            }
            //otherwise place it off to the side of the snow biome
            else
            {
                if (GenVars.dungeonSide == -1)
                {
                    PositionX = GenVars.snowOriginLeft - (Main.maxTilesX / 10);
                }
                else
                {
                    PositionX = GenVars.snowOriginRight + (Main.maxTilesX / 15);
                }
            }

            //set y position again so it is always correct before placing
            PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            //set size and height
            int Size = Main.maxTilesX / 15;
            int BiomeHeight = Main.maxTilesY / 10;

            //place the actual biome
            for (int Y = 0; Y < BiomeHeight; Y += 50)
            {
                //loop to make the sides of the spooky forest more smooth
                for (int cutOff = 0; cutOff < Main.maxTilesX / 28; cutOff += 50)
                {
                    SpookyWorldMethods.TileRunner(PositionX, PositionY + Y + cutOff, (double)Size + Y / 2, 1, ModContent.TileType<SpookyDirt>(), 
                    ModContent.WallType<SpookyGrassWall>(), 0, true, 0f, 0f, true, true, true, true, true);
                }
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

            for (int gold = 0; gold < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 5E-05); gold++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile && Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>()) 
                {
                    WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(2, 8), WorldGen.genRand.Next(3, 6), TileID.Gold, false, 0f, 0f, false, true);
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

                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>() && (!up.HasTile || !down.HasTile || !left.HasTile || !right.HasTile))
                    {
                        Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyGrass>();
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt2>() && (!up.HasTile || !down.HasTile || !left.HasTile || !right.HasTile))
                    {
                        Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt>() &&
                    (up.TileType == ModContent.TileType<SpookyGrass>() || down.TileType == ModContent.TileType<SpookyGrass>() || 
                    left.TileType == ModContent.TileType<SpookyGrass>() || right.TileType == ModContent.TileType<SpookyGrass>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyDirt>(), ModContent.TileType<SpookyGrass>(), false);
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt2>() &&
                    (up.TileType == ModContent.TileType<SpookyGrassGreen>() || down.TileType == ModContent.TileType<SpookyGrassGreen>() || 
                    left.TileType == ModContent.TileType<SpookyGrassGreen>() || right.TileType == ModContent.TileType<SpookyGrassGreen>()))
                    {
                        WorldGen.SpreadGrass(X, Y, ModContent.TileType<SpookyDirt2>(), ModContent.TileType<SpookyGrassGreen>(), false);
                    }
                }
            }
        }

        private void GrowSpookyTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            //grow trees
            for (int X = PositionX - 500; X <= PositionX + 500; X++)
            {
                //regular surface trees
                for (int Y = 0; Y < (int)Main.worldSurface - 50; Y++)
                {
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrass>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrassGreen>())
                    {
                        WorldGen.GrowTree(X, Y - 1);
                    }
                }

                //grow giant mushrooms
                for (int Y = (int)Main.worldSurface + 25; Y < Main.maxTilesY - 100; Y++)
                {
                    if ((Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyGrassGreen>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyStone>()) &&
                    !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                    {
                        if (WorldGen.genRand.NextBool(18))
                        {
                            GrowGiantMushroom(X, Y, ModContent.TileType<GiantShroom>());
                        }
                    }
                }
            }
        }

        public static bool GrowGiantMushroom(int X, int Y, int tileType)
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

            GiantShroom.Grow(X, Y - 1, 5, 8, false);

            return true;
        }

        private void SpookyForestAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            //place ambient objects
            for (int X = PositionX - 500; X < PositionX + 500; X++)
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

                    //convert leftover green grass dirt back into regular dirt
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyDirt2>())
                    {
                        Main.tile[X, Y].TileType = (ushort)ModContent.TileType<SpookyDirt>();
                    }

                    //orange spooky vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(2))
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
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<SpookyVinesGreen>());
                        }
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyVinesGreen>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<SpookyVinesGreen>());
                    }

                    //place orange grass only on orange grass
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrass>())
                    {
                        //pumpkins
                        if (WorldGen.genRand.NextBool(7))
                        {
                            ushort[] Pumpkins = new ushort[] { (ushort)ModContent.TileType<SpookyPumpkin1>(), 
                            (ushort)ModContent.TileType<SpookyPumpkin2>(), (ushort)ModContent.TileType<SpookyPumpkin3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Pumpkins));    
                        }
                    }
                }
            }

            //place stuff underground
            for (int X = PositionX - 500; X < PositionX + 500; X++)
            {
                for (int Y = (int)Main.worldSurface; Y < (int)Main.worldSurface + 250; Y++)
                { 
                    if (Main.tile[X, Y].TileType == ModContent.TileType<SpookyGrassGreen>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<SpookyStone>())
                    {
                        //hanging glow vines
                        if (WorldGen.genRand.NextBool(8))
                        {    
                            ushort[] Vines = new ushort[] { (ushort)ModContent.TileType<HangingVine1>(), 
                            (ushort)ModContent.TileType<HangingVine2>(), (ushort)ModContent.TileType<HangingVine3>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(Vines));           
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
                int x = PositionX + ((Main.maxTilesX / 12) / 6); //get the biomes size, then divide that more and get the distance from the center
                int y = PositionY; //start here to not touch floating islands

                while (!WorldGen.SolidTile(x, y) && y <= Main.worldSurface)
				{
					y++;
				}

                if (Main.tile[x, y].HasTile)
				{
                    Vector2 origin = new Vector2(x - 10, y - 18);

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
                    Generator.GenerateStructure("Content/Structures/SpookyForestHouse", origin.ToPoint16(), Mod);

                    //place little bone in the house
                    NPC.NewNPC(null, (x + 1) * 16, (y - 6) * 16, ModContent.NPCType<LittleBoneSleeping>(), 0, 0f, 0f, 0f, 0f, 255);

                    SaveHousePosition = origin;

                    placed = true;
				}
            }
        }

        public void GenerateStarterHouseAgain(GenerationProgress progress, GameConfiguration configuration)
        {
            Generator.GenerateStructure("Content/Structures/SpookyForestHouse", SaveHousePosition.ToPoint16(), Mod);
        }

        public void GenerateUndergroundCabins(GenerationProgress progress, GameConfiguration configuration)
        {
            //how much distance should be inbetween each loot room
            int ChestDistance = (Main.maxTilesX / 75);

            //depth of each loot room
            int InitialDepth = (int)Main.worldSurface + (Main.maxTilesY / 28);
            int ChestDepth = (Main.maxTilesY / 15) / 2;

            //actual loot room positions
            int x = PositionX;
            int y = InitialDepth + (ChestDepth + 35);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            Vector2 origin1 = new Vector2((x - (ChestDistance * 2)) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyForestCabin-1", origin1.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            Vector2 origin2 = new Vector2(((x - ChestDistance) - 8) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyForestCabin-2", origin2.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            Vector2 origin3 = new Vector2(x - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyForestCabin-3", origin3.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            Vector2 origin4 = new Vector2((x + ChestDistance) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyForestCabin-4", origin4.ToPoint16(), Mod);

            //reset y each time so each room is at a different position
            y = InitialDepth + WorldGen.genRand.Next(-ChestDepth, ChestDepth + 65);
            Vector2 origin5 = new Vector2((x + (ChestDistance * 2)) - 10, y - 6);
            Generator.GenerateStructure("Content/Structures/SpookyForestCabin-5", origin5.ToPoint16(), Mod);

            //lock all spooky wood chests
            for (int X = PositionX - 500; X <= PositionX + 500; X++)
			{
                for (int Y = PositionY - 100; Y <= Main.maxTilesY - 100; Y++)
				{
                    //check for the top left frame of the chest
                    if (Main.tile[X, Y].TileType == ModContent.TileType<HalloweenChest>() && //top left
                    Main.tile[X + 1, Y].TileType == ModContent.TileType<HalloweenChest>() && //top right
                    Main.tile[X, Y + 1].TileType == ModContent.TileType<HalloweenChest>() && //bottom left
                    Main.tile[X + 1, Y + 1].TileType == ModContent.TileType<HalloweenChest>()) //bottom right
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
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
            //generate biome
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("SpookyForest", GenerateSpookyForest));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("SpookyHouse", GenerateStarterHouse));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("SpookyGrass", SpreadSpookyGrass));

            //place objects and grow trees
            int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Guide"));
			if (GenIndex2 == -1)
			{
                return;
            }

            tasks.Insert(GenIndex2 + 1, new PassLegacy("SpookyTrees", GrowSpookyTrees));
            tasks.Insert(GenIndex2 + 2, new PassLegacy("SpookyAmbience", SpookyForestAmbience));

            //place house again because stupid ahh walls
            int GenIndex3 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (GenIndex3 == -1)
			{
                return;
            }

            tasks.Insert(GenIndex3 + 1, new PassLegacy("SpookyHouseAgain", GenerateStarterHouseAgain));
            tasks.Insert(GenIndex3 + 2, new PassLegacy("SpookyCabins", GenerateUndergroundCabins));
            tasks.Insert(GenIndex3 + 3, new PassLegacy("SpookyGrass", SpreadSpookyGrass));
        }

        //post worldgen to place items in the spooky biome chests
        public override void PostWorldGen()
		{
            int[] Bars = new int[] { ItemID.SilverBar, ItemID.TungstenBar, ItemID.GoldBar, ItemID.PlatinumBar };
            int[] LightSources = new int[] { ItemID.OrangeTorch, ModContent.ItemType<CandleItem>() };
            int[] Potions = new int[] { ItemID.LesserHealingPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };
            int[] Misc = new int[] { ItemID.PumpkinSeed, ItemID.Cobweb };

            for (int chestIndex = 0; chestIndex < 1000; chestIndex++) 
            {
                Chest chest = Main.chest[chestIndex]; 

                if (chest != null && Main.tile[chest.x, chest.y].TileType == ModContent.TileType<HalloweenChest>() && Main.tile[chest.x, chest.y].TileFrameX == 36)
                {
                    for (int inventoryIndex = 0; inventoryIndex < 5; inventoryIndex++) 
                    {
                        if (chest.item[inventoryIndex].type == ItemID.None) 
                        {
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
                            //pumpkin seeds or cobwebs
                            chest.item[5].SetDefaults(WorldGen.genRand.Next(Misc));
                            chest.item[5].stack = WorldGen.genRand.Next(5, 10);
                            //coins
                            chest.item[6].SetDefaults(ItemID.GoldCoin);
                            chest.item[6].stack = WorldGen.genRand.Next(1, 2);
                        }
                    }
                }
            }
        }
    }
}