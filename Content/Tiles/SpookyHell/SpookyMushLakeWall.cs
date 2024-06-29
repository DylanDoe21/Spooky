using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class SpookyMushLakeWall : ModWall 
    {
        private static Asset<Texture2D> GlowTexture1;
        private static Asset<Texture2D> GlowTexture2;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(19, 14, 37));
            DustType = DustID.Blood;
            HitSound = SoundID.Dig;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
			GlowTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/SpookyMushLakeWallGlow1");
            GlowTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/SpookyMushLakeWallGlow2");

			Tile tile = Main.tile[i, j];
			Tile tileAbove = Main.tile[i, j - 1];

			Rectangle frame = new Rectangle(tile.WallFrameX, tile.WallFrameY, 32, 32);
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (Main.drawToScreen)
				zero = Vector2.Zero;

			Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            float fade1 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;
            float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

            float time = Main.GameUpdateCount * 0.01f;

			float intensity1 = 0.7f;
			intensity1 *= (float)MathF.Sin(-j / 8f + time + i);

            float intensity2 = 0.7f;
			intensity2 *= (float)MathF.Sin(-i / 8f + time + j);

			intensity1 += 0.7f;
            intensity2 += 0.7f;

			Main.spriteBatch.Draw(GlowTexture1.Value, pos + new Vector2(-8, -8), frame, Color.White * intensity1, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(GlowTexture2.Value, pos + new Vector2(-8, -8), frame, Color.White * intensity2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

			//spawn water infront of the wall
			if ((tile.LiquidAmount == 0 || tile.LiquidType == LiquidID.Water) && !tile.HasTile)
            {
				tile.Get<LiquidData>().LiquidType = LiquidID.Water;
                tile.LiquidAmount = byte.MaxValue;
                WorldGen.SquareTileFrame(i, j);

                if (Main.netMode != NetmodeID.MultiplayerClient && Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.sendWater(i, j);
                }
            }
        }
    }
}