using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Graveyard
{
	public class GraveyardBrickMoss : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			AddMapEntry(new Color(108, 66, 48));
			ItemDrop = ModContent.ItemType<GraveyardBrickMossItem>();
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		/*
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool CanExplode(int i, int j)
        {
			return false;
        }
		*/
    }
}
