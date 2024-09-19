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
            TileID.Sets.GeneralPlacementTiles[Type] = false;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(62, 54, 59));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
            MineResist = 0.85f;
		}
	}
}
