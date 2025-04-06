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
using Spooky.Content.Tiles.Minibiomes.Desert.Ambient;

namespace Spooky.Content.Generation
{
	public class TarPits : ModSystem
	{
		public static List<ushort> BlockTypes = new()
		{
			(ushort)ModContent.TileType<DesertSand>(),
			(ushort)ModContent.TileType<DesertSandstone>()
		};

		public static List<ushort> WallTypes = new()
		{
			(ushort)ModContent.WallType<DesertSandWall>(),
			(ushort)ModContent.WallType<DesertSandstoneWall>()
		};

		List<Vector2> StructurePoints = new List<Vector2>();

		private void PlaceTarPits(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.TarPits").Value;

			int CaveNoiseSeed = WorldGen.genRand.Next();

			int SizeXInt = Main.maxTilesX < 6400 ? 37 : 53;
			int SizeYInt = Main.maxTilesY < 1800 ? 15 : 22;
			int SizeX = Main.maxTilesX / SizeXInt;
			int SizeY = Main.maxTilesY / SizeYInt;

			bool IsSmallWorld = Main.maxTilesX < 6400 && Main.maxTilesY < 1800;

			int BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + (SizeX / 2), GenVars.desertHiveRight - (SizeX / 2));
			int BiomeY = WorldGen.genRand.Next(GenVars.desertHiveHigh + (SizeY / 2), Main.maxTilesY / 2);

			if (!IsSmallWorld)
			{
				BiomeY = WorldGen.genRand.Next(GenVars.desertHiveHigh + (SizeY * 3), Main.maxTilesY / 2 - 75);
			}
			else
			{
				BiomeY = WorldGen.genRand.Next(Main.maxTilesY / 2 + 50, Main.maxTilesY - 300);
			}

			int maxBiomes = !IsSmallWorld ? 2 : 1;

			for (int numBiomesPlaced = 0; numBiomesPlaced < maxBiomes; numBiomesPlaced++)
			{
				if (numBiomesPlaced == 0)
				{
					BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + SizeX, GenVars.desertHiveRight - SizeX);

					if (!IsSmallWorld)
					{
						BiomeY = WorldGen.genRand.Next(GenVars.desertHiveHigh + (SizeY * 3), Main.maxTilesY / 2 - 75);
					}
					else
					{
						BiomeY = WorldGen.genRand.Next(GenVars.desertHiveHigh + (SizeY * 3), Main.maxTilesY - 400);
					}
				}
				else
				{
					BiomeX = WorldGen.genRand.Next(GenVars.desertHiveLeft + SizeX, GenVars.desertHiveRight - SizeX);
					BiomeY = WorldGen.genRand.Next(Main.maxTilesY / 2, GenVars.desertHiveLow - SizeY - (SizeY / 2));
				}

