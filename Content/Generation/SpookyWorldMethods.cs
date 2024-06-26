﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Tiles.Catacomb;
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

        public static void PlaceVines(int VineX, int VineY, int numVines, ushort vineType, bool finished = false)
		{
            for (int Y = VineY; Y <= VineY + numVines && !finished; Y++)
            {
                Tile tileBelow = Framing.GetTileSafely(VineX, Y + 1);

                if ((!tileBelow.HasTile || tileBelow.TileType == TileID.Cobweb) && WorldGen.InWorld(VineX, Y))
                {
                    WorldGen.PlaceTile(VineX, Y, vineType);
                }
                else
                {
                    finished = true;
				}
                
                if (numVines <= 1)
                {
                    finished = true;
                }
            }
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

	public class TileRunner
    {
        public Vector2 pos;
        public Vector2 speed;
        public Point16 hRange;
        public Point16 vRange;
        public double strength;
        public double str;
        public int steps;
        public int stepsLeft;
        public ushort type;
        public bool addTile;
        public bool overRide;

        public TileRunner(Vector2 pos, Vector2 speed, Point16 hRange, Point16 vRange, double strength, int steps, ushort type, bool addTile, bool overRide)
        {
            this.pos = pos;
            if (speed.X == 0 && speed.Y == 0)
            {
                this.speed = new Vector2(WorldGen.genRand.Next(hRange.X, hRange.Y + 1) * 0.1f, WorldGen.genRand.Next(vRange.X, vRange.Y + 1) * 0.1f);
            }
            else
            {
                this.speed = speed;
            }
            this.hRange = hRange;
            this.vRange = vRange;
            this.strength = strength;
            str = strength;
            this.steps = steps;
            stepsLeft = steps;
            this.type = type;
            this.addTile = addTile;
            this.overRide = overRide;
        }

        public void Start()
        {
            while (str > 0 && stepsLeft > 0)
            {
                str = strength * (double)stepsLeft / steps;

                int a = (int)Math.Max(pos.X - str * 0.5, 1);
                int b = (int)Math.Min(pos.X + str * 0.5, Main.maxTilesX - 1);
                int c = (int)Math.Max(pos.Y - str * 0.5, 1);
                int d = (int)Math.Min(pos.Y + str * 0.5, Main.maxTilesY - 1);

                for (int i = a; i < b; i++)
                {
                    for (int j = c; j < d; j++)
                    {
                        if (Math.Abs(i - pos.X) + Math.Abs(j - pos.Y) >= strength * StrengthRange())
                        {
                            continue;
                        }
                        
                        ChangeTile(Main.tile[i, j]);
                    }
                }

                str += 50;
                while (str > 50)
                {
                    pos += speed;
                    stepsLeft--;
                    str -= 50;
                    speed.X += WorldGen.genRand.Next(hRange.X, hRange.Y + 1) * 0.05f;
                    speed.Y += WorldGen.genRand.Next(vRange.X, vRange.Y + 1) * 0.05f;
                }

                speed = Vector2.Clamp(speed, new Vector2(-1, -1), new Vector2(1, 1));
            }
        }

        public virtual void ChangeTile(Tile tile)
        {
            if (!addTile)
            {
                tile.HasTile = false;
            }
            else
            {
                tile.TileType = type;
            }
        }

        public virtual double StrengthRange()
        {
            return 0.5 + WorldGen.genRand.Next(-10, 11) * 0.0075;
        }
    }

	class WaterTileRunner : TileRunner
    {
        public WaterTileRunner(Vector2 pos, Vector2 speed, Point16 hRange, Point16 vRange, double strength, int steps, ushort type, bool addTile, bool overRide) : base(pos, speed, hRange, vRange, strength, steps, type, addTile, overRide)
        {
        }
        public override void ChangeTile(Tile tile)
        {
            tile.HasTile = false;
            tile.LiquidType = LiquidID.Water;
			tile.LiquidAmount = 255;
        }
    }
}