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

using Spooky.Core;
using Spooky.Content.Tiles.Minibiomes.Ocean;
using Spooky.Content.Tiles.Minibiomes.Ocean.Ambient;

using StructureHelper;

namespace Spooky.Content.Generation
{
	public class ZombieOcean : ModSystem
	{
		public static List<ushort> BlockTypes = new()
		{
			(ushort)ModContent.TileType<OceanSand>(),
			(ushort)ModContent.TileType<OceanBiomass>(),
			(ushort)ModContent.TileType<OceanMeat>(),
			(ushort)ModContent.TileType<LabMetalPlate>()
		};

		public static List<ushort> WallTypes = new()
		{
			(ushort)ModContent.WallType<OceanSandWall>(),
			(ushort)ModContent.WallType<OceanBiomassWall>()
		};

		List<int> BiomePositionDistances = new List<int>();

		static int StartPositionX;
		static int StartPositionY;

		private void PlaceZombieOcean(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.ZombieOcean").Value;

			//random worldside (default option)
			if (ModContent.GetInstance<SpookyWorldgenConfig>().ZombieBiomeWorldside == ZombieBiomePosEnum.Random)
			{
				StartPositionX = !WorldGen.genRand.NextBool() ? 185 : Main.maxTilesX - 175;
			}
			//jungle side position
			if (ModContent.GetInstance<SpookyWorldgenConfig>().ZombieBiomeWorldside == ZombieBiomePosEnum.JungleSide)
			{
				StartPositionX = GenVars.JungleX < (Main.maxTilesX / 2) ? 185 : Main.maxTilesX - 175;
			}
			//dungeon side position
			if (ModContent.GetInstance<SpookyWorldgenConfig>().ZombieBiomeWorldside == ZombieBiomePosEnum.DungeonSide)
			{
				StartPositionX = GenVars.dungeonSide < 0 ? 185 : Main.maxTilesX - 175;
			}

			StartPositionY = (int)Main.worldSurface + 75;

			Flags.ZombieBiomePositions.Clear();

			int SizeXInt = 40;
			int SizeYInt = 40;
			int SizeX = Main.maxTilesX / SizeXInt;
			int SizeY = Main.maxTilesY / SizeYInt;

			//left side
			if (StartPositionX < Main.maxTilesX / 2)
			{
				for (int i = 10; i <= 260; i++)
				{
					for (int j = 100; j <= StartPositionY; j++)
					{
						if (!Main.tileDungeon[Main.tile[i, j].TileType] && !Main.wallDungeon[Main.tile[i, j].WallType])
						{
							if (j < Main.worldSurface)
							{
								if (WorldGen.SolidTile(i, j) || Main.tile[i, j].WallType > 0)
								{
									WorldGen.KillTile(i, j);
									WorldGen.PlaceTile(i, j, TileID.Sand);
								}
							}
							else
							{
								WorldGen.KillTile(i, j);
								WorldGen.PlaceTile(i, j, TileID.Sand);
							}
						}
					}
				}
			}
			else
			{
				for (int i = Main.maxTilesX - 260; i <= Main.maxTilesX - 10; i++)
				{
					for (int j = 100; j <= StartPositionY; j++)
					{
						if (!Main.tileDungeon[Main.tile[i, j].TileType] && !Main.wallDungeon[Main.tile[i, j].WallType])
						{
							if (j < Main.worldSurface)
							{
								if (WorldGen.SolidTile(i, j) || Main.tile[i, j].WallType > 0)
								{
									WorldGen.KillTile(i, j);
									WorldGen.PlaceTile(i, j, TileID.Sand);
								}
							}
							else
							{
								WorldGen.KillTile(i, j);
								WorldGen.PlaceTile(i, j, TileID.Sand);
							}
						}
					}
				}
			}

			PlaceDepthsOval(StartPositionX, StartPositionY, TileID.Sand, 0, (SizeXInt + 4) * 4, (SizeYInt + 4) * 2, 1f, false, false);
			PlaceDepthsOval(StartPositionX, StartPositionY, ModContent.TileType<OceanSand>(), ModContent.WallType<OceanSandWall>(), SizeXInt * 4, SizeYInt * 2, 1f, true, false);
			DigOutCaves(StartPositionX, StartPositionY, SizeX, SizeY);
			PlaceBiomassClumps(StartPositionX, StartPositionY, SizeX, SizeY);
			BiomePolish(StartPositionX, StartPositionY, SizeX, SizeY);
			PlaceStructures(StartPositionX, StartPositionY, SizeX, SizeY);
			TileSloping(StartPositionX, StartPositionY, SizeX, SizeY);
			PlaceAmbience(StartPositionX, StartPositionY, SizeX, SizeY);
		}