				SpookyWorldMethods.PlaceOval(BiomeX, BiomeY, ModContent.TileType<DesertSandstone>(), ModContent.WallType<DesertSandstoneWall>(), SizeX / 2, SizeY, 2f, false, false);
				DigOutCaves(BiomeX, BiomeY, SizeX, SizeY, CaveNoiseSeed);
				BiomePolish(BiomeX, BiomeY, SizeX, SizeY);
				CleanOutSmallClumps(BiomeX, BiomeY, SizeX, SizeY);
				PlaceStructures(BiomeX, BiomeY, SizeX, SizeY);
				BiomeAmbience(BiomeX, BiomeY, SizeX, SizeY);
			}
		}

		//dig out caverns inside of the area of ovals
		public void DigOutCaves(int PositionX, int PositionY, int SizeX, int SizeY, int Seed)
		{
			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					//generate caves by using noise
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSand>())
					{
						float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 80f, j / 80f, 5, unchecked(Seed + 1)) * 0.01f;
						float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(i / 1000f, j / 300f, 5, Seed) + 0.5f + horizontalOffsetNoise;
						float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(i / 1000f, j / 300f, 5, unchecked(Seed - 1)) + 0.5f;
						float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
						float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

						//remove tiles based on the noise variables to create caves
						//place the caves 15 blocks up so that the bottom of the biome has a bowl shape so that water can be placed there later
						if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
						{
							if (CanDigCaveOnBlock(i, j - 10))
							{
								WorldGen.KillTile(i, j - 10);
							}
						}
					}
				}
			}

			//place actual tar pits
			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					if (CanPlaceTarPit(i, j))
					{
						PlaceTarPit(i, j + 9, 15, 26, 0.5f);
					}
				}
			}

			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					//replace sandstone with sand using noise
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>() || Main.tile[i, j].WallType == ModContent.WallType<DesertSandstoneWall>())
					{
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
				}
			}

			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(i / 350f, j / 2000f, 5, unchecked(Seed + 1)) * 0.01f;
					float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(i / 350f, j / 2000f, 5, Seed) + 0.5f + horizontalOffsetNoise;
					float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(i / 350f, j / 2000f, 5, unchecked(Seed - 1)) + 0.5f;
					float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
					float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

					if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
					{
						if (Main.tile[i, j].WallType == ModContent.WallType<DesertSandWall>() || Main.tile[i, j].WallType == ModContent.WallType<DesertSandstoneWall>())
						{
							WorldGen.KillWall(i, j);
						}
					}
				}
			}
		}

		public void BiomePolish(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					if (BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Tile.SmoothSlope(i, j);
					}
				}
			}

			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					//clean tiles that are sticking out (basically tiles only attached to one tile on one side)
					bool OnlyRight = !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile;
					bool OnlyLeft = !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile && !Main.tile[i + 1, j].HasTile;
					bool OnlyDown = !Main.tile[i, j - 1].HasTile && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile;
					bool OnlyUp = !Main.tile[i, j + 1].HasTile && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile;

					bool SingleThick = (!Main.tile[i, j + 1].HasTile && !Main.tile[i, j - 1].HasTile) || (!Main.tile[i + 1, j].HasTile && !Main.tile[i - 1, j].HasTile);

					if (OnlyRight || OnlyLeft || OnlyDown || OnlyUp || SingleThick)
					{
						if (BlockTypes.Contains(Main.tile[i, j].TileType))
						{
							WorldGen.KillTile(i, j);
						}
					}

					//kil one block thick tiles
					if (BlockTypes.Contains(Main.tile[i, j].TileType) && !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile)
					{
						WorldGen.KillTile(i, j);
					}
					if (BlockTypes.Contains(Main.tile[i, j].TileType) && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile)
					{
						WorldGen.KillTile(i, j);
					}

					//get rid of single tiles on the ground since it looks weird
					if (BlockTypes.Contains(Main.tile[i, j].TileType) && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile)
					{
						WorldGen.KillTile(i, j);
					}

					//get rid of 1x2 tiles on the ground since it looks weird
					if (BlockTypes.Contains(Main.tile[i, j].TileType) && BlockTypes.Contains(Main.tile[i - 1, j].TileType) && !Main.tile[i - 2, j].HasTile && !Main.tile[i + 1, j].HasTile)
					{
						WorldGen.KillTile(i, j);
						WorldGen.KillTile(i - 1, j);
					}

					//get rid of 1x3 tiles on the ground since it looks weird
					if (BlockTypes.Contains(Main.tile[i, j].TileType) && BlockTypes.Contains(Main.tile[i - 1, j].TileType) && 
					BlockTypes.Contains(Main.tile[i + 1, j].TileType) && !Main.tile[i - 2, j].HasTile && !Main.tile[i + 2, j].HasTile)
					{
						WorldGen.KillTile(i, j);
						WorldGen.KillTile(i - 1, j);
						WorldGen.KillTile(i + 1, j);
					}
				}
			}
		}

		public void BiomeAmbience(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>())
					{
						//big piles
						if (WorldGen.genRand.NextBool(6))
						{
							ushort[] LargePiles = new ushort[] { (ushort)ModContent.TileType<DesertPileLarge1>(), (ushort)ModContent.TileType<DesertPileLarge2>(), 
							(ushort)ModContent.TileType<DesertPileLarge3>(), (ushort)ModContent.TileType<DesertPileLarge4>(), (ushort)ModContent.TileType<DesertPileLarge5>() };

							WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(LargePiles));
						}

						//small piles
						if (WorldGen.genRand.NextBool(6))
						{
							ushort[] SmallPiles = new ushort[] { (ushort)ModContent.TileType<DesertPileSmall1>(),
							(ushort)ModContent.TileType<DesertPileSmall2>(), (ushort)ModContent.TileType<DesertPileSmall3>() };

							WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(SmallPiles));
						}

						//stalagmites and stalactites
						if (WorldGen.genRand.NextBool(3))
						{
							ushort[] Stalactites = new ushort[] { (ushort)ModContent.TileType<DesertStalactite1>(), (ushort)ModContent.TileType<DesertStalactite2>(), (ushort)ModContent.TileType<DesertStalactite3>() };

							WorldGen.PlaceObject(i, j + 1, WorldGen.genRand.Next(Stalactites));
						}
						if (WorldGen.genRand.NextBool(3))
						{
							ushort[] Stalagmites = new ushort[] { (ushort)ModContent.TileType<DesertStalagmite1>(), (ushort)ModContent.TileType<DesertStalagmite2>(), (ushort)ModContent.TileType<DesertStalagmite3>() };

							WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Stalagmites));
						}
					}

					if ((Main.tile[i, j].TileType == ModContent.TileType<DesertSand>() || Main.tile[i, j].TileType == ModContent.TileType<DesertSandstone>()) && 
					!Main.tile[i, j - 1].HasTile && Main.tile[i, j - 1].LiquidAmount <= 0)
					{
						//grow cactuses
						if (WorldGen.genRand.NextBool() && CanPlaceCactus(i, j) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
						{
							TarPitCactus.Grow(i, j - 1, 5, 9);
						}
					}
				}
			}
		}

		//method to clean up small clumps of tiles
		public void CleanOutSmallClumps(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			int MaxPoints = 200;

			void getAttachedPoints(int x, int y, List<Point> points)
			{
				Tile tile = Main.tile[x, y];
				Point point = new(x, y);

				if (!WorldGen.InWorld(x, y))
				{
					tile = new Tile();
				}

				if (!BlockTypes.Contains(tile.TileType) || !tile.HasTile || points.Count > MaxPoints || points.Contains(point))
				{
					return;
				}

				points.Add(point);

				getAttachedPoints(x + 1, y, points);
				getAttachedPoints(x - 1, y, points);
				getAttachedPoints(x, y + 1, points);
				getAttachedPoints(x, y - 1, points);
			}

			for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
			{
				for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
				{
					List<Point> chunkPoints = new();
					getAttachedPoints(i, j, chunkPoints);

					if (chunkPoints.Count <= MaxPoints)
					{
						foreach (Point p in chunkPoints)
						{
							WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
							{
								new Actions.ClearTile(true)
							}));
						}
					}
				}
			}
		}

		public void PlaceStructures(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			bool PlacedMinecartEntrance = false;

			//place only one mineshaft entrance at the edge of the biome
			for (int j = PositionY - 35; j < PositionY + 35; j++)
			{
				for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
				{
					if (BlockTypes.Contains(Main.tile[i, j].TileType) && CanPlaceMinecartEntrance(i, j) && !PlacedMinecartEntrance)
					{
						WorldGen.digTunnel(i, j - 5, default, default, WorldGen.genRand.Next(1, 4), 5, false);
						WorldGen.digTunnel(i - 11, j - 5, default, default, WorldGen.genRand.Next(1, 4), 5, false);
						WorldGen.digTunnel(i + 11, j - 5, default, default, WorldGen.genRand.Next(1, 4), 5, false);

						if (WorldGen.genRand.NextBool())
						{
							Vector2 Origin = new Vector2(i - 11, j - 10);
							StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/MinecartEntrance-1.shstruct", Origin.ToPoint16(), Mod);
						}
						else
						{
							Vector2 Origin = new Vector2(i - 16, j - 10);
							StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/MinecartEntrance-2.shstruct", Origin.ToPoint16(), Mod);
						}

						StructurePoints.Add(new Vector2(i, j));

						PlacedMinecartEntrance = true;
					}
				}
			}

			//place other random structures
			for (int j = PositionY - SizeY - (SizeY / 2); j < PositionY + SizeY + (SizeY / 2); j++)
			{
				for (int i = PositionX - SizeX + (SizeX / 3); i < PositionX + SizeX - (SizeX / 3); i++)
				{
					if (WorldGen.genRand.NextBool(10) && CanPlaceStructure(i, j, 20) && BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Vector2 Origin = new Vector2(i - 8, j - 15);
						StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/TarWell.shstruct", Origin.ToPoint16(), Mod);
						StructurePoints.Add(new Vector2(i, j));
					}

					if (WorldGen.genRand.NextBool(10) && CanPlaceStructure(i, j, 30) && BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Vector2 Origin = new Vector2(i - 14, j - 19);
						StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/Crane-" + WorldGen.genRand.Next(1, 3) + ".shstruct", Origin.ToPoint16(), Mod);
						StructurePoints.Add(new Vector2(i, j));
					}

					if (WorldGen.genRand.NextBool(8) && CanPlaceStructure(i, j, 10) && BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Vector2 Origin = new Vector2(i - 6, j - 6);
						StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/Minecart-" + WorldGen.genRand.Next(1, 6) + ".shstruct", Origin.ToPoint16(), Mod);
						StructurePoints.Add(new Vector2(i, j));
					}

					if (WorldGen.genRand.NextBool(5) && CanPlaceStructure(i, j, 10) && BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Vector2 Origin = new Vector2(i - 6, j - 4);
						StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/BonePile.shstruct", Origin.ToPoint16(), Mod);
						StructurePoints.Add(new Vector2(i, j));
					}

					if (WorldGen.genRand.NextBool(5) && CanPlaceStructure(i, j, 15) && BlockTypes.Contains(Main.tile[i, j].TileType))
					{
						Vector2 Origin = new Vector2(i - 7, j - 10);
						StructureHelper.API.Generator.GenerateStructure("Content/Structures/TarPits/SmallHut.shstruct", Origin.ToPoint16(), Mod);
						StructurePoints.Add(new Vector2(i, j));
					}
				}
			}
		}

		public bool CanPlaceMinecartEntrance(int PositionX, int PositionY)
		{
			int numOutsideTiles = 0;

			for (int x = PositionX - 4; x <= PositionX + 4; x++)
			{
				for (int y = PositionY - 4; y <= PositionY + 4; y++)
				{
					if (!BlockTypes.Contains(Main.tile[x, y].TileType) && Main.tile[x, y].HasTile)
					{
						numOutsideTiles++;
					}
				}
			}

			for (int x = PositionX - 15; x <= PositionX + 15; x++)
			{
				for (int y = PositionY - 15; y <= PositionY + 15; y++)
				{
					if (Main.tile[x, y].LiquidAmount > 0)
					{
						return false;
					}
				}
			}

			return numOutsideTiles > 5;
		}

		public bool CanPlaceStructure(int PositionX, int PositionY, int UpCheckDist)
		{
			int numTiles = 0;

			//only place structures on biome blocks
			for (int x = PositionX - 1; x <= PositionX + 1; x++)
			{
				for (int y = PositionY; y <= PositionY + 3; y++)
				{
					if (BlockTypes.Contains(Main.tile[x, y].TileType) && Main.tile[x, y].HasTile)
					{
						numTiles++;
					}
				}
			}

			//dont allow them to place too close to liquids
			for (int x = PositionX - 6; x <= PositionX + 6; x++)
			{
				for (int y = PositionY - UpCheckDist; y <= PositionY + 5; y++)
				{
					if (Main.tile[x, y].LiquidAmount > 0)
					{
						return false;
					}
				}
			}

			//preform upward check to make sure theres enough room
			for (int y = PositionY - 6; y <= PositionY - 1; y++)
			{
				if (Main.tile[PositionX, y].HasTile)
				{
					return false;
				}
			}

			//dont let structures place too close to each other
			foreach (Vector2 pos in StructurePoints)
			{
				float Dist = Vector2.Distance(pos, new Vector2(PositionX, PositionY));
				if (Dist < 45)
				{
					return false;
				}
			}

			return numTiles > 8;
		}

		//generate a semi-oval with a pool of water in the middle
		public void PlaceTarPit(int X, int Y, int radius, int radiusY, float thickMult)
		{
			float scale = radiusY / (float)radius;
			float invertScale = (float)radius / radiusY;

			int numTiles = 0;

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

						if (BlockTypes.Contains(tile.TileType))
						{
							numTiles++;
						}

						if (tile.TileType == TileID.Sandstone || tile.TileType == TileID.HardenedSand)
						{
							return;
						}
					}
				}
			}

			if (numTiles < 60)
			{
				return;
			}

			SpookyWorldMethods.PlaceOval(X, Y + 5, ModContent.TileType<DesertSandstone>(), 0, 21, 10, 1f, false, false);
			SpookyWorldMethods.PlaceOval(X, Y + 10, ModContent.TileType<DesertSandstone>(), 0, 20, 10, 1f, false, false);
			SpookyWorldMethods.PlaceOval(X, Y + 15, ModContent.TileType<DesertSandstone>(), 0, 19, 10, 1f, false, false);

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

						WorldGen.PlaceTile(PositionX, PositionY, ModContent.TileType<DesertSandstone>());
						Main.tile[PositionX, PositionY].TileType = (ushort)ModContent.TileType<DesertSandstone>();

						if (Math.Sqrt(x * x + y * y) < radius - radialMod)
						{
							WorldGen.KillTile(PositionX, PositionY);
							tile.LiquidType = LiquidID.Water;
							tile.LiquidAmount = 255;
						}
					}
				}
			}

			SpookyWorldMethods.PlaceOval(X, Y, -1, 0, 19, 6, 1f, false, true);
		}

		//dont allow tar pits to place outside of the biome or near another tar pit
		public bool CanPlaceTarPit(int PositionX, int PositionY)
		{
			for (int i = PositionX - 30; i <= PositionX + 30; i++)
			{
				for (int j = PositionY - 30; j <= PositionY + 40; j++)
				{
					if (Main.tile[i, j].LiquidAmount > 0 && Main.tile[i, j].LiquidType == LiquidID.Water)
					{
						return false;
					}
				}
			}

			for (int x = PositionX - 5; x <= PositionX + 5; x++)
			{
				if ((Main.tile[x, PositionY].TileType == ModContent.TileType<DesertSand>() || Main.tile[x, PositionY].TileType == ModContent.TileType<DesertSandstone>()) &&
				(Main.tile[x, PositionY - 1].TileType != ModContent.TileType<DesertSand>() || Main.tile[x, PositionY - 1].TileType != ModContent.TileType<DesertSandstone>()))
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

		//dont allow caves to be dug on blocks on the edge of the biome
		public bool CanDigCaveOnBlock(int PositionX, int PositionY)
		{
			for (int i = PositionX - 3; i <= PositionX + 3; i++)
			{
				for (int j = PositionY - 3; j <= PositionY + 3; j++)
				{
					if (!WallTypes.Contains(Main.tile[i, j].WallType))
					{
						return false;
					}
				}
			}

			return true;
		}

		//dont allow cactuses to naturally grow too close to each other
		public static bool CanPlaceCactus(int X, int Y)
		{
			for (int i = X - 4; i < X + 4; i++)
			{
				for (int j = Y - 4; j < Y + 4; j++)
				{
					if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == ModContent.TileType<TarPitCactus>())
					{
						return false;
					}
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
					int[] ValidTiles = { TileID.Sand, TileID.Sandstone, TileID.HardenedSand, TileID.DesertFossil };
					int[] ValidWalls = { WallID.Sandstone, WallID.HardenedSand };

					if (WorldGen.InWorld(i, j) && Main.tile[i, j].HasTile && (ValidTiles.Contains(Main.tile[i, j].TileType) || ValidWalls.Contains(Main.tile[i, j].WallType)))
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

			int AmountOfTilesNeeded = (SizeX * SizeY) / 4;

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