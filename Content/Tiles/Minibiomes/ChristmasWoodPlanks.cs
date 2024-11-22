using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Generation.Minibiomes;
using Spooky.Content.Tiles.Minibiomes.Ambient;
using Spooky.Content.Tiles.Minibiomes.Tree;

namespace Spooky.Content.Tiles.Minibiomes
{
	public class ChristmasWoodPlanks : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(171, 97, 70));
            DustType = DustID.WoodFurniture;
		}
	}
}
