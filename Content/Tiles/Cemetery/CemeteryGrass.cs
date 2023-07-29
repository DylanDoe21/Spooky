using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.Cemetery.Ambient;

namespace Spooky.Content.Tiles.Cemetery
{
    [LegacyName("CatacombBrickMoss")]
    [LegacyName("CatacombGrass")]
	public class CemeteryGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<CemeteryDirt>();
            Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(52, 102, 46));
			RegisterItemDrop(ModContent.ItemType<CemeteryDirtItem>());
            DustType = ModContent.DustType<CemeteryGrassDust>();
            MineResist = 0.7f;
		}

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                //grow vines
                if (Main.rand.Next(12) == 0) 
                {
                    Below.TileType = (ushort)ModContent.TileType<CemeteryVines>();
                    Below.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
                    }
                }
            }

            if (!Above.HasTile && Above.LiquidType <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                if (Main.tile[i, j].WallType != ModContent.WallType<CatacombBrickWall1>() && Main.tile[i, j].WallType != ModContent.WallType<CatacombBrickWall2>())
                {
                    //grow weeds
                    if (Main.rand.Next(5) == 0)
                    {
                        Above.TileType = (ushort)ModContent.TileType<CemeteryWeeds>();
                        Above.HasTile = true;
                        Above.TileFrameY = 0;
                        Above.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                        WorldGen.SquareTileFrame(i, j + 1, true);
                        if (Main.netMode == NetmodeID.Server) 
                        {
                            NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                        }
                    }
                }
                else
                {
                    //grow weeds
                    if (Main.rand.Next(12) == 0)
                    {
                        Above.TileType = (ushort)ModContent.TileType<CatacombWeeds>();
                        Above.HasTile = true;
                        Above.TileFrameY = 0;
                        Above.TileFrameX = (short)(WorldGen.genRand.Next(16) * 18);
                        WorldGen.SquareTileFrame(i, j + 1, true);
                        if (Main.netMode == NetmodeID.Server) 
                        {
                            NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                        }
                    }

                    //grow mushrooms
                    if (Main.rand.Next(25) == 0)
                    {
                        Above.TileType = (ushort)ModContent.TileType<SporeMushroom>();
                        Above.HasTile = true;
                        Above.TileFrameY = 0;
                        Above.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                        WorldGen.SquareTileFrame(i, j + 1, true);
                        if (Main.netMode == NetmodeID.Server) 
                        {
                            NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                        }
                    }
                }

                if (Main.tile[i, j].WallType == ModContent.WallType<CatacombBrickWall2>())
                {
                    //grow giant flowers
                    if (Main.rand.Next(10) == 0)
                    {
                        GrowGiantFlower(i, j, ModContent.TileType<BigFlower>());
                    }
                }
            }

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<CemeteryDirt>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<CemeteryGrass>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
        }

        public static bool GrowGiantFlower(int X, int Y, int tileType)
        {
            int canPlace = 0;

            //do not allow giant flowers to place if another one is too close
            for (int i = X - 5; i < X + 5; i++)
            {
                for (int j = Y - 5; j < Y + 5; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == tileType)
                    {
                        canPlace++;
                        if (canPlace > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            //make sure the area is large enough for it to place in both horizontally and vertically
            for (int i = X - 2; i < X + 2; i++)
            {
                for (int j = Y - 8; j < Y - 2; j++)
                {
                    //only check for solid blocks, ambient objects dont matter
                    if (Main.tile[i, j].HasTile && Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        canPlace++;
                        if (canPlace > 0)
                        {
                            return false;
                        }
                    }
                }
            }

            BigFlower.Grow(X, Y - 1, 3, 6);

            return true;
        }

        private List<Point> OpenAdjacents(int i, int j, int type)
        {
            var tileList = new List<Point>();

            for (int k = -1; k < 2; ++k)
            {
                for (int l = -1; l < 2; ++l)
                {
                    if (!(l == 0 && k == 0) && Framing.GetTileSafely(i + k, j + l).HasTile && Framing.GetTileSafely(i + k, j + l).TileType == type)
                    {
                        tileList.Add(new Point(i + k, j + l));
                    }
                }
            }

            return tileList;
        }

        private bool HasOpening(int i, int j)
        {
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    if (!Framing.GetTileSafely(i + k, j + l).HasTile)
                    {
                        return true;
                    }
                }
            }
                    
            return false;
        }
	}
}
