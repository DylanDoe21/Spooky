using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell
{
	[LegacyName("Carapace")]
	public class ValleyStone : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            AddMapEntry(new Color(161, 140, 140));
            ItemDrop = ModContent.ItemType<ValleyStoneItem>();
			DustType = DustID.Bone;
            HitSound = SoundID.Dig;
		}
	}
}