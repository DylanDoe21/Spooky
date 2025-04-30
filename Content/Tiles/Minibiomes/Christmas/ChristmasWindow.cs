using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasWindow : ModWall 
    {
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
			Main.wallLight[Type] = true;
            DustType = -1;
        }

        public override bool Drop(int i, int j, ref int type)
		{
			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(i / 3f + time);
			intensity += 0.7f;

			r = 222f / 600f * intensity;
            g = 135f / 600f * intensity;
            b = 37f / 600f * intensity;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Tile tileAbove = Main.tile[i, j - 1];

            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasWindowGlow");

            Rectangle frame = new Rectangle(tile.WallFrameX, tile.WallFrameY, 32, 32);
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            float time = Main.GameUpdateCount * 0.01f;

            float intensity = 0.7f;
            intensity *= (float)MathF.Sin(i / 3f + time);
            intensity += 0.7f;

            Main.spriteBatch.Draw(GlowTexture.Value, pos + new Vector2(-8, -8), frame, (Color.White * 0.35f) * intensity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }

    public class ChristmasWindowSafe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasWindow";

        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
			Main.wallLight[Type] = true;
            DustType = -1;
        }

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			float time = Main.GameUpdateCount * 0.01f;

			float intensity = 0.7f;
			intensity *= (float)MathF.Sin(i / 3f + time);
			intensity += 0.7f;

			r = 222f / 600f * intensity;
            g = 135f / 600f * intensity;
            b = 37f / 600f * intensity;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Tile tileAbove = Main.tile[i, j - 1];

            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasWindowGlow");

            Rectangle frame = new Rectangle(tile.WallFrameX, tile.WallFrameY, 32, 32);
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;

            float time = Main.GameUpdateCount * 0.01f;

            float intensity = 0.7f;
            intensity *= (float)MathF.Sin(i / 3f + time);
            intensity += 0.7f;

            Main.spriteBatch.Draw(GlowTexture.Value, pos + new Vector2(-8, -8), frame, (Color.White * 0.35f) * intensity, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}