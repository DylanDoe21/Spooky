using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyGrassGreen : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyDirt>();
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(78, 120, 48));
            RegisterItemDrop(ModContent.ItemType<SpookyDirtItem>());
            DustType = ModContent.DustType<SpookyGrassDustGreen>();
            MineResist = 0.7f;
		}

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                if (Main.rand.Next(8) == 0) 
                {
                    Below.TileType = (ushort)ModContent.TileType<SpookyVinesGreen>();
                    Below.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }

            if (!Above.HasTile && Above.LiquidType <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.Next(15) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<SpookyWeedsGreen>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
				}

                //grow colored gourds
                if (WorldGen.genRand.NextBool(40))
                {
                    ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdSmall>(), (ushort)ModContent.TileType<GourdLarge>() };

                    ushort newObject = Main.rand.Next(Gourds);

                    WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //grow rotten gourd
                if (WorldGen.genRand.NextBool(45) && SpookyForest.CanGrowRottenGourd(i, j))
                {
                    WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<GourdRotten>(), true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, (ushort)ModContent.TileType<GourdRotten>(), 0, 0, -1, -1);
                }
            }

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<SpookyDirt>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<SpookyGrassGreen>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
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
