using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.NoseTemple
{
	public class NoseTempleBrickRed : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(128, 41, 39));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
			MinPick = int.MaxValue;
		}

		public override bool CanExplode(int i, int j)
        {
			return false;
        }
    }

	public class NoseTempleBrickRedSafe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/NoseTemple/NoseTempleBrickRed";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(128, 41, 39));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}
    }
}
