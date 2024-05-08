using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.IO;
using ReLogic.Utilities;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.SpiderCave.OldHunter;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.PandoraBox;
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

        int[] RoomPatternLayer1 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        int[] RoomPatternLayer2 = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

        public static int EntranceY = 0;
        public static int PositionX = 0;

        public static bool PlacedFirstBarrier = false;
        public static bool placedLootRoom1 = false;
        public static bool placedLootRoom2 = false;
        public static bool placedLootRoom3 = false;
        public static bool placedLootRoom4 = false;
        public static bool placedMoyaiRoom = false;

        Vector2[] Layer2LootRooms = new Vector2[2];
        Vector2 PandoraRoomPosition;

        private void PlaceCatacomb(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Catacombs").Value;

            int XStart = PositionX - (Cemetery.BiomeWidth / 2);
            int XMiddle = PositionX;

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
                    SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick1>(), ModContent.WallType<CatacombBrickWall1>(), 40, true, true);
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

                    switchRoom += 1; //Main.rand.Next(1, 3);

                    if (switchRoom >= RoomPatternLayer1.Length)
                    {
                        switchRoom = 0;
                    }

                    //origin offset for each room so it places at the center of the position its placed at
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

                        //place entrance room in the top row in the center
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
                        else if (WorldGen.genRand.NextBool(10))
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
            }

            //place hallways
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    //actual hallway positions
                    Vector2 horizontalHallOrigin = new Vector2(X + 17, WorldGen.genRand.NextBool() ? Y + 3 : Y - 14);
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
                        if (WorldGen.genRand.NextBool())
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer1/VerticalHall-" + WorldGen.genRand.Next(1, 5), verticalHallOrigin.ToPoint16(), Mod);
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
            int layer2Width = Main.maxTilesX >= 8400 ? 240 : (Main.maxTilesX >= 6400 ? 160 : 80);

            //sets the height for the catacombs second layer (how many rooms it has vertically)
            //350 = large worlds (6 rooms deep), 300 = medium worlds (5 rooms deep), 250 = small worlds (4 rooms deep)
            int layer2Depth = Main.maxTilesY >= 2400 ? 350 : (Main.maxTilesY >= 1800 ? 300 : 250);

            //randomize room pattern
            RoomPatternLayer2 = RoomPatternLayer2.OrderBy(x => Main.rand.Next()).ToArray();

            int layer2Start = (int)Main.worldSurface + layer1Depth + 118;

            //again, place a circle of bricks where each catacomb room will be
            //since the rooms in layer 2 are wider, place two circles side by side
            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
                for (int Y = layer2Start; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    SpookyWorldMethods.PlaceCircle(X - 20, Y, ModContent.TileType<CatacombBrick2>(), ModContent.WallType<CatacombBrickWall2>(), 40, true, true);
                    SpookyWorldMethods.PlaceCircle(X + 20, Y, ModContent.TileType<CatacombBrick2>(), ModContent.WallType<CatacombBrickWall2>(), 40, true, true);
                }
            }

            //place circles around where the big bone arena will generate
            //this is done before generating the layer two rooms so it doesnt destroy them
            int BigBoneArenaY = (int)Main.worldSurface + layer1Depth + layer2Depth + 50;

            for (int X = XMiddle - 100; X <= XMiddle + 100; X += 5)
            {
                for (int Y = BigBoneArenaY - 35; Y <= BigBoneArenaY + 45; Y += 5)
                {
                    SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick2>(), ModContent.WallType<CatacombBrickWall2>(), 10, true, true);
                }
            }

            //place the actual rooms
            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
                for (int Y = layer2Start; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    chosenRoom = RoomPatternLayer2[switchRoom];

                    switchRoom += 1; //Main.rand.Next(1, 3);

                    if (switchRoom >= RoomPatternLayer2.Length)
                    {
                        switchRoom = 0;
                    }

                    //origin offset for each room so it places at the center
                    Vector2 origin = new Vector2(X - 35, Y - 18);

                    //first row
                    if (Y == layer2Start)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        //do not place the first loot room in the middle where the entrance is either
                        if (!placedLootRoom1 && X != XMiddle && (WorldGen.genRand.NextBool(3) || X == XMiddle + layer2Width))
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

                        //place entrance room in the top row in the center
                        if (X == XMiddle)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/EntranceRoom", origin.ToPoint16(), Mod);
                        }
                    }

                    //second row
                    if (Y == layer2Start + 42)
                    {
                        //randomly place the loot room, or place it automatically if it reaches the edge
                        if (!placedLootRoom2 && (WorldGen.genRand.NextBool(3) || X == XMiddle + layer2Width))
                        {
                            Layer2LootRooms[0] = new Vector2(origin.X, origin.Y);
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
                        if (!placedLootRoom3 && X != XMiddle && (WorldGen.genRand.NextBool(3) || X == XMiddle + layer2Width))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/LootRoom-3", origin.ToPoint16(), Mod);
                            placedLootRoom3 = true;
                        }
                        else if (X == XMiddle)
                        {
                            PandoraRoomPosition = new Vector2(origin.X, origin.Y);
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
                        if (!placedLootRoom4 && X != XMiddle && (WorldGen.genRand.NextBool(3) || X == XMiddle + layer2Width))
                        {
                            Layer2LootRooms[1] = new Vector2(origin.X, origin.Y);
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
                for (int Y = layer2Start; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    //actual hallway positions
                    Vector2 horizontalHallOrigin = new Vector2(X + 34, WorldGen.genRand.NextBool() ? Y + 3 : Y - 14);
                    Vector2 verticalHallOrigin = new Vector2(X - 7, Y + 15);

                    //for all rows besides the bottom, place horizontal halls between each room, which a chance to place a vertical hall on the bottom
                    if (Y < (int)Main.worldSurface + layer1Depth + layer2Depth - 20)
                    {
                        //dont place a hall on the last room
                        if (X < XMiddle + layer2Width)
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOrigin.ToPoint16(), Mod);
                        }

                        //place a vertical hall randomly under any room except for the pandoras box room
                        if (X != XMiddle || (Y != layer2Start + 84 && Y != layer2Start + 42))
                        {
                            Generator.GenerateStructure("Content/Structures/CatacombLayer2/VerticalHall-" + WorldGen.genRand.Next(1, 5), verticalHallOrigin.ToPoint16(), Mod);
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

            //place loot rooms for the second layer
            for (int numPoints = 0; numPoints < Layer2LootRooms.Length; numPoints++)
            {
                if (numPoints == 0)
                {
                    Generator.GenerateStructure("Content/Structures/CatacombLayer2/LootRoom-2", Layer2LootRooms[numPoints].ToPoint16(), Mod);
                }
                else
                {
                    Generator.GenerateStructure("Content/Structures/CatacombLayer2/LootRoom-4", Layer2LootRooms[numPoints].ToPoint16(), Mod);
                }

                Vector2 horizontalHallOriginLeft = new Vector2(Layer2LootRooms[numPoints].X - 11, Layer2LootRooms[numPoints].Y + 21);
                Vector2 horizontalHallOriginRight = new Vector2(Layer2LootRooms[numPoints].X + 69, Layer2LootRooms[numPoints].Y + 21);

                //manually place hallways since the loot rooms here only have one entrance

                //if the room is placed at the very left of the catacombs, place a horizontal hall on the right side
                if (Layer2LootRooms[numPoints].X <= XMiddle - layer2Width - 35)
                {
                    Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOriginRight.ToPoint16(), Mod);
                }
                //if the room is placed at the very right of the catacombs, place a horizontal hall on the left side
                if (Layer2LootRooms[numPoints].X >= XMiddle + layer2Width - 35)
                {
                    Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOriginLeft.ToPoint16(), Mod);
                }
                //otherwise place horizontal halls on both sides of the room
                if (Layer2LootRooms[numPoints].X > XMiddle - layer2Width - 35 && Layer2LootRooms[numPoints].X < XMiddle + layer2Width - 35)
                {
                    Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOriginLeft.ToPoint16(), Mod);
                    Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall-" + WorldGen.genRand.Next(1, 5), horizontalHallOriginRight.ToPoint16(), Mod);
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
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick1>(), ModContent.WallType<CatacombBrickWall1>(), 10, true, true);
                    }
                    //on the bottom half, place the second layer bricks
                    else
                    {
                        SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CatacombBrick2>(), ModContent.WallType<CatacombBrickWall2>(), 10, true, true);
                    }
                }
            }

            //place the daffodil arena
            Generator.GenerateStructure("Content/Structures/CatacombLayer1/DaffodilArena", DaffodilArenaOrigin.ToPoint16(), Mod);

            //spawn daffodil itself in the arena
            Flags.DaffodilPosition = new Vector2(XMiddle * 16, DaffodilArenaY * 16);
            int Daffodil = NPC.NewNPC(null, (int)Flags.DaffodilPosition.X, (int)Flags.DaffodilPosition.Y, ModContent.NPCType<DaffodilBody>());
            Main.npc[Daffodil].position.X -= 9;
            Main.npc[Daffodil].position.Y += 10;

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


            //pandora's box arena
            Generator.GenerateStructure("Content/Structures/CatacombLayer2/PandoraRoom", PandoraRoomPosition.ToPoint16(), Mod);

            int layer2StartThing = (int)Main.worldSurface + layer1Depth + 118;
            int PandoraBoxSpawnY = layer2StartThing + 84;

            //spawn pandoras box
            Flags.PandoraPosition = new Vector2(XMiddle * 16, PandoraBoxSpawnY * 16);
            int PandoraBox = NPC.NewNPC(null, (int)Flags.PandoraPosition.X, (int)Flags.PandoraPosition.Y, ModContent.NPCType<PandoraBox>());
            Main.npc[PandoraBox].position.X -= 8;


            //place big bone arena
            Vector2 BigBoneArenaOrigin = new Vector2(XMiddle - 53, BigBoneArenaY - 35);

            Generator.GenerateStructure("Content/Structures/CatacombLayer2/BigBoneArena", BigBoneArenaOrigin.ToPoint16(), Mod);

            //spawn giant flower pot in the big bone arena
            Flags.FlowerPotPosition = new Vector2(XMiddle * 16, BigBoneArenaY * 16);
            int FlowerPot = NPC.NewNPC(null, (int)Flags.FlowerPotPosition.X, (int)Flags.FlowerPotPosition.Y, ModContent.NPCType<BigFlowerPot>());
            Main.npc[FlowerPot].position.X -= 6;

            //dig entrance to big bone's arena
            for (int tunnelX = XMiddle - 3; tunnelX <= XMiddle + 1; tunnelX++)
            {
                //this determines how far down the big bone arena entrance is
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

            //ambient tiles and grass walls
            for (int X = XMiddle - 300; X <= XMiddle + 300; X++)
            {
                for (int Y = (int)Main.worldSurface - 10; Y <= Main.maxTilesY - 200; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];

                    //place grass walls
                    if (!tile.HasTile && Y <= DaffodilArenaY - 40 && tile.WallType == ModContent.WallType<CatacombBrickWall1>() && WorldGen.genRand.NextBool(250))
                    {
                        SpookyWorldMethods.ModifiedTileRunner(X, Y, WorldGen.genRand.Next(8, 15), 1, ModContent.TileType<CatacombBrick1>(),
                        ModContent.WallType<CatacombGrassWall1>(), false, 0f, 0f, true, false, true, true);
                    }

                    //catacomb vines and weeds
                    if (tile.TileType == ModContent.TileType<CatacombBrick1Grass>())
                    {
                        if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile)
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<CatacombVines>());
                        }

                        if (WorldGen.genRand.NextBool(12) && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SporeMushroom>());
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                        }

                        if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<CatacombWeeds>());
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                        }
                    }
                    if (tile.TileType == ModContent.TileType<CatacombVines>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 3), (ushort)ModContent.TileType<CatacombVines>());
                    }
                }

                //ambient tiles for layer 2
                for (int Y = DaffodilArenaY + 50; Y <= Main.maxTilesY - 100; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];

                    //place grass walls
                    if (!tile.HasTile && Y < BigBoneArenaY - 50 && tile.WallType == ModContent.WallType<CatacombBrickWall2>() && WorldGen.genRand.NextBool(250))
                    {
                        SpookyWorldMethods.ModifiedTileRunner(X, Y, WorldGen.genRand.Next(10, 25), 1, ModContent.TileType<CatacombBrick2>(), 
                        ModContent.WallType<CatacombGrassWall2>(), false, 0f, 0f, true, false, true, true);
                    }
 
                    //catacomb vines and weeds
                    if (tile.TileType == ModContent.TileType<CatacombBrick2Grass>())
                    {
                        if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile)
                        {
                            WorldGen.PlaceTile(X, Y + 1, (ushort)ModContent.TileType<CatacombVines2>());
                        }

                        if (WorldGen.genRand.NextBool(12) && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<SporeMushroom>());
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                        }

                        if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<CatacombWeeds>());
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                        }
                    }
                    if (tile.TileType == ModContent.TileType<CatacombVines2>())
                    {
                        SpookyWorldMethods.PlaceVines(X, Y, WorldGen.genRand.Next(1, 3), (ushort)ModContent.TileType<CatacombVines2>());
                    }
                }
            }
        }

        //determine if theres no snow blocks nearby so the biome doesnt place in the snow biome
        public static bool NoJungleNearby(int X, int Y)
        {
            for (int i = X - 50; i < X + 50; i++)
            {
                for (int j = Y - 50; j < Y + 50; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.JungleGrass)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool GrowGiantFlower(int X, int Y, int tileType)
        {
            //do not allow giant flowers to place if another one is too close
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        return false;
                    }
                }
            }

            //make sure the area is large enough for it to place in both horizontally and vertically
            for (int i = X - 2; i < X + 2; i++)
            {
                for (int j = Y - 8; j < Y - 2; j++)
                {
                    //only check for solid blocks, ambient objects dont matter
                    if (Main.tile[i, j].HasTile)
                    {
                        return false;
                    }
                }
            }

            BigFlower.Grow(X, Y - 1, 3, 6);

            return true;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Creepy Catacombs", PlaceCatacomb));

            //re-locate the jungle temple deeper underground and further horizontally so it never gets generated over by the catacombs
            int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
            {
                //first define the y-position 
                int newTempleY = Main.maxTilesY - (Main.maxTilesY / 2) + WorldGen.genRand.Next(20, 80);

                //middle of the where the cemetery/catacombs is
                int XStart = PositionX - (Cemetery.BiomeWidth / 2);
                int XMiddle = PositionX;

                //attempt to find a valid position for the jungle temple to place in, just in case it generates far away from the jungle
                bool foundValidPosition = false;
                int attempts = 0;

                //keep moving towards the center of the world until a valid position in the jungle is found
                while (!foundValidPosition && attempts++ < 100000)
                {
                    while (NoJungleNearby(XMiddle, newTempleY))
                    {
                        XMiddle += (XMiddle > (Main.maxTilesX / 2) ? -100 : 100);
                    }
                    if (!NoJungleNearby(XMiddle, newTempleY))
                    {
                        foundValidPosition = true;
                    }
                }

                //define the x-position and then place the temple after finding a valid position
                int newTempleX = XMiddle < (Main.maxTilesX / 2) ? XMiddle + 400 : XMiddle - 400;

                WorldGen.makeTemple(newTempleX, newTempleY);
            });

            //re-locate the shimmer to be closer to the edge of the world so it also never gets generated over by the catacombs
            int shimmerIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shimmer"));
            tasks[shimmerIndex] = new PassLegacy("Shimmer", (progress, config) =>
            {
                //copy-pasted and slightly modified shimmer generation code from terraria itself
                int RandomY1 = (int)(Main.worldSurface + Main.rockLayer) / 2 + 100;
                int RandomY2 = (int)((double)((Main.maxTilesY - 250) * 2) + Main.rockLayer) / 3;

                if (RandomY2 > Main.maxTilesY - 200)
                {
                    RandomY2 = Main.maxTilesY - 200;
                }
                if (RandomY2 <= RandomY1)
                {
                    RandomY2 = RandomY1 + 50;
                }

                int ShimmerX = GenVars.dungeonSide < 0 ? Main.maxTilesX - 100 : 100;
                int ShimmerY = WorldGen.genRand.Next(RandomY1, RandomY2);

                int ShimmerXAnniversary = (int)Main.worldSurface + 150;
                int ShimmerYAnniversary = (int)(Main.rockLayer + Main.worldSurface + 200) / 2;

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
                    //this changes the shimmer position to be closer to the edge of the world
                    ShimmerX = (GenVars.dungeonSide < 0) ? (int)(Main.maxTilesX * 0.95f) : (int)(Main.maxTilesX * 0.05f);
                    ShimmerY = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2 + 22, RandomY2);
                }

                GenVars.shimmerPosition = new Vector2D((double)ShimmerX, (double)ShimmerY);

                //add the shimmer as a protected structure so nothing attempts to generate over it
                GenVars.structures.AddProtectedStructure(new Rectangle(ShimmerX - 200 / 2, ShimmerY - 200 / 2, 200, 200));
            });
        }

        //place items in chests
        public override void PostWorldGen()
		{
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex];

				if (chest == null)
                {
					continue;
				}

				Tile chestTile = Main.tile[chest.x, chest.y];

                //place loot in the first layer chests
                if (chestTile.TileType == TileID.Containers && (chestTile.WallType == ModContent.WallType<CatacombBrickWall1>() || chestTile.WallType == ModContent.WallType<CatacombGrassWall1>()))
                {
                    //place stuff in barrels
                    if (chestTile.TileFrameX == 5 * 36)
                    {
                        int[] RareItem = new int[] { ItemID.WaterBolt, ItemID.BoneSword, ItemID.BonePickaxe };

                        int[] Ammo = new int[] { ItemID.MusketBall, ItemID.WoodenArrow, ItemID.Flare };

                        if (WorldGen.genRand.NextBool(10))
                        {
                            chest.item[0].SetDefaults(WorldGen.genRand.Next(RareItem));
                        }
                        else if (WorldGen.genRand.NextBool(5))
                        {
                            chest.item[0].SetDefaults(ItemID.GoodieBag);
                            chest.item[0].stack = WorldGen.genRand.Next(1, 3);
                        }
                        else
                        {
                            chest.item[0].SetDefaults(WorldGen.genRand.Next(Ammo));
                            chest.item[0].stack = WorldGen.genRand.Next(10, 21);
                        }

                        chest.item[1].SetDefaults(ItemID.SilverCoin);
                        chest.item[1].stack = WorldGen.genRand.Next(2, 16);
                    }

                    //place stuff in pumpkin chests, do not put stuff in the trapped chest
                    if (chestTile.TileFrameX == 45 * 36 && chest.item[0].type != ItemID.GasTrap) 
                    {
                        //potions
                        int[] Potions1 = new int[] { ItemID.FeatherfallPotion, ItemID.NightOwlPotion, ItemID.WaterWalkingPotion,
                        ItemID.ArcheryPotion, ItemID.PotionOfReturn, ItemID.SwiftnessPotion };

                        //more potions
                        int[] Potions2 = new int[] { ItemID.IronskinPotion, ItemID.RegenerationPotion, ItemID.HunterPotion,
                        ItemID.InvisibilityPotion, ItemID.RagePotion, ItemID.WrathPotion };

                        //ammos
                        int[] Ammo = new int[] { ItemID.MusketBall, ItemID.WoodenArrow, ItemID.Flare };

                        //demonite or crimtane bar depending on crimson or corruption worlds
                        int Bars = !WorldGen.crimson ? ItemID.DemoniteBar : ItemID.CrimtaneBar;

                        //bars
                        chest.item[1].SetDefaults(Bars);
                        chest.item[1].stack = WorldGen.genRand.Next(5, 11);
                        //spike balls
                        chest.item[2].SetDefaults(ItemID.SpikyBall);
                        chest.item[2].stack = WorldGen.genRand.Next(12, 19);
                        //potions
                        chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions1));
                        chest.item[3].stack = WorldGen.genRand.Next(1, 3);
                        //even more potions
                        chest.item[4].SetDefaults(WorldGen.genRand.Next(Potions2));
                        chest.item[4].stack = WorldGen.genRand.Next(1, 3);
                        //ammo
                        chest.item[5].SetDefaults(WorldGen.genRand.Next(Ammo));
                        chest.item[5].stack = WorldGen.genRand.Next(20, 41);
                        //goodie bags
                        chest.item[6].SetDefaults(ItemID.GoodieBag);
                        chest.item[6].stack = WorldGen.genRand.Next(1, 3);
                        //gold coins
                        chest.item[7].SetDefaults(ItemID.GoldCoin);
                        chest.item[7].stack = WorldGen.genRand.Next(1, 6);
                    }
                }

                //place loot in the second layer chests
                if (chestTile.TileType == TileID.Containers && (chestTile.WallType == ModContent.WallType<CatacombBrickWall2>() || chestTile.WallType == ModContent.WallType<CatacombGrassWall2>()))
                {
                    //place stuff in barrels
                    if (chestTile.TileFrameX == 5 * 36)
                    {
                        int[] Ammo = new int[] { ItemID.VenomArrow, ModContent.ItemType<RustedBullet>() };

                        if (chest.item[0].type != ModContent.ItemType<Fertilizer>())
                        {
                            if (WorldGen.genRand.NextBool(5))
                            {
                                chest.item[0].SetDefaults(ItemID.GoodieBag);
                                chest.item[0].stack = WorldGen.genRand.Next(1, 3);
                            }
                            else
                            {
                                chest.item[0].SetDefaults(WorldGen.genRand.Next(Ammo));
                                chest.item[0].stack = WorldGen.genRand.Next(10, 21);
                            }
                        }

                        chest.item[1].SetDefaults(ItemID.SilverCoin);
                        chest.item[1].stack = WorldGen.genRand.Next(2, 16);
                    }

                    //place stuff in pumpkin chests
                    if (chestTile.TileFrameX == 45 * 36)
                    {
                        //potions
                        int[] Potions1 = new int[] { ItemID.AmmoReservationPotion, ItemID.BattlePotion, ItemID.CratePotion, ItemID.EndurancePotion };

                        //more potions
                        int[] Potions2 = new int[] { ItemID.LuckPotion, ItemID.InfernoPotion, ItemID.ShinePotion, ItemID.LifeforcePotion };

                        //recorvery potions
                        int[] RecoveryPotions = new int[] { ItemID.GreaterHealingPotion, ItemID.GreaterManaPotion };

                        //ammos
                        int[] Ammo = new int[] { ItemID.GoldenBullet, ItemID.HellfireArrow };

                        //bars
                        int[] Bars = new int[] { ItemID.AdamantiteBar, ItemID.TitaniumBar };

                        //bars
                        chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
                        chest.item[1].stack = WorldGen.genRand.Next(10, 23);
                        //potions
                        chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions1));
                        chest.item[2].stack = WorldGen.genRand.Next(1, 3);
                        //even more potions
                        chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions2));
                        chest.item[3].stack = WorldGen.genRand.Next(1, 3);
                        //ammo
                        chest.item[4].SetDefaults(WorldGen.genRand.Next(Ammo));
                        chest.item[4].stack = WorldGen.genRand.Next(20, 41);
                        //recovery potions
                        chest.item[5].SetDefaults(WorldGen.genRand.Next(RecoveryPotions));
                        chest.item[5].stack = WorldGen.genRand.Next(3, 7);
                        //goodie bags
                        chest.item[6].SetDefaults(ItemID.GoodieBag);
                        chest.item[6].stack = WorldGen.genRand.Next(1, 3);
                        //gold coins
                        chest.item[7].SetDefaults(ItemID.GoldCoin);
                        chest.item[7].stack = WorldGen.genRand.Next(1, 6);
                    }

                    //place stuff in bone chests
                    if (chestTile.TileFrameX == 41 * 36) 
                    {
                        //recorvery potions
                        int[] RecoveryPotions = new int[] { ItemID.GreaterHealingPotion, ItemID.GreaterManaPotion };

                        //ammos
                        int[] Ammo = new int[] { ItemID.GoldenBullet, ItemID.JestersArrow };

                        //bars
                        int[] Bars = new int[] { ItemID.AdamantiteBar, ItemID.TitaniumBar };

                        //bars
                        chest.item[0].SetDefaults(WorldGen.genRand.Next(Bars));
                        chest.item[0].stack = WorldGen.genRand.Next(3, 11);
                        //recovery potions
                        chest.item[1].SetDefaults(WorldGen.genRand.Next(RecoveryPotions));
                        chest.item[1].stack = WorldGen.genRand.Next(1, 4);
                        //ammos
                        chest.item[2].SetDefaults(WorldGen.genRand.Next(Ammo));
                        chest.item[2].stack = WorldGen.genRand.Next(8, 16);
                    }
                }
            }
        }
    }
}