using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using System.Linq;

using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Generation
{
    public class TileConversionMethods
    {
        //convert spooky mod blocks into purity when used with green or brown solution
        public static void ConvertSpookyIntoPurity(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with regular grass
                        int[] GrassReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), ModContent.TileType<CemeteryGrass>() };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Grass;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        int[] DirtReplace = { ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>() };

                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with stone
                        int[] StoneReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>() };

                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Stone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with grass walls
                        int[] GrassWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>() };

                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.GrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into hallowed blocks when used with light blue solution
        public static void ConvertSpookyIntoHallow(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with hallowed grass
                        int[] GrassReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), ModContent.TileType<CemeteryGrass>() };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.HallowedGrass;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        int[] DirtReplace = { ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>() };

                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with pearlstone
                        int[] StoneReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>() };

                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Pearlstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with hallowed grass walls
                        int[] GrassWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>() };

                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.HallowedGrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into corrupt blocks when used with purple solution
        public static void ConvertSpookyIntoCorruption(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with corrupt grass
                        int[] GrassReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), ModContent.TileType<CemeteryGrass>() };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.CorruptGrass;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        int[] DirtReplace = { ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>() };

                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with ebonstone
                        int[] StoneReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>() };

                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Ebonstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with corrupt grass walls
                        int[] GrassWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>() };

                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.CorruptGrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into crimson blocks when used with red solution
        public static void ConvertSpookyIntoCrimson(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses with crimson grass
                        int[] GrassReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), ModContent.TileType<CemeteryGrass>() };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.CrimsonGrass;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky dirt with dirt 
                        int[] DirtReplace = { ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>() };

                        if (DirtReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Dirt;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with crimstone
                        int[] StoneReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>() };

                        if (StoneReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Crimstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with crimson grass walls
                        int[] GrassWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>() };

                        if (GrassWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.CrimsonGrassUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into snow blocks when used with white solution
        public static void ConvertSpookyIntoSnow(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses and dirt with snow
                        int[] SnowReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(), 
                        ModContent.TileType<CemeteryGrass>(), ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>() };

                        if (SnowReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.SnowBlock;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with ice
                        int[] IceReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>() };

                        if (IceReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.IceBlock;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with snow walls
                        int[] SnowWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>() };

                        if (SnowWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.SnowWallUnsafe;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert spooky mod blocks into desert blocks when used with yellow solution
        public static void ConvertSpookyIntoDesert(int i, int j, int size = 4)
        {
            for (int k = i - size; k <= i + size; k++)
            {
                for (int l = j - size; l <= j + size; l++)
                {
                    if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size)))
                    {
                        //replace spooky grasses and dirt with sand
                        int[] SnowReplace = { ModContent.TileType<SpookyGrass>(), ModContent.TileType<SpookyGrassGreen>(),
                        ModContent.TileType<CemeteryGrass>(), ModContent.TileType<SpookyDirt>(), ModContent.TileType<CemeteryDirt>() };

                        if (SnowReplace.Contains(Main.tile[k, l].TileType))
                        {
                            if (!Main.tile[k, l + 1].HasTile)
                            {
                                Main.tile[k, l].TileType = TileID.HardenedSand;
                            }
                            else
                            {
                                Main.tile[k, l].TileType = TileID.Sand;
                            }

                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky stone with sandstone
                        int[] IceReplace = { ModContent.TileType<SpookyStone>(), ModContent.TileType<CemeteryStone>() };

                        if (IceReplace.Contains(Main.tile[k, l].TileType))
                        {
                            Main.tile[k, l].TileType = TileID.Sandstone;
                            WorldGen.SquareTileFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }

                        //replace spooky grass walls with sandstone walls
                        int[] SnowWallReplace = { ModContent.WallType<SpookyGrassWall>(), ModContent.WallType<CemeteryGrassWall>() };

                        if (SnowWallReplace.Contains(Main.tile[k, l].WallType))
                        {
                            Main.tile[k, l].WallType = WallID.Sandstone;
                            WorldGen.SquareWallFrame(k, l);
                            NetMessage.SendTileSquare(-1, k, l, 1);
                        }
                    }
                }
            }
        }

        //convert blocks into spooky forest ones with orange solution
        public static void ConvertPurityIntoSpooky(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {
                        //replace normal grass with orange grass
                        int[] GrassReplace = { TileID.Grass, TileID.HallowedGrass };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType)) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyGrass>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace corrupt biome grasses with green grass
                        int[] GreenGrassReplace = { TileID.CorruptGrass, TileID.CrimsonGrass };

                        if (GreenGrassReplace.Contains(Main.tile[k, l].TileType)) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace dirt blocks with spooky dirt
						if (Main.tile[k, l].TileType == TileID.Dirt) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyDirt>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace stone blocks with spooky stone
						if (TileID.Sets.Conversion.Stone[Main.tile[k, l].TileType]) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyStone>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace grass walls with spooky grass walls
                        int[] WallReplace = { WallID.GrassUnsafe, WallID.FlowerUnsafe, WallID.Grass, WallID.Flower, 
                        WallID.CorruptGrassUnsafe, WallID.HallowedGrassUnsafe, WallID.CrimsonGrassUnsafe };

						if (WallReplace.Contains(Main.tile[k, l].WallType)) 
                        {
							Main.tile[k, l].WallType = (ushort)ModContent.WallType<SpookyGrassWall>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
					}
				}
			}
		}

        //convert blocks into swampy cemetery ones with orange solution
        public static void ConvertPurityIntoCemetery(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {
                        //replace grass with cemetery grass
                        int[] GrassReplace = { TileID.Grass, TileID.HallowedGrass, TileID.CorruptGrass, TileID.CrimsonGrass };

                        if (GrassReplace.Contains(Main.tile[k, l].TileType)) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<CemeteryGrass>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace dirt blocks with spooky dirt
						if (Main.tile[k, l].TileType == TileID.Dirt) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<CemeteryDirt>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

                        //replace stone blocks with spooky stone
						if (TileID.Sets.Conversion.Stone[Main.tile[k, l].TileType]) 
                        {
							Main.tile[k, l].TileType = (ushort)ModContent.TileType<CemeteryStone>();
							WorldGen.SquareTileFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}

						//replace grass walls with cemetery grass walls
                        int[] WallReplace = { WallID.GrassUnsafe, WallID.FlowerUnsafe, WallID.Grass, WallID.Flower, 
                        WallID.CorruptGrassUnsafe, WallID.HallowedGrassUnsafe, WallID.CrimsonGrassUnsafe };

						if (WallReplace.Contains(Main.tile[k, l].WallType)) 
                        {
							Main.tile[k, l].WallType = (ushort)ModContent.WallType<CemeteryGrassWall>();
							WorldGen.SquareWallFrame(k, l);
							NetMessage.SendTileSquare(-1, k, l, 1);
						}
					}
				}
			}
		}

        //convert underworld blocks into valley of eyes blocks
        public static void ConvertHellIntoEyeValley(int i, int j, int size = 4) 
        {
			for (int k = i - size; k <= i + size; k++) 
            {
				for (int l = j - size; l <= j + size; l++) 
                {
					if (WorldGen.InWorld(k, l, 1) && Math.Abs(k - i) + Math.Abs(l - j) < Math.Sqrt((size * size) + (size * size))) 
                    {
						if (Main.tile[k, l].TileType == TileID.Ash) 
                        {
							if (!Main.tile[k - 1, l].HasTile || !Main.tile[k + 1, l].HasTile || !Main.tile[k, l - 1].HasTile || !Main.tile[k, l + 1].HasTile)
							{
								Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyMushGrass>();
								WorldGen.SquareTileFrame(k, l);
								NetMessage.SendTileSquare(-1, k, l, 1);
							}
							else
							{
								Main.tile[k, l].TileType = (ushort)ModContent.TileType<SpookyMush>();
								WorldGen.SquareTileFrame(k, l);
								NetMessage.SendTileSquare(-1, k, l, 1);
							}
						}
					}
				}
			}
		}
    }
}