using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Tiles.Catacomb
{
	[LegacyName("CatacombBrick")]
	public class CatacombBrick1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(73, 82, 85));
			RegisterItemDrop(ModContent.ItemType<CatacombBrick1Item>());
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 288; //288 is the width of each individual sheet
        }

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return Flags.downedDaffodil && tileTypeBeingPlaced != ModContent.TileType<CatacombBrick1Safe>();
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

	public class CatacombBrick1Arena : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick1";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(73, 82, 85));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
		{
			frameXOffset = i % 3 * 288; //288 is the width of each individual sheet
		}

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return false;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return false;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}
	}

	public class CatacombBrick1Safe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick1";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(73, 82, 85));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 288; //288 is the width of each individual sheet
        }
    }
}
