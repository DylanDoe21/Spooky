using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyWood : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            AddMapEntry(new Color(106, 84, 53));
			DustType = DustID.WoodFurniture;
			ItemDrop = ModContent.ItemType<SpookyWoodItem>();
		}
	}
}
