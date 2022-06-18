using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Core
{
	public class SpookyWorldMethods
	{
		//clear/make a circle
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

		/*
		//idk if i even still need these

		//small tunnel, specifically made to have ore in some tunnels for the spooky hell biome
		public static void SmallTunnel(int x, int y, int tunnellength, int oreTile)
		{
			int PositionX = x + 30;
			int PositionY = y;

			bool Left = false;

			if (Main.rand.Next(2) == 0)
			{
				Left = true;
			}

			//dig tunnels
			if (Left)
			{
				//dig left tunnel
				for (int TunnelX = PositionX; TunnelX > PositionX - tunnellength; TunnelX--)
				{
					WorldGen.digTunnel(TunnelX, PositionY, 0, 0, 1, 3, false);
				}
			}
			else
			{
				//dig right tunnel
				for (int TunnelX = PositionX; TunnelX < PositionX + tunnellength; TunnelX++)
				{
					WorldGen.digTunnel(TunnelX, PositionY, 0, 0, 1, 3, false);
				}
			}

			//dig a special glow ore room at the end of some tunnels
			if (Main.rand.Next(5) == 0)
			{
				int SmallPitX = Main.rand.Next(PositionX, PositionX + tunnellength);
				int SmallPitX2 = Main.rand.Next(PositionX - tunnellength, PositionX);

				if (Left)
				{
					WorldGen.digTunnel(SmallPitX2, PositionY, 0, 0, 8, 8, false);
					WorldGen.TileRunner(SmallPitX2, PositionY, 35, 1, oreTile, false, 0f, 0f, false, true);
				}
				else
				{
					WorldGen.digTunnel(SmallPitX, PositionY, 0, 0, 8, 8, false);
					WorldGen.TileRunner(SmallPitX, PositionY, 35, 1, oreTile, false, 0f, 0f, false, true);
				}
			}
		}

		//dig 2 large tunnels next to each other, then dig smaller holes within those tunnels
		public static void ConnectingTunnel(int x, int y, int tunnellength)
		{
			int PositionX = x + 30;
			int PositionY = y + 40;

			//dig left tunnel
			for (int TunnelX = PositionX; TunnelX > PositionX - tunnellength; TunnelX--)
			{
				WorldGen.digTunnel(TunnelX - 50, PositionY, 0, 0, 1, 6, false);
			}

			//dig right tunnel
			for (int TunnelX = PositionX; TunnelX < PositionX + tunnellength; TunnelX++)
			{
				WorldGen.digTunnel(TunnelX + 50, PositionY, 0, 0, 1, 6, false);
			}

			//dig smaller pits within the main tunnel
			for (int SmallPits = 0; SmallPits < 5; SmallPits++)
			{
				int SmallPitX = Main.rand.Next(PositionX, PositionX + tunnellength);
				int SmallPitX2 = Main.rand.Next(PositionX - tunnellength, PositionX);

				int SmallPitDepth = Main.rand.Next(20, 40);

				//dig smaller pits in the right tunnel
				for (int SmallPitY = PositionY; SmallPitY < PositionY + SmallPitDepth; SmallPitY++)
				{
					WorldGen.digTunnel(SmallPitX, SmallPitY, 0, 0, 1, 4, false);
				}

				//dig smaller pits in the left tunnel
				for (int SmallPitY2 = PositionY; SmallPitY2 < PositionY + SmallPitDepth; SmallPitY2++)
				{
					WorldGen.digTunnel(SmallPitX2, SmallPitY2, 0, 0, 1, 4, false);
				}
			}
		}
		*/

		//large pit for big craters, specifically made to have wall blending for the spooky hell biomes underground and surface
		public static void LargePit(int x, int y, int height, int tileType, int tileType2, int wallType)
		{
			int digDir = 0;
			int[] xAdds = new int[height];
			int x3 = x;
			int x2 = x;

			for (int i = 0; i < height; i++)
			{
				if (WorldGen.genRand.NextBool())
				{
					xAdds[i] = digDir;
					x += xAdds[i];
				}

				for (int j = 0; j < 20; j++)
				{
					WorldGen.PlaceTile(x - 10 + j, y + i, tileType2, true, true);
					if (i < height / 2 + 15)
					{
						Framing.GetTileSafely(x - 10 + j, y + i).WallType = (ushort)wallType;
					}

					if (i >= height / 2 + 15 && i < height / 2 + 20)
					{
						if (Main.rand.Next(3) == 0)
						{
							Framing.GetTileSafely(x - 10 + j, y + i).WallType = (ushort)wallType;
						}
					}
				}

				if (i < height / 2)
				{
					WorldGen.TileRunner(x, y + i, WorldGen.genRand.Next(20, 28), 5, tileType2, true);
				}
			}

			for (int i = 0; i < 10; i++)
			{
				WorldGen.digTunnel(x2, y - i, digDir, 5f, 2, 6, false);
			}

			for (int i = 0; i < height; i++)
			{
				x2 += xAdds[i];
				WorldGen.digTunnel(x2, y + i, digDir, 1f, 2, 6, false);
			}

			WorldGen.TileRunner(x, y + height + 4, WorldGen.genRand.Next(18, 25), 6, tileType, false);
			WorldGen.digTunnel(x - digDir, y + height - 6, digDir, 1f, 5, 6, false);
			WorldGen.digTunnel(x - digDir, y + height - 5, digDir, 1f, 5, 6, false);
			WorldGen.digTunnel(x - digDir, y + height - 4, digDir, 1f, 5, 6, false);
			WorldGen.digTunnel(x, y + height - 3, digDir, 1f, 5, 6, false);
			WorldGen.digTunnel(x, y + height - 2, digDir, 1f, 5, 6, false);
			WorldGen.digTunnel(x, y + height - 1, digDir, 1f, 5, 6, false);
			WorldGen.digTunnel(x, y + height, digDir, 1f, 5, 6, false);
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