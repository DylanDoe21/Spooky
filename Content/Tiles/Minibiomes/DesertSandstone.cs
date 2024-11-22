using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes
{
	public class DesertSandstone : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(173, 101, 39));
            DustType = DustID.YellowStarfish;
			HitSound = SoundID.Tink;
		}
	}
}
