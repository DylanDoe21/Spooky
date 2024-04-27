using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb
{
	[LegacyName("CatacombTiles")]
	public class CatacombFlooring : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(101, 90, 79));
			DustType = DustID.Bone;
			HitSound = SoundID.Tink;
			MinPick = int.MaxValue;
		}

        public override bool CanExplode(int i, int j)
        {
			return false;
        }
    }

	public class CatacombFlooringSafe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombFlooring";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(101, 90, 79));
			DustType = DustID.Bone;
			HitSound = SoundID.Tink;
		}
    }
}
