using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.IO;
using Terraria.WorldBuilding;
using Terraria.GameContent.Generation;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Minibiomes.Desert;

namespace Spooky.Content.Generation.Minibiomes
{
	public class TarPits : ModSystem
	{
		private void PlaceTarPits(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.TarPits").Value;

			int CaveNoiseSeed = WorldGen.genRand.Next();

			int SizeXInt = Main.maxTilesX < 6400 ? 48 : 56;
			int SizeYInt = Main.maxTilesY < 1800 ? 20 : 28;
			int SizeX = Main.maxTilesX / SizeXInt;
			int SizeY = Main.maxTilesY / SizeYInt;

			int BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + (SizeX / 2), GenVars.desertHiveRight - (SizeX / 2));
			int BiomeY = WorldGen.genRand.Next(GenVars.desertHiveHigh + (SizeY * 3), Main.maxTilesY / 2 - 50);

			int maxBiomes = Main.maxTilesX >= 6400 && Main.maxTilesY >= 1800 ? 2 : 1;

			for (int numBiomesPlaced = 0; numBiomesPlaced < maxBiomes; numBiomesPlaced++)
			{
				while (!CanPlaceBiome(BiomeX, BiomeY, SizeX, SizeY))
				{
					if (numBiomesPlaced == 0)
					{
						BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + (SizeX / 2), GenVars.desertHiveRight - (SizeX / 2));
						BiomeY = WorldGen.genRand.Next(GenVars.desertHiveHigh + (SizeY * 3), Main.maxTilesY / 2 - 50);
					}
					else
					{
						BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + (SizeX / 2), GenVars.desertHiveRight - (SizeX / 2));
						BiomeY = WorldGen.genRand.Next(Main.maxTilesY / 2, GenVars.desertHiveLow - SizeY - (SizeY / 2));
					}
				}

				PlaceOvalCluster(BiomeX, BiomeY + 5, SizeX, SizeY, Main.maxTilesX / 300, Main.maxTilesX / 190);
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
							Main.tile[i, j].WallType = (ushort)ModContent.WallType<DesertSandWall>();
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
								SpookyWorldMethods.PlaceOval(i, j + 5, -1, 0, 6, 6, 1f, false);
								break;
							}
						}
					}
				}
			}

			//place actual tar pits
			for (int i = GenVars.desertHiveLeft; i < GenVars.desertHiveRight; i++)
			{
				for (int j = GenVars.desertHiveHigh; j < GenVars.desertHiveLow; j++)
				{
					if (CanPlaceTarPit(i, j))
					{
						PlaceTarPit(i, j, WorldGen.genRand.Next(8, 11), WorldGen.genRand.Next(21, 39), 0.5f);
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

		//generate a semi-oval with a pool of water in the middle
		public static void PlaceTarPit(int X, int Y, int radius, int radiusY, float thickMult)
		{
			List<ushort> WallTypes = new()
			{
				(ushort)ModContent.WallType<DesertSandWall>(),
				(ushort)ModContent.WallType<DesertSandstoneWall>(),
			};

			float scale = radiusY / (float)radius;
			float invertScale = (float)radius / radiusY;

			for (int x = -radius; x <= radius; x++)
			{
				for (float y = 0; y <= radius; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(2.5f, 4.5f) * thickMult;
					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);

						if (!WallTypes.Contains(tile.WallType))
						{
							return;
						}
					}
				}
			}

			for (int x = -radius; x <= radius; x++)
			{
				for (float y = 0; y <= radius; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(5.5f, 6.5f) * thickMult;
					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);

						if (tile.TileType == ModContent.TileType<DesertSand>() || tile.WallType == ModContent.WallType<DesertSandWall>())
						{
							WorldGen.KillTile(PositionX, PositionY);
							WorldGen.PlaceTile(PositionX, PositionY, ModContent.TileType<DesertSand>());
						}
						if (tile.TileType == ModContent.TileType<DesertSandstone>() || tile.WallType == ModContent.WallType<DesertSandstoneWall>())
						{
							WorldGen.KillTile(PositionX, PositionY);
							WorldGen.PlaceTile(PositionX, PositionY, ModContent.TileType<DesertSandstone>());
						}

						if (Math.Sqrt(x * x + y * y) < radius - radialMod)
						{
							WorldGen.KillTile(PositionX, PositionY);
							tile.LiquidType = LiquidID.Water;
							tile.LiquidAmount = 255;
						}
					}
				}
			}
		}

		public static bool CanPlaceTarPit(int PositionX, int PositionY)
		{
			for (int i = PositionX - 6; i <= PositionX + 6; i++)
			{
				for (int j = PositionY - 20; j <= PositionY + 30; j++)
				{
					if (Main.tile[i, j].LiquidAmount > 0 && Main.tile[i, j].LiquidType == LiquidID.Water)
					{
						return false;
					}
				}
			}

			for (int i = PositionX - 2; i <= PositionX + 1; i++)
			{
				if ((Main.tile[i, PositionY].TileType != ModContent.TileType<DesertSand>() && Main.tile[i, PositionY].TileType != ModContent.TileType<DesertSandstone>()) ||
				Main.tile[i, PositionY - 1].HasTile || Main.tile[i, PositionY - 1].LiquidAmount > 0)
				{
					return false;
				}
			}

			return true;
		}

		//place the biome if there isnt already another tar pits biome nearby
		public bool CanPlaceBiome(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			int numDesertTiles = 0;

			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
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