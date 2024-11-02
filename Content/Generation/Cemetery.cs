using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.BossSummon;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.Cemetery.Ambient;
using Spooky.Content.Tiles.Cemetery.Furniture;

using StructureHelper;

namespace Spooky.Content.Generation
{
    public class Cemetery : ModSystem
    {
        public int PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

        public static int BiomeWidth = Main.maxTilesX >= 8400 ? 500 : (Main.maxTilesX >= 6400 ? 420 : 250);

        static int initialStartPosX;

        //place a giant dirt area for the graveyard to generate on
        private void PlaceCemetery(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.Cemetery").Value;

            BiomeWidth = Main.maxTilesX >= 8400 ? 500 : (Main.maxTilesX >= 6400 ? 420 : 250);

            PositionY = (int)Main.worldSurface - (Main.maxTilesY / 8);

            //place biome based on the opposite side of the dungeon
            if (GenVars.dungeonSide == -1)
			{
                Catacombs.PositionX = Main.maxTilesX - (Main.maxTilesX / 5);
                initialStartPosX = Main.maxTilesX - (Main.maxTilesX / 5);
			}
			else
			{
                Catacombs.PositionX = (Main.maxTilesX / 5);
                initialStartPosX = (Main.maxTilesX / 5);
            }

            //move away from the jungle and desert so the cemetery doesnt destroy it
            bool foundValidPosition = false;
            int XPosAttempts = 0;

            while (!foundValidPosition && XPosAttempts++ < 100000)
            {
                while (!CanPlaceBiome(Catacombs.PositionX, (int)Main.worldSurface))
                {
                    Catacombs.PositionX += (initialStartPosX < (Main.maxTilesX / 2) ? -10 : 10);
                }
                if (CanPlaceBiome(Catacombs.PositionX, (int)Main.worldSurface))
                {
                    foundValidPosition = true;
                }
            }

            int DistanceMax = Main.maxTilesX < 6400 ? 8 : 9;

            //if the catacombs placement position is too close to the edge of the world, cap it to prevent it from generating too close to the oceans
            if (Catacombs.PositionX <= (Main.maxTilesX / DistanceMax))
            {
                Catacombs.PositionX = (Main.maxTilesX / DistanceMax);
            }
            if (Catacombs.PositionX >= Main.maxTilesX - (Main.maxTilesX / DistanceMax))
            {
                Catacombs.PositionX = Main.maxTilesX - (Main.maxTilesX / DistanceMax);
            }

            int XStart = Catacombs.PositionX - (BiomeWidth / 2);
            int XMiddle = Catacombs.PositionX;
            int XEdge = Catacombs.PositionX + (BiomeWidth / 2);

            //place biome exactly on the surface by finding a valid surface
            bool foundSurface = false;
            int attempts = 0;

            while (!foundSurface && attempts++ < 100000)
            {
                while (!WorldGen.SolidTile(XMiddle, PositionY) && PositionY <= Main.maxTilesY)
				{
					PositionY++;
				}
                if ((WorldGen.SolidTile(XMiddle, PositionY) || Main.tile[XMiddle, PositionY].WallType > 0) && NoFloatingIsland(XMiddle, PositionY))
                {
					foundSurface = true;
                }
            }

            //place the terrain itself and replace blocks with cemetery blocks
            for (int X = XMiddle - (BiomeWidth / 2); X <= XMiddle + (BiomeWidth / 2); X++)
            {
                for (int Y = PositionY - 100; Y <= Main.worldSurface; Y++)
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

					tile.LiquidAmount = 0;
                }

                //place block clusters right above the world surface to prevent the cemetery from generating too low
                for (int FillY = (int)Main.worldSurface - 50; FillY <= Main.worldSurface; FillY += 2)
                {
                    SpookyWorldMethods.PlaceCircle(X, FillY, ModContent.TileType<CemeteryDirt>(), 0, WorldGen.genRand.Next(2, 3), true, true);
                }
            }

            //place more blocks in the middle of the cemetery to prevent the entrance from placing too low
            for (int X = XMiddle - 30; X <= XMiddle + 30; X += 2)
            {
                for (int Y = (int)Main.worldSurface - 65; Y <= Main.worldSurface; Y += 2)
                {
                    SpookyWorldMethods.PlaceCircle(X, Y, ModContent.TileType<CemeteryDirt>(), ModContent.WallType<CemeteryGrassWall>(), WorldGen.genRand.Next(2, 3), true, false);
                }
            }

