using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Catacomb
{
	public class CatacombBrick2Ghost : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.DrawsWalls[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(87, 52, 37));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

        public override void HitWire(int i, int j)
        {
            Main.tile[i, j].Get<TileWallWireStateData>().IsActuated = !Main.tile[i, j].Get<TileWallWireStateData>().IsActuated;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (tile.Get<TileWallWireStateData>().IsActuated)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 2 * 288; //288 is the width of each individual sheet
			frameYOffset = j % 2 * 270; //270 is the height of each individual sheet
        }

        /*
        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override bool CanExplode(int i, int j)
        {
			return false;
        }
		*/
    }
}
