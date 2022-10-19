using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

using Spooky.Content.Tiles.Catacomb.Ambient;

namespace Spooky.Content.Tiles.Catacomb
{
	public class CatacombBrickMoss : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			AddMapEntry(new Color(75, 78, 0));
			ItemDrop = ModContent.ItemType<CatacombBrickMossItem>();
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool CanExplode(int i, int j)
        {
			return false;
        }

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);

			if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                if (Main.rand.Next(3) == 0) 
                {
                    Below.TileType = (ushort)ModContent.TileType<CatacombVines>();
                    Below.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }
		}
	}
}
