using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Generation.Minibiomes;
using Spooky.Content.Tiles.Minibiomes.Ambient;
using Spooky.Content.Tiles.Minibiomes.Tree;

namespace Spooky.Content.Tiles.Minibiomes
{
	public class ChristmasCarpet : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(204, 159, 99));
            DustType = -1;
			HitSound = SoundID.NPCHit11;
			MineResist = 0.65f;
		}

		public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 288; //288 is the width of each individual sheet
        }
	}
}
