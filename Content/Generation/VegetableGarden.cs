using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Minibiomes.Jungle;
using Spooky.Content.Tiles.Minibiomes.Jungle.Ambient;
using Spooky.Content.Tiles.Minibiomes.Jungle.Tree;

using StructureHelper;

namespace Spooky.Content.Generation
{
	public class VegetableGarden : ModSystem
	{
		private void PlaceVegetableGarden(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.VegetableGarden").Value;

			int CaveNoiseSeed = WorldGen.genRand.Next();

			int SizeXInt = Main.maxTilesX < 6400 ? 56 : 66;
			int SizeYInt = Main.maxTilesY < 1800 ? 40 : 50;
			int SizeX = Main.maxTilesX / SizeXInt;
			int SizeY = Main.maxTilesY / SizeYInt;

			int JungleLimitX = Main.maxTilesX / 18;

			bool JungleOnLeftSide = GenVars.JungleX < (Main.maxTilesX / 2);

			int Start = JungleOnLeftSide ? GenVars.JungleX + JungleLimitX : GenVars.JungleX - JungleLimitX;
			int End = JungleOnLeftSide ? GenVars.JungleX - JungleLimitX : GenVars.JungleX + JungleLimitX;

			int Increment = JungleOnLeftSide ? -20 : 20;

			int numBiomes = 0;
			int maxBiomes = Main.maxTilesX >= 6400 && Main.maxTilesY >= 1800 ? 2 : 1;
			int delayBeforeNext = 0;

			//find a valid position in the jungle away from other structures
			for (int X = Start; JungleOnLeftSide ? X >= End : X <= End; X += Increment)
			{
				for (int Y = (int)Main.rockLayer; Y <= Main.maxTilesY - 300; Y += 20)
				{
					if (delayBeforeNext > 0)
					{
						delayBeforeNext--;
					}

					if (CanPlaceBiome(X, Y, Main.maxTilesX / (SizeXInt - 12), Main.maxTilesY / (SizeYInt - 20)))
					{
						if (delayBeforeNext == 0)
						{
							PlaceOvalCluster(X, Y, Main.maxTilesX / WorldGen.genRand.Next(SizeXInt - 5, SizeXInt + 6), Main.maxTilesY / WorldGen.genRand.Next(SizeYInt - 5, SizeYInt + 6), Main.maxTilesX / 210, Main.maxTilesX / 175);
							DigOutCaves(X, Y, SizeX, SizeY, CaveNoiseSeed);
							BiomePolish(X, Y, SizeX, SizeY);
							PlaceAmbience(X, Y, SizeX, SizeY);
							numBiomes++;
							delayBeforeNext = 600;
						}

						if (numBiomes >= maxBiomes)
						{
							return;
						}
					}
				}
			}
		}

		//place a cluster of varied ovals that will serve as the shape of the biome
		public void PlaceOvalCluster(int PositionX, int PositionY, int SizeX, int SizeY, int SizeForLoop, int SizeForRandom)
		{
			for (int i = PositionX - (SizeX / 2); i < PositionX + (SizeX / 2); i += SizeForLoop)
			{
				for (int j = PositionY - (SizeY / 2); j < PositionY + (SizeY / 2); j += SizeForLoop)
				{
					int randomPositionX = WorldGen.genRand.Next(-SizeForRandom, SizeForRandom);
					int randomPositionY = WorldGen.genRand.Next(-SizeForRandom, SizeForRandom);

					SpookyWorldMethods.PlaceOval(i + randomPositionX, j + 5 + (randomPositionY / 2), ModContent.TileType<JungleSoil>(), ModContent.WallType<JungleSoilWall>(), SizeX / 2, SizeY / 2, 1f, false, false);
				}
			}
		}

