using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Desert
{
	public class PlantFossil : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(142, 118, 46));
            DustType = DustID.YellowStarfish;
			HitSound = SoundID.Tink;
		}
	}
}
