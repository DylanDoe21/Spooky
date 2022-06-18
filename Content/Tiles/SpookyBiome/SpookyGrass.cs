using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            AddMapEntry(new Color(193, 97, 18));
			ItemDrop = ModContent.ItemType<SpookyGrassItem>();
            DustType = ModContent.DustType<HalloweenGrassDust>();
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                if (Main.rand.Next(15) == 0) 
                {
                    Below.TileType = (ushort)ModContent.TileType<SpookyVines>();
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
                if (Main.rand.Next(15) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<SpookyWeeds>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(11) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
				}

                if (Main.rand.Next(20) == 0) 
                {
                    ushort[] TallWeeds = new ushort[] { (ushort)ModContent.TileType<SpookyWeedTall1>(), 
                    (ushort)ModContent.TileType<SpookyWeedTall2>(), (ushort)ModContent.TileType<SpookyWeedTall3>() };

                    WorldGen.PlaceObject(i, j, Main.rand.Next(TallWeeds));
                }

                if (Main.rand.Next(25) == 0) 
                {
                    ushort[] BigWeed = new ushort[] { (ushort)ModContent.TileType<SpookyWeedBig1>(), 
                    (ushort)ModContent.TileType<SpookyWeedBig2>(), (ushort)ModContent.TileType<SpookyWeedBig3>() };

                    WorldGen.PlaceObject(i, j - 1, Main.rand.Next(BigWeed));
                }
                
                if (Main.rand.Next(35) == 0) 
                {
                    ushort[] Pumpkins = new ushort[] { (ushort)ModContent.TileType<SpookyPumpkin1>(), 
                    (ushort)ModContent.TileType<SpookyPumpkin2>(), (ushort)ModContent.TileType<SpookyPumpkin3>() };

                    WorldGen.PlaceObject(i, j - 1, Main.rand.Next(Pumpkins));
                }
            }
        }
	}
}
