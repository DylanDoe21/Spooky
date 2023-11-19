using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Tiles.Cemetery;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class Cemetery : ModSystem
    {
        public static int BiomeWidth = Main.maxTilesX >= 8400 ? 500 : (Main.maxTilesX >= 6400 ? 420 : 250);

        //place a giant dirt area for the graveyard to generate on
        private void PlaceCemetery(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Cemetery").Value;

            BiomeWidth = Main.maxTilesX >= 8400 ? 500 : (Main.maxTilesX >= 6400 ? 420 : 250);

            Catacombs.PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            float worldEdgeOffset = Main.maxTilesX >= 6400 ? 8.65f : 8.55f;

            //place biome based on the opposite side of the dungeon
            if (GenVars.dungeonSide == -1)
			{
                Catacombs.PositionX = Main.maxTilesX - (Main.maxTilesX / (int)worldEdgeOffset) - 100;
			}
			else
			{
                Catacombs.PositionX = (Main.maxTilesX / (int)worldEdgeOffset) - 160;
            }

            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            bool foundSurface = false;
            int attempts = 0;

            //place biome exactly on the surface by finding a valid surface
            while (!foundSurface && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(XMiddle, Catacombs.PositionY) && Catacombs.PositionY <= Main.worldSurface)
				{
					Catacombs.PositionY++;
				}
                if (!Main.tile[XMiddle, Catacombs.PositionY].HasTile)
                {
					continue;
				}

                foundSurface = true;
            }

            //place the terrain itself and replace blocks with cemetery blocks
            for (int X = XMiddle - (BiomeWidth / 2); X <= XMiddle + (BiomeWidth / 2); X++)
            {
                for (int Y = Catacombs.PositionY - 100; Y <= Main.worldSurface; Y++)
                {
                    Tile tile = Main.tile[X, Y];

                    //place cemetery dirt blocks on crimstone and ebonstone walls because they are annoying
                    if (!tile.HasTile && (tile.WallType == WallID.EbonstoneUnsafe || tile.WallType == WallID.CrimstoneUnsafe))
                    {
                        WorldGen.PlaceTile(X, Y, (ushort)ModContent.TileType<CemeteryDirt>());
                    }

                    //convert all tiles into cemetery dirt
                    if (tile.HasTile && tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud && tile.TileType != ModContent.TileType<CemeteryDirt>())
                    {
                        tile.TileType = (ushort)ModContent.TileType<CemeteryDirt>();
                    }

                    //reaplce walls with cemetery grass walls
                    if (tile.WallType > 0)
                    {
                        tile.WallType = (ushort)ModContent.WallType<CemeteryGrassWall>();
                    }
                }

                //place block clusters right above the world surface to prevent the cemetery from generating too low
                for (int FillY = (int)Main.worldSurface - 50; FillY <= Main.worldSurface; FillY += 2)
                {
                    SpookyWorldMethods.PlaceCircle(X, FillY, ModContent.TileType<CemeteryDirt>(), WorldGen.genRand.Next(2, 3), true, true);
                }
            }

            //place more blocks in the middle of the cemetery to prevent the entrance from placing too low
            for (int X = XMiddle - 30; X <= XMiddle + 30; X += 2)
            {
                for (int Y = (int)Main.worldSurface - 65; Y <= Main.worldSurface; Y += 2)
                {
                    SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CemeteryDirt>(), WorldGen.genRand.Next(2, 3), true, true);
                }
            }

            //add tile dithering on the edges of the biome
            for (int X = XMiddle - (BiomeWidth / 2) - 20; X <= XMiddle + (BiomeWidth / 2) + 20; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    if (WorldGen.genRand.NextBool(2))
                    {
                        Tile tile = Main.tile[X, Y];

                        //place dirt blocks
                        if (tile.HasTile && tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud && tile.TileType != ModContent.TileType<CemeteryDirt>())
                        {
                            tile.TileType = (ushort)ModContent.TileType<CemeteryDirt>();
                        }

                        //reaplce walls with cemetery grass walls
                        if (tile.WallType > 0)
                        {
                            tile.WallType = (ushort)ModContent.WallType<CemeteryGrassWall>();
                        }
                    }
                }
            }

            //place clumps of stone in the biome
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY * 27) * 1E-04); i++)
            {
                int X = WorldGen.genRand.Next(0, Main.maxTilesX);
                int Y = WorldGen.genRand.Next(0, Main.maxTilesY - 2);

                if (Main.tile[X, Y] != null && Main.tile[X, Y].HasTile)
                {
                    if (Main.tile[X, Y].TileType == ModContent.TileType<CemeteryDirt>())
                    {
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(10, 18), WorldGen.genRand.Next(10, 18), 
                        ModContent.TileType<CemeteryStone>(), false, 0f, 0f, false, true);
                    }
                }
            }
        }

        private void CemeteryGrassAndTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            for (int X = XMiddle - (BiomeWidth / 2) - 100; X <= XMiddle + (BiomeWidth / 2) + 100; X++)
            {
                for (int Y = Catacombs.PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    WorldGen.SpreadGrass(X, Y, ModContent.TileType<CemeteryDirt>(), ModContent.TileType<CemeteryGrass>());

                    if (Main.tile[X, Y].TileType == (ushort)ModContent.TileType<CemeteryGrass>())
                    {
                        WorldGen.GrowTree(X, Y - 1);
                    }
                }
            }
        }

        public void GenerateCemeteryStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = Catacombs.PositionX;
            int XMiddle = XStart + (BiomeWidth / 2);
            int XEdge = XStart + BiomeWidth;

            int StartPosY = Catacombs.PositionY - 100;

            //structures
            if (Main.maxTilesX >= 6400)
            {
                GenerateStructure((XStart + XMiddle) / 2 - 95, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
                GenerateStructure((XStart + XMiddle) / 2 - 72, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
                GenerateStructure((XStart + XMiddle) / 2 - 35, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);

                //first ruined house
                GenerateStructure((XStart + XMiddle) / 2, StartPosY, "RuinedHouse-1", 14, 20);

                //lake
                GenerateStructure((XStart + XMiddle) / 2 + 35, StartPosY, "FishingLake", 15, 5);

                //catacomb entrance
                GenerateStructure(XMiddle, StartPosY, "CemeteryEntrance", 38, 28);

                //second ruined house
                GenerateStructure((XMiddle + XEdge) / 2, StartPosY, "RuinedHouse-2", 14, 20);

                //graveyards
                GenerateStructure((XMiddle + XEdge) / 2 - 35, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 14, 8);
                GenerateStructure((XMiddle + XEdge) / 2 + 35, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
                GenerateStructure((XMiddle + XEdge) / 2 + 72, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
                GenerateStructure((XMiddle + XEdge) / 2 + 95, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
            }
            else
            {
                //first ruined house
                GenerateStructure((XStart + XMiddle) / 2 - 40, StartPosY, "RuinedHouse-1", 14, 20);

                //lake
                GenerateStructure((XStart + XMiddle) / 2, StartPosY, "FishingLake", 15, 11);

                //catacomb entrance
                GenerateStructure(XMiddle, StartPosY, "CemeteryEntrance", 38, 28);

                //second ruined house
                GenerateStructure((XMiddle + XEdge) / 2 + 40, StartPosY, "RuinedHouse-2", 14, 20);
            }
        }

        //method for finding a valid surface and placing the structure on it
        public void GenerateStructure(int startX, int startY, string StructureFile, int offsetX, int offsetY)
        {
            bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(startX, startY) && startY <= Main.worldSurface)
				{
					startY++;
				}
                if (Main.tile[startX, startY].HasTile && NoFloatingIsland(startX, startY))
                {
                    Vector2 origin = new Vector2(startX - offsetX, startY - offsetY);
                    Generator.GenerateStructure("Content/Structures/Cemetery/" + StructureFile, origin.ToPoint16(), Mod);

                    //when the cemetery catacomb crypt is placed, save the position for the catacomb entrance
                    if (StructureFile == "CemeteryEntrance")
                    {
                        Catacombs.EntranceY = startY - 33;
                    }
                    else
                    {
                        //place blocks below structure to prevent them from floating
                        for (int fillX = (int)origin.X + 5; fillX <= (int)origin.X + 15; fillX += 2)
                        {
                            for (int fillY = startY + 5; fillY <= (int)Main.worldSurface - 35; fillY += 2)
                            {
                                if (Main.tile[fillX, fillY].WallType < 0 && !Main.tile[fillX, fillY].HasTile)
                                {
                                    SpookyWorldMethods.PlaceCircle(fillX, fillY, WorldGen.genRand.NextBool(5) ? ModContent.TileType<CemeteryStone>() : ModContent.TileType<CemeteryDirt>(), WorldGen.genRand.Next(2, 3), true, true);
                                }
                            }
                        }
                    }
                }

                placed = true;
            }
        }

        //check the area around the given position for cloud blocks, to prevent structures from placing on floating islands
        public static bool NoFloatingIsland(int X, int Y)
        {
            for (int i = X - 20; i < X + 20; i++)
            {
                for (int j = Y - 20; j < Y + 20; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == TileID.Sunplate))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Dirt Rock Wall Runner"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("CemeteryTerrain", PlaceCemetery));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("CemeteryStructures", GenerateCemeteryStructures));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("CemeteryTrees", CemeteryGrassAndTrees));
        }

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

                if (chestTile.TileFrameX == 15 * 36 && (chest.item[0].type == ItemID.BladedGlove || chest.item[0].type == ItemID.BloodyMachete))
                {
                    int[] Bars = new int[] { ItemID.SilverBar, ItemID.TungstenBar };
                    int[] Potions = new int[] { ItemID.LesserHealingPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };
                    int[] Misc = new int[] { ItemID.PumpkinSeed, ItemID.Cobweb };

                    //iron or lead bars
                    chest.item[1].SetDefaults(WorldGen.genRand.Next(Bars));
                    chest.item[1].stack = WorldGen.genRand.Next(5, 10);
                    //light sources
                    chest.item[2].SetDefaults(ItemID.GreenTorch);
                    chest.item[2].stack = WorldGen.genRand.Next(3, 8);
                    //potions
                    chest.item[3].SetDefaults(WorldGen.genRand.Next(Potions));
                    chest.item[3].stack = WorldGen.genRand.Next(2, 3);
                    //pumpkin seeds or cobwebs
                    chest.item[4].SetDefaults(WorldGen.genRand.Next(Misc));
                    chest.item[4].stack = WorldGen.genRand.Next(5, 10);
                    //coins
                    chest.item[5].SetDefaults(ItemID.GoldCoin);
                    chest.item[5].stack = WorldGen.genRand.Next(1, 2);
                }
            }
        }
    }
}