		//dig out caverns inside of the area of ovals
		public void DigOutCaves(int PositionX, int PositionY, int SizeX, int SizeY, int Seed)
		{
			for (int i = PositionX - SizeX * 2; i < PositionX + SizeX * 2; i++)
			{
				for (int j = PositionY - SizeY * 2; j < PositionY + SizeY * 2; j++)
				{
					//generate caves by using noise
					if (Main.tile[i, j].TileType == ModContent.TileType<JungleSoil>())
					{
						//generate perlin noise caves
						//uses extremely high values for the X and low values for the Y to create horizontally long but vertically short caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 80f, j / 80f, 5, unchecked(Seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(i / 1500f, j / 320f, 5, Seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(i / 1500f, j / 320f, 5, unchecked(Seed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//remove tiles based on the noise variables to create caves
						//also remove the caves higher up by 5 tiles so that the bottom of the biome has a floor
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							WorldGen.KillTile(i, j - 5);
						}
					}

					if (Main.tile[i, j].WallType == ModContent.WallType<JungleSoilWall>())
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 120f, j / 550f, 5, unchecked(Seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(i / 120f, j / 550f, 5, Seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(i / 120f, j / 550f, 5, unchecked(Seed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//dig out walls to create unique holes where the background shows through
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							WorldGen.KillWall(i, j);
						}
					}
				}
			}
		}

		//method to clean up small clumps of tiles
        public void BiomePolish(int PositionX, int PositionY, int SizeX, int SizeY)
        {
            List<ushort> BlockTypes = new()
            {
                (ushort)ModContent.TileType<JungleSoil>(),
				(ushort)ModContent.TileType<JungleSoilGrass>(),
                (ushort)ModContent.TileType<JungleMoss>(),
				TileID.Mudstone
            };
            
            void getAttachedPoints(int x, int y, List<Point> points)
            {
                Tile tile = Main.tile[x, y];
                Point point = new(x, y);

				if (!WorldGen.InWorld(x, y))
				{
					tile = new Tile();
				}
                
                if (!BlockTypes.Contains(tile.TileType) || !tile.HasTile || points.Count > 90 || points.Contains(point))
                {
                    return;
                }

                points.Add(point);

                getAttachedPoints(x + 1, y, points);
                getAttachedPoints(x - 1, y, points);
                getAttachedPoints(x, y + 1, points);
                getAttachedPoints(x, y - 1, points);
            }

			for (int i = PositionX - SizeX * 2; i < PositionX + SizeX * 2; i++)
			{
				for (int j = PositionY - SizeY * 2; j < PositionY + SizeY * 2; j++)
				{
					Main.tile[i, j].LiquidAmount = 0;

					Tile tile = Main.tile[i, j];
					Tile tileAbove = Main.tile[i, j - 1];
					Tile tileBelow1 = Main.tile[i, j + 1];
					Tile tileBelow2 = Main.tile[i, j + 2];
					Tile tileBelow3 = Main.tile[i, j + 3];
					Tile tileBelow4 = Main.tile[i, j + 4];
					Tile tileBelow5 = Main.tile[i, j + 5];

					if (!tileAbove.HasTile && tile.TileType == ModContent.TileType<JungleSoil>() && tileBelow1.TileType == ModContent.TileType<JungleSoil>() && tileBelow2.TileType == ModContent.TileType<JungleSoil>() &&
					tileBelow3.TileType == ModContent.TileType<JungleSoil>() && tileBelow4.TileType == ModContent.TileType<JungleSoil>() && tileBelow5.TileType == ModContent.TileType<JungleSoil>())
					{
						SpookyWorldMethods.PlaceOval(i, j, ModContent.TileType<JungleMoss>(), 0, 4, 4, 1f, true, false);
						SpookyWorldMethods.PlaceOval(i, j, -1, ModContent.WallType<JungleMossWall>(), 6, 5, 1f, false, false);
					}
				}
			}

			for (int i = PositionX - SizeX * 2; i < PositionX + SizeX * 2; i++)
			{
				for (int j = PositionY - SizeY * 2; j < PositionY + SizeY * 2; j++)
				{
					if (CanPlaceGreenhouse(i, j, 20))
					{
						Vector2 GreenhouseOrigin = new Vector2(i - 14, j - 14);
						Generator.GenerateStructure("Content/Structures/VegetableGarden/Greenhouse-1", GreenhouseOrigin.ToPoint16(), Mod);
					}
				}
			}

			for (int i = PositionX - SizeX * 2; i < PositionX + SizeX * 2; i++)
			{
				for (int j = PositionY - SizeY * 2; j < PositionY + SizeY * 2; j++)
				{
					Tile tile = Main.tile[i, j];

					//clean tiles that are sticking out (basically tiles only attached to one tile on one side)
					bool OnlyRight = !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile;
					bool OnlyLeft = !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile && !Main.tile[i + 1, j].HasTile;
					bool OnlyDown = !Main.tile[i, j - 1].HasTile && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile;
					bool OnlyUp = !Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile;

					bool SingleThick = !Main.tile[i, j + 1].HasTile && !Main.tile[i, j - 1].HasTile;

					if (OnlyRight || OnlyLeft || OnlyDown || OnlyUp)
					{
						if (BlockTypes.Contains(Main.tile[i, j].TileType))
						{
							WorldGen.KillTile(i, j);
						}
					}

					//get rid of groups of 2 tiles on the ground since it looks weird
                    if (BlockTypes.Contains(Main.tile[i, j].TileType) && BlockTypes.Contains(Main.tile[i - 1, j].TileType) && !Main.tile[i - 2, j].HasTile && !Main.tile[i + 1, j].HasTile)
                    {
                        WorldGen.KillTile(i, j);
                        WorldGen.KillTile(i - 1, j);
                    }

					List<Point> chunkPoints = new();
                    getAttachedPoints(i, j, chunkPoints);

					int cutoffLimit = 32;
					if (chunkPoints.Count >= 1 && chunkPoints.Count < cutoffLimit)
					{
						foreach (Point p in chunkPoints)
						{
							WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
							{
								new Actions.ClearTile(true)
							}));
						}
					}

					WorldGen.SpreadGrass(i, j, ModContent.TileType<JungleSoil>(), ModContent.TileType<JungleSoilGrass>(), false);

					if (BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Tile.SmoothSlope(i, j);
					}
				}
			}
        }

