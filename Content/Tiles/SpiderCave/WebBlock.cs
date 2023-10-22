using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpiderCave
{
	public class WebBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(210, 203, 200));
			HitSound = SoundID.NPCHit11;
            DustType = DustID.Web;
			MineResist = 0.65f;
		}

        public override bool HasWalkDust()
        {
			return true;
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
			dustType = DustID.Web;

			Main.LocalPlayer.velocity.X *= 0.97f;
        }
    }
}
