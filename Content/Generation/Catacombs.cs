using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Generation;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.IO;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Cemetery;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.PandoraBox;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.Catacomb.Furniture;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.Painting;
using Spooky.Content.Tiles.SpiderCave.Furniture;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Generation
{
    public class Catacombs : ModSystem
    {
        int chosenRoom = 0;
        int switchRoom = 0;

        public static int EntranceY = 0;
        public static int EntranceBottomY = 0;
        public static int PositionX = 0;

		Vector2 PandoraRoomPosition;

        public static List<ushort> Paintings = new()
        {
            (ushort)ModContent.TileType<AlienPainting>(),
            (ushort)ModContent.TileType<BaxterWitchPainting>(),
            (ushort)ModContent.TileType<DavePainting>(),
            (ushort)ModContent.TileType<GrannyBatPainting>(),
            (ushort)ModContent.TileType<GrannySkeletonPainting>(),
            (ushort)ModContent.TileType<GrannyWitchPainting>(),
            (ushort)ModContent.TileType<GravestonePainting>(),
            (ushort)ModContent.TileType<HogPainting>(),
            (ushort)ModContent.TileType<HorsePainting>(),
            (ushort)ModContent.TileType<ShoebillPainting>(),
            (ushort)ModContent.TileType<SmilingFriendsPainting>(),
            (ushort)ModContent.TileType<SurprisedSkullPainting>(),
            (ushort)ModContent.TileType<TheKillerPainting>(),
            (ushort)ModContent.TileType<ZomboidThinkPainting>()
        };

		private void PlaceCatacomb(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Catacombs1").Value;

            int XStart = PositionX - (Cemetery.BiomeWidth / 2);
            int XMiddle = PositionX;

            bool IsSmallWorld = Main.maxTilesX < 6400 && Main.maxTilesY < 1800;

            //LAYER 1

            //set the width for the catacombs (how many rooms it has horizontally)
            //200 = large worlds (9 rooms wide), 150 = medium worlds (7 rooms wide), 100 = small worlds (5 rooms wide)
            int layer1Width = Main.maxTilesX >= 8400 ? 200 : (Main.maxTilesX >= 6400 ? 150 : 100);

            //sets the height for the catacombs (how many rooms it has vertically)
            //235 = large worlds (6 rooms deep), 190 = medium worlds (5 rooms deep), 145 = small worlds (4 rooms deep)
            int layer1Depth = Main.maxTilesY >= 2400 ? 235 : (Main.maxTilesY >= 1800 ? 190 : 145);

            //first, place giant square where the catacombs will be
            for (int X = XMiddle - layer1Width - 40; X <= XMiddle + layer1Width + 40; X++)
            {
				int StartValue = XMiddle - layer1Width - 40;
				int EndValue = XMiddle + layer1Width + 40;
				progress.Set((float)(X - StartValue) / (EndValue - StartValue));

				for (int Y = (int)Main.worldSurface - 32; Y <= (int)Main.worldSurface + layer1Depth + 45; Y++)
                {
                    Main.tile[X, Y].ClearEverything();
                    WorldGen.PlaceTile(X, Y, ModContent.TileType<CatacombBrick1>());
                    WorldGen.PlaceWall(X, Y, ModContent.WallType<CatacombBrickWall1>());
                }
            }

            int DaffodilArenaY = (int)Main.worldSurface + layer1Depth + 55;

            //place square of brick around where daffodils arena will generate
            for (int X = XMiddle - layer1Width - 40; X <= XMiddle + layer1Width + 40; X++)
            {
                for (int Y = DaffodilArenaY - 30; Y <= DaffodilArenaY + 30; Y++)
                {
                    Main.tile[X, Y].ClearEverything();
                    
                    //place the first layer bricks around the top half of the arena
                    if (Y <= DaffodilArenaY)
                    {
                        WorldGen.PlaceTile(X, Y, ModContent.TileType<CatacombBrick1>());
                        WorldGen.PlaceWall(X, Y, ModContent.WallType<CatacombBrickWall1>());
                    }
                    else if (Y > DaffodilArenaY && Y <= DaffodilArenaY + 10)
                    {
                        if (WorldGen.genRand.NextBool())
                        {
                            WorldGen.PlaceTile(X, Y, ModContent.TileType<CatacombBrick2>());
                            WorldGen.PlaceWall(X, Y, ModContent.WallType<CatacombBrickWall2>());
                        }
                        else
                        {
                            WorldGen.PlaceTile(X, Y, ModContent.TileType<CatacombBrick1>());
                            WorldGen.PlaceWall(X, Y, ModContent.WallType<CatacombBrickWall1>());
                        }
                    }
                    //on the bottom half, place the second layer bricks
                    else
                    {
                        WorldGen.PlaceTile(X, Y, ModContent.TileType<CatacombBrick2>());
                        WorldGen.PlaceWall(X, Y, ModContent.WallType<CatacombBrickWall2>());
                    }
                }
            }

            bool PlacedMoyaiRoom = false;
            bool PlacedMineRoom = false;

            //place the actual rooms in a grid
            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
				for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth; Y += 45)
                {
                    //origin offset for each room so it places at the center of the position its placed at
                    Vector2 origin = new Vector2(X - 18, Y - 18);

                    int WoodenRoomChance = Main.maxTilesY >= 2400 ? 7 : (Main.maxTilesY >= 1800 ? 6 : 5);
                    int TrapRoomChance = Main.maxTilesY >= 2400 ? 6 : (Main.maxTilesY >= 1800 ? 5 : 4);
                    int MineRoomChance = Main.maxTilesY >= 2400 ? 8 : (Main.maxTilesY >= 1800 ? 7 : 6);

                    //painting or library room room
                    if (WorldGen.genRand.NextBool(WoodenRoomChance))
                    {
                        if (WorldGen.genRand.NextBool())
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/PaintingRoom" + WorldGen.genRand.Next(1, 3) + ".shstruct", origin.ToPoint16(), Mod);

                            List<ushort> ActualPainting = new List<ushort>(Paintings);

                            //place paintings in the room
                            for (int paintingX = (int)origin.X + 4; paintingX <= (int)origin.X + 32; paintingX++)
                            {
                                for (int paintingY = (int)origin.Y + 4; paintingY <= (int)origin.Y + 32; paintingY++)
                                {
                                    if (WorldGen.genRand.NextBool(15))
                                    {
                                        if (ActualPainting.Count == 0)
                                        {
                                            ActualPainting = new List<ushort>(Paintings);
                                        }

                                        int PaintingToPlace = WorldGen.genRand.Next(ActualPainting.Count);
                                        bool Success = WorldGen.PlaceObject(paintingX, paintingY, ActualPainting[PaintingToPlace]);
                                        if (Success)
                                        {
                                            ActualPainting.RemoveAt(PaintingToPlace);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/LibraryRoom" + WorldGen.genRand.Next(1, 3) + ".shstruct", origin.ToPoint16(), Mod);

                            //place furniture in the room
                            for (int furnitureX = (int)origin.X; furnitureX <= (int)origin.X + 36; furnitureX++)
                            {
                                for (int furnitureY = (int)origin.Y; furnitureY <= (int)origin.Y + 36; furnitureY++)
                                {
                                    if (CanPlaceFurniture(furnitureX, furnitureY, 7, CheckWood: true))
                                    {
                                        switch (WorldGen.genRand.Next(2))
                                        {
                                            case 0:
                                            {
                                                WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodBookcase>());
                                                break;
                                            }
                                            case 1:
                                            {
                                                if (WorldGen.genRand.NextBool())
                                                {
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodTable>());
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 3, ModContent.TileType<OldWoodCandle>());
                                                }
                                                else
                                                {
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodWorkBench>());
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //trap rooms
                    else if (WorldGen.genRand.NextBool(TrapRoomChance))
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/TrapRoom" + WorldGen.genRand.Next(1, 3) + ".shstruct", origin.ToPoint16(), Mod);
                    }
                    //default room
                    else
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/Room" + WorldGen.genRand.Next(1, 21) + ".shstruct", origin.ToPoint16(), Mod);
                    }

                    //mine room
                    if (WorldGen.genRand.NextBool(MineRoomChance) && !PlacedMineRoom)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/MineRoom.shstruct", origin.ToPoint16(), Mod);
                        PlacedMineRoom = true;
                    }

                    //rare moyai room
                    if (WorldGen.genRand.NextBool(50) && !PlacedMoyaiRoom)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/MoyaiRoom.shstruct", origin.ToPoint16(), Mod);
                        PlacedMoyaiRoom = true;
                    }
                }
            }

            //place hallways
            bool PlaceVerticalHall = true;
            bool PlaceAdditionalHorizontalHall = false;

            List<int> GuaranteedVerticalHallPositions = new() { };

            for (int X = XMiddle - layer1Width; X <= XMiddle + layer1Width; X += 50)
            {
                GuaranteedVerticalHallPositions.Add(X);
            }
            for (int Y = (int)Main.worldSurface + 10; Y <= (int)Main.worldSurface + layer1Depth - 45; Y += 45)
            {
                //place at least one vertical hall per row
                Vector2 guaranteedVerticalHallOrigin = new Vector2(WorldGen.genRand.Next(GuaranteedVerticalHallPositions) - 7, Y + 15);
                StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/VerticalHall.shstruct", guaranteedVerticalHallOrigin.ToPoint16(), Mod);
            }

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
                        //randomly place a vertical hall, otherwise place a horizontal hall
                        if (WorldGen.genRand.NextBool(3))
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/VerticalHall.shstruct", verticalHallOrigin.ToPoint16(), Mod);
                            PlaceVerticalHall = true;
                        }
                        else
                        {
                            //dont place a hall on the last room
                            if (X < XMiddle + layer1Width)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16(), Mod);
                            }
                            else
                            {
                                if (WorldGen.genRand.NextBool())
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/VerticalHall.shstruct", verticalHallOrigin.ToPoint16(), Mod);
                                }
                                else
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16() - new Vector2(50, 0).ToPoint16(), Mod);
                                }
                            }
                        }

                        //swap between placing a guaranteed vertical or horizontal hall, which adds randomness
                        //this is done after the code above to ensure that every single room in the catacombs is accessible
                        if (!PlaceVerticalHall)
                        {
                            PlaceVerticalHall = true;
                        }
                        else
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/VerticalHall.shstruct", verticalHallOrigin.ToPoint16(), Mod);

                            if (!PlaceAdditionalHorizontalHall)
                            {
                                PlaceAdditionalHorizontalHall = true;
                            }
                            else
                            {
                                if (X < XMiddle + layer1Width)
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16(), Mod);
                                }

                                PlaceAdditionalHorizontalHall = false;
                            }

                            PlaceVerticalHall = false;
                        }
                    }
                    //on the bottom row of rooms, only place horizontal halls
                    else
                    {
                        if (X < XMiddle + layer1Width)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                }
            }


			//LAYER 2

			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Catacombs2").Value;

            //sets the width for the catacombs second layer (how many rooms it has horizontally)
            //240 = large worlds (9 rooms wide), 160 = medium worlds (5 rooms wide), 80 = small worlds (3 rooms wide)
            int layer2Width = Main.maxTilesX >= 8400 ? 240 : (Main.maxTilesX >= 6400 ? 160 : 80);

            //sets the height for the catacombs second layer (how many rooms it has vertically)
            //350 = large worlds (6 rooms deep), 300 = medium worlds (5 rooms deep), 250 = small worlds (4 rooms deep)
            int layer2Depth = Main.maxTilesY >= 2400 ? 350 : (Main.maxTilesY >= 1800 ? 300 : 250);

            int layer2Start = (int)Main.worldSurface + layer1Depth + 118;

            //first, place giant square where the catacombs will be
            for (int X = XMiddle - layer2Width - 60; X <= XMiddle + layer2Width + 60; X++)
            {
				int StartValue = XMiddle - layer2Width - 60;
				int EndValue = XMiddle + layer2Width + 60;
				progress.Set((float)(X - StartValue) / (EndValue - StartValue));

				for (int Y = layer2Start - 45; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth + 30; Y++)
                {
                    Main.tile[X, Y].ClearEverything();
                    WorldGen.PlaceTile(X, Y, ModContent.TileType<CatacombBrick2>());
                    WorldGen.PlaceWall(X, Y, ModContent.WallType<CatacombBrickWall2>());
                }
            }

            //place box around where the big bone arena will generate
            //this is done before generating the layer two rooms so it doesnt destroy them
            int BigBoneArenaY = (int)Main.worldSurface + layer1Depth + layer2Depth + 50;

            for (int X = XMiddle - 100; X <= XMiddle + 100; X++)
            {
                for (int Y = BigBoneArenaY - 35; Y <= BigBoneArenaY + 70; Y++)
                {
                    Main.tile[X, Y].ClearEverything();
                    WorldGen.PlaceTile(X, Y, ModContent.TileType<CatacombBrick2>());
                    WorldGen.PlaceWall(X, Y, ModContent.WallType<CatacombBrickWall2>());
                }
            }

            bool PlacedAvariceRoom = false;

            //place the actual rooms
            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
				for (int Y = layer2Start; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    //origin offset for each room so it places at the center
                    Vector2 origin = new Vector2(X - 35, Y - 18);

                    if (X == XMiddle && Y == layer2Start + 84)
                    {
                        PandoraRoomPosition = origin;
                    }
                    
                    int WoodenRoomChance = Main.maxTilesY >= 2400 ? 7 : (Main.maxTilesY >= 1800 ? 6 : 5);
                    int PuzzleRoomChance = Main.maxTilesY >= 2400 ? 6 : (Main.maxTilesY >= 1800 ? 5 : 4);
                    int MineRoomChance = Main.maxTilesY >= 2400 ? 8 : (Main.maxTilesY >= 1800 ? 7 : 6);

                    //library or living quarters room
                    if (WorldGen.genRand.NextBool(WoodenRoomChance))
                    {
                        if (WorldGen.genRand.NextBool())
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/LibraryRoom" + WorldGen.genRand.Next(1, 3) + ".shstruct", origin.ToPoint16(), Mod);

                            //place furniture in the room
                            for (int furnitureX = (int)origin.X; furnitureX <= (int)origin.X + 69; furnitureX++)
                            {
                                for (int furnitureY = (int)origin.Y; furnitureY <= (int)origin.Y + 35; furnitureY++)
                                {
                                    if (CanPlaceFurniture(furnitureX, furnitureY, 7, CheckWood: true))
                                    {
                                        switch (WorldGen.genRand.Next(2))
                                        {
                                            case 0:
                                            {
                                                WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodBookcase>());
                                                break;
                                            }
                                            case 1:
                                            {
                                                //table with candle and chairs
                                                if (WorldGen.genRand.NextBool(3))
                                                {
                                                    WorldGen.PlaceObject(furnitureX - 2, furnitureY - 1, ModContent.TileType<OldWoodChair>(), direction: 1);
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodTable>());
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 3, ModContent.TileType<OldWoodCandle>());
                                                    WorldGen.PlaceObject(furnitureX + 2, furnitureY - 1, ModContent.TileType<OldWoodChair>(), direction: -1);
                                                }
                                                //table with books
                                                else if (WorldGen.genRand.NextBool())
                                                {
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodTable>());
                                                    WorldGen.PlaceObject(furnitureX - 1, furnitureY - 3, TileID.Books, true, WorldGen.genRand.Next(0, 5));
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 3, TileID.Books, true, WorldGen.genRand.Next(0, 5));
                                                    WorldGen.PlaceObject(furnitureX + 1, furnitureY - 3, TileID.Books, true, WorldGen.genRand.Next(0, 5));
                                                }
                                                //workbench
                                                else
                                                {
                                                    WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodWorkBench>());
                                                }
                                                
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/LivingQuarterRoom" + WorldGen.genRand.Next(1, 3) + ".shstruct", origin.ToPoint16(), Mod);

                            //place furniture in the room
                            for (int furnitureX = (int)origin.X; furnitureX <= (int)origin.X + 69; furnitureX++)
                            {
                                for (int furnitureY = (int)origin.Y; furnitureY <= (int)origin.Y + 35; furnitureY++)
                                {
                                    if (CanPlaceFurniture(furnitureX, furnitureY, 7, CheckWood: true))
                                    {
                                        switch (WorldGen.genRand.Next(3))
                                        {
                                            case 0:
                                            {
                                                WorldGen.PlaceObject(furnitureX - 2, furnitureY - 1, ModContent.TileType<OldWoodChair>(), direction: 1);
                                                WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodTable>());
                                                WorldGen.PlaceObject(furnitureX, furnitureY - 3, ModContent.TileType<OldWoodCandle>());
                                                WorldGen.PlaceObject(furnitureX + 2, furnitureY - 1, ModContent.TileType<OldWoodChair>(), direction: -1);
                                                break;
                                            }
                                            case 1:
                                            {
                                                WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodSofa>());
                                                break;
                                            }
                                            case 2:
                                            {
                                                WorldGen.PlaceObject(furnitureX, furnitureY - 1, ModContent.TileType<OldWoodBed>(), direction: WorldGen.genRand.NextBool() ? -1 : 1);
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //puzzle rooms
                    else if (WorldGen.genRand.NextBool(PuzzleRoomChance))
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/PuzzleRoom" + WorldGen.genRand.Next(1, 3) + ".shstruct", origin.ToPoint16(), Mod);
                    }
                    //default room
                    else
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/Room" + WorldGen.genRand.Next(1, 19) + ".shstruct", origin.ToPoint16(), Mod);
                    }

                    if (WorldGen.genRand.NextBool(25) && !PlacedAvariceRoom)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/AvaricePotRoom.shstruct", origin.ToPoint16(), Mod);
                        PlacedAvariceRoom = true;
                    }

                    //special biome chest room
                    if (X == XMiddle && Y >= (int)Main.worldSurface + layer1Depth + layer2Depth - 40)
                    {
                        StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/BiomeChestRoom.shstruct", origin.ToPoint16(), Mod);
                    }
                }
            }

            //place hallways
            PlaceVerticalHall = false;
            PlaceAdditionalHorizontalHall = false;

            GuaranteedVerticalHallPositions.Clear();

            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
                GuaranteedVerticalHallPositions.Add(X);
            }
            for (int Y = layer2Start; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth - 42; Y += 42)
            {
                //place at least one vertical hall per row
                Vector2 guaranteedVerticalHallOrigin = new Vector2(WorldGen.genRand.Next(GuaranteedVerticalHallPositions) - 7, Y + 15);
                StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/VerticalHall.shstruct", guaranteedVerticalHallOrigin.ToPoint16(), Mod);
            }

            for (int X = XMiddle - layer2Width; X <= XMiddle + layer2Width; X += 80)
            {
                for (int Y = layer2Start; Y <= (int)Main.worldSurface + layer1Depth + layer2Depth; Y += 42)
                {
                    //actual hallway positions
                    Vector2 horizontalHallOrigin = new Vector2(X + 34, WorldGen.genRand.NextBool() ? Y + 3 : Y - 14);
                    Vector2 verticalHallOrigin = new Vector2(X - 7, Y + 15);

                    //for all rows besides the bottom, place horizontal halls between each room, which a chance to place a vertical hall on the bottom
                    if (Y < (int)Main.worldSurface + layer1Depth + layer2Depth - 40)
                    {
                        //always place a vertical hall below the pandora box room
                        if (X == XMiddle && (Y == layer2Start + 42 || Y == layer2Start + 84))
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/VerticalHall.shstruct", verticalHallOrigin.ToPoint16(), Mod);
                        }

                        //randomly place a vertical hall, otherwise place a horizontal hall
                        if (WorldGen.genRand.NextBool(3))
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/VerticalHall.shstruct", verticalHallOrigin.ToPoint16(), Mod);

                            PlaceVerticalHall = true;
                        }
                        else
                        {
                            //dont place a hall on the last room
                            if (X < XMiddle + layer2Width)
                            {
                                StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16(), Mod);
                            }
                            else
                            {
                                if (WorldGen.genRand.NextBool())
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/VerticalHall.shstruct", verticalHallOrigin.ToPoint16(), Mod);
                                }
                                else
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16() - new Vector2(80, 0).ToPoint16(), Mod);
                                }
                            }
                        }

                        //swap between placing a guaranteed vertical or horizontal hall, which adds randomness
                        //this is done after the code above to ensure that every single room in the catacombs is accessible
                        if (!PlaceVerticalHall)
                        {
                            PlaceVerticalHall = true;
                        }
                        else
                        {
                            if (!PlaceAdditionalHorizontalHall)
                            {
                                PlaceAdditionalHorizontalHall = true;
                            }
                            else
                            {
                                if (X < XMiddle + layer2Width)
                                {
                                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16(), Mod);
                                }

                                PlaceAdditionalHorizontalHall = false;
                            }

                            PlaceVerticalHall = false;
                        }
                    }
                    //on the bottom row of rooms, only place horizontal halls
                    else
                    {
                        if (X < XMiddle + layer2Width)
                        {
                            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/HorizontalHall.shstruct", horizontalHallOrigin.ToPoint16(), Mod);
                        }
                    }
                }
            }


            //EXTRA STUFF

            //place catacomb platform shelves
            for (int X = XMiddle - 300; X <= XMiddle + 300; X++)
            {
                for (int Y = (int)Main.worldSurface - 10; Y <= BigBoneArenaY - 30; Y++)
				{
                    //randomly place paintings in the catacombs
                    if (WorldGen.genRand.NextBool(550) && !Main.tile[X, Y].HasTile)
					{
                        if (CanPlacePainting(X, Y, Paintings, true))
                        {
						    WorldGen.PlaceObject(X, Y, WorldGen.genRand.Next(Paintings));
                        }
					}

					//place catacomb platform shelves with stuff on them
					if (WorldGen.genRand.NextBool(20))
					{
						int Length = WorldGen.genRand.Next(3, 5);

						if (WorldGen.SolidTile(X, Y) && !Main.tile[X + 1, Y].HasTile && Main.tile[X, Y - 1].HasTile && Main.tile[X, Y + 1].HasTile)
						{
							if (CanPlacePlatform(X, Y, Length, false))
							{
								for (int x = X + 1; x <= X + Length; x++)
								{
									WorldGen.PlaceTile(x, Y, Main.tile[x, Y].WallType == ModContent.WallType<CatacombBrickWall1>() ? ModContent.TileType<CatacombBrickPlatform1>() : ModContent.TileType<CatacombBrickPlatform2>());

                                    if (WorldGen.genRand.NextBool(10))
									{
										int Type = Main.tile[x, Y].WallType == ModContent.WallType<CatacombBrickWall1>() ? ModContent.TileType<UpperCatacombCandle>() : ModContent.TileType<LowerCatacombCandle>();
										WorldGen.PlaceObject(x, Y - 1, Type);
									}
									else if (WorldGen.genRand.NextBool(5))
									{
										WorldGen.PlaceObject(x, Y - 1, TileID.ClayPot);
										if (WorldGen.genRand.NextBool(3))
                                        {
										    WorldGen.PlaceObject(x, Y - 2, TileID.BloomingHerbs, true, WorldGen.genRand.Next(0, 7));
                                        }
									}
									else
									{	
										WorldGen.PlaceObject(x, Y - 1, TileID.Books, true, WorldGen.genRand.Next(0, 5));
									}
								}
							}
						}
					}

					if (WorldGen.genRand.NextBool(20))
					{
						int Length = WorldGen.genRand.Next(3, 5);

						if (WorldGen.SolidTile(X, Y) && !Main.tile[X - 1, Y].HasTile && Main.tile[X, Y - 1].HasTile && Main.tile[X, Y + 1].HasTile)
						{
							if (CanPlacePlatform(X, Y, Length, true))
							{
								for (int x = X - Length; x <= X - 1; x++)
								{
									WorldGen.PlaceTile(x, Y, Main.tile[x, Y].WallType == ModContent.WallType<CatacombBrickWall1>() ? ModContent.TileType<CatacombBrickPlatform1>() : ModContent.TileType<CatacombBrickPlatform2>());

									if (WorldGen.genRand.NextBool(10))
									{
                                        int Type = Main.tile[x, Y].WallType == ModContent.WallType<CatacombBrickWall1>() ? ModContent.TileType<UpperCatacombCandle>() : ModContent.TileType<LowerCatacombCandle>();
										WorldGen.PlaceObject(x, Y - 1, Type);
									}
									else if (WorldGen.genRand.NextBool(5))
									{
										WorldGen.PlaceObject(x, Y - 1, TileID.ClayPot);
                                        if (WorldGen.genRand.NextBool(3))
                                        {
										    WorldGen.PlaceObject(x, Y - 2, TileID.BloomingHerbs, true, WorldGen.genRand.Next(0, 7));
                                        }
									}
									else
									{	
										WorldGen.PlaceObject(x, Y - 1, TileID.Books, true, WorldGen.genRand.Next(0, 5));
									}
								}
							}
						}
					}
                }
            }

            //place the entrance to the catacombs from the bottom of the cemetery crypt building
            int EntranceX = XMiddle - 5;

            for (int EntranceNewY = EntranceY + 60; EntranceNewY <= (int)Main.worldSurface - 6; EntranceNewY += 5)
            {
                Vector2 EntranceOrigin = new Vector2(EntranceX - 3, EntranceNewY + 1);
                Vector2 EntranceBarrierOrigin = new Vector2(EntranceX - 3, EntranceNewY - 1);

                //place the yellow barrier entrance once the catacombs is reached
                if (EntranceNewY == EntranceY + 60)
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/CatacombEntrance.shstruct", EntranceOrigin.ToPoint16(), Mod);
                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/CryptEntranceBarrier.shstruct", EntranceBarrierOrigin.ToPoint16(), Mod);
                }
                else
                {
                    StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/CatacombEntrance.shstruct", EntranceOrigin.ToPoint16(), Mod);
                }
            }

            //place the daffodil arena
            Vector2 DaffodilArenaOrigin = new Vector2(XMiddle - 52, DaffodilArenaY - 21);

            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer1/DaffodilArena.shstruct", DaffodilArenaOrigin.ToPoint16(), Mod);

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

                                //in the middle of the tunnel, place a chain that goes down
                                if (tunnelX == X - 1)
                                {
                                    WorldGen.PlaceTile(tunnelX, tunnelY - 1, TileID.Chain);
                                }
                            }
                            //place stuff in the tunnel
                            else
                            {
                                //in the middle of the tunnel, place a chain that goes down
                                if (tunnelX == X - 1)
                                {
                                    WorldGen.PlaceTile(tunnelX, tunnelY, TileID.Chain);
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

                StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/Entrance-" + WorldGen.genRand.Next(1, 5) + ".shstruct", entranceOrigin.ToPoint16(), Mod);
            }


            //pandora's box arena
            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/PandoraRoom.shstruct", PandoraRoomPosition.ToPoint16(), Mod);

            int layer2StartThing = (int)Main.worldSurface + layer1Depth + 118;
            int PandoraBoxSpawnY = layer2StartThing + 84;

            //spawn pandoras box
            Flags.PandoraPosition = new Vector2(XMiddle * 16, PandoraBoxSpawnY * 16);
            int PandoraBox = NPC.NewNPC(null, (int)Flags.PandoraPosition.X, (int)Flags.PandoraPosition.Y, ModContent.NPCType<PandoraBox>());
            Main.npc[PandoraBox].position.X -= 8;


            //place big bone arena
            Vector2 BigBoneArenaOrigin = new Vector2(XMiddle - 53, BigBoneArenaY - 35);

            StructureHelper.API.Generator.GenerateStructure("Content/Structures/CatacombLayer2/BigBoneArena", BigBoneArenaOrigin.ToPoint16(), Mod);

            //spawn giant flower pot in the big bone arena
            Flags.FlowerPotPosition = new Vector2(XMiddle * 16, BigBoneArenaY * 16);
            int FlowerPot = NPC.NewNPC(null, (int)Flags.FlowerPotPosition.X, (int)Flags.FlowerPotPosition.Y, ModContent.NPCType<BigFlowerPot>());
            Main.npc[FlowerPot].position.X -= 6;

            //dig entrance to big bone's arena
            for (int tunnelX = XMiddle - 3; tunnelX <= XMiddle + 1; tunnelX++)
            {
                //this determines how far down the big bone arena entrance is
                int extraDepthForEntrance = Main.maxTilesY >= 2400 ? -7 : (Main.maxTilesY >= 1800 ? 1 : 9);

                for (int tunnelY = (int)Main.worldSurface + layer1Depth + layer2Depth + extraDepthForEntrance; tunnelY <= BigBoneArenaY - 36; tunnelY++)
                {
                    Main.tile[tunnelX, tunnelY].ClearEverything();

                    //place brick walls in the tunnel
                    WorldGen.PlaceWall(tunnelX, tunnelY, (ushort)ModContent.WallType<CatacombBrickWall2>());

                    if (tunnelY == (int)Main.worldSurface + layer1Depth + layer2Depth + extraDepthForEntrance)
                    {
                        WorldGen.PlaceTile(tunnelX, tunnelY, ModContent.TileType<OldWoodPlatform>());

                        //place chain above the middle of the platform
                        if (tunnelX == XMiddle - 1)
                        {
                            WorldGen.PlaceTile(tunnelX, tunnelY - 1, TileID.Chain);
                        }
                    }
                    else
                    {
                        //in the middle of the tunnel, place a chain that goes down
                        if (tunnelX == XMiddle - 1)
                        {
                            WorldGen.PlaceTile(tunnelX, tunnelY, TileID.Chain);
                        }
                    }
                }
            }

            for (int X = XMiddle - 300; X <= XMiddle + 300; X++)
            {
                //furniture tiles for layer 1
				for (int Y = (int)Main.worldSurface - 10; Y <= DaffodilArenaY - 20; Y++)
				{
					Tile tile = Main.tile[X, Y];
					Tile tileAbove = Main.tile[X, Y - 1];
					Tile tileBelow = Main.tile[X, Y + 1];

                    if (tile.WallType == ModContent.WallType<CatacombBrickWall1>())
                    {
                        //place grass walls
                        if (WorldGen.genRand.NextBool(350) && !tile.HasTile && Y < BigBoneArenaY - 50)
                        {
                            int[] ValidTiles = { ModContent.TileType<CatacombBrick1>(), ModContent.TileType<CatacombFlooring>() };

                            SpookyWorldMethods.PlaceOval(X, Y, ModContent.TileType<CatacombBrick1Grass>(), ModContent.WallType<CatacombGrassWall1>(),
                            WorldGen.genRand.Next(5, 9), WorldGen.genRand.Next(5, 9), 1f, true, false, true, ValidTiles);
                        }

                        //tables and chairs with candles/candelabras on them
                        if (WorldGen.genRand.NextBool() && CanPlaceFurniture(X, Y, 10))
                        {
                            switch (WorldGen.genRand.Next(7))
                            {
                                //two tables with chairs and sometimes candles/candelabras on the tables
                                case 0:
                                {
                                    WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<UpperCatacombTable>());
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        int Type = WorldGen.genRand.NextBool() ? ModContent.TileType<UpperCatacombCandelabra>() : ModContent.TileType<UpperCatacombCandle>();
                                        WorldGen.PlaceObject(X - 3, Y - 3, Type);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X - 5, Y - 1, ModContent.TileType<UpperCatacombChair>(), direction: 1);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X - 1, Y - 1, ModContent.TileType<UpperCatacombChair>(), direction: -1);
                                    }

                                    WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<UpperCatacombTable>());
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        int Type = WorldGen.genRand.NextBool() ? ModContent.TileType<UpperCatacombCandelabra>() : ModContent.TileType<UpperCatacombCandle>();
                                        WorldGen.PlaceObject(X + 3, Y - 3, Type);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X + 5, Y - 1, ModContent.TileType<UpperCatacombChair>(), direction: -1);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X + 1, Y - 1, ModContent.TileType<UpperCatacombChair>(), direction: 1);
                                    }
                                    break;
                                }
                                //bookcases with lamps on either side of them
                                case 1:
                                {
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<UpperCatacombBookcase>());
                                        WorldGen.PlaceObject(X - 6, Y - 1, ModContent.TileType<UpperCatacombLamp>());
                                    }
                                    else
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<UpperCatacombLamp>());
                                    }
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<UpperCatacombBookcase>());
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<UpperCatacombBookcase>());
                                        WorldGen.PlaceObject(X + 6, Y - 1, ModContent.TileType<UpperCatacombLamp>());
                                    }
                                    else
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<UpperCatacombLamp>());
                                    }
                                    break;
                                }
                                //bed with some dressers and candle
                                case 2:
                                {
                                    WorldGen.PlaceObject(X - 5, Y - 1, (ushort)ModContent.TileType<UpperCatacombBed>(), direction: 1);

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceChest(X, Y - 1, (ushort)ModContent.TileType<UpperCatacombDresser>());
                                        WorldGen.PlaceObject(X, Y - 3, (ushort)ModContent.TileType<UpperCatacombCandle>());
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceChest(X + 3, Y - 1, (ushort)ModContent.TileType<UpperCatacombDresser>());
                                        WorldGen.PlaceObject(X + 5, Y - 1, (ushort)ModContent.TileType<UpperCatacombChair>());
                                    }
                                    break;
                                }
                                //piano and tables with stuff on them
                                case 3:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<UpperCatacombPiano>());

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<UpperCatacombTable>());
                                        for (int tableLength = X + 2; tableLength <= X + 4; tableLength++)
                                        {
                                            if (WorldGen.genRand.NextBool(6))
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.ClayPot);
                                            }
                                            else
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.Books, true, WorldGen.genRand.Next(5));
                                            }
                                        }
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<UpperCatacombTable>());
                                        for (int tableLength = X - 4; tableLength <= X - 2; tableLength++)
                                        {
                                            if (WorldGen.genRand.NextBool(6))
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.ClayPot);
                                            }
                                            else
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.Books, true, WorldGen.genRand.Next(5));
                                            }
                                        }
                                    }
                                    break;
                                }
                                //clock and tables
                                case 4:
                                {
                                    WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<UpperCatacombTable>());
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<UpperCatacombClock>());
                                    WorldGen.PlaceObject(X + 4, Y - 1, ModContent.TileType<UpperCatacombTable>());
                                    break;
                                }
                                //bathtub and sink
                                case 5:
                                {
                                    WorldGen.PlaceObject(X + 1, Y - 1, ModContent.TileType<UpperCatacombBathtub>());
                                    WorldGen.PlaceObject(X - 1, Y - 1, ModContent.TileType<UpperCatacombSink>());
                                    break;
                                }
                                //sofa and lamps
                                case 6:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<UpperCatacombSofa>());

                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<UpperCatacombLamp>());
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<UpperCatacombLamp>());
                                    }
                                    break;
                                }
                            }
                        }

                        if (tile.TileType != ModContent.TileType<SpookyWood>())
                        {
                            //lamps
                            if (WorldGen.genRand.NextBool(120))
                            {
                                WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<UpperCatacombLamp>());
                            }
                            //candelabras
                            if (WorldGen.genRand.NextBool(140))
                            {
                                WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<UpperCatacombCandelabra>());
                            }
                            //lanterns
                            if (WorldGen.genRand.NextBool(130))
                            {
                                WorldGen.PlaceObject(X, Y + 1, ModContent.TileType<UpperCatacombLantern>());
                            }
                            //chandeliers
                            if (WorldGen.genRand.NextBool(160))
                            {
                                WorldGen.PlaceObject(X, Y + 1, ModContent.TileType<UpperCatacombChandelier>());
                            }
                        }

                        //place skeletoid wall catacombs
                        if (WorldGen.genRand.NextBool(150) && !tile.HasTile)
                        {
                            WorldGen.PlaceObject(X, Y, ModContent.TileType<SkeletoidCatacomb1>(), true, WorldGen.genRand.Next(8));
                        }
                        //place loot chests
                        if (WorldGen.genRand.NextBool(45) && CanPlaceChest(X, Y))
                        {
                            WorldGen.PlaceChest(X, Y - 1, (ushort)ModContent.TileType<UpperCatacombChest>());
                        }

                        //place rows of caskets, tombs, and decorative flower pots
                        if (WorldGen.genRand.NextBool() && CanPlaceFurniture(X, Y, 9))
                        {
                            switch (WorldGen.genRand.Next(5))
                            {
                                //row of caskets
                                case 0:
                                {
                                    List<int> TombDecorations = new() { ModContent.TileType<Casket1>(), ModContent.TileType<Casket2>(), ModContent.TileType<Casket3>(), ModContent.TileType<Casket4>() };
                                    WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(TombDecorations));

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, WorldGen.genRand.Next(TombDecorations));
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, WorldGen.genRand.Next(TombDecorations));
                                    }

                                    break;
                                }

                                //row of tombstones
                                case 1:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<Tombstone>(), true, WorldGen.genRand.Next(0, 3));

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<Tombstone>(), true, WorldGen.genRand.Next(0, 3));
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<Tombstone>(), true, WorldGen.genRand.Next(0, 3));
                                    }

                                    break;
                                }

                                //row of giant coffins
                                case 2:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<GiantCoffin>());

                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<GiantCoffin>());
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<GiantCoffin>());
                                    }

                                    break;
                                }

                                //row of large burial caskets
                                case 3:
                                {
                                    List<int> TombDecorations = new() { ModContent.TileType<BurialCasket1>(), ModContent.TileType<BurialCasket2>() };

                                    WorldGen.PlaceObject(X - 2, Y - 1, WorldGen.genRand.Next(TombDecorations));
                                    WorldGen.PlaceObject(X + 2, Y - 1, WorldGen.genRand.Next(TombDecorations));

                                    break;
                                }

                                //row of flower pots
                                case 4:
                                {
                                    List<int> PottedPlants = new() { ModContent.TileType<PottedPlant1>(), ModContent.TileType<PottedPlant2>(), 
                                    ModContent.TileType<PottedPlant3>(), ModContent.TileType<PottedPlant4>() };

                                    WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(PottedPlants));

                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, WorldGen.genRand.Next(PottedPlants));
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, WorldGen.genRand.Next(PottedPlants));
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                //ambient tiles for layer 1
				for (int Y = (int)Main.worldSurface - 10; Y <= DaffodilArenaY - 20; Y++)
				{
                    Tile tile = Main.tile[X, Y];
					Tile tileAbove = Main.tile[X, Y - 1];
					Tile tileBelow = Main.tile[X, Y + 1];

					//catacomb vines and weeds
					if (tile.TileType == ModContent.TileType<CatacombBrick1Grass>() || tile.TileType == ModContent.TileType<CatacombBrick1GrassArena>())
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
						int[] ValidTiles = { ModContent.TileType<CatacombBrick1Grass>(), ModContent.TileType<CatacombBrick1GrassSafe>(),
						ModContent.TileType<CatacombBrick2Grass>(), ModContent.TileType<CatacombBrick2GrassSafe>(),
						ModContent.TileType<CatacombBrick1GrassArena>(), ModContent.TileType<CatacombBrick2GrassArena>() };

						SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<CatacombVines>(), ValidTiles);
					}

					if (tile.TileType == ModContent.TileType<CatacombBrick1Grass>() || tile.TileType == ModContent.TileType<CatacombBrick1GrassArena>() || tile.TileType == ModContent.TileType<CatacombFlooring>())
					{
						if (WorldGen.genRand.NextBool(50))
						{
							WorldGen.PlaceChest(X, Y - 1, 21, false, 5);
						}
					}
				}

                //furniture tiles for layer 2
                for (int Y = DaffodilArenaY + 50; Y <= BigBoneArenaY - 30; Y++)
                {
                    Tile tile = Main.tile[X, Y];
					Tile tileAbove = Main.tile[X, Y - 1];
					Tile tileBelow = Main.tile[X, Y + 1];

                    if (tile.WallType == ModContent.WallType<CatacombBrickWall2>())
                    {
                        //place grass walls
                        if (WorldGen.genRand.NextBool(250) && !tile.HasTile && Y < BigBoneArenaY - 50)
                        {
                            int[] ValidTiles = { ModContent.TileType<CatacombBrick2>(), ModContent.TileType<GildedBrick>() };

                            SpookyWorldMethods.PlaceOval(X, Y, ModContent.TileType<CatacombBrick2Grass>(), ModContent.WallType<CatacombGrassWall2>(),
                            WorldGen.genRand.Next(7, 13), WorldGen.genRand.Next(7, 13), 1f, true, false, true, ValidTiles);
                        }

                        //tables and chairs with candles/candelabras on them
                        if (WorldGen.genRand.NextBool() && CanPlaceFurniture(X, Y, 10))
                        {
                            switch (WorldGen.genRand.Next(7))
                            {
                                //two tables with chairs and sometimes candles/candelabras on the tables
                                case 0:
                                {
                                    WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<LowerCatacombTable>());
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        int Type = WorldGen.genRand.NextBool() ? ModContent.TileType<LowerCatacombCandelabra>() : ModContent.TileType<LowerCatacombCandle>();
                                        WorldGen.PlaceObject(X - 3, Y - 3, Type);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X - 5, Y - 1, ModContent.TileType<LowerCatacombChair>(), direction: 1);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X - 1, Y - 1, ModContent.TileType<LowerCatacombChair>(), direction: -1);
                                    }

                                    WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<LowerCatacombTable>());
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        int Type = WorldGen.genRand.NextBool() ? ModContent.TileType<LowerCatacombCandelabra>() : ModContent.TileType<LowerCatacombCandle>();
                                        WorldGen.PlaceObject(X + 3, Y - 3, Type);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X + 5, Y - 1, ModContent.TileType<LowerCatacombChair>(), direction: -1);
                                    }
                                    if (WorldGen.genRand.NextBool()) 
                                    {
                                        WorldGen.PlaceObject(X + 1, Y - 1, ModContent.TileType<LowerCatacombChair>(), direction: 1);
                                    }
                                    break;
                                }
                                //bookcases with lamps on either side of them
                                case 1:
                                {
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<LowerCatacombBookcase>());
                                        WorldGen.PlaceObject(X - 6, Y - 1, ModContent.TileType<LowerCatacombLamp>());
                                    }
                                    else
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<LowerCatacombLamp>());
                                    }
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<LowerCatacombBookcase>());
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<LowerCatacombBookcase>());
                                        WorldGen.PlaceObject(X + 6, Y - 1, ModContent.TileType<LowerCatacombLamp>());
                                    }
                                    else
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<LowerCatacombLamp>());
                                    }
                                    break;
                                }
                                //bed with some dressers and candle
                                case 2:
                                {
                                    WorldGen.PlaceObject(X - 5, Y - 1, (ushort)ModContent.TileType<LowerCatacombBed>(), direction: 1);

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceChest(X, Y - 1, (ushort)ModContent.TileType<LowerCatacombDresser>());
                                        WorldGen.PlaceObject(X, Y - 3, (ushort)ModContent.TileType<LowerCatacombCandle>());
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceChest(X + 3, Y - 1, (ushort)ModContent.TileType<LowerCatacombDresser>());
                                        WorldGen.PlaceObject(X + 5, Y - 1, (ushort)ModContent.TileType<LowerCatacombChair>());
                                    }
                                    break;
                                }
                                //piano and tables with stuff on them
                                case 3:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<LowerCatacombPiano>());

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<LowerCatacombTable>());
                                        for (int tableLength = X + 2; tableLength <= X + 4; tableLength++)
                                        {
                                            if (WorldGen.genRand.NextBool(6))
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.ClayPot);
                                            }
                                            else
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.Books, true, WorldGen.genRand.Next(5));
                                            }
                                        }
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<LowerCatacombTable>());
                                        for (int tableLength = X - 4; tableLength <= X - 2; tableLength++)
                                        {
                                            if (WorldGen.genRand.NextBool(6))
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.ClayPot);
                                            }
                                            else
                                            {
                                                WorldGen.PlaceObject(tableLength, Y - 3, TileID.Books, true, WorldGen.genRand.Next(5));
                                            }
                                        }
                                    }
                                    break;
                                }
                                //clock and tables
                                case 4:
                                {
                                    WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<LowerCatacombTable>());
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<LowerCatacombClock>());
                                    WorldGen.PlaceObject(X + 4, Y - 1, ModContent.TileType<LowerCatacombTable>());
                                    break;
                                }
                                //bathtub and sink
                                case 5:
                                {
                                    WorldGen.PlaceObject(X + 1, Y - 1, ModContent.TileType<LowerCatacombBathtub>());
                                    WorldGen.PlaceObject(X - 1, Y - 1, ModContent.TileType<LowerCatacombSink>());
                                    break;
                                }
                                //sofa and lamps
                                case 6:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<LowerCatacombBench>());

                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<LowerCatacombLamp>());
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<LowerCatacombLamp>());
                                    }
                                    break;
                                }
                            }
                        }

                        if (tile.TileType != ModContent.TileType<SpookyWood>())
                        {
                            //lamps
                            if (WorldGen.genRand.NextBool(120))
                            {
                                WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<LowerCatacombLamp>());
                            }
                            //candelabras
                            if (WorldGen.genRand.NextBool(140))
                            {
                                WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<LowerCatacombCandelabra>());
                            }
                            //lanterns
                            if (WorldGen.genRand.NextBool(130))
                            {
                                WorldGen.PlaceObject(X, Y + 1, ModContent.TileType<LowerCatacombLantern>());
                            }
                            //chandeliers
                            if (WorldGen.genRand.NextBool(160))
                            {
                                WorldGen.PlaceObject(X, Y + 1, ModContent.TileType<LowerCatacombChandelier>());
                            }
                        }

                        //place skeletoid wall catacombs
                        if (WorldGen.genRand.NextBool(150) && !tile.HasTile)
                        {
                            WorldGen.PlaceObject(X, Y, ModContent.TileType<SkeletoidCatacomb2>(), true, WorldGen.genRand.Next(8));
                        }
                        //place loot chests
                        if (WorldGen.genRand.NextBool(45) && CanPlaceChest(X, Y))
                        {
                            WorldGen.PlaceChest(X, Y - 1, (ushort)ModContent.TileType<LowerCatacombChest>());
                        }

                        //place rows of caskets, tombs, and decorative flower pots
                        if (WorldGen.genRand.NextBool() && CanPlaceFurniture(X, Y, 9))
                        {
                            switch (WorldGen.genRand.Next(5))
                            {
                                //row of caskets
                                case 0:
                                {
                                    List<int> TombDecorations = new() { ModContent.TileType<Casket1>(), ModContent.TileType<Casket2>(), ModContent.TileType<Casket3>(), ModContent.TileType<Casket4>() };
                                    WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(TombDecorations));

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, WorldGen.genRand.Next(TombDecorations));
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, WorldGen.genRand.Next(TombDecorations));
                                    }

                                    break;
                                }

                                //row of tombstones
                                case 1:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<Tombstone>(), true, WorldGen.genRand.Next(0, 3));

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<Tombstone>(), true, WorldGen.genRand.Next(0, 3));
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<Tombstone>(), true, WorldGen.genRand.Next(0, 3));
                                    }

                                    break;
                                }

                                //row of giant coffins
                                case 2:
                                {
                                    WorldGen.PlaceObject(X, Y - 1, ModContent.TileType<GiantCoffin>());

                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, ModContent.TileType<GiantCoffin>());
                                    }
                                    if (WorldGen.genRand.NextBool(3))
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, ModContent.TileType<GiantCoffin>());
                                    }

                                    break;
                                }

                                //row of large burial caskets
                                case 3:
                                {
                                    List<int> TombDecorations = new() { ModContent.TileType<BurialCasket1>(), ModContent.TileType<BurialCasket2>() };

                                    WorldGen.PlaceObject(X - 2, Y - 1, WorldGen.genRand.Next(TombDecorations));
                                    WorldGen.PlaceObject(X + 2, Y - 1, WorldGen.genRand.Next(TombDecorations));

                                    break;
                                }

                                //row of flower pots
                                case 4:
                                {
                                    List<int> PottedPlants = new() { ModContent.TileType<PottedPlant1>(), ModContent.TileType<PottedPlant2>(), 
                                    ModContent.TileType<PottedPlant3>(), ModContent.TileType<PottedPlant4>() };

                                    WorldGen.PlaceObject(X, Y - 1, WorldGen.genRand.Next(PottedPlants));

                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X - 3, Y - 1, WorldGen.genRand.Next(PottedPlants));
                                    }
                                    if (WorldGen.genRand.NextBool())
                                    {
                                        WorldGen.PlaceObject(X + 3, Y - 1, WorldGen.genRand.Next(PottedPlants));
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                //ambient tiles for layer 2
                for (int Y = DaffodilArenaY + 50; Y <= BigBoneArenaY - 30; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];
 
                    //catacomb vines and weeds
                    if (tile.TileType == ModContent.TileType<CatacombBrick2Grass>() || tile.TileType == ModContent.TileType<CatacombBrick2GrassArena>())
                    {
                        if (WorldGen.genRand.NextBool(5) && !tileAbove.HasTile)
                        {
                            GrowGiantFlower(X, Y, ModContent.TileType<BigFlower>());
                        }

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
                        int[] ValidTiles = { ModContent.TileType<CatacombBrick1Grass>(), ModContent.TileType<CatacombBrick1GrassSafe>(),
			            ModContent.TileType<CatacombBrick2Grass>(), ModContent.TileType<CatacombBrick2GrassSafe>(), 
                        ModContent.TileType<CatacombBrick1GrassArena>(), ModContent.TileType<CatacombBrick2GrassArena>() };

                        SpookyWorldMethods.PlaceVines(X, Y, ModContent.TileType<CatacombVines>(), ValidTiles);
                    }

                    //biome chests need to be locked
                    if (tile.TileType == ModContent.TileType<CemeteryBiomeChest>() || tile.TileType == ModContent.TileType<SpiderCaveChest>() ||
                    tile.TileType == ModContent.TileType<SpookyBiomeChest>() || tile.TileType == ModContent.TileType<SpookyHellChest>())
                    {
                        tile.TileFrameX += 36;
                    }
				}
            }
        }

        //if a catacomb platform can be placed off of the side of a block
        public bool CanPlacePlatform(int PositionX, int PositionY, int Length, bool LeftSide)
		{
			if (LeftSide)
			{
				for (int x = PositionX - Length; x <= PositionX - 1; x++)
				{
					for (int y = PositionY - 2; y <= PositionY + 1; y++)
					{
						if (Main.tile[x, y].HasTile || (Main.tile[x, y].WallType != ModContent.WallType<CatacombBrickWall1>() && Main.tile[x, y].WallType != ModContent.WallType<CatacombBrickWall2>()))
						{
							return false;
						}
					}
				}
			}
			else
			{
				for (int x = PositionX + 1; x <= PositionX + Length; x++)
				{
					for (int y = PositionY - 1; y <= PositionY + 1; y++)
					{
						if (Main.tile[x, y].HasTile || (Main.tile[x, y].WallType != ModContent.WallType<CatacombBrickWall1>() && Main.tile[x, y].WallType != ModContent.WallType<CatacombBrickWall2>()))
						{
							return false;
						}
					}
				}
			}

			return true;
		}

        //if a painting can place
        public bool CanPlacePainting(int PositionX, int PositionY, List<ushort> PaintingsToCheckFor, bool DoCheckForNearbyPaintings)
		{
			//first check for enough walls
			for (int i = PositionX - 3; i <= PositionX + 3; i++)
			{
				for (int j = PositionY - 3; j <= PositionY + 3; j++)
				{
					if (Main.tile[i, j].WallType != ModContent.WallType<CatacombBrickWall1>() && Main.tile[i, j].WallType != ModContent.WallType<CatacombBrickWall2>())
					{
						return false;
					}
				}
			}

            if (DoCheckForNearbyPaintings)
            {
                for (int i = PositionX - 6; i <= PositionX + 6; i++)
                {
                    for (int j = PositionY - 6; j <= PositionY + 6; j++)
                    {	
                        if (PaintingsToCheckFor.Contains(Main.tile[i, j].TileType))
                        {
                            return false;
                        }
                    }
                }
            }

			return true;
		}

        //determine if theres no chests nearby another chest thats about to place
        public static bool CanPlaceChest(int X, int Y)
        {
            for (int i = X - 70; i < X + 70; i++)
            {
                for (int j = Y - 40; j < Y + 40; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == ModContent.TileType<UpperCatacombChest>() || Main.tile[i, j].TileType == ModContent.TileType<LowerCatacombChest>()))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //check for a flat surface in the dungeon that also has no tiles above the entire flat space
		//use to check for a specific width to place individual pieces of furniture, or in other cases multiple pieces of furniture (such as tables with chairs next to them)
		public bool CanPlaceFurniture(int PositionX, int PositionY, int Width, bool Ceiling = false, bool CheckWood = false)
		{
			if (!Ceiling)
			{
				for (int x = PositionX - (Width / 2); x <= PositionX + (Width / 2); x++)
				{
                    if (!CheckWood)
                    {
                        //check specifically for both catacomb flooring blocks
                        if ((Main.tile[x, PositionY].TileType == ModContent.TileType<CatacombBrick1>() || Main.tile[x, PositionY].TileType == ModContent.TileType<CatacombBrick2>() ||
                        Main.tile[x, PositionY].TileType == ModContent.TileType<CatacombFlooring>() || Main.tile[x, PositionY].TileType == ModContent.TileType<GildedBrick>()) && 
                        !Main.tile[x, PositionY - 1].HasTile)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        //check specifically for both catacomb flooring blocks
                        if ((Main.tile[x, PositionY].TileType == ModContent.TileType<SpookyWood>() || Main.tile[x, PositionY].TileType == ModContent.TileType<OldWoodPlatform>()) && !Main.tile[x, PositionY - 1].HasTile)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
				}
			}

			return true;
		}

        //determine if theres no jungle blocks nearby so the biome doesnt place in the jungle biome
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
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Stalac"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Creepy Catacombs", PlaceCatacomb));
		}

		//post worldgen to place items in the spooky biome chests
        public override void PostWorldGen()
		{
            //upper catacomb main items
			List<int> MainItem1 = new List<int>
			{
				ModContent.ItemType<BoneBow>(), ModContent.ItemType<GraveCrossbow>(), ModContent.ItemType<HarvesterScythe>(), 
                ModContent.ItemType<HighVelocitySlingshot>(), ModContent.ItemType<HunterScarf>(), 
                ModContent.ItemType<NineTails>(), ModContent.ItemType<ThornStaff>()
			};

			List<int> ActualMainItem1 = new List<int>(MainItem1);

            //lower catacomb main items
            List<int> MainItem2 = new List<int>
			{
				ModContent.ItemType<CrossCharm>(), ModContent.ItemType<FlameIdol>(), ModContent.ItemType<GlowBulb>(), 
				ModContent.ItemType<GodGun>(), ModContent.ItemType<HunterSoulScepter>(), 
                ModContent.ItemType<LegalShotgun>(), ModContent.ItemType<OldRifle>()
			};

			List<int> ActualMainItem2 = new List<int>(MainItem2);

			for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++) 
            {
				Chest chest = Main.chest[chestIndex];

				if (chest == null) 
                {
					continue;
				}

				if (WorldGen.InWorld(chest.x, chest.y))
				{
					Tile chestTile = Main.tile[chest.x, chest.y];

                    //layer 1 chests
					if (chestTile.TileType == ModContent.TileType<UpperCatacombChest>())
					{
                        List<int> Potions = new List<int>
                        {
                            ItemID.IronskinPotion, ItemID.SwiftnessPotion, ItemID.RegenerationPotion, ItemID.ManaRegenerationPotion,
                            ItemID.ShinePotion, ItemID.ThornsPotion, ItemID.GravitationPotion, ItemID.PotionOfReturn
                        };

                        if (ActualMainItem1.Count == 0)
						{
							ActualMainItem1 = new List<int>(MainItem1);
						}

						int ItemToPutInChest = WorldGen.genRand.Next(ActualMainItem1.Count);

                        //demonite or crimtane bar depending on corruption or crimson worlds
						int Bars = !WorldGen.crimson ? ItemID.DemoniteBar : ItemID.CrimtaneBar;
                        
						//main items
						chest.item[0].SetDefaults(ActualMainItem1[ItemToPutInChest]);
						chest.item[0].stack = 1;
						ActualMainItem1.RemoveAt(ItemToPutInChest);
                        //chance to place eye valley compass instead of bars
                        if (WorldGen.genRand.NextBool(3))
                        {
                            chest.item[1].SetDefaults(ModContent.ItemType<EyeValleyCompass>());
						    chest.item[1].stack = 1;
                        }
						//bars
						else
						{
							chest.item[1].SetDefaults(Bars);
							chest.item[1].stack = WorldGen.genRand.Next(3, 7);
						}
						chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions));
						chest.item[2].stack = WorldGen.genRand.Next(1, 4);
						//torches
						chest.item[3].SetDefaults(ModContent.ItemType<CatacombTorch1Item>());
						chest.item[3].stack = WorldGen.genRand.Next(12, 19);
						//gold coins
						chest.item[4].SetDefaults(ItemID.GoldCoin);
						chest.item[4].stack = WorldGen.genRand.Next(1, 3);
                    }

					//layer 1 barrels
					if (chestTile.TileType == TileID.Containers && (chestTile.WallType == ModContent.WallType<CatacombBrickWall1>() || chestTile.WallType == ModContent.WallType<CatacombGrassWall1>()))
					{
						//place stuff in barrels
						if (chestTile.TileFrameX == 5 * 36)
						{
							int[] RareItem = new int[] { ModContent.ItemType<SkullAmulet>(), ModContent.ItemType<RustyRing>() };
							int[] Ammo = new int[] { ModContent.ItemType<RustedBullet>(), ModContent.ItemType<OldWoodArrow>(), ModContent.ItemType<MossyPebble>() };

							if (WorldGen.genRand.NextBool(15))
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
						}
                    }

                    //layer 2 chests
                    if (chestTile.TileType == ModContent.TileType<LowerCatacombChest>())
					{
                        List<int> Potions = new List<int>
                        {
                            ItemID.LuckPotion, ItemID.InfernoPotion, ItemID.LifeforcePotion, ItemID.RagePotion, 
                            ItemID.SummoningPotion, ItemID.TitanPotion, ItemID.WrathPotion, ItemID.PotionOfReturn
                        };

                        List<int> Bars = new List<int>
                        {
                            ItemID.MythrilBar, ItemID.OrichalcumBar, ItemID.AdamantiteBar, ItemID.TitaniumBar 
                        };
                        
                        if (ActualMainItem2.Count == 0)
						{
							ActualMainItem2 = new List<int>(MainItem2);
						}

						int ItemToPutInChest = WorldGen.genRand.Next(ActualMainItem2.Count);
                        
						//main items
						chest.item[0].SetDefaults(ActualMainItem2[ItemToPutInChest]);
						chest.item[0].stack = 1;
						ActualMainItem2.RemoveAt(ItemToPutInChest);
                        //bars
                        chest.item[1].SetDefaults(Main.rand.Next(Bars));
						chest.item[1].stack = WorldGen.genRand.Next(8, 16);
                        //potions
						chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions));
						chest.item[2].stack = WorldGen.genRand.Next(1, 4);
						//torches
						chest.item[3].SetDefaults(ModContent.ItemType<CatacombTorch2Item>());
						chest.item[3].stack = WorldGen.genRand.Next(12, 19);
						//gold coins
						chest.item[4].SetDefaults(ItemID.GoldCoin);
						chest.item[4].stack = WorldGen.genRand.Next(2, 4);
                    }

                    //spooky mod biome chest loot
					if (chest != null && (chestTile.TileType == ModContent.TileType<SpookyBiomeChest>() || chestTile.TileType == ModContent.TileType<SpookyHellChest>() ||
					chestTile.TileType == ModContent.TileType<CemeteryBiomeChest>() || chestTile.TileType == ModContent.TileType<SpiderCaveChest>()))
					{
						//potions
						int[] Potions1 = new int[] { ItemID.AmmoReservationPotion, ItemID.BattlePotion, ItemID.CratePotion, ItemID.EndurancePotion };

						//more potions
						int[] Potions2 = new int[] { ItemID.LuckPotion, ItemID.InfernoPotion, ItemID.ShinePotion, ItemID.LifeforcePotion };

						//cemetery biome chest main item
						if (chestTile.TileType == ModContent.TileType<CemeteryBiomeChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<DiscoSkull>());
							chest.item[0].stack = 1;
						}

						//spider cave biome chest main item
						if (chestTile.TileType == ModContent.TileType<SpiderCaveChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<VenomHarpoon>());
							chest.item[0].stack = 1;
						}

						//spooky forest biome chest main item
						if (chestTile.TileType == ModContent.TileType<SpookyBiomeChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<ElGourdo>());
							chest.item[0].stack = 1;
						}

						//eye valley biome chest main item
						if (chestTile.TileType == ModContent.TileType<SpookyHellChest>())
						{
							chest.item[0].SetDefaults(ModContent.ItemType<BrainJar>());
							chest.item[0].stack = 1;
						}

						//potions
						chest.item[1].SetDefaults(WorldGen.genRand.Next(Potions1));
						chest.item[1].stack = WorldGen.genRand.Next(1, 3);
						//even more potions
						chest.item[2].SetDefaults(WorldGen.genRand.Next(Potions2));
						chest.item[2].stack = WorldGen.genRand.Next(1, 3);
						//recovery potions
						chest.item[3].SetDefaults(ItemID.GreaterHealingPotion);
						chest.item[3].stack = WorldGen.genRand.Next(5, 11);
						//gold coins
						chest.item[4].SetDefaults(ItemID.GoldCoin);
						chest.item[4].stack = WorldGen.genRand.Next(5, 16);
					}
                }
            }
        }
    }
}