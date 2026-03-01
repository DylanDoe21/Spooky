using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class GlowshroomBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(196, 144, 224));
			DustType = DustID.Slush;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float divide = 600f;

            r = 155f / divide;
            g = 83f / divide;
            b = 250f / divide;
        }
	}
}
