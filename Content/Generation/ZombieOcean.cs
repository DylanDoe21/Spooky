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
using Spooky.Content.Items.Minibiomes.Ocean;
using Spooky.Content.Tiles.Minibiomes.Ocean;
using Spooky.Content.Tiles.Minibiomes.Ocean.Ambient;
using Spooky.Content.Tiles.Minibiomes.Ocean.Furniture;
using Spooky.Content.Tiles.Minibiomes.Ocean.Tree;

namespace Spooky.Content.Generation
{
	public class ZombieOcean : ModSystem
	{
		public static List<ushort> BlockTypes = new()
		{
			(ushort)ModContent.TileType<OceanSand>(),
			(ushort)ModContent.TileType<OceanBiomass>(),
			(ushort)ModContent.TileType<OceanRock>(),
			(ushort)ModContent.TileType<LabMetalPlate>()
		};

		public static List<ushort> WallTypes = new()
		{
			(ushort)ModContent.WallType<OceanSandWall>(),
			(ushort)ModContent.WallType<OceanBiomassWall>()
		};

		List<int> BiomePositionDistances = new List<int>();

		public static int StartPositionX;
		public static int StartPositionY;

		int LabsPlaced = 0;

		private void DecideZombieOceanPosition(GenerationProgress progress, GameConfiguration configuration)
		{
			LabsPlaced = 0;

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
		}

		private void PlaceZombieOcean(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = Language.GetOrRegister("Mods.Spooky.WorldgenTasks.ZombieOcean").Value;

			StartPositionY = (int)Main.worldSurface + 120;

			Flags.ZombieBiomePositions.Clear();

			int SizeXInt = 40;
			int SizeYInt = 40;
			int SizeX = 300;
			int SizeY = 150;

			//first place a box of sand under the ocean so the rotten depths circle doesnt look out of place
			int Start = StartPositionX < Main.maxTilesX / 2 ? 10 : Main.maxTilesX - 260;
			int End = StartPositionX < Main.maxTilesX / 2 ? 260 : Main.maxTilesX - 10;

			for (int i = Start; i <= End; i++)
			{
				for (int j = 100; j <= StartPositionY; j++)
				{
					Tile tile = Framing.GetTileSafely(i, j);

					if (!Main.tileDungeon[tile.TileType] && !Main.wallDungeon[tile.WallType] && tile.TileType != TileID.ShellPile &&
					tile.TileType != TileID.BeachPiles && tile.TileType != TileID.Coral)
					{
						if (j < Main.worldSurface)
						{
							if (WorldGen.SolidTile(i, j) || tile.WallType > 0)
							{
								tile.TileType = TileID.Sand;
								tile.HasTile = true;
							}
						}
						else
						{
							tile.TileType = TileID.Sand;
							tile.HasTile = true;
						}
					}
				}
			}

			PlaceDepthsOval(StartPositionX, StartPositionY, TileID.Sand, 0, (SizeXInt + 3) * 5, (SizeYInt + 3) * 3, 1f, false, false);
			PlaceDepthsOval(StartPositionX, StartPositionY, ModContent.TileType<OceanSand>(), ModContent.WallType<OceanSandWall>(), SizeXInt * 5, SizeYInt * 3, 1f, true, false);
			progress.Set(0.5);
			DigOutCaves(StartPositionX, StartPositionY, SizeX, SizeY);
			BiomePolish(StartPositionX, StartPositionY, SizeX, SizeY);

			for (int i = 0; i < Flags.ZombieBiomePositions.Count; i++)
			{
				if (LabsPlaced < 5)
				{
					PlaceLabs((int)Flags.ZombieBiomePositions[i].X, (int)Flags.ZombieBiomePositions[i].Y);
				}
				else
				{
					break;
				}
			}

			if (LabsPlaced < 5)
			{
				for (int i = 0; i < Flags.ZombieBiomePositions.Count; i++)
				{
					if (LabsPlaced < 5)
					{
						PlaceChest((int)Flags.ZombieBiomePositions[i].X, (int)Flags.ZombieBiomePositions[i].Y);
					}
					else
					{
						break;
					}
				}
			}
			
			progress.Set(1);
			TileSloping(StartPositionX, StartPositionY, SizeX, SizeY);
			PlaceAmbience(StartPositionX, StartPositionY, SizeX, SizeY);
		}