		public static void PlaceDepthsOval(int X, int Y, int tileType, int wallType, int radius, int radiusY, float thickMult, bool Walls, bool ReplaceOnly)
		{
			int Seed = WorldGen.genRand.Next();

			float scale = radiusY / (float)radius;
			float invertScale = (float)radius / radiusY;
			for (int x = -radius; x <= radius; x++)
			{
				for (float y = -radius; y <= radius; y += (invertScale * 0.85f))
				{
					float radialMod = WorldGen.genRand.NextFloat(2.5f, 4.5f) * thickMult;
					if (Math.Sqrt(x * x + y * y) <= radius + 0.5)
					{
						int PositionX = X + x;
						int PositionY = Y + (int)(y * scale);
						
						if (WorldGen.InWorld(PositionX, PositionY, 10))
						{
							Tile tile = Framing.GetTileSafely(PositionX, PositionY);

							if (!Main.tileDungeon[tile.TileType] && !Main.wallDungeon[tile.WallType])
							{
								if (ReplaceOnly)
								{
									if (tile.HasTile)
									{
										WorldGen.KillTile(PositionX, PositionY);
										tile.TileType = (ushort)tileType;
										tile.HasTile = true;
									}
								}
								else
								{
									WorldGen.KillTile(PositionX, PositionY);
									tile.TileType = (ushort)tileType;
									tile.HasTile = true;
								}

								WorldGen.KillWall(PositionX, PositionY);

								if (Walls)
								{
									float horizontalOffsetNoise = SpookyWorldMethods.PerlinNoise2D(PositionX / 80f, PositionY / 80f, 5, unchecked(Seed + 1)) * 0.01f;
									float cavePerlinValue = SpookyWorldMethods.PerlinNoise2D(PositionX / 300f, PositionY / 1000f, 5, Seed) + 0.5f + horizontalOffsetNoise;
									float cavePerlinValue2 = SpookyWorldMethods.PerlinNoise2D(PositionX / 300f, PositionY / 1000f, 5, unchecked(Seed - 1)) + 0.5f;
									float caveNoiseMap = (cavePerlinValue + cavePerlinValue2) * 0.5f;
									float caveCreationThreshold = horizontalOffsetNoise * 3.5f + 0.235f;

									if (caveNoiseMap * caveNoiseMap > caveCreationThreshold)
									{
										tile.WallType = (ushort)wallType;
										WorldGen.PlaceWall(PositionX, PositionY, (ushort)wallType);
									}
								}
							}
						}
					}
				}
			}
		}