            //add tile dithering on the edges of the biome
            for (int X = XMiddle - (BiomeWidth / 2) - 20; X <= XMiddle + (BiomeWidth / 2) + 20; X++)
            {
                for (int Y = PositionY - 75; Y <= Main.worldSurface; Y++)
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
                        WorldGen.TileRunner(X, Y, WorldGen.genRand.Next(10, 18), WorldGen.genRand.Next(10, 18), ModContent.TileType<CemeteryStone>(), false, 0f, 0f, false, true);
                    }
                }
            }
        }

        private void CemeteryGrassAndTrees(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = Catacombs.PositionX - (BiomeWidth / 2);
            int XMiddle = Catacombs.PositionX;
            int XEdge = Catacombs.PositionX + (BiomeWidth / 2);

            for (int X = XMiddle - (BiomeWidth / 2) - 100; X <= XMiddle + (BiomeWidth / 2) + 100; X++)
            {
                for (int Y = PositionY - 75; Y <= Main.worldSurface; Y++)
                {
                    Tile tile = Main.tile[X, Y];
                    Tile tileAbove = Main.tile[X, Y - 1];
                    Tile tileBelow = Main.tile[X, Y + 1];

                    WorldGen.SpreadGrass(X, Y, ModContent.TileType<CemeteryDirt>(), ModContent.TileType<CemeteryGrass>());

                    if (tile.TileType == (ushort)ModContent.TileType<CemeteryGrass>())
                    {
                        //grow trees
                        WorldGen.GrowTree(X, Y - 1);

                        //grow cemetery weeds
                        if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
                        {
                            WorldGen.PlaceTile(X, Y - 1, (ushort)ModContent.TileType<CemeteryWeeds>());
                            tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                            WorldGen.SquareTileFrame(X, Y + 1, true);
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, X, Y - 1, 1, TileChangeType.None);
                            }
                        }
                    }
                }
            }
        }

        public void GenerateCemeteryStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            int XStart = Catacombs.PositionX - (BiomeWidth / 2);
            int XMiddle = Catacombs.PositionX;
            int XEdge = Catacombs.PositionX + (BiomeWidth / 2);

            int StartPosY = PositionY - 100;

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
                while ((!IsCemeteryTile(startX, startY) || !NoFloatingIsland(startX, startY)) && startY <= Main.worldSurface)
				{
					startY++;
				}
                if (IsCemeteryTile(startX, startY) && NoFloatingIsland(startX, startY))
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
                        if (StructureFile != "FishingLake")
                        {
                            //place blocks below structure to prevent them from floating
                            for (int fillX = startX - 10; fillX <= startX + 10; fillX++)
                            {
                                for (int fillY = startY + 7; fillY <= (int)Main.worldSurface - 35; fillY++)
                                {
                                    if (!Main.tile[fillX, fillY].HasTile)
                                    {
                                        SpookyWorldMethods.PlaceCircle(fillX, fillY, WorldGen.genRand.NextBool(5) ? ModContent.TileType<CemeteryStone>() : ModContent.TileType<CemeteryDirt>(), 0, WorldGen.genRand.Next(1, 3), true, true);
                                    }
                                }
                            }
                        }
                    }

                    placed = true;
                }
            }
        }

        //check the area around the given position for cloud blocks, to prevent structures from placing on floating islands
        public static bool NoFloatingIsland(int X, int Y)
        {
            for (int i = X - 35; i < X + 35; i++)
            {
                for (int j = Y - 35; j < Y + 35; j++)
                {
                    if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == TileID.Sunplate))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        //determine if theres no jungle or desert blocks nearby
        public static bool CanPlaceBiome(int X, int Y)
        {
            int numJungleTiles = 0;

            for (int i = X - 50; i < X + 50; i++)
            {
                for (int j = Y - 50; j < Y + 50; j++)
                {
                    if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.Mud)
                    {
                        numJungleTiles++;
                    }
                }
            }

            int numDesertTiles = 0;

            for (int i = X - 200; i < X + 200; i++)
            {
                for (int j = Y - 50; j < Y + 50; j++)
                {
                    if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.Sand)
                    {
                        numDesertTiles++;
                    }
                }
            }

            if (numJungleTiles > 250)
            {
                return false;
            }
            else if (numDesertTiles > 250)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool IsCemeteryTile(int X, int Y)
        {
            return Main.tile[X, Y].TileType == ModContent.TileType<CemeteryGrass>() || 
            Main.tile[X, Y].TileType == ModContent.TileType<CemeteryDirt>() ||
            Main.tile[X, Y].TileType == ModContent.TileType<CemeteryStone>();
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Dirt Rock Wall Runner"));
			if (GenIndex1 == -1)
			{
				return;
			}

            tasks.Insert(GenIndex1 + 1, new PassLegacy("Cemetery", PlaceCemetery));
            tasks.Insert(GenIndex1 + 2, new PassLegacy("Cemetery Structures", GenerateCemeteryStructures));
            tasks.Insert(GenIndex1 + 3, new PassLegacy("Cemetery Trees", CemeteryGrassAndTrees));
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

				if (WorldGen.InWorld(chest.x, chest.y))
				{
					Tile chestTile = Main.tile[chest.x, chest.y];

					if (chestTile.TileFrameX == 15 * 36 && (chest.item[0].type == ItemID.BladedGlove || chest.item[0].type == ItemID.BloodyMachete))
					{
						int[] Bars = new int[] { ItemID.SilverBar, ItemID.TungstenBar };
						int[] Potions = new int[] { ItemID.LesserHealingPotion, ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion };

                        //broken emf reader
						chest.item[1].SetDefaults(ModContent.ItemType<EMFReaderBroke>());
						chest.item[1].stack = 1;
						//iron or lead bars
						chest.item[2].SetDefaults(WorldGen.genRand.Next(Bars));
						chest.item[2].stack = WorldGen.genRand.Next(8, 15);
						//light sources
						chest.item[3].SetDefaults(ModContent.ItemType<CemeteryBiomeTorchItem>());
						chest.item[3].stack = WorldGen.genRand.Next(3, 8);
						//potions
						chest.item[4].SetDefaults(WorldGen.genRand.Next(Potions));
						chest.item[4].stack = WorldGen.genRand.Next(3, 5);
						//coins
						chest.item[5].SetDefaults(ItemID.GoldCoin);
						chest.item[5].stack = WorldGen.genRand.Next(1, 2);
					}
				}
            }
        }
    }
}