		private void PlaceSurfaceLab(GenerationProgress progress, GameConfiguration configuration)
		{
			//place surface lab
			bool placedSurfaceLab = false;
            int surfaceLabAttempts = 0;
            while (!placedSurfaceLab && surfaceLabAttempts++ < 100000)
            {
				int LabX = StartPositionX + (StartPositionX > (Main.maxTilesX / 2) ? -100 : 100);
				int LabY = 10;

				while (!WorldGen.SolidTile(LabX, LabY) && LabY <= Main.worldSurface)
				{
					LabY++;
				}
				if (WorldGen.SolidTile(LabX, LabY))
				{
					Vector2 LabOrigin = new Vector2(LabX - 11, LabY - 35);
					StructureHelper.API.Generator.GenerateStructure("Content/Structures/ZombieOcean/SurfaceLab.shstruct", LabOrigin.ToPoint16(), Mod);

					placedSurfaceLab = true;
				}
			}
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
								if (tileType != -1)
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
								}
								else
								{
									WorldGen.KillTile(PositionX, PositionY);
									tile.HasTile = false;
									tile.LiquidType = LiquidID.Water;
									tile.LiquidAmount = 255;
								}

								if (Walls)
								{
									WorldGen.KillWall(PositionX, PositionY);

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

		//dig out caverns inside of the area of ovals
		public void DigOutCaves(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			int StartX = PositionX > (Main.maxTilesX / 2) ? PositionX - SizeX : PositionX + SizeX;
			int EndX = PositionX > (Main.maxTilesX / 2) ? PositionX + SizeX : PositionX - SizeX;
			int Increment = PositionX > (Main.maxTilesX / 2) ? 5 : -5;

			float MinDistanceBetweenCaves = 60;
			int ValidTileCheckDistance = 35;

			for (int j = PositionY - SizeY; j < PositionY + SizeY; j += 5)
			{
				for (int i = StartX; PositionX > (Main.maxTilesX / 2) ? i < EndX : i > EndX; i += Increment)
				{
					if (WorldGen.InWorld(i, j, 40))
					{
						Tile tile = Framing.GetTileSafely(i, j);

						if (!BlockTypes.Contains(tile.TileType))
						{
							continue;
						}

						bool DontPlace = false;
						for (int x = i - ValidTileCheckDistance; x <= i + ValidTileCheckDistance; x++)
						{
							if (!BlockTypes.Contains(Framing.GetTileSafely(x, j).TileType))
							{
								DontPlace = true;
								break;
							}
						}
						for (int y = j - ValidTileCheckDistance; y <= j + ValidTileCheckDistance; y++)
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
							if (Vector2.DistanceSquared(PositionToCheck, ExistingPosition) < MinDistanceBetweenCaves * MinDistanceBetweenCaves)
							{
								tooClose = true;
								break;
							}
						}

						if (tooClose)
						{
							continue;
						}

						if (WorldGen.InWorld(i, j, 80) && NoDungeonBlocksNearby(i, j, 20, false))
						{
							int OvalSizeX = WorldGen.genRand.Next(16, 19);
							int OvalSizeY = WorldGen.genRand.Next(8, 14);

							int YOffset = WorldGen.genRand.Next(-10, 11);

							SpookyWorldMethods.PlaceOval(i, j + YOffset, -1, 0, OvalSizeX, OvalSizeY, 1f, true, false);
							PlaceDepthsOval(i, j + YOffset, ModContent.TileType<OceanSand>(), ModContent.WallType<OceanSandWall>(), OvalSizeX + 2, OvalSizeY + 2, 1f, true, true);

							Flags.ZombieBiomePositions.Add(new Vector2(i, j + YOffset));
						}
					}
				}
			}

			//connect all cave points in the biome in the order they were placed
			for (int i = 0; i < Flags.ZombieBiomePositions.Count; i++)
			{
				if (i < Flags.ZombieBiomePositions.Count - 1)
				{
					ConnectCavePoints(Flags.ZombieBiomePositions[i], Flags.ZombieBiomePositions[i + 1], false);
				}
			}

			//place randomized tunnels between a chosen cave point and the closest cave point to it
			for (int i = 0; i < Flags.ZombieBiomePositions.Count; i++)
			{
				if (WorldGen.genRand.NextBool(2))
				{
					int MinDistanceIndex = GetClosestNodeIndex(Flags.ZombieBiomePositions[i]);

					ConnectCavePoints(Flags.ZombieBiomePositions[i], Flags.ZombieBiomePositions[MinDistanceIndex], false);
				}
			}

			//place layer of marine rock to prevent breaking through the biome
			for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
			{
				for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
				{
					if (WorldGen.InWorld(i, j))
					{
						if (BlockTypes.Contains(Main.tile[i, j].TileType) && CanPlaceRock(i, j, 2))
						{
							Main.tile[i, j].TileType = (ushort)ModContent.TileType<OceanRock>();
						}
					}
				}
			}

			//dig tunnel from surface to rotten depths
			bool placed = false;
            int attempts = 0;
            while (!placed && attempts++ < 100000)
            {
				int TunnelX = StartPositionX; // + (StartPositionX < (Main.maxTilesX / 2) ? 0 : 0);
				int TunnelY = 10;

				while (!WorldGen.SolidTile(TunnelX, TunnelY) && TunnelY <= Main.worldSurface)
				{
					TunnelY++;
				}
				if (WorldGen.SolidTile(TunnelX, TunnelY))
				{
					TunnelY += 3;

					int MinDistanceIndex = GetClosestNodeIndex(new Vector2(TunnelX, TunnelY));
					ConnectCavePoints(Flags.ZombieBiomePositions[MinDistanceIndex], new Vector2(TunnelX, TunnelY), true);

					placed = true;
				}
			}
		}

