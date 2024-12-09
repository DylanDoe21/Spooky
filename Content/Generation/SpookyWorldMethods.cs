﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Generation
{
	public class SpookyWorldMethods
	{
		public static void PlaceMound(int X, int Y, int tileType, int halfWidth, int height)
		{
			ShapeData mound = new ShapeData();
			GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Mound(halfWidth, height), Actions.Chain(new GenAction[]
			{
				blotchMod.Output(mound)
			}));

			WorldUtils.Gen(new Point(X, Y), new ModShapes.All(mound), Actions.Chain(new GenAction[]
			{
				new Actions.ClearTile(), new Actions.PlaceTile((ushort)tileType)
			}));
		}

		public static void PlaceOval(int X, int Y, int tileType, int wallType, int radius, int radiusY, float thickMult, bool ReplaceOnly)
		{
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
						Tile tile = Framing.GetTileSafely(PositionX, PositionY);

						if (!ReplaceOnly)
						{
							if (tileType > -1)
							{
								WorldGen.KillTile(PositionX, PositionY);
								tile.TileType = (ushort)tileType;
								tile.HasTile = true;
							}

							if (wallType > 0)
							{
								tile.WallType = (ushort)wallType;
								tile.LiquidAmount = 0;
							}
						}
						else if (ReplaceOnly && tile.HasTile)
						{
							if (tileType > -1)
							{
								tile.TileType = (ushort)tileType;
							}
							else
							{
								WorldGen.KillTile(PositionX, PositionY);
							}

							if (wallType > 0 && tile.WallType > 0)
							{
								tile.WallType = (ushort)wallType;
								tile.LiquidAmount = 0;
							}
						}

						//if (Math.Sqrt(x * x + y * y) >= radius - radialMod)
						//{
						//}
					}
				}
			}
		}

		public static void PlaceCircle(int X, int Y, int tileType, int wallType, int radius, bool clearTiles, bool clearWalls)
		{
			ShapeData circle = new ShapeData();
			GenAction blotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius), Actions.Chain(new GenAction[]
			{
				blotchMod.Output(circle)
			}));

			//clear tiles
			if (clearTiles)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
				{
					new Actions.ClearTile(), new Actions.SetLiquid(0, 0)
				}));
			}

			//clear walls
			if (clearWalls)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
				{
					new Actions.ClearWall()
				}));
			}

			//place tiles for the circle
			if (tileType > -1)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(circle), Actions.Chain(new GenAction[]
				{
					new Actions.PlaceTile((ushort)tileType)
				}));
			}

			//wall placing stuff
			ShapeData wallCircle = new ShapeData();
			GenAction wallBlotchMod = new Modifiers.Blotches(2, 0.4);
			WorldUtils.Gen(new Point(X, Y), new Shapes.Circle(radius - 1), Actions.Chain(new GenAction[]
			{
				wallBlotchMod.Output(wallCircle)
			}));

			//dont place walls if it is not set to place any
			if (wallType > 0)
			{
				WorldUtils.Gen(new Point(X, Y), new ModShapes.All(wallCircle), Actions.Chain(new GenAction[]
				{
					new Actions.PlaceWall((ushort)wallType)
				}));
			}
		}

		public static void PlaceVines(int X, int Y, int vineType, int[] ValidTiles)
		{
			Tile tileBelow = Framing.GetTileSafely(X, Y + 1);
			if (WorldGen.genRand.NextBool() && !tileBelow.HasTile && tileBelow.LiquidAmount <= 0)
			{
				bool PlaceVine = false;
				int Test = Y;
				while (Test > Y - 10)
				{
					Tile testTile = Framing.GetTileSafely(X, Test);
					if (testTile.BottomSlope)
					{
						break;
					}
					else if (!testTile.HasTile || !ValidTiles.Contains(testTile.TileType))
					{
						Test--;
						continue;
					}
					PlaceVine = true;
					break;
				}

				if (PlaceVine)
				{
					tileBelow.TileType = (ushort)vineType;
					tileBelow.HasTile = true;
					WorldGen.SquareTileFrame(X, Y + 1, true);
				}
			}
		}

		internal static readonly List<Vector2> Directions = new List<Vector2>()
		{
			new Vector2(-1f, -1f),
			new Vector2(1f, -1f),
			new Vector2(-1f, 1f),
			new Vector2(1f, 1f),
			new Vector2(0f, -1f),
			new Vector2(-1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 0f),
		};

		public static float PerlinNoise2D(float x, float y, int octaves, int seed)
		{
			float SmoothFunction(float n) => 3f * n * n - 2f * n * n * n;
			float NoiseGradient(int s, int noiseX, int noiseY, float xd, float yd)
			{
				int hash = s;
				hash ^= 1619 * noiseX;
				hash ^= 31337 * noiseY;

				hash = hash * hash * hash * 60493;
				hash = (hash >> 13) ^ hash;

				Vector2 g = Directions[hash & 7];

				return xd * g.X + yd * g.Y;
			}

			int frequency = (int)Math.Pow(2D, octaves);
			x *= frequency;
			y *= frequency;

			int flooredX = (int)x;
			int flooredY = (int)y;
			int ceilingX = flooredX + 1;
			int ceilingY = flooredY + 1;
			float interpolatedX = x - flooredX;
			float interpolatedY = y - flooredY;
			float interpolatedX2 = interpolatedX - 1;
			float interpolatedY2 = interpolatedY - 1;

			float fadeX = SmoothFunction(interpolatedX);
			float fadeY = SmoothFunction(interpolatedY);

			float smoothX = MathHelper.Lerp(NoiseGradient(seed, flooredX, flooredY, interpolatedX, interpolatedY), NoiseGradient(seed, ceilingX, flooredY, interpolatedX2, interpolatedY), fadeX);
			float smoothY = MathHelper.Lerp(NoiseGradient(seed, flooredX, ceilingY, interpolatedX, interpolatedY2), NoiseGradient(seed, ceilingX, ceilingY, interpolatedX2, interpolatedY2), fadeX);

			return MathHelper.Lerp(smoothX, smoothY, fadeY);
		}

		//this is basically a heavily modified version of vanillas tile runner code (dreadful)
		public static void ModifiedTileRunner(int i, int j, double strength, int steps, int tileType, int wallType, bool addTile = false,
		float speedX = 0f, float speedY = 0f, bool noYChange = false, bool placeWalls = false, bool replaceWalls = true, bool noTiles = false)
		{
			double num = strength;
			float num2 = (float)steps;
			Vector2 pos;
			pos.X = (float)i;
			pos.Y = (float)j;
			Vector2 randVect;
			randVect.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			randVect.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			if (speedX != 0f || speedY != 0f)
			{
				randVect.X = speedX;
				randVect.Y = speedY;
			}

			while (num > 0.0 && num2 > 0f)
			{
				num = strength * (double)(num2 / (float)steps);
				num2 -= 1f;
				int num3 = (int)((double)pos.X - num * 0.5);
				int num4 = (int)((double)pos.X + num * 0.5);
				int num5 = (int)((double)pos.Y - num * 0.5);
				int num6 = (int)((double)pos.Y + num * 0.5);
				if (num3 < 1)
				{
					num3 = 1;
				}
				if (num5 < 1)
				{
					num5 = 1;
				}
				if (num6 > Main.maxTilesY - 1)
				{
					num6 = Main.maxTilesY - 1;
				}
				for (int k = num3; k < num4; k++)
				{
					for (int l = num5; l < num6; l++)
					{
						if ((double)(Math.Abs((float)k - pos.X) + Math.Abs((float)l - pos.Y)) < strength * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							//do not generate or replace anything above the surface (basically to stop it from replacing blocks on floating islands)
							float divide = 7.5f;
							int heightLimit = (int)Main.worldSurface - (Main.maxTilesY / (int)divide);

							if (l > heightLimit && Main.tile[k, l].TileType != TileID.Cloud && Main.tile[k, l].TileType != TileID.RainCloud)
							{
								//kill all of these tiles
								int[] Kill = { 19, 24, 27, 61, 71, 73, 74, 80, 81, 82, 83, 84, 110, 113, 129,
								162, 165, 184, 185, 186, 187, 201, 227, 233, 236, 254, 324, 444, 461, 3, 21,
								63, 64, 65, 66, 67, 68, 192, 10, 11, 12, 14, 15, 16, 17, 18, 19, 26, 28, 31,
								32, 33, 34, 42, 79, 86, 87, 88, 89, 90, 91, 92, 93, 100, 101, 104, 105, 374 };

								if (!noTiles)
								{
									if (Kill.Contains(Main.tile[k, l].TileType))
									{
										WorldGen.KillTile(k, l);
									}

									//replace tiles if it is not in the kill list
									if (!Kill.Contains(Main.tile[k, l].TileType))
									{
										Main.tile[k, l].TileType = (ushort)tileType;
									}

									if (Main.tile[k, l].WallType == WallID.EbonstoneUnsafe || Main.tile[k, l].WallType == WallID.CrimstoneUnsafe)
									{
										PlaceCircle(k, l, tileType, wallType, 6, false, false);
									}

									if (addTile)
									{
										Main.tile[k, l].TileType = (ushort)tileType;
									}
								}

								//replace all walls
								if (Main.tile[k, l].WallType > 0 && replaceWalls)
								{
									Main.tile[k, l].WallType = (ushort)wallType;
								}

								//place walls below each block
								if (Main.tile[k, l].HasTile && Main.tile[k - 1, l].HasTile && Main.tile[k + 1, l].HasTile && placeWalls)
								{
									Main.tile[k, l + 6].WallType = (ushort)wallType;
								}

								//place walls
								if (wallType == ModContent.WallType<SpookyGrassWall>())
								{
									//this loop is for placing walls in the underground part of the spooky biome
									for (int WallY = (int)Main.worldSurface; WallY < l; WallY++)
									{
										if (placeWalls)
										{
											Main.tile[k, WallY + 6].WallType = 0;
										}
									}

									//randomized wall placement so the underground and surface walls transition nicely
									for (int WallY = (int)Main.worldSurface; WallY < Main.worldSurface + 15; WallY++)
									{
										if (placeWalls)
										{
											if (WorldGen.genRand.NextBool(2))
											{
												Main.tile[k, WallY].WallType = (ushort)wallType;
											}
										}
									}
								}
							}
						}
					}
				}

				//this looks wack but im too afraid to mess with it
				pos += randVect;
				if (num > 50.0)
				{
					pos += randVect;
					num2 -= 1f;
					randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
					randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
					if (num > 100.0)
					{
						pos += randVect;
						num2 -= 1f;
						randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
						randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
						if (num > 150.0)
						{
							pos += randVect;
							num2 -= 1f;
							randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
							randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
							if (num > 200.0)
							{
								pos += randVect;
								num2 -= 1f;
								randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
								randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
								if (num > 250.0)
								{
									pos += randVect;
									num2 -= 1f;
									randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
									randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
									if (num > 300.0)
									{
										pos += randVect;
										num2 -= 1f;
										randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
										randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
										if (num > 400.0)
										{
											pos += randVect;
											num2 -= 1f;
											randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
											randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
											if (num > 500.0)
											{
												pos += randVect;
												num2 -= 1f;
												randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
												randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
												if (num > 600.0)
												{
													pos += randVect;
													num2 -= 1f;
													randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
													randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
													if (num > 700.0)
													{
														pos += randVect;
														num2 -= 1f;
														randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
														randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
														if (num > 800.0)
														{
															pos += randVect;
															num2 -= 1f;
															randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
															randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
															if (num > 900.0)
															{
																pos += randVect;
																num2 -= 1f;
																randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
																randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				randVect.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (randVect.X > 1f)
				{
					randVect.X = 1f;
				}
				if (randVect.X < -1f)
				{
					randVect.X = -1f;
				}
				if (!noYChange)
				{
					randVect.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
					if (randVect.Y > 1f)
					{
						randVect.Y = 1f;
					}
					if (randVect.Y < -1f)
					{
						randVect.Y = -1f;
					}
				}
			}
		}
	}
}