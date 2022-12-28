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
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(114, 78, 53));
			ItemDrop = ModContent.ItemType<SpookyDirtItem>();
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
				if (up.TileType == ModContent.TileType<SpookyGrass>() || down.TileType == ModContent.TileType<SpookyGrass>() || 
				left.TileType == ModContent.TileType<SpookyGrass>() || right.TileType == ModContent.TileType<SpookyGrass>())
				{
					WorldGen.SpreadGrass(i, j, Type, ModContent.TileType<SpookyGrass>(), true);
				}

				if (up.TileType == ModContent.TileType<SpookyGrassGreen>() || down.TileType == ModContent.TileType<SpookyGrassGreen>() || 
				left.TileType == ModContent.TileType<SpookyGrassGreen>() || right.TileType == ModContent.TileType<SpookyGrassGreen>())
				{
					WorldGen.SpreadGrass(i, j, Type, ModContent.TileType<SpookyGrassGreen>(), true);
				}
			}
		}
	}

    public class SpookyDirt2 : SpookyDirt
	{
        public override string Texture => "Spooky/Content/Tiles/SpookyBiome/SpookyDirt";
    }
}
