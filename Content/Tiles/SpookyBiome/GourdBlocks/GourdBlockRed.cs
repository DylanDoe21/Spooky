using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
	public class GourdBlockRed : ModTile
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
			DustType = 288;
            HitSound = SoundID.Dig;
		}
	}
}
