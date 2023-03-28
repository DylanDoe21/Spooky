using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyDirt : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(114, 78, 53));
			ItemDrop = ModContent.ItemType<SpookyDirtItem>();
            DustType = DustID.Dirt;
			MineResist = 0.65f;
		}
	}

    public class SpookyDirt2 : SpookyDirt
	{
        public override string Texture => "Spooky/Content/Tiles/SpookyBiome/SpookyDirt";
    }
}
