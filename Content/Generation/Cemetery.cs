using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.Localization;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.Cemetery.Ambient;
using Spooky.Content.Tiles.Cemetery.Furniture;

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

			double heightLimit = Main.worldSurface * 0.35f;

			//place the terrain itself and replace blocks with cemetery blocks
			for (int X = XMiddle - (BiomeWidth / 2); X <= XMiddle + (BiomeWidth / 2); X++)
            {
                for (int Y = (int)heightLimit; Y <= Main.worldSurface; Y++)
                {
                    if (Y >= (int)heightLimit + 150 || (Y < (int)heightLimit + 150 && NoFloatingIsland(X, Y)))
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
                            tile.WallType = (ushort)ModContent.WallType<CemeteryDirtWall>();
                        }

                        tile.LiquidAmount = 0;
                    }
                }

                //place block clusters right above the world surface to prevent the cemetery from generating too low
                for (int FillY = (int)Main.worldSurface - 50; FillY <= Main.worldSurface; FillY += 3)
                {
                    SpookyWorldMethods.PlaceCircle(X, FillY, ModContent.TileType<CemeteryDirt>(), 0, WorldGen.genRand.Next(2, 3), true, true);
                }
            }

            //place dirt walls and replace open dirt walls with grass walls
            for (int X = XMiddle - (BiomeWidth / 2) - 20; X <= XMiddle + (BiomeWidth / 2) + 20; X++)
            {
                for (int Y = (int)heightLimit; Y <= Main.worldSurface; Y++)
                {
                    if (Y >= (int)heightLimit + 70 || (Y < (int)heightLimit + 70 && NoFloatingIsland(X, Y)))
                    {
                        Tile tile = Main.tile[X, Y];
                        Tile tileAbove = Main.tile[X, Y - 1];
                        Tile tileBelow = Main.tile[X, Y + 1];
                        Tile tileLeft = Main.tile[X - 1, Y];
                        Tile tileRight = Main.tile[X + 1, Y];

                        if (CanPlaceWall(X, Y) && tile.WallType == 0)
                        {
                            WorldGen.PlaceWall(X, Y, ModContent.WallType<CemeteryDirtWall>());
                        }

                        if (tile.WallType == ModContent.WallType<CemeteryDirtWall>() && (!tileAbove.HasTile || !tileBelow.HasTile || !tileLeft.HasTile || !tileRight.HasTile))
                        {
                            tile.WallType = (ushort)ModContent.WallType<CemeteryGrassWall>();
                        }
                    }
                }
            }

            //add tile dithering on the edges of the biome
            for (int X = XMiddle - (BiomeWidth / 2) - 20; X <= XMiddle + (BiomeWidth / 2) + 20; X++)
            {
                for (int Y = (int)heightLimit; Y <= Main.worldSurface; Y++)
                {
                    if (WorldGen.genRand.NextBool(2))
                    {
                        if (Y >= (int)heightLimit + 70 || (Y < (int)heightLimit + 70 && NoFloatingIsland(X, Y)))
                        {
                            Tile tile = Main.tile[X, Y];

                            //place dirt blocks
                            if (tile.HasTile && tile.TileType != TileID.Cloud && tile.TileType != TileID.RainCloud && tile.TileType != ModContent.TileType<CemeteryDirt>())
                            {
                                tile.TileType = (ushort)ModContent.TileType<CemeteryDirt>();
                            }

                            //reaplce walls with cemetery grass walls
                            if (tile.WallType > 0 && tile.WallType != ModContent.WallType<CemeteryGrassWall>())
                            {
                                tile.WallType = (ushort)ModContent.WallType<CemeteryDirtWall>();
                            }
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

		private void CemeteryFlattening(GenerationProgress progress, GameConfiguration configuration)
		{
			int XStart = Catacombs.PositionX - (BiomeWidth / 2);
			int XMiddle = Catacombs.PositionX;
			int XEdge = Catacombs.PositionX + (BiomeWidth / 2);

			int LeftY = 0;
			int RightY = 0;

			bool foundSurfaceLeft = false;
			int attemptsLeft = 0;

            //get the two surface points at the left and right of the cemetery biome
			while (!foundSurfaceLeft && attemptsLeft++ < 100000)
			{
				while ((!IsCemeteryTile(XStart, LeftY) || !NoFloatingIsland(XStart, LeftY)) && LeftY <= Main.maxTilesY)
				{
					LeftY++;
				}
				if ((WorldGen.SolidTile(XStart, LeftY) || Main.tile[XStart, LeftY].WallType > 0) && NoFloatingIsland(XStart, LeftY))
				{
					foundSurfaceLeft = true;
				}
			}

			bool foundSurfaceRight = false;
			int attemptsRight = 0;

			while (!foundSurfaceRight && attemptsRight++ < 100000)
			{
				while ((!IsCemeteryTile(XEdge, RightY) || !NoFloatingIsland(XEdge, RightY)) && RightY <= Main.maxTilesY)
				{
					RightY++;
				}
				if ((WorldGen.SolidTile(XEdge, RightY) || Main.tile[XEdge, RightY].WallType > 0) && NoFloatingIsland(XEdge, RightY))
				{
					foundSurfaceRight = true;
				}
			}

            //flatten the terrain by making a line
			ConnectPoints(new Vector2(XStart, LeftY), new Vector2(XEdge, RightY));

			//tile sloping
            double heightLimit = Main.worldSurface * 0.35f;

			for (int X = XMiddle - (BiomeWidth / 2); X <= XMiddle + (BiomeWidth / 2); X++)
            {
                for (int Y = (int)heightLimit; Y <= Main.worldSurface; Y++)
                {
                    if (IsCemeteryTile(X, Y))
                    {
                        Tile.SmoothSlope(X, Y);
                    }
                }
            }
		}

		public void ConnectPoints(Vector2 Start, Vector2 End)
		{
			int segments = 10000;

			Vector2 myCenter = Start;
			Vector2 p0 = End;
			Vector2 p1 = End;
			Vector2 p2 = myCenter;
			Vector2 p3 = myCenter;

			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;

                //place tiles below the line to create flattened terrain
				for (int Y = (int)Position.Y; Y <= Main.worldSurface; Y++)
				{
					if (!Main.tile[(int)Position.X, Y].HasTile)
					{
						WorldGen.PlaceTile((int)Position.X, Y, ModContent.TileType<CemeteryDirt>());
                        WorldGen.PlaceWall((int)Position.X, Y + 3, ModContent.WallType<CemeteryDirtWall>());
                        Main.tile[(int)Position.X, Y + 3].WallType = (ushort)ModContent.WallType<CemeteryDirtWall>();
					}
				}

                //destory all tiles above the line to get rid of unwanted hills/mountains
                double heightLimit = Main.worldSurface * 0.35f;

                for (int Y = (int)heightLimit; Y < (int)Position.Y; Y++)
				{
					if (IsCemeteryTile((int)Position.X, Y) || Main.tile[(int)Position.X, Y].WallType == ModContent.WallType<CemeteryGrassWall>() || Main.tile[(int)Position.X, Y].WallType == ModContent.WallType<CemeteryDirtWall>())
					{
                        Main.tile[(int)Position.X, Y].ClearEverything();
					}
				}

                if (Main.tile[(int)Position.X, (int)Position.Y].WallType == ModContent.WallType<CemeteryGrassWall>() || Main.tile[(int)Position.X, (int)Position.Y].WallType == ModContent.WallType<CemeteryDirtWall>())
                {
                    WorldGen.KillWall((int)Position.X, (int)Position.Y);
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
                GenerateStructure((XStart + XMiddle) / 2, StartPosY, "RuinedHouse1", 14, 20);

                //lake
                GenerateStructure((XStart + XMiddle) / 2 + 35, StartPosY, "FishingLake", 15, 5);

                //catacomb entrance
                GenerateStructure(XMiddle, StartPosY, "CemeteryEntrance", 38, 28);

                //second ruined house
                GenerateStructure((XMiddle + XEdge) / 2, StartPosY, "RuinedHouse2", 14, 20);

                //graveyards
                GenerateStructure((XMiddle + XEdge) / 2 - 35, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 14, 8);
                GenerateStructure((XMiddle + XEdge) / 2 + 35, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
                GenerateStructure((XMiddle + XEdge) / 2 + 72, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
                GenerateStructure((XMiddle + XEdge) / 2 + 95, StartPosY, "Graveyard-" + Main.rand.Next(1, 7), 12, 8);
            }
            else
            {
                //first ruined house
                GenerateStructure((XStart + XMiddle) / 2 - 40, StartPosY, "RuinedHouse1", 14, 20);

                //lake
                GenerateStructure((XStart + XMiddle) / 2, StartPosY, "FishingLake", 15, 11);

                //catacomb entrance
                GenerateStructure(XMiddle, StartPosY, "CemeteryEntrance", 38, 28);

                //second ruined house
                GenerateStructure((XMiddle + XEdge) / 2 + 40, StartPosY, "RuinedHouse2", 14, 20);
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
                    //when the cemetery catacomb crypt is placed, save the position for the catacomb entrance
                    if (StructureFile == "CemeteryEntrance")
                    {
                        Catacombs.EntranceY = startY - 33;
                    }

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

					Vector2 origin = new Vector2(startX - offsetX, startY - offsetY);
					StructureHelper.API.Generator.GenerateStructure("Content/Structures/Cemetery/" + StructureFile + ".shstruct", origin.ToPoint16(), Mod);

					placed = true;
                }
            }
        }

        //check the area around the given position for cloud blocks, to prevent structures from placing on floating islands
        public static bool NoFloatingIsland(int X, int Y)
        {
            for (int i = X - 45; i < X + 45; i++)
            {
                for (int j = Y - 45; j < Y + 45; j++)
                {
                    if (WorldGen.InWorld(i, j))
                    {
                        if (Main.tile[i, j].TileType == TileID.Cloud || Main.tile[i, j].TileType == TileID.RainCloud || Main.tile[i, j].TileType == TileID.Sunplate)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

		public bool CanPlaceWall(int X, int Y)
		{
			for (int i = X - 1; i <= X + 1; i++)
			{
				for (int j = Y - 1; j <= Y + 1; j++)
				{
					if (Main.tile[i, j].TileType != ModContent.TileType<CemeteryDirt>() && Main.tile[i, j].TileType != ModContent.TileType<CemeteryStone>())
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
			tasks.Insert(GenIndex1 + 2, new PassLegacy("Cemetery Flattening", CemeteryFlattening));
			tasks.Insert(GenIndex1 + 3, new PassLegacy("Cemetery Structures", GenerateCemeteryStructures));
            tasks.Insert(GenIndex1 + 4, new PassLegacy("Cemetery Trees", CemeteryGrassAndTrees));
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