using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class SpookyMush : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            AddMapEntry(new Color(100, 12, 150));
            ItemDrop = ModContent.ItemType<SpookyMushItem>();
			DustType = ModContent.DustType<SpookyHellPurpleDust>();
            HitSound = SoundID.Dig;
		}

        public override void RandomUpdate(int i, int j)
		{
			Tile up = Main.tile[i, j - 1];
			Tile down = Main.tile[i, j + 1];
			Tile left = Main.tile[i - 1, j];
			Tile right = Main.tile[i + 1, j];

            if ((up.TileType == ModContent.TileType<SpookyMushGrass>() || down.TileType == ModContent.TileType<SpookyMushGrass>() || 
            left.TileType == ModContent.TileType<SpookyMushGrass>() || right.TileType == ModContent.TileType<SpookyMushGrass>()))
			{
                if (WorldGen.genRand.Next(1) == 0)
                {
				    WorldGen.SpreadGrass(i, j, Type, ModContent.TileType<SpookyMushGrass>(), false);
                }
			}
        }
	}
}
