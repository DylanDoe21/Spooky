using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
	public class LabMetalPlate : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
            Main.tileBlockLight[Type] = true;
			Main.tileLargeFrames[Type] = 1;
			TileID.Sets.ForcedDirtMerging[Type] = true;
            AddMapEntry(new Color(119, 124, 149));
            DustType = -1;
            HitSound = SoundID.Item52;
		}

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 234;
        }
    }
}