		public int GetClosestNodeIndex(Vector2 Position)
		{
			BiomePositionDistances.Clear();

			foreach (Vector2 pos in Flags.ZombieBiomePositions)
			{
				float Dist = Vector2.Distance(pos, Position);

				if (Collision.CanHitLine(pos * 16 - new Vector2(1, 1), 2, 2, Position * 16 - new Vector2(1, 1), 2, 2))
				{
					Dist = 0;
				}

				BiomePositionDistances.Add((int)Dist);
			}

			//get the index of the minimum value in the array that isnt zero
			int minimumValueIndex = BiomePositionDistances.IndexOf(BiomePositionDistances.Where(x => x > 0 && x < 1000).Min());

			return minimumValueIndex;
		}

		public void ConnectCavePoints(Vector2 Start, Vector2 End, bool GoingToSurface)
		{
			int segments = 100;

			Vector2 myCenter = Start;
			Vector2 p0 = End;
			Vector2 p1 = End;
			Vector2 p2 = myCenter;
			Vector2 p3 = myCenter;

			if (!Collision.CanHitLine(End * 16 - new Vector2(1, 1), 2, 2, Start * 16 - new Vector2(1, 1), 2, 2))
			{
				if (GoingToSurface)
				{
					for (int i = 0; i < segments; i++)
					{
						float t = i / (float)segments;
						Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
						t = (i + 1) / (float)segments;

						if (i % 3 == 0 && NoDungeonBlocksNearby((int)Position.X, (int)Position.Y, 6, false))
						{
							if (CanPlaceCave((int)Position.X, (int)Position.Y, 12))
							{
								int Size = WorldGen.genRand.Next(9, 11); //tragic random numbering
								PlaceDepthsOval((int)Position.X, (int)Position.Y, ModContent.TileType<OceanSand>(), ModContent.WallType<OceanSandWall>(), Size, Size, 1f, true, true);
							}
						}
					}
				}

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 Position = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;

					if (i % 3 == 0 && NoDungeonBlocksNearby((int)Position.X, (int)Position.Y, 6, false))
					{
						if (CanPlaceCave((int)Position.X, (int)Position.Y, 12))
						{
							int Size = GoingToSurface ? WorldGen.genRand.Next(5, 7) : WorldGen.genRand.Next(7, 9);
							PlaceDepthsOval((int)Position.X, (int)Position.Y, -1, 0, Size, Size, 1f, false, false);
						}
					}
				}
			}
		}

