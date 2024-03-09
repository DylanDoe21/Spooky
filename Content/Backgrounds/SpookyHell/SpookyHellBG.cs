using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Backgrounds.SpookyHell
{
    internal static class SpookyHellBG
    {
        public static void Load()
        {
            if (Main.dedServ)
            {
                return;
            }

            On_Main.DrawUnderworldBackground += DrawHellBG;
        }

        public static float Transparency;

        public static float TransitionSpeed => 0.02f;

        private static void DrawHellBG(On_Main.orig_DrawUnderworldBackground orig, Main self, bool flat)
        {
            if (Main.gameMenu || Main.screenPosition.Y + Main.screenHeight < (Main.maxTilesY - 220) * 16f)
            {
                orig(self, flat);

                return;
            }

            orig(self, flat);

            if (ModContent.GetInstance<TileCount>().spookyHellTiles >= 500)
            {
                Transparency += TransitionSpeed;

                if (Transparency > 1f)
                {
                    Transparency = 1f;
                }
            }
            else
            {
                Transparency -= TransitionSpeed;

                if (Transparency < 0f)
                {
                    Transparency = 0f;
                }
            }

            //dont bother running any of the background drawing if the transparency is zero (meaning the background isnt actually active)
            //also do not run any of the background drawing if you have the vanilla background config option turned off
            if (Transparency > 0f && Main.BackgroundEnabled)
            {
                Vector2 vector = Main.screenPosition + new Vector2((Main.screenWidth >> 1), (Main.screenHeight >> 1));
                float num = (Main.GameViewMatrix.Zoom.Y - 1f) * 0.5f * 200f;
                int bg0Height = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/SpookyHell/SpookyHellBG" + "0").Height();
                bg0Height += bg0Height >> 1;
                float Scale = 1.4f;
                for (int Layers = 4; Layers >= 0; Layers--)
                {
                    //get each background texture
                    Texture2D BGTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/SpookyHell/SpookyHellBG" + Layers).Value;

                    Vector2 vector2 = new Vector2(BGTexture.Width, BGTexture.Height) * 0.5f;
                    float num2 = flat ? 1f : (Layers * 2 + 3f);
                    Vector2 vector3 = new Vector2(1f / num2);
                    Rectangle rectangle = new Rectangle(0, 0, BGTexture.Width, BGTexture.Height);
                    Vector2 zero = Vector2.Zero;

                    switch (Layers)
                    {
                        case 0:
                        {
                            zero.Y += 160f;
                            break;
                        }
                        case 1:
                        {
                            zero.Y += 60f;
                            break;
                        }
                        case 2:
                        {
                            zero.Y += 20f;
                            break;
                        }
                        case 3:
                        {
                            zero.Y += 10f;
                            break;
                        }
                        case 4:
                        {
                            zero.Y += 10f;
                            break;
                        }
                    }

                    /*
                    if (flat)
                    {
                        zero.Y += bg0Height * 1.3f - vector2.Y;
                    }
                    */

                    vector2 *= Scale;

                    zero.Y -= num;
                    float num5 = Scale * rectangle.Width;
                    int num6 = (int)((vector.X * vector3.X - vector2.X + zero.X - (Main.screenWidth >> 1)) / num5);

                    for (int j = num6 - 2; j < num6 + 4 + (int)(Main.screenWidth / num5); j++)
                    {
                        Vector2 drawPosition = (new Vector2(j * Scale * (rectangle.Width / vector3.X), (Main.maxTilesY - 200) * 16f) + vector2 - vector) * vector3 + vector - Main.screenPosition - vector2 + zero;
                        var frame = rectangle;
                        var clr = new Color(140, 140, 140) * Transparency;

                        Main.spriteBatch.Draw(BGTexture, drawPosition, frame, clr, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

                        if (Layers < 3)
                        {
                            Texture2D BGTextureGlow = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/SpookyHell/SpookyHellBG" + Layers + "_Glow").Value;

                            Main.spriteBatch.Draw(BGTextureGlow, drawPosition, frame, Color.White * Transparency, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                        }
                    }
                }
            }
        }
    }
}