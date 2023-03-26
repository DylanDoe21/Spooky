using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Cemetery
{
	public class CemeteryDirt : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            AddMapEntry(new Color(73, 62, 49));
			ItemDrop = ModContent.ItemType<CemeteryDirtItem>();
            DustType = DustID.Dirt;
			MineResist = 0.65f;
		}
	}
}
