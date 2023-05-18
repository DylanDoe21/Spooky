using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.Catacomb.Furniture;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class Catacombs : ModSystem
    {
        public static int PositionX = 0;
        public static int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);
        public static int EntranceY = 0;
        public static int BiomeWidth = 420;

        //shimmer re-location stuff
        public override void Load()
        {
            On_WorldGen.ShimmerMakeBiome += On_WorldGen_ShimmerMakeBiome;
        }

        private static bool On_WorldGen_ShimmerMakeBiome(On_WorldGen.orig_ShimmerMakeBiome orig, int X, int Y) 
        {
            X = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.95f), Main.maxTilesX - 150) : WorldGen.genRand.Next(150, (int)((double)Main.maxTilesX * 0.05f)));

            return orig(X, Y);
        }

        private void PlaceCatacomb(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Catacombs").Value;

            //catacomb main entrance = 15 x 6
            //entrance room = 35 x 35 blocks
            //normal rooms = 35 x 35 blocks
            //horizontal halls = 15 x 14 blocks
            //vertical hallways = 13 x 18 blocks
            //second layer rooms = 61 x 28 blocks
            //second layer halls = 4 x 9 blocks

            int XStart = PositionX;
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            //200 = large, 150 = anything smaller than large
            int layer1Width = Main.maxTilesX >= 8400 ? 200 : 150;

            //100 = small, 145 = medium, 190 = large
            int layer1Depth = Main.maxTilesY >= 2400 ? (Main.maxTilesY >= 1800 ? 145 : 100) : 190;

            //first, place a circle of bricks where each catacomb room will be
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick1>(), WorldGen.genRand.Next(25, 30), true, true);
                }
            }

            //place actual rooms
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    //choose a random room
                    Vector2 origin = new Vector2(X - 18, Y - 18);
                    Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + WorldGen.genRand.Next(1, 8), origin.ToPoint16(), Mod);
                }
            }

            /*
            //TODO: this might not be needed, can just place a giant circle of bricks around each room to make it look cleaner
            //place initial square
            SpookyWorldMethods.Square(XMiddle + 2, (int)Main.worldSurface + 135, 365, 300, ModContent.TileType<CatacombBrick>(), 
            ModContent.WallType<CatacombBrickWall>(), true);

            //first layer
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    //place entrance room in the middle of the structure
                    if (X == XMiddle && (Y == (int)Main.worldSurface + 10 || Y == (int)Main.worldSurface + 100))
                    {
                        PlaceCatacombRoom(X - 15, Y + 12, EntranceRoom, EntranceRoomObjects, 0, 0);
                    }
                    else
                    {
                        //place normal rooms here
                    }
                }
            }

            //layer one hallways
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    //always place a tunnel down on the first entrance room
                    if (X == XMiddle && Y == (int)Main.worldSurface + 10)
                    {
                        //place vertical hallway here
                    }

                    //always place a hall on the very center room
                    if (X == XMiddle && Y == (int)Main.worldSurface + 55)
                    {
                        switch (WorldGen.genRand.Next(4))
                        {
                            //place horizontal hallway here
                        }
                    }

                    //always place a hall on the row two edge rooms
                    if ((X == XMiddle - layer1Width && Y == (int)Main.worldSurface + 55) || (X == XMiddle + layer1Width - 50 && Y == (int)Main.worldSurface + 55))
                    {
                        //place horizontal hallway here
                    }

                    //on the first and last room in the first row always place a tunnel
                    if ((X == XMiddle - layer1Width && Y == (int)Main.worldSurface + 10) || (X == XMiddle + layer1Width && Y == (int)Main.worldSurface + 10))
                    {
                        //place vertical hallway here
                    }

                    //for the top two rows, chance to place a sideways hallway or a tunnel, otherwise place a hallway normally
                    if (Y < (int)Main.worldSurface + 100)
                    {
                        if (WorldGen.genRand.Next(3) <= 1)
                        {
                            //place horizontal hallway here
                        }
                        else
                        {
                            //place vertical hallway here
                        }
                    }
                    //otherwise place a side hall normally
                    else
                    {
                        //place horizontal hallway
                    }
                }
            }
            */

            /*
            //oh my goodness gracious i will edit this tomorrow
            //second layer
            for (int X = XMiddle - 150; X <= XMiddle + 150; X += 65)
            {
                for (int Y = (int)Main.worldSurface + 140; Y <= (int)Main.worldSurface + 220; Y += 40)
                {
                    //on the bottom left, place an ambush room
                    if (X == XMiddle - 150 && Y == (int)Main.worldSurface + 220)
                    {
                        PlaceCatacombRoom(X - 8, Y, AmbushRoom, AmbushRoomObjects, 3, 0);
                    }
                    //on the bottom right, place another ambush room
                    else if (X == XMiddle + 110 && Y == (int)Main.worldSurface + 220)
                    {
                        PlaceCatacombRoom(X - 8, Y, AmbushRoom, AmbushRoomObjects, 3, 0);
                    }
                    //in the middle place the entrance room
                    else if (X == XMiddle - 20 && (Y == (int)Main.worldSurface + 140))
                    {
                        PlaceCatacombRoom(X - 8, Y, GiantEntranceRoom, GiantEntranceRoomObjects, 0, 0);
                    }

                    //place chest rooms in the left, right, and bottom
                    else if (X == XMiddle - 85 && Y == (int)Main.worldSurface + 180)
                    {
                        PlaceCatacombRoom(X - 8, Y, GiantChestRoom, GiantChestRoomObjects, 0, 4);
                    }
                    else if (X == XMiddle + 45 && Y == (int)Main.worldSurface + 180) 
                    {
                        PlaceCatacombRoom(X - 8, Y, GiantChestRoom, GiantChestRoomObjects, 0, 5);
                    }
                    else if (X == XMiddle - 20 && Y == (int)Main.worldSurface + 220)
                    {
                        PlaceCatacombRoom(X - 8, Y, GiantChestRoom, GiantChestRoomObjects, 0, 6);
                    }
                    //otherwise, place a regular room
                    else
                    {
                        //place rooms
                        //evil, mouth, skull, soul
                        switch (WorldGen.genRand.Next(3))
                        {
                            case 0:
                            {
                                PlaceCatacombRoom(X - 8, Y, GiantRoom1, GiantRoomObjects1, 0, 0);
                                break;
                            }
                            case 1:
                            {
                                PlaceCatacombRoom(X - 8, Y, GiantRoom2, GiantRoomObjects2, 1, 0);
                                break;
                            }
                            case 2:
                            {
                                PlaceCatacombRoom(X - 8, Y, GiantRoom3, GiantRoomObjects3, 2, 0);
                                break;
                            }
                        }
                    }
                }
            }

            //layer two hallways
            for (int X = XMiddle - 150; X <= XMiddle + 150; X += 65)
            {
                for (int Y = (int)Main.worldSurface + 140; Y <= (int)Main.worldSurface + 220; Y += 40)
                {
                    //always place a hall on the very center room
                    if (X == XMiddle - 20)
                    {
                        switch (WorldGen.genRand.Next(2))
                        {
                            case 0:
                            {
                                PlaceCatacombRoom(X + 53, Y + 7, BigRoomHallway, BlankObjects, 0, 0);
                                break;
                            }
                            case 1:
                            {
                                PlaceCatacombRoom(X + 53, Y + 19, BigRoomHallway, BlankObjects, 0, 0);
                                break;
                            }
                        }
                    }

                    if (X == XMiddle - 20 && Y == (int)Main.worldSurface + 220)
                    {
                        PlaceCatacombRoom(X + 14, Y + 25, CatacombEntranceBarrier3, BlankObjects, 0, 0);
                    }

                    //for the top two rows, chance to place a sideways hallway or a tunnel, otherwise place a hallway normally
                    if (Y < (int)Main.worldSurface + 220)
                    {
                        if (WorldGen.genRand.Next(3) <= 1)
                        {
                            //check to not place sideways hallways on the last room
                            if (X < XMiddle + 110)
                            {
                                //place a hallway at the bottom or top side of the room
                                switch (WorldGen.genRand.Next(2))
                                {
                                    case 0:
                                    {
                                        PlaceCatacombRoom(X + 53, Y + 7, BigRoomHallway, BlankObjects, 0, 0);
                                        break;
                                    }
                                    case 1:
                                    {
                                        PlaceCatacombRoom(X + 53, Y + 19, BigRoomHallway, BlankObjects, 0, 0);
                                        break;
                                    }
                                }
                            }
                        }
                        //else place a tunnel
                        else
                        {
                            if (X != XMiddle - 20)
                            {
                                switch (WorldGen.genRand.Next(2))
                                {
                                    case 0:
                                    {
                                        PlaceCatacombRoom(X + 16, Y + 25, Tunnel1, TunnelObjects1, 0, 0);
                                        break;
                                    }
                                    case 1:
                                    {
                                        PlaceCatacombRoom(X + 16, Y + 25, Tunnel2, TunnelObjects2, 0, 0);
                                        break;
                                    }
                                }
                            }
                        }

                        //on the first and last room always place a tunnel
                        if (X == XMiddle - 150 || X == XMiddle + 110)
                        {
                            switch (WorldGen.genRand.Next(2))
                            {
                                case 0:
                                {
                                    PlaceCatacombRoom(X + 16, Y + 25, Tunnel1, TunnelObjects1, 0, 0);
                                    break;
                                }
                                case 1:
                                {
                                    PlaceCatacombRoom(X + 16, Y + 25, Tunnel2, TunnelObjects2, 0, 0);
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (X < XMiddle + 110)
                        {
                            //on the last row, place a hallway at the bottom or top side of the room
                            switch (WorldGen.genRand.Next(2))
                            {
                                case 0:
                                {
                                    PlaceCatacombRoom(X + 53, Y + 7, BigRoomHallway, BlankObjects, 0, 0);
                                    break;
                                }
                                case 1:
                                {
                                    PlaceCatacombRoom(X + 53, Y + 19, BigRoomHallway, BlankObjects, 0, 0);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //TODO: again, can probably just be removed since big bones arena is getting redone and because of the circle thing
            //place an extra box at the bottom so big bone arena has room to generate
            SpookyWorldMethods.Square(XMiddle + 2, (int)Main.worldSurface + 296, 159, 50, ModContent.TileType<CatacombBrick>(), 
            ModContent.WallType<CatacombBrickWall>(), ModContent.WallType<CatacombBrickWall2>(), true);

            //big bone arena
            PlaceCatacombRoom(XMiddle - 76, (int)Main.worldSurface + 250, BigBoneArena, BigBoneArenaObjects, 0, 0);


            //entrance stuff
            int EntranceX = XMiddle - 5;
            bool PlacedBarrier = false;

            //place the catacombs tunnel down to the first two rooms
            for (int EntranceNewY = (int)Main.worldSurface - 15; EntranceNewY <= (int)Main.worldSurface + 25; EntranceNewY += 6)
            {
                PlaceCatacombRoom(EntranceX, EntranceNewY, CatacombEntrance2, BlankObjects, 0, 0);
            }

            //place tunnel between layer one and two, and an entrance to big bone's room
            for (int EntranceNewY = (int)Main.worldSurface + 123; EntranceNewY <= (int)Main.worldSurface + 157; EntranceNewY += 6)
            {
                //place barrier entrance
                if (EntranceNewY == (int)Main.worldSurface + 135)
                {
                    PlaceCatacombRoom(EntranceX, EntranceNewY, CatacombEntranceBarrier2, BlankObjects, 0, 0);
                }
                else //place normal entrance
                {
                    PlaceCatacombRoom(EntranceX, EntranceNewY, CatacombEntrance2, BlankObjects, 0, 0);
                }
            }

            //place the entrance down from the middle of the surface structure
            for (int EntranceNewY = EntranceY + 62; EntranceNewY <= (int)Main.worldSurface - 10 && !PlacedBarrier; EntranceNewY += 6)
            {
                //place barrier entrance
                if (EntranceNewY >= (int)Main.worldSurface - 15)
                {
                    PlaceCryptTunnel(EntranceX, EntranceNewY, CatacombEntranceBarrier, BlankObjects);
                    PlacedBarrier = true;
                }
                else //place normal entrance
                {
                    PlaceCryptTunnel(EntranceX, EntranceNewY, CatacombEntrance, BlankObjects);
                }
            }

            PlaceCatacombRoom(EntranceX, (int)Main.worldSurface + 245, CatacombEntranceBarrier3, BlankObjects, 0, 0);
            */
        }

        /*
        bool placedChest = false;
        bool placedChest2 = false;

        private void PlaceCatacombAmbience(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = PositionX;
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            int layer1Width = Main.maxTilesX >= 8400 ? 200 : 150;
            int layer1Depth = Main.maxTilesY >= 2400 ? (Main.maxTilesY >= 1800 ? 145 : 100) : 190;
            
            //place spooky biome chest
            for (int X = XMiddle - layer1Width; X <= XMiddle; X++)
            {
                for (int Y = (int)Main.worldSurface + 20; Y <= (int)Main.worldSurface + 250; Y++)
                {
                    if ((Main.tile[X, Y].TileType == ModContent.TileType<CatacombBrick>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<CatacombBrickMoss>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<CatacombTiles>()) &&
                    !Main.tile[X, Y - 1].HasTile && !Main.tile[X - 1, Y - 1].HasTile && 
                    !Main.tile[X, Y - 2].HasTile && !Main.tile[X - 1, Y - 2].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(350) && !placedChest)
                        {    
                            WorldGen.PlaceChest(X, Y - 1, (ushort)ModContent.TileType<SpookyBiomeChest>(), true, 1);
                        }
                    }

                    //if the spooky biome chest has been placed, do not place it again
                    if (Main.tile[X - 1, Y - 2].TileType == ModContent.TileType<SpookyBiomeChest>())
                    {
                        placedChest = true;
                    }

                    //if the chest didnt place, try again
                    if (X >= XMiddle && Y >= (int)Main.worldSurface + 249 && !placedChest)
                    {
                        X = XMiddle - 165;
                        Y = (int)Main.worldSurface + 20;
                    }
                }
            }

            //place eye biome chest
            for (int X = XMiddle; X <= XMiddle + layer1Width; X++)
            {
                for (int Y = (int)Main.worldSurface + 20; Y <= (int)Main.worldSurface + 250; Y++)
                {
                    if ((Main.tile[X, Y].TileType == ModContent.TileType<CatacombBrick>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<CatacombBrickMoss>() ||
                    Main.tile[X, Y].TileType == ModContent.TileType<CatacombTiles>()) &&
                    !Main.tile[X, Y - 1].HasTile && !Main.tile[X - 1, Y - 1].HasTile && 
                    !Main.tile[X, Y - 2].HasTile && !Main.tile[X - 1, Y - 2].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(350) && !placedChest2)
                        {    
                            WorldGen.PlaceChest(X, Y - 1, (ushort)ModContent.TileType<SpookyHellChest>(), true, 1);
                        }
                    }

                    //if the eye biome chest has been placed, do not place it again
                    if (Main.tile[X - 1, Y - 2].TileType == ModContent.TileType<SpookyHellChest>())
                    {
                        placedChest2 = true;
                    }

                    //if the chest didnt place, try again
                    if (X >= XMiddle + 164 && Y >= (int)Main.worldSurface + 249 && !placedChest2)
                    {
                        X = XMiddle;
                        Y = (int)Main.worldSurface + 20;
                    }
                }
            }

            //place final ambient stuff
            for (int X = XMiddle - 165; X <= XMiddle + 165; X++)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + 350; Y++)
                {
                    //vines
                    if (Main.tile[X, Y].TileType == ModContent.TileType<CatacombBrickMoss>() && !Main.tile[X, Y + 1].HasTile)
                    {
                        if (WorldGen.genRand.NextBool(2))
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<CatacombVines>());
                        }
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<CatacombVines>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 4), (ushort)ModContent.TileType<CatacombVines>());
                    }

                    if (Main.tile[X, Y].TileType == ModContent.TileType<CatacombBrickMoss>())
                    {
                        //ambient moss
                        if (WorldGen.genRand.NextBool(3))
                        {    
                            ushort[] Moss = new ushort[] { (ushort)ModContent.TileType<Moss1>(), (ushort)ModContent.TileType<Moss2>(), 
                            (ushort)ModContent.TileType<Moss3>(), (ushort)ModContent.TileType<Moss4>(), (ushort)ModContent.TileType<Moss5>() };

                            WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(Moss));
                        }

                        //ceiling roots
                        if (WorldGen.genRand.NextBool(5))
                        {    
                            ushort[] Roots = new ushort[] { (ushort)ModContent.TileType<CatacombRoot1>(), 
                            (ushort)ModContent.TileType<CatacombRoot2>(), (ushort)ModContent.TileType<CatacombRoot3>() };

                            WorldGen.PlaceObject(X, Y + 1, WorldGen.genRand.Next(Roots)); 
                        }
                    }

                    if (!Main.tile[X, Y].HasTile && (Main.tile[X, Y].WallType == ModContent.WallType<CatacombBrickWall>() ||
                    Main.tile[X, Y].WallType == ModContent.WallType<CatacombBrickWall2>()))
                    {
                        //catacombs
                        if (WorldGen.genRand.NextBool(150))
                        {    
                            //dunno why catacombs internal id is Painting4X3 but whatever
                            WorldGen.PlaceObject(X, Y, TileID.Painting4X3, true, Main.rand.Next(0, 8));
                        }

                        //wall skeletons
                        if (WorldGen.genRand.NextBool(150))
                        {    
                            WorldGen.PlaceObject(X, Y, TileID.Painting3X3, true, Main.rand.Next(17, 18));
                        }
                    }
                }
            }
        }
        */

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("PlaceCatacomb", PlaceCatacomb));
            //tasks.Insert(GenIndex1 + 2, new PassLegacy("PlaceCatacombAmbience", PlaceCatacombAmbience));

            //re-locate the jungle temple deeper underground so it never conflicts with the catacombs
            int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
            {
                WorldGen.makeTemple(GenVars.JungleX, Main.maxTilesY - (Main.maxTilesY / 2) + 75);
            });
        }

        /*
        //place items in chests
        public override void PostWorldGen()
		{
            for (int chestIndex = 0; chestIndex < 1000; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex]; 

                //spooky biome chest items
				if (chest != null && (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyBiomeChest>() || 
                Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyHellChest>()))
                {
                    int[] Potions = new int[] { ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };

                    //spooky biome chest main item
                    if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyBiomeChest>())
                    {
                        //el gourdo
                        chest.item[0].SetDefaults(ModContent.ItemType<ElGourdo>());
                        chest.item[0].stack = 1;
                    }

                    //eye biome chest main item
                    if (Main.tile[chest.x, chest.y].TileType == ModContent.TileType<SpookyHellChest>())
                    {
                        //el gourdo
                        chest.item[0].SetDefaults(ModContent.ItemType<BrainJar>());
                        chest.item[0].stack = 1;
                    }

                    //candles
                    chest.item[1].SetDefaults(ModContent.ItemType<CandleItem>());
                    chest.item[1].stack = WorldGen.genRand.Next(5, 12);
                    //potions
                    chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions));
                    chest.item[2].stack = WorldGen.genRand.Next(5, 8);
                    //healing potions
                    chest.item[3].SetDefaults(ItemID.GreaterHealingPotion);
                    chest.item[3].stack = WorldGen.genRand.Next(12, 20);
                    //mana potions
                    chest.item[4].SetDefaults(ItemID.GreaterManaPotion);
                    chest.item[4].stack = WorldGen.genRand.Next(12, 20);
                    //gold coins
                    chest.item[5].SetDefaults(ItemID.GoldCoin);
                    chest.item[5].stack = WorldGen.genRand.Next(10, 15);
                }
            }
        }
        */
    }
}