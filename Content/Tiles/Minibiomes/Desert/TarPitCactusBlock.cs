using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Generation;
using Spooky.Content.Tiles.Minibiomes.Desert.Ambient;

namespace Spooky.Content.Tiles.Minibiomes.Desert
{
	public class TarPitCactusBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(59, 100, 52));
            DustType = DustID.Grass;
		}
	}
}
