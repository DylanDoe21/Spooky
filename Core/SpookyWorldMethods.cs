﻿using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace Spooky.Core
{
	public class SpookyWorldMethods
	{
		//clear or make a circle
		public static void Circle(int i, int j, int size, int tileType, bool killTile = true)
		{
			int BaseRadius = size;
			int radius = BaseRadius;

			for (int y = j - radius; y <= j + radius; y++)
			{
				for (int x = i - radius; x <= i + radius + 1; x++)
				{
					if ((int)Vector2.Distance(new Vector2(x, y), new Vector2(i, j)) <= radius)
                    {
						if (killTile)
						{
							WorldGen.KillTile(x, y);
						}
						if (!killTile)
						{
							WorldGen.PlaceTile(x, y, tileType);
						}
                    }
				}

				radius = BaseRadius - WorldGen.genRand.Next(-1, 2);
			}
		}

		//this is basically a heavily modified version of vanillas tile runner specifically for the spooky forest biome
		public static void TileRunner(int i, int j, double strength, int steps, int tileType, int wallType, int wallType2, bool addTile = false, 
		float speedX = 0f, float speedY = 0f, bool noYChange = false, bool placeWalls = false, bool SpookyWalls = false)
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
							//do not generate or replace anything above the surface (basically to stop it from genning on floating islands)
							if (l > (int)Main.worldSurface - 175)
							{
								//I think this checks if the tile is air????
								if (tileType < 0)
								{
									Main.tile[k, l].HasTile.Equals(false);
								}

								//kill all of these tiles
								int[] Kill = { 19, 24, 27, 61, 71, 73, 74, 80, 81, 82, 83, 84, 110, 113, 129,
								162, 165, 184, 185, 186, 187, 201, 227, 233, 236, 254, 324, 444, 461, 3, 21,
								63, 64, 65, 66, 67, 68, 192, 10, 11, 12, 14, 15, 16, 17, 18, 19, 26, 28, 31,
								32, 33, 34, 42, 79, 86, 87, 88, 89, 90, 91, 92, 93, 100, 101, 104, 105, 374 };

								//dont replace dungeon bricks
								int[] NoReplace = { 41, 43, 44 };

								if (Kill.Contains(Main.tile[k, l].TileType))
								{
									WorldGen.KillTile(k, l);
								}

								//replace tiles if it is not in either of these lists
								if (!NoReplace.Contains(Main.tile[k, l].TileType) && !Kill.Contains(Main.tile[k, l].TileType))
								{
									Main.tile[k, l].TileType = (ushort)tileType;
								}

								if (addTile)
								{
									Main.tile[k, l].TileType = (ushort)tileType;
								}

								//do not replace dungeon walls
								int[] NoWallReplace = { 7, 8, 9, 94, 96, 98, 95, 97, 99 };

								//replace walls if not a dungeon wall
								if (!NoWallReplace.Contains(Main.tile[k, l].WallType) && Main.tile[k, l].WallType > 0)
								{
									Main.tile[k, l].WallType = (ushort)wallType;
								}

								//place walls below each block
								if (Main.tile[k, l].HasTile && Main.tile[k - 1, l].HasTile && Main.tile[k + 1, l].HasTile && !NoWallReplace.Contains(Main.tile[k, l].WallType))
								{
									Main.tile[k, l + 1].WallType = (ushort)wallType;
								}

								//place walls
								if (SpookyWalls)
								{
									//this loop is for placing walls in the underground part of the spooky biome
									for (int WallY = (int)Main.worldSurface; WallY < l; WallY++)
									{
										if (placeWalls)
										{
											Main.tile[k, WallY + 5].WallType = (ushort)wallType2;
										}
									}

									//randomized wall placement so the underground and surface walls transition nicely
									for (int WallY = (int)Main.worldSurface; WallY < Main.worldSurface + 10; WallY++)
									{
										if (placeWalls)
										{
											if (Main.rand.Next(2) == 0)
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