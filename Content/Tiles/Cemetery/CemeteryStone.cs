using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Cemetery
{
	[LegacyName("CemeteryBrick")]
	public class CemeteryStone : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(65, 79, 88));
            DustType = DustID.Stone;
			HitSound = SoundID.Tink;
			MineResist = 0.85f;
		}
	}
}