		public bool CanPlaceCave(int PositionX, int PositionY, int Size)
		{
			int numTiles = 0;

			for (int j = PositionY - Size; j < PositionY + Size; j++)
			{
				for (int i = PositionX - Size; i < PositionX + Size; i++)
				{
					if (Main.tile[i, j].HasTile)
					{
						numTiles++;
					}
				}
			}

			float AmountOfTilesNeeded = (Size * Size) / 1.1f;

			if (numTiles > (int)AmountOfTilesNeeded)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void BiomePolish(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			//place zombie biomass clumps on the floor
			for (int j = (int)Main.worldSurface; j < PositionY + SizeY; j++)
			{
				for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if (WorldGen.genRand.NextBool(20) && NoDungeonBlocksNearby(i, j, 2, false) && Main.tile[i, j].TileType == ModContent.TileType<OceanSand>() && !Main.tile[i, j - 1].HasTile)
						{
							SpookyWorldMethods.PlaceOval(i, j, ModContent.TileType<OceanBiomass>(), 0, 6, 4, 1f, true, false);
							SpookyWorldMethods.PlaceOval(i, j - 1, -1, ModContent.WallType<OceanBiomassWall>(), 8, 5, 1f, false, false);
						}
					}
				}
			}

			//place more water in the biome to fill it up more and so it doesnt drain the entire ocean
			for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
			{
				for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						Tile tile = Main.tile[i, j];

						if (WallTypes.Contains(tile.WallType))
						{
							tile.LiquidType = LiquidID.Water;
							tile.LiquidAmount = 255;
						}

						if (BlockTypes.Contains(Main.tile[i, j].TileType) && !Main.tile[i, j + 1].HasTile)
						{
							for (int waterY = j; waterY < j + 15; waterY++)
							{
								Main.tile[i, waterY].LiquidAmount = 255;
							}
						}
					}
				}
			}

			for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
			{
				for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
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

						//kill one block thick tiles
						if ((BlockTypes.Contains(Main.tile[i, j].TileType) && !Main.tile[i, j - 1].HasTile && !Main.tile[i, j + 1].HasTile) ||
						(BlockTypes.Contains(Main.tile[i, j].TileType) && !Main.tile[i - 1, j].HasTile && !Main.tile[i + 1, j].HasTile))
						{
							for (int fillX = i - 1; fillX < i + 1; fillX++)
							{
								for (int fillY = j - 1; fillY < j + 1; fillY++)
								{
									WorldGen.PlaceTile(fillX, fillY, ModContent.TileType<OceanSand>());
								}
							}
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

			//clean out small clumps of blocks in the biome
			int cutoffLimit = 150;

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

			void getAttachedWallPoints(int x, int y, List<Point> points)
			{
				if (!WorldGen.InWorld(x, y, 10))
				{
					return;
				}

				Tile tile = Main.tile[x, y];
				Point point = new(x, y);

				if (!WallTypes.Contains(tile.WallType) || points.Count > cutoffLimit || points.Contains(point))
				{
					return;
				}

				points.Add(point);

				getAttachedWallPoints(x + 1, y, points);
				getAttachedWallPoints(x - 1, y, points);
				getAttachedWallPoints(x, y + 1, points);
				getAttachedWallPoints(x, y - 1, points);
			}

			for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
			{
				for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
				{
					//clean up floating clumps of tiles in the dungeon
					List<Point> chunkPoints = new();
					getAttachedPoints(i, j, chunkPoints);

					if (WorldGen.InWorld(i, j, 10) && chunkPoints.Count >= 1 && chunkPoints.Count <= cutoffLimit)
					{
						foreach (Point p in chunkPoints)
						{
							WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
							{
								new Actions.ClearTile(true)
							}));
						}
					}

					//clean up floating clumps of tiles in the dungeon
					List<Point> WallPoints = new();
					getAttachedWallPoints(i, j, WallPoints);

					if (WorldGen.InWorld(i, j, 10) && WallPoints.Count >= 1 && WallPoints.Count <= cutoffLimit)
					{
						foreach (Point p in WallPoints)
						{
							WorldUtils.Gen(p, new Shapes.Rectangle(1, 1), Actions.Chain(new GenAction[]
							{
								new Actions.ClearWall(true)
							}));
						}
					}
				}
			}
		}

		public void TileSloping(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			for (int j = PositionY - SizeY - 100; j < PositionY + SizeY; j++)
			{
				for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
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

		public void PlaceAmbience(int PositionX, int PositionY, int SizeX, int SizeY)
		{
			//place flesh vents first so that tube worms can be placed nearby
			for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
			{
				for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
				{
					if (WorldGen.InWorld(i, j, 25))
					{
						Tile tile = Main.tile[i, j];
						Tile tileAbove = Main.tile[i, j - 1];
						Tile tileBelow = Main.tile[i, j + 1];

						if ((Main.tile[i, j].TileType == ModContent.TileType<OceanSand>() || Main.tile[i, j].TileType == ModContent.TileType<OceanBiomass>()) && !tileAbove.HasTile)
						{
							if (WorldGen.genRand.NextBool(32))
							{
								WorldGen.PlaceObject(i, j - 1, ModContent.TileType<FleshVent>());
							}
						}
					}
				}
			}

			for (int j = PositionY - SizeY; j < PositionY + SizeY; j++)
			{
				for (int i = PositionX - SizeX; i < PositionX + SizeX; i++)
				{
					if (WorldGen.InWorld(i, j, 25))
					{
						Tile tile = Main.tile[i, j];
						Tile tileAbove = Main.tile[i, j - 1];
						Tile tileBelow = Main.tile[i, j + 1];

						//place skulls on any tiles
						if ((Main.tile[i, j].TileType == ModContent.TileType<OceanSand>() || Main.tile[i, j].TileType == ModContent.TileType<OceanBiomass>()) && !tileAbove.HasTile)
						{
							if (WorldGen.genRand.NextBool() && CanPlaceTubeWorm(i, j))
							{
								if (!Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
								{
									TubeWorm.Grow(i, j - 1, 3, 7);
								}
							}

							if (WorldGen.genRand.NextBool(9) && CanPlaceFishTree(i, j))
							{
								if (!Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
								{
									BoneFishTree.Grow(i, j - 1, 2, 8);

									if (Main.tile[i, j - 1].TileType == ModContent.TileType<BoneFishTree>())
									{
										TryToPlaceHangingFishBone(i, j);
									}
								}
							}

							if (WorldGen.genRand.NextBool(15))
							{
								WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<LockerTile>());
							}

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
							//big light plants
							if (WorldGen.genRand.NextBool(15))
							{
								ushort[] BigLightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlantBig1>(), (ushort)ModContent.TileType<LightPlantBig2>(), 
								(ushort)ModContent.TileType<LightPlantBig3>(), (ushort)ModContent.TileType<LightPlantBig4>() };

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(BigLightPlants));
							}

							//light plants
							if (WorldGen.genRand.NextBool(8))
							{
								ushort[] LightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlant1>(), (ushort)ModContent.TileType<LightPlant2>(), (ushort)ModContent.TileType<LightPlant3>() };

								WorldGen.PlaceObject(i, j - 1, WorldGen.genRand.Next(LightPlants));
							}

							//corals
							if (WorldGen.genRand.NextBool())
							{
								ushort[] Corals = new ushort[] { (ushort)ModContent.TileType<CoralGreen1>(), (ushort)ModContent.TileType<CoralGreen2>(), (ushort)ModContent.TileType<CoralGreen3>(),
								(ushort)ModContent.TileType<CoralPurple1>(), (ushort)ModContent.TileType<CoralPurple2>(), (ushort)ModContent.TileType<CoralPurple3>(),
								(ushort)ModContent.TileType<CoralRed1>(), (ushort)ModContent.TileType<CoralRed2>(), (ushort)ModContent.TileType<CoralRed3>(),
								(ushort)ModContent.TileType<CoralYellow1>(), (ushort)ModContent.TileType<CoralYellow2>(), (ushort)ModContent.TileType<CoralYellow3>() };

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
							if (WorldGen.genRand.NextBool(6))
							{
								ushort[] HangingLightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlantHanging1>(), (ushort)ModContent.TileType<LightPlantHanging2>(), (ushort)ModContent.TileType<LightPlantHanging3>() };

								WorldGen.PlaceObject(i, j + 1, WorldGen.genRand.Next(HangingLightPlants));
							}

							if (WorldGen.genRand.NextBool())
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

		public void PlaceLabs(int PositionX, int PositionY)
		{
			if (WorldGen.InWorld(PositionX, PositionY, 10))
			{
				bool foundSpot = false;
				int attempts = 0;
				while (!foundSpot && attempts++ < 100000)
				{
					while (!WorldGen.SolidTile(PositionX, PositionY) && PositionY <= (Main.maxTilesY / 2))
					{
						PositionY++;
					}
					if (WorldGen.SolidTile(PositionX, PositionY))
					{
						foundSpot = true;
					}
				}
				
				if (CanPlaceLab(PositionX, PositionY))
				{
					Vector2 LabOrigin = new Vector2(PositionX - 16, PositionY - 4);
					StructureHelper.API.Generator.GenerateStructure("Content/Structures/ZombieOcean/Lab-" + WorldGen.genRand.Next(1, 5) + ".shstruct", LabOrigin.ToPoint16(), Mod);

					LabsPlaced++;
				}
			}
		}

		//for if not enough labs generate
		public void PlaceChest(int PositionX, int PositionY)
		{
			if (WorldGen.InWorld(PositionX, PositionY, 10))
			{
				bool foundSpot = false;
				int attempts = 0;
				while (!foundSpot && attempts++ < 100000)
				{
					while (!WorldGen.SolidTile(PositionX, PositionY) && PositionY <= (Main.maxTilesY / 2))
					{
						PositionY++;
					}
					if (WorldGen.SolidTile(PositionX, PositionY))
					{
						foundSpot = true;
					}
				}
				
				if (CanPlaceLab(PositionX, PositionY, true))
				{
					Vector2 LabOrigin = new Vector2(PositionX - 6, PositionY - 6);
					StructureHelper.API.Generator.GenerateStructure("Content/Structures/ZombieOcean/LabChest.shstruct", LabOrigin.ToPoint16(), Mod);

					LabsPlaced++;
				}
			}
		}

		public bool CanPlaceLab(int PositionX, int PositionY, bool ChestOnly = false)
		{
			if (!ChestOnly)
			{
				int numTiles = 0;

				//downward box check to make sure theres enough solid floor
				for (int x = PositionX - 10; x < PositionX + 10; x++)
				{
					for (int y = PositionY; y < PositionY + 30; y++)
					{
						if (WorldGen.InWorld(x, y, 10))
						{
							if (Main.tile[x, y].HasTile)
							{
								numTiles++;
							}
						}
					}
				}

				//make sure the floor is thick enough for the lab to place without it sticking out through ceilings
				for (int y = PositionY; y <= PositionY + 18; y++)
				{
					if (WorldGen.InWorld(PositionX, y, 10))
					{
						if (!Main.tile[PositionX, y].HasTile)
						{
							return false;
						}
					}
				}

				//dont allow labs to place too close to each other
				for (int i = PositionX - 50; i < PositionX + 50; i++)
				{
					for (int j = PositionY - 35; j < PositionY + 35; j++)
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

				return numTiles >= 575;
			}
			else
			{
				//make sure the floor is thick enough for the lab to place without it sticking out through ceilings
				for (int i = PositionX - 4; i < PositionX + 4; i++)
				{
					for (int j = PositionY; j < PositionY + 5; j++)
					{
						if (WorldGen.InWorld(i, j, 10))
						{
							if (!Main.tile[i, j].HasTile)
							{
								return false;
							}
						}
					}
				}

				//dont allow labs to place too close to each other
				for (int i = PositionX - 25; i < PositionX + 25; i++)
				{
					for (int j = PositionY - 25; j < PositionY + 25; j++)
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
		}

		public static bool CanPlaceTubeWorm(int X, int Y)
		{
			int numFleshVent = 0;

			//only allow them to place nearby thermal vents
			for (int i = X - 10; i <= X + 10; i++)
			{
				for (int j = Y - 5; j <= Y + 5; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<FleshVent>())
					{
						numFleshVent++;
					}
				}
			}

			//dont allow tube worms to place too close to each other
			for (int i = X - 1; i <= X + 1; i++)
			{
				for (int j = Y - 1; j <= Y + 1; j++)
				{
					if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == ModContent.TileType<TubeWorm>() || Main.tile[i, j].TileType == ModContent.TileType<FleshVent>()))
					{
						return false;
					}
				}
			}

			return numFleshVent > 0;
		}

		public static bool CanPlaceFishTree(int X, int Y)
		{
			int numFleshVent = 0;

			for (int i = X - 12; i < X + 12; i++)
			{
				for (int j = Y - 5; j < Y + 5; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<FleshVent>())
					{
						numFleshVent++;
					}
				}
			}

			for (int i = X - 5; i < X + 5; i++)
			{
				for (int j = Y - 5; j < Y + 5; j++)
				{
					if (Main.tile[i, j].HasTile && (Main.tile[i, j].TileType == ModContent.TileType<BoneFishTree>() || Main.tile[i, j].TileType == ModContent.TileType<TubeWorm>()))
					{
						return false;
					}
				}
			}

			return numFleshVent <= 0;
		}

		public static bool TryToPlaceHangingFishBone(int X, int Y)
		{
			for (int j = Y; j < Y + 35; j++)
			{
				if ((Main.tile[X, j].TileType == ModContent.TileType<OceanSand>() || Main.tile[X, j].TileType == ModContent.TileType<OceanBiomass>()) && !Main.tile[X, j + 1].HasTile)
				{
					if (!Main.tile[X, j].LeftSlope && !Main.tile[X, j].RightSlope && !Main.tile[X, j].IsHalfBlock)
					{
						BoneFishTreeHanging.Grow(X, j + 1, 2, 8);
						return true;
					}
				}
			}

			return false;
		}

		public bool NoDungeonBlocksNearby(int PositionX, int PositionY, int Distance, bool DoEmptyTileCheck)
		{
			for (int i = PositionX - Distance; i <= PositionX + Distance; i++)
			{
				for (int j = PositionY - Distance; j <= PositionY + Distance; j++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if ((DoEmptyTileCheck && !Main.tile[i, j].HasTile) || Main.tileDungeon[Main.tile[i, j].TileType] || Main.wallDungeon[Main.tile[i, j].WallType])
						{
							return false;
						}
					}
				}
			}

			return true;
		}

		public bool CanPlaceRock(int PositionX, int PositionY, int Distance)
		{
			for (int i = PositionX - Distance; i <= PositionX + Distance; i++)
			{
				for (int j = PositionY - Distance; j <= PositionY + Distance; j++)
				{
					if (WorldGen.InWorld(i, j, 10))
					{
						if (!BlockTypes.Contains(Main.tile[i, j].TileType) && Main.tile[i, j].TileType != TileID.Sand)
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
			//Create Ocean Caves is the gen step right before shimmer
			//This is to decide the position of the biome first because the shimmer needs to be moved based on what side of the world it will generate on to prevent the shimmer from being destroyed
			int GenIndex1 = tasks.FindIndex(genpass => genpass.Name.Equals("Create Ocean Caves"));
			if (GenIndex1 == -1)
			{
				return;
			}

			tasks.Insert(GenIndex1 + 1, new PassLegacy("Rotten Depths Position", DecideZombieOceanPosition));

			int GenIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Broken Traps"));
			if (GenIndex2 == -1)
			{
				return;
			}

			tasks.Insert(GenIndex2 + 1, new PassLegacy("Rotten Depths", PlaceZombieOcean));

			int GenIndex3 = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (GenIndex3 == -1)
			{
				return;
			}

			tasks.Insert(GenIndex3 + 1, new PassLegacy("Rotten Depths Lab", PlaceSurfaceLab));
		}

		//post worldgen to place items in the spooky biome chests
        public override void PostWorldGen()
		{
			List<int> MainItem = new List<int>
			{
				ModContent.ItemType<MineDynamite>(), ModContent.ItemType<MineMetalPlates>(), ModContent.ItemType<MinePressureSensor>(), ModContent.ItemType<MineTimer>()
			};

			List<int> ActualMainItem = new List<int>(MainItem);

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

					if (chestTile.TileType == ModContent.TileType<LabSafe>())
					{
						if (ActualMainItem.Count == 0)
						{
							ActualMainItem = new List<int>(MainItem);
						}

						int ItemToPutInChest = WorldGen.genRand.Next(ActualMainItem.Count);

						//bomb material
						chest.item[0].SetDefaults(ActualMainItem[ItemToPutInChest]);
						chest.item[0].stack = 1;
						ActualMainItem.RemoveAt(ItemToPutInChest);

						if (chest.y > (int)Main.worldSurface)
						{
							//blood water gun
							chest.item[1].SetDefaults(ModContent.ItemType<BloodSoaker>());
							chest.item[1].stack = 1;
							//steroids
							chest.item[2].SetDefaults(ModContent.ItemType<UnstableSteroid>());
							chest.item[2].stack = WorldGen.genRand.Next(5, 16);
							//glowstick
							chest.item[3].SetDefaults(ItemID.Glowstick);
							chest.item[3].stack = WorldGen.genRand.Next(10, 21);
						}
						else
						{
							for (int slot = 1; slot < 5; slot++)
							{
								chest.item[slot].SetDefaults(ItemID.DivingGear);
								chest.item[slot].stack = 1;
							}
						}
					}
				}
            }
        }
	}
}