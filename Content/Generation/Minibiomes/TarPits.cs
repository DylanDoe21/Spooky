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
using Spooky.Content.Tiles.Minibiomes;
using Terraria.DataStructures;

namespace Spooky.Content.Generation.Minibiomes
{
	public class TarPits : ModSystem
	{
		private void PlaceTarPits(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.TarPits").Value;

			int CaveNoiseSeed = WorldGen.genRand.Next();

			int SizeX = Main.maxTilesX / 60;
			int SizeY = Main.maxTilesY / 32;

			int BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + (SizeX / 2), GenVars.desertHiveRight - (SizeX / 2));
			int BiomeY = WorldGen.genRand.Next(Main.maxTilesY / 2, GenVars.desertHiveLow - SizeY - 50);

			int maxBiomes = Main.maxTilesX >= 6400 && Main.maxTilesY >= 1800 ? 2 : 1;

			//place tiles in the box that the biome can spawn in for debugging purposes
			//for (int i = GenVars.desertHiveLeft + (SizeX / 2); i <= GenVars.desertHiveRight - (SizeX / 2); i++)
			//{
				//for (int j = Main.maxTilesY / 2; j <= GenVars.desertHiveLow - SizeY - 50; j++)
				//{
					//WorldGen.PlaceTile(i, j, TileID.Adamantite);
				//}
			//}

			for (int numBiomesPlaced = 0; numBiomesPlaced < maxBiomes; numBiomesPlaced++)
			{
				while (!CanPlaceBiome(BiomeX, BiomeY, Main.maxTilesX / 46, Main.maxTilesY / 23))
				{
					BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + (SizeX / 2), GenVars.desertHiveRight - (SizeX / 2));
					BiomeY = WorldGen.genRand.Next(Main.maxTilesY / 2, GenVars.desertHiveLow - SizeY - 50);
				}

