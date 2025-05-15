using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
	public class ChristmasSlabRed : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(134, 33, 21));
            DustType = -1;
			HitSound = SoundID.Tink;
		}
	}

	public class ChristmasSlabBlue : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(50, 56, 116));
            DustType = -1;
			HitSound = SoundID.Tink;
		}
	}

	public class ChristmasSlabGreen : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(33, 104, 49));
            DustType = -1;
			HitSound = SoundID.Tink;
		}
	}
}
