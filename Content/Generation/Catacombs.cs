using Terraria;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.Catacomb;

using StructureHelper;
using ReLogic.Utilities;

namespace Spooky.Content.Generation
{
    public class Catacombs : ModSystem
    {
        int chosenRoom = 0;
        int switchRoom = 0;
        int[] RoomPattern = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        public static int PositionX = 0;
        public static int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);
        public static int EntranceY = 0;
        public static int BiomeWidth = 420;

        public static bool placedLootRoom1 = false;
        public static bool placedLootRoom2 = false;
        public static bool placedLootRoom3 = false;
        public static bool placedLootRoom4 = false;
        public static bool placedMoyaiRoom = false;

        private void PlaceCatacomb(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Catacombs").Value;

            //first layer room stuff
            //catacomb main entrance = 15 x 6
            //entrance room = 35 x 35 blocks
            //normal rooms = 35 x 35 blocks
            //horizontal halls = 15 x 14 blocks
            //vertical hallways = 7 x 18 blocks

            //second layer room stuff
            //second layer rooms = 61 x 28 blocks
            //second layer halls = 4 x 9 blocks

            int XStart = PositionX;
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            //sets the width for the catacombs (how many horizontal rows of rooms it has)
            //200 = large (7 rooms horizontally), 150 = anything smaller than large (5 rooms)
            int layer1Width = Main.maxTilesX >= 8400 ? 200 : 150;

            //sets the height for the catacombs (how far down the vertical rows go)
            //235 = large (6 rooms deep), 190 = medium (5 rooms deep), 145 = small (4 rooms deep)
            int layer1Depth = Main.maxTilesY >= 2400 ? 235 : (Main.maxTilesY >= 1800 ? 190 : 145);

            //first, place a circle of bricks where each catacomb room will be
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick1>(), 40, true, true);
                }
            }

            //randomize room pattern
            RoomPattern = RoomPattern.OrderBy(x => Main.rand.Next()).ToArray();

            //place actual rooms
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    chosenRoom = RoomPattern[switchRoom];

                    switchRoom++;

                    if (switchRoom >= RoomPattern.Length)
                    {
                        switchRoom = 0;
                    }

                    //origin offset for each room
                    Vector2 origin = new Vector2(X - 18, Y - 18);

                    //first row
                    if (Y == (int)Main.worldSurface + 10)
                    {
                        //randomly place the first loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom1 && (WorldGen.genRand.NextBool(7) || X == XMiddle + layer1Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/LootRoom-1", origin.ToPoint16(), Mod);
                            placedLootRoom1 = true;
                        }
                        else
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                        }
                    }

                    //second row
                    if (Y == (int)Main.worldSurface + 55)
                    {
                        //randomly place the first loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom2 && (WorldGen.genRand.NextBool(7) || X == XMiddle + layer1Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/LootRoom-2", origin.ToPoint16(), Mod);
                            placedLootRoom2 = true;
                        }
                        else
                        {
                            //rare chance to place a golden treasure room
                            if (WorldGen.genRand.NextBool(200) && !placedMoyaiRoom)
                            {
                                //only one treasure room can be placed in a world
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/MoyaiRoom", origin.ToPoint16(), Mod);
                                placedMoyaiRoom = true;
                            }
                            else
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                            }
                        }
                    }

                    //third row
                    if (Y == (int)Main.worldSurface + 100)
                    {
                        //randomly place the first loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom3 && (WorldGen.genRand.NextBool(7) || X == XMiddle + layer1Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/LootRoom-3", origin.ToPoint16(), Mod);
                            placedLootRoom3 = true;
                        }
                        else
                        {
                            //rare chance to place a golden treasure room
                            if (WorldGen.genRand.NextBool(200) && !placedMoyaiRoom)
                            {
                                //only one treasure room can be placed in a world
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/MoyaiRoom", origin.ToPoint16(), Mod);
                                placedMoyaiRoom = true;
                            }
                            //place trap rooms sometimes
                            else if (WorldGen.genRand.NextBool(35))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                            }
                            else
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                            }
                        }
                    }

                    //fourth row
                    if (Y == (int)Main.worldSurface + 145)
                    {
                        //randomly place the first loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom4 && (WorldGen.genRand.NextBool(7) || X == XMiddle + layer1Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/LootRoom-4", origin.ToPoint16(), Mod);
                            placedLootRoom4 = true;
                        }
                        else
                        {
                            //rare chance to place a golden treasure room
                            if (WorldGen.genRand.NextBool(200) && !placedMoyaiRoom)
                            {
                                //only one treasure room can be placed in a world
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/MoyaiRoom", origin.ToPoint16(), Mod);
                                placedMoyaiRoom = true;
                            }
                            //place trap rooms sometimes
                            else if (WorldGen.genRand.NextBool(35))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                            }
                            else
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                            }
                        }
                    }

                    //fifth row
                    if (Y == (int)Main.worldSurface + 190)
                    {
                        //rare chance to place a golden treasure room
                        if (WorldGen.genRand.NextBool(200) && !placedMoyaiRoom)
                        {
                            //only one treasure room can be placed in a world
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/MoyaiRoom", origin.ToPoint16(), Mod);
                            placedMoyaiRoom = true;
                        }
                        //place trap rooms sometimes
                        else if (WorldGen.genRand.NextBool(35))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                        }
                        else
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                        }
                    }

                    //sixth row
                    if (Y == (int)Main.worldSurface + 235)
                    {
                        //rare chance to place a golden treasure room
                        if (WorldGen.genRand.NextBool(200) && !placedMoyaiRoom)
                        {
                            //only one treasure room can be placed in a world
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/MoyaiRoom", origin.ToPoint16(), Mod);
                            placedMoyaiRoom = true;
                        }
                        //place trap rooms sometimes
                        else if (WorldGen.genRand.NextBool(35))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                        }
                        else
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                        }
                    }
                }
            }

            //place hallways
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    //actual hallway positions
                    Vector2 horizontalHallOrigin = new Vector2(X + 17, WorldGen.genRand.NextBool(2) ? Y + 3 : Y - 14);
                    Vector2 verticalHallOrigin = new Vector2(X - 7, Y + 15);

                    //for all rows besides the bottom, place horizontal halls between each room and a change to place a vertical hall on the bottom
                    if (Y < (int)Main.worldSurface + layer1Depth)
                    {
                        //place a horizontal hallway on every room besides the one on the end
                        if (X < XMiddle + layer1Width)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall-" + WorldGen.genRand.Next(1, 4), horizontalHallOrigin.ToPoint16(), Mod);
                        }

                        //place a vertical hall randomly under any room
                        if (WorldGen.genRand.NextBool(2))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/VerticalHall-" + WorldGen.genRand.Next(1, 2), verticalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                    //on the bottom row just place a horizontal hall
                    else
                    {
                        //dont place a hall on the last room
                        if (X < XMiddle + layer1Width)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall-" + WorldGen.genRand.Next(1, 4), horizontalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                }
            }

            int EntranceX = XMiddle - 5;
            bool PlacedBarrier = false;

            for (int EntranceNewY = EntranceY + 60; EntranceNewY <= (int)Main.worldSurface - 6 && !PlacedBarrier; EntranceNewY += 6)
            {
                Vector2 entranceOrigin = new Vector2(EntranceX - 3, EntranceNewY);

                //place barrier entrance
                if (EntranceNewY >= (int)Main.worldSurface - 11)
                {
                    Generator.GenerateStructure("Content/Structures/CatacombLayer1/CryptEntranceBarrier", entranceOrigin.ToPoint16(), Mod);
                    PlacedBarrier = true;
                }
                //place normal entrance
                else 
                {
                    if (WorldGen.genRand.NextBool(3))
                    {
                        Generator.GenerateStructure("Content/Structures/CatacombLayer1/CryptEntrance-" + WorldGen.genRand.Next(2, 4), entranceOrigin.ToPoint16(), Mod);
                    }
                    else
                    {
                        Generator.GenerateStructure("Content/Structures/CatacombLayer1/CryptEntrance-1", entranceOrigin.ToPoint16(), Mod);
                    }
                }
            }

            /*
            //oh my goodness gracious, i will edit this later
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

            //re-locate the shimmer to be closer to the edge of the world so it also never conflicts with the catacombs
            //let it be known that i could not find any other way to relocate it other than modifying all of the vanilla gen code
            //this is a huge moment of weakness and i will eventually make a youtuber apology for this
            int shimmerIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shimmer"));
            tasks[shimmerIndex] = new PassLegacy("Shimmer", (progress, config) =>
            {
                int num702 = 50;
                int num703 = (int)(Main.worldSurface + Main.rockLayer) / 2 + num702;
                int num704 = (int)((double)((Main.maxTilesY - 250) * 2) + Main.rockLayer) / 3;
                if (num704 > Main.maxTilesY - 330 - 100 - 30)
                {
                    num704 = Main.maxTilesY - 330 - 100 - 30;
                }
                if (num704 <= num703)
                {
                    num704 = num703 + 50;
                }
                int num705 = WorldGen.genRand.Next(num703, num704);
                int num706 = GenVars.dungeonSide < 0 ? Main.maxTilesX - 100 : 100;
                int num707 = (int)Main.worldSurface + 150;
                int num708 = (int)(Main.rockLayer + Main.worldSurface + 200.0) / 2;
                if (num708 <= num707)
                {
                    num708 = num707 + 50;
                }
                if (WorldGen.tenthAnniversaryWorldGen)
                {
                    num705 = WorldGen.genRand.Next(num707, num708);
                }
                while (!WorldGen.ShimmerMakeBiome(num706, num705))
                {
                    num705 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 22, num704);
                    num706 = ((GenVars.dungeonSide < 0) ? WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.95), Main.maxTilesX - 150) : WorldGen.genRand.Next(150, (int)((double)Main.maxTilesX * 0.05)));
                }

                GenVars.shimmerPosition = new Vector2D((double)num706, (double)num705);
                int num710 = 200;
                GenVars.structures.AddProtectedStructure(new Rectangle(num706 - num710 / 2, num705 - num710 / 2, num710, num710));
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