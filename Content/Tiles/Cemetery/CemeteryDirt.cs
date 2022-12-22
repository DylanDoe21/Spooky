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
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile up = Main.tile[i, j - 1];
			Tile down = Main.tile[i, j + 1];
			Tile left = Main.tile[i - 1, j];
			Tile right = Main.tile[i + 1, j];

			if (WorldGen.genRand.Next(5) == 0)
			{
				if (up.TileType == ModContent.TileType<CemeteryGrass>() || down.TileType == ModContent.TileType<CemeteryGrass>() || 
				left.TileType == ModContent.TileType<CemeteryGrass>() || right.TileType == ModContent.TileType<CemeteryGrass>())
				{
					WorldGen.SpreadGrass(i, j, Type, ModContent.TileType<CemeteryGrass>(), true);
				}
			}
		}
	}
}