		public void PlaceAmbience(int PositionX, int PositionY, int SizeX, int SizeY)
        {
			for (int i = PositionX - SizeX * 2; i < PositionX + SizeX * 2; i++)
			{
				for (int j = PositionY - SizeY * 2; j < PositionY + SizeY * 2; j++)
				{
					Tile tile = Main.tile[i, j];
					Tile tileAbove = Main.tile[i, j - 1];
					Tile tileBelow = Main.tile[i, j + 1];

					if ((tile.TileType == ModContent.TileType<JungleSoilGrass>() || tile.TileType == ModContent.TileType<JungleMoss>()) && !tileAbove.HasTile)
					{
						//grow broccoli trees
						if (WorldGen.genRand.NextBool(12) && CanPlaceBroccoli(i, j) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
                        {
                            Broccoli.Grow(i, j - 1, 5, 9);
                        }

						//cabbage boulders
						if (WorldGen.genRand.NextBool(13) && CanPlaceCabbageBoulder(i, j))
                        {
                            WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<JungleCabbageBoulder>());
                        }

						//ambient objects
						if (WorldGen.genRand.NextBool(12))
						{
							ushort[] LeafyPlants = new ushort[] { (ushort)ModContent.TileType<JunglePlant1>(), (ushort)ModContent.TileType<JunglePlant2>(), (ushort)ModContent.TileType<JunglePlant3>(),
							(ushort)ModContent.TileType<JunglePlant4>(), (ushort)ModContent.TileType<JunglePlant5>(), (ushort)ModContent.TileType<JunglePlant6>() };

							WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(LeafyPlants));
						}
						if (WorldGen.genRand.NextBool(20))
						{
							ushort[] Carrots = new ushort[] { (ushort)ModContent.TileType<Carrot1>(), (ushort)ModContent.TileType<Carrot2>(), (ushort)ModContent.TileType<Carrot3>() };

							WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Carrots), true, WorldGen.genRand.Next(0, 2));
						}
						if (WorldGen.genRand.NextBool(20))
						{
							ushort[] Corns = new ushort[] { (ushort)ModContent.TileType<Corn1>(), (ushort)ModContent.TileType<Corn2>() };

							WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Corns), true);
						}
						if (WorldGen.genRand.NextBool(20))
						{
							WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<Garlic>());
						}
						if (WorldGen.genRand.NextBool(20))
						{
							ushort[] Potatos = new ushort[] { (ushort)ModContent.TileType<Potato1>(), (ushort)ModContent.TileType<Potato2>(), (ushort)ModContent.TileType<Potato3>(), (ushort)ModContent.TileType<Potato4>() };

							WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Potatos));
						}
						if (WorldGen.genRand.NextBool(20))
						{
							WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<Pepper>(), true, WorldGen.genRand.Next(0, 2));
						}

						//grow weeds
						if (WorldGen.genRand.NextBool() && !tileAbove.HasTile && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
						{
							WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<JungleMossWeeds>(), true);
							tileAbove.TileFrameX = (short)(WorldGen.genRand.Next(14) * 18);
						}
					}

					//ceiling tiles
                    if ((Main.tile[i, j].TileType == ModContent.TileType<JungleSoilGrass>() || Main.tile[i, j].TileType == ModContent.TileType<JungleMoss>()) && !tileBelow.HasTile)
                    {
						if (WorldGen.genRand.NextBool(8))
						{
							WorldGen.PlaceObject(i, j + 1, (ushort)ModContent.TileType<Eggplant>(), true, WorldGen.genRand.Next(0, 2));
						}

						if (WorldGen.genRand.NextBool(3))
						{
                        	WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<JungleVines>());
						}
                    }

                    if (Main.tile[i, j].TileType == ModContent.TileType<JungleVines>())
                    {
						int[] ValidTiles = { ModContent.TileType<JungleSoilGrass>(), ModContent.TileType<JungleMoss>() };

                        SpookyWorldMethods.PlaceVines(i, j, ModContent.TileType<JungleVines>(), ValidTiles);
                    }
				}
			}
		}

		//check for a flat surface when placing structures
		public bool CanPlaceGreenhouse(int PositionX, int PositionY, int Size)
		{
			for (int x = PositionX - Size; x <= PositionX + Size; x++)
			{
				for (int y = PositionY - Size; y <= PositionY + Size; y++)
				{
					if (Main.tile[x, y].TileType == TileID.TinPlating || Main.tile[x, y].TileType == TileID.Mudstone)
					{
						return false;
					}
				}
			}

			for (int x = PositionX - (Size / 2); x <= PositionX + (Size / 2); x++)
			{
				//check specifically for christmas carpet since the entire floor will be made out of that
				if (Main.tile[x, PositionY].TileType == ModContent.TileType<JungleMoss>() && !Main.tile[x, PositionY - 1].HasTile && !Main.tile[x, PositionY - 2].HasTile && !Main.tile[x, PositionY - 3].HasTile && !Main.tile[x, PositionY - 4].HasTile)
				{
					continue;
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		//dont allow broccoli trees to naturally grow too close to each other
		public static bool CanPlaceBroccoli(int X, int Y)
        {
            for (int i = X - 4; i < X + 4; i++)
            {
                for (int j = Y - 4; j < Y + 4; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<Broccoli>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

		//dont allow cabbage boulders to naturally grow too close to each other
		public static bool CanPlaceCabbageBoulder(int X, int Y)
        {
            for (int i = X - 6; i < X + 6; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<JungleCabbageBoulder>())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

		//place the biome if no important structures are nearby and theres enough jungle blocks
		public bool CanPlaceBiome(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			int numJungleTiles = 0;

			for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
			{
				for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
				{
					//if (WorldGen.InWorld(i, j))
						//WorldGen.PlaceTile(i, j, TileID.Adamantite);

					int[] ValidTiles = { TileID.JungleGrass, TileID.Mud, TileID.LivingMahoganyLeaves };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && ValidTiles.Contains(Main.tile[i, j].TileType))
					{
						numJungleTiles++;
					}

					int[] InvalidTiles = { TileID.LihzahrdBrick, TileID.Hive, TileID.Dirt, ModContent.TileType<CatacombBrick1>(), ModContent.TileType<CatacombBrick2>(),
					ModContent.TileType<JungleSoilGrass>(), ModContent.TileType<JungleSoil>(), ModContent.TileType<JungleMoss>() };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && InvalidTiles.Contains(Main.tile[i, j].TileType))
					{
						return false;
					}
				}
			}

			int AmountOfTilesNeeded = (SizeX * SizeY) / 2;

			if (numJungleTiles > AmountOfTilesNeeded)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps"));
			if (GenIndex1 == -1)
			{
				return;
			}

			tasks.Insert(GenIndex1 + 1, new PassLegacy("Vegetable Garden", PlaceVegetableGarden));
		}
	}
}