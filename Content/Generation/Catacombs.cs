using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.IO;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.Cemetery.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell.Furniture;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class Catacombs : ModSystem
    {
        int chosenRoom = 0;
        int switchRoom = 0;
        int numAmbushRooms = 0;

        int[] RoomPatternLayer1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
        int[] RoomPatternLayer2 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public static int PositionX = 0;
        public static int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);
        public static int EntranceY = 0;

        public static bool PlacedFirstBarrier = false;
        public static bool placedLootRoom1 = false;
        public static bool placedLootRoom2 = false;
        public static bool placedLootRoom3 = false;
        public static bool placedLootRoom4 = false;
        public static bool placedMoyaiRoom = false;

        Vector2[] Layer2TrapRoomPoints = new Vector2[6];

        private void PlaceCatacomb(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Catacombs").Value;

            int XStart = PositionX;
            int XMiddle = XStart + (Cemetery.BiomeWidth / 2);
            int XEdge = XStart + Cemetery.BiomeWidth;

            //LAYER 1

            //set the width for the catacombs (how many rooms it has horizontally)
            //200 = large worlds (9 rooms wide), 150 = medium worlds (7 rooms wide), 100 = small worlds (5 rooms wide)
            int layer1Width = Main.maxTilesX >= 8400 ? 200 : (Main.maxTilesX >= 6400 ? 150 : 100);

            //sets the height for the catacombs (how many rooms it has vertically)
            //235 = large worlds (6 rooms deep), 190 = medium worlds (5 rooms deep), 145 = small worlds (4 rooms deep)
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
            RoomPatternLayer1 = RoomPatternLayer1.OrderBy(x => Main.rand.Next()).ToArray();

            //place the actual rooms
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    chosenRoom = RoomPatternLayer1[switchRoom];

                    switchRoom += Main.rand.Next(1, 3);

                    if (switchRoom >= RoomPatternLayer1.Length)
                    {
                        switchRoom = 0;
                    }

                    //origin offset for each room so it places at the center
                    Vector2 origin = new Vector2(X - 18, Y - 18);

                    //first row
                    if (Y == (int)Main.worldSurface + 10)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        //do not place the first loot room in the middle where the entrance is either
                        if (!placedLootRoom1 && X != XMiddle && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer1Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/LootRoom-1", origin.ToPoint16(), Mod);
                            placedLootRoom1 = true;
                        }
                        else 
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                        }

                        if (X == XMiddle)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/EntranceRoom", origin.ToPoint16(), Mod);
                        }
                    }

                    //second row
                    if (Y == (int)Main.worldSurface + 55)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom2 && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer1Width))
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
                            //place trap rooms sometimes
                            else if (WorldGen.genRand.NextBool(12))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 4), origin.ToPoint16(), Mod);
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
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom3 && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer1Width))
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
                            //place ambush rooms sometimes
                            else if (WorldGen.genRand.NextBool(7) && numAmbushRooms < 3)
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/AmbushRoom", origin.ToPoint16(), Mod);
                                numAmbushRooms++;
                            }
                            //place trap rooms sometimes
                            else if (WorldGen.genRand.NextBool(10))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 4), origin.ToPoint16(), Mod);
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
                        if (!placedLootRoom4 && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer1Width))
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
                            //place ambush rooms sometimes
                            else if (WorldGen.genRand.NextBool(7) && numAmbushRooms < 3)
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/AmbushRoom", origin.ToPoint16(), Mod);
                                numAmbushRooms++;
                            }
                            //place trap rooms sometimes
                            else if (WorldGen.genRand.NextBool(8))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 4), origin.ToPoint16(), Mod);
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
                        //place ambush rooms sometimes
                        else if (WorldGen.genRand.NextBool(7) && numAmbushRooms < 3)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/AmbushRoom", origin.ToPoint16(), Mod);
                            numAmbushRooms++;
                        }
                        //place trap rooms sometimes
                        else if (WorldGen.genRand.NextBool(8))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 4), origin.ToPoint16(), Mod);
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
                        //place ambush rooms sometimes
                        else if (WorldGen.genRand.NextBool(7) && numAmbushRooms < 3)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/AmbushRoom", origin.ToPoint16(), Mod);
                            numAmbushRooms++;
                        }
                        //place trap rooms sometimes
                        else if (WorldGen.genRand.NextBool(8))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom-" + WorldGen.genRand.Next(1, 4), origin.ToPoint16(), Mod);
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

                    //for all rows besides the bottom, place horizontal halls between each room, which a chance to place a vertical hall on the bottom
                    if (Y < (int)Main.worldSurface + layer1Depth)
                    {
                        //dont place a hall on the last room
                        if (X < XMiddle + layer1Width)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOrigin.ToPoint16(), Mod);
                        }

                        //place a vertical hall randomly under any room
                        if (WorldGen.genRand.NextBool(2))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/VerticalHall-" + WorldGen.genRand.Next(1, 4), verticalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                    //on the bottom row of rooms, only place horizontal halls
                    else
                    {
                        //dont place a hall on the last room
                        if (X < XMiddle + layer1Width)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                }
            }


            //LAYER 2

            //reset the room pattern stuff
            switchRoom = 0;
            chosenRoom = 0;

            //reset the loot room bools
            placedLootRoom1 = false;
            placedLootRoom2 = false;
            placedLootRoom3 = false;
            placedLootRoom4 = false;

            //sets the width for the catacombs second layer (how many rooms it has horizontally)
            //240 = large worlds (9 rooms wide), 160 = medium worlds (5 rooms wide), 80 = small worlds (3 rooms wide)
            int layer2Width = Main.maxTilesX >= 8400 ? 240 : 160; //(Main.maxTilesX >= 6400 ? 160 : 80);

            //sets the height for the catacombs second layer (how many rooms it has vertically)
            //350 = large worlds (6 rooms deep), 300 = medium worlds (5 rooms deep), 250 = small worlds (4 rooms deep)
            int layer2Depth = Main.maxTilesY >= 2400 ? 350 : (Main.maxTilesY >= 1800 ? 300 : 250);

            //randomize room pattern
            RoomPatternLayer2 = RoomPatternLayer2.OrderBy(x => Main.rand.Next()).ToArray();

            //again, place a circle of bricks where each catacomb room will be
            //since the rooms in layer 2 are wider, place two circles side by side
            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
                for (int Y = (int)Main.worldSurface + layer1Depth + 118; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    SpookyWorldMethods.PlaceCircle(X - 20, Y, ModContent.TileType<CatacombBrick2>(), 40, true, true);
                    SpookyWorldMethods.PlaceCircle(X + 20, Y, ModContent.TileType<CatacombBrick2>(), 40, true, true);
                }
            }

            //place circles around where the big bone arena will generate
            //this is done before generating the layer two rooms so it doesnt destroy them
            int BigBoneArenaY = (int)Main.worldSurface + layer1Depth + layer2Depth + 50;

            for (int X = XMiddle - 100; X <= XMiddle + 100; X += 5)
            {
                for (int Y = BigBoneArenaY - 35; Y <= BigBoneArenaY + 45; Y += 5)
                {
                    SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick2>(), 10, true, true);
                }
            }

            //place the actual rooms
            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
                for (int Y = (int)Main.worldSurface + layer1Depth + 118; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    chosenRoom = RoomPatternLayer2[switchRoom];

                    switchRoom += Main.rand.Next(1, 3);

                    if (switchRoom >= RoomPatternLayer2.Length)
                    {
                        switchRoom = 0;
                    }

                    //origin offset for each room so it places at the center
                    Vector2 origin = new Vector2(X - 35, Y - 18);

                    int layer2Start = (int)Main.worldSurface + layer1Depth + 118;

                    //first row
                    if (Y == layer2Start)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        //do not place the first loot room in the middle where the entrance is either
                        if (!placedLootRoom1 && X != XMiddle && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer2Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/LootRoom-1", origin.ToPoint16(), Mod);
                            placedLootRoom1 = true;
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(10))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/PuzzleRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                            }
                            else
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                            }
                        }
                    }

                    //second row
                    if (Y == layer2Start + 42)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom2 && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer2Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/LootRoom-2", origin.ToPoint16(), Mod);
                            placedLootRoom2 = true;
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(10))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/PuzzleRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                            }
                            else
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                            }
                        }
                    }

                    //third row
                    if (Y == layer2Start + 84)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom3 && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer2Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/LootRoom-3", origin.ToPoint16(), Mod);
                            placedLootRoom3 = true;
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(10))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/PuzzleRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                            }
                            else
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                            }
                        }
                    }

                    //fourth row
                    if (Y == layer2Start + 126)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom4 && (WorldGen.genRand.NextBool(5) || X == XMiddle + layer2Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/LootRoom-4", origin.ToPoint16(), Mod);
                            placedLootRoom4 = true;
                        }
                        else
                        {
                            if (WorldGen.genRand.NextBool(10))
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/PuzzleRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                            }
                            else
                            {
                                Generator.GenerateStructure("Content/Structures/CatacombLayer2/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                            }
                        }
                    }

                    //fifth row
                    if (Y == layer2Start + 168)
                    {
                        if (WorldGen.genRand.NextBool(10))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/PuzzleRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                        }
                        else
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                        }
                    }   

                    //sixth row
                    if (Y == layer2Start + 210)
                    {
                        if (WorldGen.genRand.NextBool(10))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/PuzzleRoom-" + WorldGen.genRand.Next(1, 3), origin.ToPoint16(), Mod);
                        }
                        else
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/Room-" + chosenRoom, origin.ToPoint16(), Mod);
                        }
                    }
                }
            }

            //place hallways
            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
                for (int Y = (int)Main.worldSurface + layer1Depth + 118; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    //actual hallway positions
                    Vector2 horizontalHallOrigin = new Vector2(X + 34, WorldGen.genRand.NextBool(2) ? Y + 3 : Y - 14);
                    Vector2 verticalHallOrigin = new Vector2(X - 7, Y + 15);

                    //for all rows besides the bottom, place horizontal halls between each room, which a chance to place a vertical hall on the bottom
                    if (Y < (int)Main.worldSurface + layer1Depth + layer2Depth - 20)
                    {
                        //dont place a hall on the last room
                        if (X < XMiddle + layer2Width)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOrigin.ToPoint16(), Mod);
                        }

                        //place a vertical hall randomly under any room
                        if (WorldGen.genRand.NextBool(2))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/VerticalHall-" + WorldGen.genRand.Next(1, 4), verticalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                    //on the bottom row of rooms, only place horizontal halls
                    else
                    {
                        //dont place a hall on the last room
                        if (X < XMiddle + layer2Width)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                }
            }


            //EXTRA STUFF

            //place the entrance to the catacombs from of the cemetery crypt building
            int EntranceX = XMiddle - 5;

            for (int EntranceNewY = EntranceY + 60; EntranceNewY <= (int)Main.worldSurface - 6; EntranceNewY += 6)
            {
                Vector2 entranceOrigin = new Vector2(EntranceX - 3, EntranceNewY);
                Vector2 entranceBarrierOrigin = new Vector2(EntranceX - 3, EntranceNewY + 1);
                Vector2 cryptEntranceOrigin = new Vector2(EntranceX - 3, EntranceNewY + 2);

                //place the yellow barrier entrance once the catacombs is reached
                if (Main.tile[EntranceX, EntranceNewY].TileType == ModContent.TileType<CatacombBrick1>())
                {
                    Generator.GenerateStructure("Content/Structures/CatacombLayer1/CryptEntrance-" + WorldGen.genRand.Next(1, 5), entranceOrigin.ToPoint16(), Mod);
                    Generator.GenerateStructure("Content/Structures/CatacombLayer1/CryptEntranceBarrier", entranceBarrierOrigin.ToPoint16(), Mod);
                    PlacedFirstBarrier = true;
                }
                else
                {
                    //place catacomb wall entrances after placing the actual barrier
                    if (PlacedFirstBarrier)
                    {
                        Generator.GenerateStructure("Content/Structures/CatacombLayer1/CatacombEntrance", entranceBarrierOrigin.ToPoint16(), Mod);
                        Generator.GenerateStructure("Content/Structures/CatacombLayer1/CatacombEntrance", cryptEntranceOrigin.ToPoint16(), Mod);
                    }
                    //place a normal crypt entrance otherwise
                    else 
                    {
                        Generator.GenerateStructure("Content/Structures/CatacombLayer1/CryptEntrance-" + WorldGen.genRand.Next(1, 5), entranceOrigin.ToPoint16(), Mod);
                    }
                }
            }


            //place the daffodil arena below the first layer
            int DaffodilArenaY = (int)Main.worldSurface + layer1Depth + 55;
            Vector2 DaffodilArenaOrigin = new Vector2(XMiddle - 52, DaffodilArenaY - 21);

            //place circles of brick around where the arena will generate
            for (int X = XMiddle - 100; X <= XMiddle + 100; X += 5)
            {
                for (int Y = DaffodilArenaY - 21; Y <= DaffodilArenaY + 21; Y += 3)
                {
                    //place the first layer bricks around the top half of the arena
                    if (Y <= DaffodilArenaY + 10)
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick1>(), 10, true, true);
                    }
                    //on the bottom half, place the second layer bricks
                    else
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick2>(), 10, true, true);
                    }
                }
            }

            //place the daffodil arena
            Generator.GenerateStructure("Content/Structures/CatacombLayer1/DaffodilArena", DaffodilArenaOrigin.ToPoint16(), Mod);

            //spawn daffodil itself in the arena
            NPC.NewNPC(null, (XMiddle - 1) * 16, (DaffodilArenaY) * 16, ModContent.NPCType<DaffodilBody>());

            //place tunnels leading into the daffodil arena from the two rooms on the sides of it
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                int Y = (int)Main.worldSurface + layer1Depth;

                //dig tunnels downward on the two side rooms
                if (X == XMiddle - 50 || X == XMiddle + 50)
                {
                    for (int tunnelX = X - 3; tunnelX <= X + 1; tunnelX++)
                    {
                        for (int tunnelY = Y + 15; tunnelY <= (int)Main.worldSurface + layer1Depth + 65; tunnelY++)
                        {
                            Main.tile[tunnelX, tunnelY].ClearEverything();

                            //place brick walls in the tunnel
                            WorldGen.PlaceWall(tunnelX, tunnelY, (ushort)ModContent.WallType<CatacombBrickWall1>());

                            //place platforms at the top of the hole, on the floor in the room
                            if (tunnelY == Y + 15)
                            {
                                WorldGen.PlaceTile(tunnelX, tunnelY, ModContent.TileType<OldWoodPlatform>());
                            }
                            //place stuff in the tunnel
                            else
                            {
                                //in the middle of the tunnel, place a chain that goes down
                                if (tunnelX == X - 1)
                                {
                                    WorldGen.PlaceTile(tunnelX, tunnelY, TileID.Chain);
                                }
                                //place cobwebs randomly around the chain
                                else
                                {
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceTile(tunnelX, tunnelY, TileID.Cobweb);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //place entrance from daffodil's arena to the second layer
            for (int EntranceNewY = DaffodilArenaY + 21; EntranceNewY <= (int)Main.worldSurface + layer1Depth + 100; EntranceNewY += 6)
            {
                Vector2 entranceOrigin = new Vector2(XMiddle - 8, EntranceNewY);

                Generator.GenerateStructure("Content/Structures/CatacombLayer2/Entrance-" + WorldGen.genRand.Next(1, 5), entranceOrigin.ToPoint16(), Mod);
            }


            //place big bone arena
            Vector2 BigBoneArenaOrigin = new Vector2(XMiddle - 53, BigBoneArenaY - 35);

            Generator.GenerateStructure("Content/Structures/CatacombLayer2/BigBoneArena", BigBoneArenaOrigin.ToPoint16(), Mod);

            //spawn giant flower pot in the big bone arena
            NPC.NewNPC(null, (XMiddle) * 16, (BigBoneArenaY) * 16, ModContent.NPCType<BigFlowerPot>());

            //dig entrance to big bone's arena
            for (int tunnelX = XMiddle - 3; tunnelX <= XMiddle + 1; tunnelX++)
            {
                //this determines how far down the big bone entrance is
                int extraDepthForEntrance = Main.maxTilesX >= 8400 ? -7 : (Main.maxTilesX >= 6400 ? 1 : 9);

                for (int tunnelY = (int)Main.worldSurface + layer1Depth + layer2Depth + extraDepthForEntrance; tunnelY <= BigBoneArenaY - 36; tunnelY++)
                {
                    Main.tile[tunnelX, tunnelY].ClearEverything();

                    //place brick walls in the tunnel
                    WorldGen.PlaceWall(tunnelX, tunnelY, (ushort)ModContent.WallType<CatacombBrickWall2>());

                    if (tunnelY == (int)Main.worldSurface + layer1Depth + layer2Depth + extraDepthForEntrance)
                    {
                        WorldGen.PlaceTile(tunnelX, tunnelY, ModContent.TileType<OldWoodPlatform>());
                    }
                    else
                    {
                        //in the middle of the tunnel, place a chain that goes down
                        if (tunnelX == XMiddle - 1)
                        {
                            WorldGen.PlaceTile(tunnelX, tunnelY, TileID.Chain);
                        }
                        //place cobwebs randomly around the chain
                        else
                        {
                            if (WorldGen.genRand.NextBool(3))
                            {
                                WorldGen.PlaceTile(tunnelX, tunnelY, TileID.Cobweb);
                            }
                        }
                    }
                }
            }
        }

        private void KillVinesAndPlants(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = PositionX;
            int XMiddle = XStart + (Cemetery.BiomeWidth / 2);
            int XEdge = XStart + Cemetery.BiomeWidth;

            //kill plants and vines that are not on valid tiles
            for (int X = XMiddle - 300; X <= XMiddle + 300; X++)
            {
                for (int Y = (int)Main.worldSurface - 10; Y <= Main.maxTilesY - 100; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];

                    //place grass walls in layer one
                    if (!tile.HasTile && tile.WallType == ModContent.WallType<CatacombBrickWall1>() && WorldGen.genRand.NextBool(250))
                    {
                        SpookyWorldMethods.ModifiedTileRunner(X, Y, WorldGen.genRand.Next(8, 15), 1, ModContent.TileType<CatacombBrick1>(),
                        ModContent.WallType<CatacombGrassWall1>(), ModContent.WallType<CatacombGrassWall1>(), false, 0f, 0f, true, false, false, true, true);
                    }

                    //place grass walls in layer two
                    if (!tile.HasTile && tile.WallType == ModContent.WallType<CatacombBrickWall2>() && WorldGen.genRand.NextBool(250))
                    {
                        SpookyWorldMethods.ModifiedTileRunner(X, Y, WorldGen.genRand.Next(10, 25), 1, ModContent.TileType<CatacombBrick2>(), 
                        ModContent.WallType<CatacombGrassWall2>(), ModContent.WallType<CatacombGrassWall2>(), false, 0f, 0f, true, false, false, true, true);
                    }

                    //kill vines if the tile above it is not valid
                    if (tile.TileType == ModContent.TileType<CemeteryVines>() && tileAbove.TileType != ModContent.TileType<CemeteryGrass>() && tileAbove.TileType != ModContent.TileType<CemeteryVines>())
                    {
                        WorldGen.KillTile(X, Y);
                    }

                    //kill any remaining weeds that are not on catacomb grass blocks
                    if ((tile.TileType == ModContent.TileType<CatacombWeeds>() || tile.TileType == ModContent.TileType<SporeMushroom>()) && tileBelow.TileType != ModContent.TileType<CemeteryGrass>())
                    {
                        WorldGen.KillTile(X, Y);
                    }
                }
            }
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("PlaceCatacomb", PlaceCatacomb));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("KillVinesAndPlants", KillVinesAndPlants));

            //re-locate the jungle temple deeper underground and further horizontally so it never gets generated over by the catacombs
            int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
            {
                int newTempleX = GenVars.JungleX < (Main.maxTilesX / 2) ? GenVars.JungleX + 250 : GenVars.JungleX - 250;

                WorldGen.makeTemple(newTempleX, Main.maxTilesY - (Main.maxTilesY / 2) + 75);
            });

            //re-locate the shimmer to be closer to the edge of the world so it also never gets generated over by the catacombs
            int shimmerIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shimmer"));
            tasks[shimmerIndex] = new PassLegacy("Shimmer", (progress, config) =>
            {
                //copy pasted and slightly modified shimmer generation code from terraria itself
                int RandomY1 = (int)(Main.worldSurface + Main.rockLayer) / 2 + 100;
                int RandomY2 = (int)((double)((Main.maxTilesY - 250) * 2) + Main.rockLayer) / 3;

                if (RandomY2 > Main.maxTilesY - 330 - 100 - 30)
                {
                    RandomY2 = Main.maxTilesY - 330 - 100 - 30;
                }
                if (RandomY2 <= RandomY1)
                {
                    RandomY2 = RandomY1 + 50;
                }

                int ShimmerX = GenVars.dungeonSide < 0 ? Main.maxTilesX - 100 : 100;
                int ShimmerY = WorldGen.genRand.Next(RandomY1, RandomY2);

                int ShimmerXAnniversary = (int)Main.worldSurface + 150;
                int ShimmerYAnniversary = (int)(Main.rockLayer + Main.worldSurface + 200.0) / 2;

                if (ShimmerYAnniversary <= ShimmerXAnniversary)
                {
                    ShimmerYAnniversary = ShimmerXAnniversary + 50;
                }

                if (WorldGen.tenthAnniversaryWorldGen)
                {
                    ShimmerY = WorldGen.genRand.Next(ShimmerXAnniversary, ShimmerYAnniversary);
                }

                while (!WorldGen.ShimmerMakeBiome(ShimmerX, ShimmerY))
                {
                    ShimmerX = (GenVars.dungeonSide < 0) ? (int)(Main.maxTilesX * 0.95f) : (int)(Main.maxTilesX * 0.05f);
                    ShimmerY = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 22, RandomY2);
                }

                GenVars.shimmerPosition = new Vector2D((double)ShimmerX, (double)ShimmerY);

                int num710 = 200;

                //add the shimmer as a protected structure so nothing attempts to generate over it
                GenVars.structures.AddProtectedStructure(new Rectangle(ShimmerX - num710 / 2, ShimmerY - num710 / 2, num710, num710));
            });
        }
    }
}