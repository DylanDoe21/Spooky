using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyGrassGreen : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.JungleSpecial[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyDirt>();
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(78, 120, 48));
            ItemDrop = ModContent.ItemType<SpookyDirtItem>();
			DustType = ModContent.DustType<SpookyGrassDustGreen>();
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
                if (Main.rand.Next(5) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<SpookyWeedsGreen>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(7) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
				}

                if (Main.rand.Next(8) == 0) 
                {
                    ushort[] TallWeed = new ushort[] { (ushort)ModContent.TileType<SpookyWeedsTallGreen1>(), 
                    (ushort)ModContent.TileType<SpookyWeedsTallGreen2>(),(ushort)ModContent.TileType<SpookyWeedsTallGreen3>() };

                    ushort newObject = Main.rand.Next(TallWeed);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                if (Main.rand.Next(15) == 0) 
                {
                    ushort[] BigWeeds = new ushort[] { (ushort)ModContent.TileType<SpookyWeedBig3>(), (ushort)ModContent.TileType<SpookyWeedBig4>() };

                    ushort newObject = Main.rand.Next(BigWeeds);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
            }
        }
	}
}