				PlaceOvalCluster(BiomeX, BiomeY, SizeX, SizeY, Main.maxTilesX / 300, Main.maxTilesX / 190);
				DigOutCaves(CaveNoiseSeed);
				BiomePolish();
			}
		}

		//place a cluster of varied ovals that will serve as the shape of the biome
		public void PlaceOvalCluster(int PositionX, int PositionY, int SizeX, int SizeY, int SizeForLoop, int SizeForRandom)
		{
			for (int i = PositionX - (SizeX / 3); i < PositionX + (SizeX / 3); i += SizeForLoop)
			{
				for (int j = PositionY - (SizeY / 2); j < PositionY + (SizeY / 2); j += SizeForLoop)
				{
					int randomPositionX = WorldGen.genRand.Next(-SizeForRandom / 2, SizeForRandom / 2);
					int randomPositionY = WorldGen.genRand.Next(-SizeForRandom, SizeForRandom);

					SpookyWorldMethods.PlaceOval(i + randomPositionX, j + randomPositionY, ModContent.TileType<DesertSandstone>(), ModContent.WallType<DesertSandstoneWall>(), SizeX / 5, SizeY / 2, 2f, false);
				}
			}
		}

		//dig out caverns inside of the area of ovals
		public void DigOutCaves(int Seed)
		{
			for (int i = GenVars.desertHiveLeft; i < GenVars.desertHiveRight; i++)
			{
				for (int j = GenVars.desertHiveHigh; j < GenVars.desertHiveLow; j++)
				{
					//replace sandstone with sand using noise
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>())
					{
						//generate perlin noise caves
						//uses extremely high values for the X and low values for the Y to create horizontally long but vertically short caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 80f, j / 80f, 5, unchecked(Seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(i / 200f, j / 100f, 5, Seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(i / 200f, j / 100f, 5, unchecked(Seed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							Main.tile[i, j].TileType = (ushort)ModContent.TileType<DesertSand>();
						}
					}

					//generate caves by using noise
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSand>())
					{
						//generate perlin noise caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 80f, j / 80f, 5, unchecked(Seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(i / 300f, j / 550f, 5, Seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(i / 300f, j / 550f, 5, unchecked(Seed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//remove tiles based on the noise variables to create caves
						//place the caves 15 blocks up so that the bottom of the biome has a bowl shape so that water can be placed there later
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							WorldGen.KillTile(i, j - 15);
						}
					}

					//replace sandstone walls with sand walls
					if (Main.tile[i, j].WallType == ModContent.WallType<DesertSandstoneWall>())
					{
						//generate perlin noise caves
						//uses extremely high values for the X and low values for the Y to create horizontally long but vertically short caves
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 80f, j / 80f, 5, unchecked(Seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(i / 200f, j / 100f, 5, Seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(i / 200f, j / 100f, 5, unchecked(Seed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							Main.tile[i, j].WallType = (ushort)ModContent.WallType<DesertSandWall>();
						}
					}
				}
			}
		}

		//method to clean up small clumps of tiles
		public static void BiomePolish()
		{
			List<ushort> BlockTypes = new()
			{
				(ushort)ModContent.TileType<DesertSand>(),
				(ushort)ModContent.TileType<DesertSandstone>(),
			};

			void getAttachedPoints(int x, int y, List<Point> points)
			{
				Tile tile = Main.tile[x, y];
				Point point = new(x, y);

				if (!WorldGen.InWorld(x, y))
				{
					tile = new Tile();
				}

				if (!BlockTypes.Contains(tile.TileType) || !tile.HasTile || points.Count > 50 || points.Contains(point))
				{
					return;
				}

				points.Add(point);

				getAttachedPoints(x + 1, y, points);
				getAttachedPoints(x - 1, y, points);
				getAttachedPoints(x, y + 1, points);
				getAttachedPoints(x, y - 1, points);
			}

			//flatten out surfaces a little
			for (int i = GenVars.desertHiveLeft; i < GenVars.desertHiveRight; i++)
			{
				for (int j = GenVars.desertHiveHigh; j < GenVars.desertHiveLow; j++)
				{
					if ((Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>()) && !Main.tile[i, j - 1].HasTile)
					{
						int TileCheck = 0;

						for (int XCheck = i - 2; XCheck <= i + 3; XCheck++)
						{
							if (Main.tile[XCheck, j].HasTile)
							{
								TileCheck++;
							}

							if (XCheck == i + 2 && TileCheck <= 3)
							{
								SpookyWorldMethods.PlaceOval(i, j + 2, -1, 0, 4, 4, 1f, false);
								break;
							}
						}
					}
				}
			}

			//clean up tiles
			for (int i = GenVars.desertHiveLeft; i < GenVars.desertHiveRight; i++)
			{
				for (int j = GenVars.desertHiveHigh; j < GenVars.desertHiveLow; j++)
				{
					List<Point> chunkPoints = new();
					getAttachedPoints(i, j, chunkPoints);

					int cutoffLimit = 50;
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

					if (BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Tile.SmoothSlope(i, j);
					}
				}
			}
		}

		//place the biome if there isnt already another tar pits biome nearby
		public bool CanPlaceBiome(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			int numDesertTiles = 0;

			for (int i = PositionX - (SizeX / 3); i < PositionX + (SizeX / 3); i++)
			{
				for (int j = PositionY - (SizeY / 2); j < PositionY + (SizeY / 2); j++)
				{
					//if (WorldGen.InWorld(i, j))
						//WorldGen.PlaceTile(i, j, TileID.Adamantite);

					int[] ValidTiles = { TileID.Sand, TileID.Sandstone, TileID.HardenedSand };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && ValidTiles.Contains(Main.tile[i, j].TileType))
					{
						numDesertTiles++;
					}

					int[] InvalidTiles = { TileID.Stone, TileID.Dirt, ModContent.TileType<DesertSand>(), ModContent.TileType<DesertSandstone>(),
					ModContent.TileType<CatacombBrick1>(), ModContent.TileType<CatacombBrick2>(), };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && InvalidTiles.Contains(Main.tile[i, j].TileType))
					{
						return false;
					}
				}
			}

			int AmountOfTilesNeeded = (SizeX * SizeY) / 3;

			if (numDesertTiles > AmountOfTilesNeeded)
			{
				return true;
			}
			else
			{
				return false;
			}

			return true;
		}

		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
		{
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps"));
			if (GenIndex1 == -1)
			{
				return;
			}

			tasks.Insert(GenIndex1 + 1, new PassLegacy("Tar Pits", PlaceTarPits));
		}
	}
}