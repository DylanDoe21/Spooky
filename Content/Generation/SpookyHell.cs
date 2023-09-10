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

        //clear area for the biome to generate in
        private void ClearArea(GenerationProgress progress, GameConfiguration configuration)
        {
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
        }

        private void GenerateSpookyHell(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.EyeValley").Value;

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

            //place clumps of blocks along the edges of the biome so it doesnt look weird
            for (int X = StartPosition - 50; X <= StartPosition; X++)
            {
                for (int Y = Main.maxTilesY - 110; Y < Main.maxTilesY - 20; Y++)
                {
                    if (WorldGen.genRand.NextBool(30))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), WorldGen.genRand.Next(5, 20), true, true);
                    }
                }
            }
            for (int X = BiomeEdge; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 110; Y < Main.maxTilesY - 20; Y++)
                {
                    if (WorldGen.genRand.NextBool(30))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), WorldGen.genRand.Next(5, 20), true, true);
                    }
                }
            }

            //place ceiling across the top of the biome
            for (int X = StartPosition - 50; X <= BiomeEdge + 50; X++)
            {
                for (int Y = Main.maxTilesY - 215; Y <= Main.maxTilesY - 192; Y++)
                {
                    if (WorldGen.genRand.NextBool(15))
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<SpookyMush>(), WorldGen.genRand.Next(3, 5), true, true);
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
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>())
                    {
                        if (WorldGen.genRand.NextBool(20))
                        {
                            PlaceTree(X, Y, ModContent.TileType<EyeTree>());
                        }
                    }

                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        if (WorldGen.genRand.NextBool(10) && Main.tile[X, Y - 1].WallType <= 0 &&
                        !Main.tile[X, Y].LeftSlope && !Main.tile[X, Y].RightSlope && !Main.tile[X, Y].IsHalfBlock)
                        {
                            PlaceTree(X, Y, ModContent.TileType<EyeTree>());
                        }
                    }
                }
            }
        }

        public static bool PlaceTree(int X, int Y, int tileType)
        {
            int minDistance = 5;
            int treeNearby = 0;

            for (int i = X - minDistance; i < X + minDistance; i++)
            {
                for (int j = Y - minDistance; j < Y + minDistance; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        treeNearby++;
                        if (treeNearby > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            EyeTree.Grow(X, Y - 1, 12, 35, false);

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
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>() ||
                    Main.tile[X, Y].TileType == (ushort)ModContent.TileType<EyeBlock>())
                    {
                        //eye bushes
                        if (WorldGen.genRand.NextBool(8))
                        {
                            ushort[] EyeBushes = new ushort[] { (ushort)ModContent.TileType<EyeBush1>(), (ushort)ModContent.TileType<EyeBush2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeBushes));    
                        }

                        //tall eye stalks
                        if (WorldGen.genRand.NextBool(10))
                        {
                            ushort[] EyeStalks = new ushort[] { (ushort)ModContent.TileType<TallEyeStalk1>(), 
                            (ushort)ModContent.TileType<TallEyeStalk2>(), (ushort)ModContent.TileType<TallEyeStalk3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeStalks)); 
                        }

                        //eye piles
                        if (WorldGen.genRand.NextBool(10))
                        {
                            ushort[] EyePiles = new ushort[] { (ushort)ModContent.TileType<EyePile1>(), (ushort)ModContent.TileType<EyePile2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyePiles));   
                        }
                    }

                    //mush grass plants
                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<SpookyMushGrass>())
                    {
                        //eye flowers
                        if (WorldGen.genRand.NextBool(10))
                        {
                            ushort[] EyeFlowers = new ushort[] { (ushort)ModContent.TileType<EyeFlower1>(), (ushort)ModContent.TileType<EyeFlower2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeFlowers));
                        }

                        //tendrils
                        if (WorldGen.genRand.NextBool(10))
                        {
                            ushort[] EyeFlowers = new ushort[] { (ushort)ModContent.TileType<Tentacle1>(), (ushort)ModContent.TileType<Tentacle2>(),
                            (ushort)ModContent.TileType<Tentacle3>(), (ushort)ModContent.TileType<Tentacle4>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(EyeFlowers)); 
                        }

                        //fingers
                        if (WorldGen.genRand.NextBool(8))
                        {
                            ushort[] Fingers = new ushort[] { (ushort)ModContent.TileType<Finger1>(), (ushort)ModContent.TileType<Finger2>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Fingers));    
                        }

                        //hanging fingers
                        if (WorldGen.genRand.NextBool(8))
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
                        if (WorldGen.genRand.NextBool(4))
                        {
                            ushort[] Arteries = new ushort[] { (ushort)ModContent.TileType<Artery1>(), 
                            (ushort)ModContent.TileType<Artery2>(), (ushort)ModContent.TileType<Artery3>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Arteries));    
                        }

                        //bone piles
                        if (WorldGen.genRand.NextBool(4))
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
            //define the center of the biome
            int XMiddle = (StartPosition + BiomeEdge) / 2;

            int StartPosY = Main.maxTilesY - 160;

            //place first flesh pillar
            GenerateStructure(StartPosition + 150, StartPosY, "FleshPillar-1", 16, 32);

            ///place little eye's house
            int HouseX = (GenVars.JungleX > Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 : (XMiddle + BiomeEdge) / 2;
            GenerateStructure(HouseX, StartPosY, "LittleEyeHouse", 23, 18);

            //place second flesh pillar
            GenerateStructure(XMiddle - 150, StartPosY, "FleshPillar-2", 15, 38);

            //place orroboro nest
            GenerateStructure(XMiddle, StartPosY, "OrroboroNest", 6, 16);

            //place third flesh pillar
            GenerateStructure(XMiddle + 150, StartPosY, "FleshPillar-3", 15, 29);

            //place moco shrine
            int ShrineX = (GenVars.JungleX < Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 - 45 : (XMiddle + BiomeEdge) / 2 - 45;
            GenerateStructure(ShrineX, StartPosY, "MocoShrine", 19, 18);

            //place blood lake
            int LakeX = (GenVars.JungleX < Main.maxTilesX / 2) ? (StartPosition + XMiddle) / 2 + 75 : (XMiddle + BiomeEdge) / 2 + 75;
            GenerateStructure(LakeX, StartPosY, "BloodLake", 47, 30);

            //place fourth flesh pillar
            GenerateStructure(BiomeEdge - 150, StartPosY, "FleshPillar-4", 15, 39);

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
        }

        //method for finding a valid surface and placing the structure on it
        public void GenerateStructure(int startX, int startY, string StructureFile, int offsetX, int offsetY)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(startX, startY) && startY <= Main.maxTilesY)
				{
					startY++;
				}
                if (!Main.tile[startX, startY].HasTile)
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
    }
}