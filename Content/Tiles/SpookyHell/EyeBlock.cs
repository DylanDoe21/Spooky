using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpookyHell.Ambient;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class EyeBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(135, 7, 0));
            DustType = DustID.Blood;
            HitSound = SoundID.NPCHit20;
            MineResist = 0.7f;
		}

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (WorldGen.genRand.NextBool(3) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
            }
        }

        public override bool HasWalkDust()
        {
            return true;
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            dustType = DustID.Blood;
            makeDust = true;
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            bool isPlayerNear = WorldGen.PlayerLOS(i, j);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Main.gamePaused && Main.instance.IsActive && !Above.HasTile && isPlayerNear)
            {
                if (Main.rand.NextBool(550))
                {
                    int newDust = Dust.NewDust(new Vector2((i) * 16, (j - 1) * 16), 5, 5, ModContent.DustType<SpookyHellParticle>());

                    Main.dust[newDust].velocity.Y += 0.09f;
                }
            }
        }
    }
}
