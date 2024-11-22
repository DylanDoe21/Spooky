using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes
{
	public class JungleSoil : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(61, 63, 67));
            DustType = 109;
			MineResist = 0.65f;
		}
	}
}
