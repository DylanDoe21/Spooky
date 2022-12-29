using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyStone : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(62, 54, 59));
			ItemDrop = ModContent.ItemType<SpookyStoneItem>();
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Above.HasTile && Above.LiquidType <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.Next(15) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<SpookyMushroomSmall>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(2) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
				}

                if (Main.rand.Next(20) == 0) 
                {
                    ushort[] TallMushroom = new ushort[] { (ushort)ModContent.TileType<SpookyMushroomTall1>(), (ushort)ModContent.TileType<SpookyMushroomTall2>() };

                    ushort newObject = Main.rand.Next(TallMushroom);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
			}
		}
	}
}
