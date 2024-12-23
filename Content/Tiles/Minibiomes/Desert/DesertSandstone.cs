using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Generation.Minibiomes;
using Spooky.Content.Tiles.Minibiomes.Desert.Ambient;

namespace Spooky.Content.Tiles.Minibiomes.Desert
{
	public class DesertSandstone : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(173, 101, 39));
            DustType = DustID.YellowStarfish;
			HitSound = SoundID.Tink;
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);

			//grow broccoli trees
			if (Main.rand.NextBool(50) && TarPits.CanPlaceCactus(i, j) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
			{
				TarPitCactus.Grow(i, j - 1, 5, 12);
			}
		}
	}
}
