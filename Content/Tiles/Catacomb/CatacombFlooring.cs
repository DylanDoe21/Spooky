using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Tiles.Catacomb
{
	[LegacyName("CatacombTiles")]
	public class CatacombFlooring : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(101, 90, 79));
			RegisterItemDrop(ModContent.ItemType<CatacombFlooringItem>());
			DustType = DustID.Bone;
			HitSound = SoundID.Tink;
		}

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return Flags.downedDaffodil;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			if (!Flags.downedDaffodil)
			{
				MinPick = int.MaxValue;
			}
			else
			{
				MinPick = 0;
			}

			return true;
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