		//place clumps of zombie biomass and flesh
		public void PlaceBiomassClumps(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int j = (int)Main.worldSurface; j < PositionY + SizeY * 4; j++)
			{
				for (int i = PositionX - SizeX * 4; i < PositionX + SizeX * 4; i++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if (WorldGen.genRand.NextBool(65) && CanPlaceBiomass(i, j, 7) && Main.tile[i, j].TileType == ModContent.TileType<OceanSand>())
						{
							WorldGen.TileRunner(i, j, WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), ModContent.TileType<OceanMeat>());
						}

						if (WorldGen.genRand.NextBool(4) && CanPlaceBiomass(i, j, 2) && Main.tile[i, j].TileType == ModContent.TileType<OceanSand>() && !Main.tile[i, j - 3].HasTile)
						{
							SpookyWorldMethods.PlaceOval(i, j, ModContent.TileType<OceanBiomass>(), 0, 6, 4, 1f, true, false);
							SpookyWorldMethods.PlaceOval(i, j - 3, -1, ModContent.WallType<OceanBiomassWall>(), 8, 5, 1f, false, false);
						}
					}
				}
			}
		}

		//dig out caverns inside of the area of ovals
		public void DigOutCaves(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			int StartX = PositionX > (Main.maxTilesX / 2) ? PositionX - SizeX * 4 : PositionX + SizeX * 4;
			int EndX = PositionX > (Main.maxTilesX / 2) ? PositionX + SizeX * 4 : PositionX - SizeX * 4;
			int Increment = PositionX > (Main.maxTilesX / 2) ? 1 : -1;

			for (int j = PositionY - SizeY * 4; j < PositionY + SizeY * 4; j += 10)
			{
				for (int i = StartX; PositionX > (Main.maxTilesX / 2) ? i < EndX : i > EndX; i += Increment)
				{
					float RandomCaveDistance = 50; //WorldGen.genRand.Next(50, 75);
					int Distance = 27;

					if (WorldGen.InWorld(i, j, 10))
					{
						Tile tile = Framing.GetTileSafely(i, j);

						if (!BlockTypes.Contains(tile.TileType))
						{
							continue;
						}

						bool DontPlace = false;
						for (int x = i - Distance; x <= i + Distance; x++)
						{
							if (!BlockTypes.Contains(Framing.GetTileSafely(x, j).TileType))
							{
								DontPlace = true;
								break;
							}
						}
						for (int y = j - Distance; y <= j + Distance; y++)
						{
							if (!BlockTypes.Contains(Framing.GetTileSafely(i, y).TileType))
							{
								DontPlace = true;
								break;
							}
						}
						if (DontPlace)
						{
							continue;
						}

						//too close to other points
						Vector2 PositionToCheck = new Vector2(i, j);
						bool tooClose = false;

						foreach (var ExistingPosition in Flags.ZombieBiomePositions)
						{
							if (Vector2.DistanceSquared(PositionToCheck, ExistingPosition) < RandomCaveDistance * RandomCaveDistance)
							{
								tooClose = true;
								break;
							}
						}

						if (tooClose)
						{
							continue;
						}

						if (WorldGen.genRand.NextBool())
						{
							int OvalSizeX = WorldGen.genRand.Next(12, 19);
							int OvalSizeY = WorldGen.genRand.Next(8, 16);

							int YOffset = WorldGen.genRand.Next(-15, 16);

							SpookyWorldMethods.PlaceOval(i, j + YOffset, -1, 0, OvalSizeX, OvalSizeY, 1f, true, false);

							Flags.ZombieBiomePositions.Add(new Vector2(i, j + YOffset));
						}
					}
				}
			}

			for (int i = 0; i < Flags.ZombieBiomePositions.Count; i++)
			{
				if (i < Flags.ZombieBiomePositions.Count - 1)
				{
					ConnectCavePoints(Flags.ZombieBiomePositions[i], Flags.ZombieBiomePositions[i + 1], 5, false);
				}
			}

			for (int i = 0; i < Flags.ZombieBiomePositions.Count; i++)
			{
				if (WorldGen.genRand.NextBool(3))
				{
					int MinDistanceIndex = GetClosestNodeIndex(Flags.ZombieBiomePositions[i]);

					ConnectCavePoints(Flags.ZombieBiomePositions[i], Flags.ZombieBiomePositions[MinDistanceIndex], 5, false);
				}
			}

			bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
				int OceanTopX = StartPositionX;
				int OceanTopY = 10;

				while (!WorldGen.SolidTile(OceanTopX, OceanTopY) && OceanTopY <= Main.worldSurface)
				{
					OceanTopY++;
				}
				if (WorldGen.SolidTile(OceanTopX, OceanTopY))
				{
					OceanTopY += 3;
					ConnectCavePoints(Flags.ZombieBiomePositions[1], new Vector2(OceanTopX, OceanTopY), 5, true);

					placed = true;
				}
			}
		}

		public int GetClosestNodeIndex(Vector2 Position)
		{
			BiomePositionDistances.Clear();

			//get the distance between the player and every position in the zombie biome and add them to the position distances list
			foreach (Vector2 pos in Flags.ZombieBiomePositions)
			{
				float Dist = Vector2.Distance(pos, Position);

				if (Collision.CanHitLine(pos * 16 - new Vector2(10, 10), 20, 20, Position * 16 - new Vector2(10, 10), 20, 20))
				{
					Dist = 0;
				}

				BiomePositionDistances.Add((int)Dist);
			}

			//get the index of the minimum value in the array that isnt zero
			int minimumValueIndex = BiomePositionDistances.IndexOf(BiomePositionDistances.Where(x => x > 0 && x < 1000).Min());

			return minimumValueIndex;
		}

		public void ConnectCavePoints(Vector2 Start, Vector2 End, int Size, bool GoingToSurface)
		{
			int segments = 200;

			Vector2 myCenter = Start;
			Vector2 p0 = End;
			Vector2 p1 = End;
			Vector2 p2 = myCenter;
			Vector2 p3 = myCenter;

			if (!Collision.CanHitLine(End * 16 - new Vector2(10, 10), 20, 20, Start * 16 - new Vector2(10, 10), 20, 20))
			{
				if (GoingToSurface)
				{
					for (int i = 0; i < segments; i++)
					{
						float t = i / (float)segments;
						Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
						t = (i + 1) / (float)segments;

						if (Main.tile[(int)Position.X, (int)Position.Y].HasTile)
						{
							PlaceDepthsOval((int)Position.X, (int)Position.Y + WorldGen.genRand.Next(-2, 3), ModContent.TileType<OceanSand>(), ModContent.WallType<OceanSandWall>(), 9, 9, 1f, false, true);
						}
					}
				}

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;

					WorldGen.digTunnel((int)Position.X, (int)Position.Y + WorldGen.genRand.Next(-1, 2), default, default, WorldGen.genRand.Next(1, 4), Size, true);
				}
			}
		}

		public void PlaceAmbience(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int j = PositionY - SizeY * 4; j < PositionY + SizeY * 4; j++)
			{
				for (int i = PositionX - SizeX * 4; i < PositionX + SizeX * 4; i++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						Tile tile = Main.tile[i, j];
						Tile tileAbove = Main.tile[i, j - 1];
						Tile tileBelow = Main.tile[i, j + 1];

						//place skulls on any tiles
						if ((Main.tile[i, j].TileType == ModContent.TileType<OceanSand>() || Main.tile[i, j].TileType == ModContent.TileType<OceanBiomass>() || Main.tile[i, j].TileType == ModContent.TileType<OceanMeat>()) && !tileAbove.HasTile)
						{
							if (WorldGen.genRand.NextBool(12))
							{
								ushort[] Skulls = new ushort[] { (ushort)ModContent.TileType<FishFossil1>(), (ushort)ModContent.TileType<FishFossil2>(), 
								(ushort)ModContent.TileType<FishFossil3>(), (ushort)ModContent.TileType<FishFossil4>(), (ushort)ModContent.TileType<FishFossil5>() };

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Skulls));
							}
						}

						//floor tiles
						if (Main.tile[i, j].TileType == ModContent.TileType<OceanSand>() && !tileAbove.HasTile)
						{
							//light plants
							if (WorldGen.genRand.NextBool(22))
							{
								ushort[] LightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlant1>(), (ushort)ModContent.TileType<LightPlant2>(), (ushort)ModContent.TileType<LightPlant3>() };

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(LightPlants));
							}

							//big light plants
							if (WorldGen.genRand.NextBool(30))
							{
								ushort[] BigLightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlantBig1>(), (ushort)ModContent.TileType<LightPlantBig2>(), 
								(ushort)ModContent.TileType<LightPlantBig3>(), (ushort)ModContent.TileType<LightPlantBig4>() };

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(BigLightPlants));
							}

							//corals
							if (WorldGen.genRand.NextBool(6))
							{
								ushort[] Corals = new ushort[] { (ushort)ModContent.TileType<CoralGreen1>(), (ushort)ModContent.TileType<CoralGreen2>(), (ushort)ModContent.TileType<CoralGreen3>(),
								(ushort)ModContent.TileType<CoralPurple1>(), (ushort)ModContent.TileType<CoralPurple2>(), (ushort)ModContent.TileType<CoralPurple3>(),
								(ushort)ModContent.TileType<CoralRed1>(), (ushort)ModContent.TileType<CoralRed2>(), (ushort)ModContent.TileType<CoralRed3>(),
								(ushort)ModContent.TileType<CoralYellow1>(), (ushort)ModContent.TileType<CoralYellow2>(), (ushort)ModContent.TileType<CoralYellow3>()};

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Corals));
							}

							//grow weeds
							if (WorldGen.genRand.NextBool())
							{
								WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<OceanWeeds>(), true, WorldGen.genRand.Next(0, 12));
							}
						}

						//ceiling tiles
						if (Main.tile[i, j].TileType == ModContent.TileType<OceanSand>() && !tileBelow.HasTile)
						{
							//hanging light plants
							if (WorldGen.genRand.NextBool(12))
							{
								ushort[] HangingLightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlantHanging1>(), (ushort)ModContent.TileType<LightPlantHanging2>(), (ushort)ModContent.TileType<LightPlantHanging3>() };

								WorldGen.PlaceObject(i, j + 1, WorldGen.genRand.Next(HangingLightPlants));
							}

							if (WorldGen.genRand.NextBool(3))
							{
								WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<OceanVines>());
							}
						}

						//grow vines
						if (Main.tile[i, j].TileType == ModContent.TileType<OceanVines>())
						{
							int[] ValidTiles = { ModContent.TileType<OceanSand>() };

							SpookyWorldMethods.PlaceVines(i, j, ModContent.TileType<OceanVines>(), ValidTiles);
						}

						//zombie tiles
						if (Main.tile[i, j].TileType == ModContent.TileType<OceanBiomass>() && !tileAbove.HasTile)
						{
							//brains
							if (WorldGen.genRand.NextBool(5))
							{
								ushort[] Brains = new ushort[] { (ushort)ModContent.TileType<Brain1>(), (ushort)ModContent.TileType<Brain2>(),
								(ushort)ModContent.TileType<Brain3>(), (ushort)ModContent.TileType<Brain4>() };

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Brains));
							}

							//fingers
							if (WorldGen.genRand.NextBool(5))
							{
								ushort[] Fingers = new ushort[] { (ushort)ModContent.TileType<ZombieFinger1>(), (ushort)ModContent.TileType<ZombieFinger2>() };

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(Fingers));
							}

							//zomboid piles
							if (WorldGen.genRand.NextBool())
							{
								WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<ZombiePiles>(), true, WorldGen.genRand.Next(0, 9));
							}
						}
					}
				}
			}
		}

		public void BiomePolish(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int j = PositionY - SizeY * 4; j < PositionY + SizeY * 4; j++)
			{
				for (int i = PositionX - SizeX * 4; i < PositionX + SizeX * 4; i++)
				{
					if (WorldGen.InWorld(i, j, 10))
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

						Tile tile = Main.tile[i, j];

						if (WallTypes.Contains(tile.WallType))
						{
							tile.LiquidType = LiquidID.Water;
							tile.LiquidAmount = 255;
						}
					}
				}
			}

			int cutoffLimit = 65;

			void getAttachedPoints(int x, int y, List<Point> points)
			{
				if (!WorldGen.InWorld(x, y, 10))
				{
					return;
				}

				Tile tile = Main.tile[x, y];
				Point point = new(x, y);

				if (!BlockTypes.Contains(tile.TileType) || !tile.HasTile || points.Count > cutoffLimit || points.Contains(point))
				{
					return;
				}

				points.Add(point);

				getAttachedPoints(x + 1, y, points);
				getAttachedPoints(x - 1, y, points);
				getAttachedPoints(x, y + 1, points);
				getAttachedPoints(x, y - 1, points);
			}

			for (int i = PositionX - SizeX * 4; i < PositionX + SizeX * 4; i++)
			{
				for (int j = PositionY - SizeY * 4; j < PositionY + SizeY * 4; j++)
				{
					//clean up floating clumps of tiles in the dungeon
					List<Point> chunkPoints = new();
					getAttachedPoints(i, j, chunkPoints);

					if (WorldGen.InWorld(i, j, 10) && chunkPoints.Count >= 1 && chunkPoints.Count < cutoffLimit)
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

		public void TileSloping(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int j = PositionY - SizeY * 4; j < PositionY + SizeY * 4; j++)
			{
				for (int i = PositionX - SizeX * 4; i < PositionX + SizeX * 4; i++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if (BlockTypes.Contains(Main.tile[i, j].TileType))
						{
							Tile.SmoothSlope(i, j);
						}
					}
				}
			}
		}

		public void PlaceStructures(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int i = PositionX - SizeX * 4; i < PositionX + SizeX * 4; i++)
			{
				for (int j = (int)Main.worldSurface; j < PositionY + SizeY * 4; j++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if (BlockTypes.Contains(Main.tile[i, j].TileType) && CanPlaceLab(i, j))
						{
							PlaceDepthsOval(i, j + 11, ModContent.TileType<OceanSand>(), ModContent.WallType<OceanSandWall>(), 13, 7, 1f, true, false);

							Vector2 LabOrigin = new Vector2(i - 11, j - 5);
							Generator.GenerateStructure("Content/Structures/ZombieOcean/OceanLab-" + WorldGen.genRand.Next(1, 7), LabOrigin.ToPoint16(), Mod);
						}
					}
				}
			}
		}

		public bool CanPlaceLab(int PositionX, int PositionY)
		{
			int numOpenSpace = 0;

			//make sure the floor is thick enough for the lab to place without it sticking out through ceilings
			for (int y = PositionY; y <= PositionY + 15; y++)
			{
				if (WorldGen.InWorld(PositionX, y, 10))
				{
					if (!Main.tile[PositionX, y].HasTile)
					{
						return false;
					}
				}
			}

			//upward check to make sure theres enough room
			for (int x = PositionX - 10; x < PositionX + 10; x++)
			{
				for (int y = PositionY - 25; y < PositionY - 3; y++)
				{
					if (WorldGen.InWorld(x, y, 10))
					{
						if (Main.tile[x, y].HasTile)
						{
							return false;
						}
					}
				}
			}

			//dont allow labs to place too close to each other
			for (int i = PositionX - 45; i < PositionX + 45; i++)
			{
				for (int j = PositionY - 45; j < PositionY + 45; j++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if (Main.tile[i, j].TileType == ModContent.TileType<LabMetalPlate>())
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		public bool CanPlaceBiomass(int PositionX, int PositionY, int Distance)
		{
			for (int i = PositionX - Distance; i <= PositionX + Distance; i++)
			{
				for (int j = PositionY - Distance; j <= PositionY + Distance; j++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if (!Main.tile[i, j].HasTile || Main.tileDungeon[Main.tile[i, j].TileType] || Main.wallDungeon[Main.tile[i, j].WallType])
						{
							return false;
						}
					}
				}
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

			tasks.Insert(GenIndex1 + 1, new PassLegacy("Rotten Depths", PlaceZombieOcean));
		}
	}
}