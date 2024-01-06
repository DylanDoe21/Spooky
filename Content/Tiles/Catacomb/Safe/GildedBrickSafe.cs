using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Safe
{
	public class GildedBrickSafe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/GildedBrick";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(173, 114, 12));
			DustType = DustID.Gold;
			HitSound = SoundID.Tink;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 2 * 288; //288 is the width of each individual sheet
			frameYOffset = j % 2 * 270; //270 is the height of each individual sheet
        }
    }
}
