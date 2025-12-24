using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Backgrounds.SpiderCave
{ 
    internal static class SpiderCaveBG
    {
        public static void Load()
        {
            if (Main.dedServ)
            {
                return;
            }

            On_Main.DrawBackgroundBlackFill += DrawUndergroundBG;
        }

        public static float Transparency;

        public static float WarLightBeamOpacity;

        private static void DrawUndergroundBG(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            if (Main.gameMenu || Main.screenPosition.Y + Main.screenHeight < ((int)Main.worldSurface) * 16f)
            {
                orig(self);

                return;
            }

            orig(self);

            if (ModContent.GetInstance<TileCount>().spiderCaveTiles >= 1200)
            {
                Transparency += 0.02f;

                if (Transparency > 1f)
                {
                    Transparency = 1f;
                }
            }
            else
            {
                //make transparency immediately go down so it doesnt look weird, since vanilla underground backgrounds dont have the fade-in effect this background has
                Transparency -= 1f;

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
                float Scale = 1.5f;

                for (int Layers = 4; Layers >= 0; Layers--)
                {
                    //get each background texture
                    Texture2D BGTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/SpiderCave/SpiderCaveBG" + Layers).Value;

                    Vector2 vector2 = new Vector2(BGTexture.Width, BGTexture.Height) * 0.5f;
                    float num2 = (Layers * 2 + 3f);
                    Vector2 vector3 = new Vector2(1f / num2);
                    Rectangle rectangle = new Rectangle(0, 0, BGTexture.Width, BGTexture.Height);
                    Vector2 zero = Vector2.Zero;

                    switch (Layers)
                    {
                        case 0:
                        {
                            zero.Y -= 550f;
                            break;
                        }
                        case 1:
                        {
                            zero.Y -= 70f;
                            break;
                        }
                        case 2:
                        {
                            zero.Y += 350f;
                            break;
                        }
                        case 3:
                        {
                            zero.Y += 0f;
                            break;
                        }
                        case 4:
                        {
                            zero.Y -= 20f;
                            break;
                        }
                    }

                    vector2 *= Scale;
                    
                    zero.Y -= num;
					zero.Y += 50f;
                    float num5 = Scale * rectangle.Width;
                    int num6 = (int)((vector.X * vector3.X - vector2.X + zero.X - (Main.screenWidth >> 1)) / num5);

                    for (int j = num6 - 2; j < num6 + 4 + (int)(Main.screenWidth / num5); j++)
                    {
                        Vector2 drawPosition = (new Vector2(j * Scale * (rectangle.Width / vector3.X), ((Main.LocalPlayer.Center.Y / 16f) - 90) * 16f) + vector2 - vector) * vector3 + vector - Main.screenPosition - vector2 + zero;

                        var frame = rectangle;
						var clr = (Flags.SporeEventHappening || SpiderWarWorld.SpiderWarActive ? new Color(25, 25, 25) : new Color(70, 57, 44)) * Transparency;

                        Main.spriteBatch.Draw(BGTexture, drawPosition, frame, clr, 0f, zero, Scale, SpriteEffects.None, 0f);

                        /*
                        if (Layers == 2)
                        {
                            float time = Main.GameUpdateCount * 0.01f;

                            float intensity1 = 0.7f;
                            intensity1 *= (float)MathF.Sin(8f + time);

                            float intensity2 = 0.7f;
                            intensity2 *= (float)MathF.Cos(8f + time);

                            Texture2D EyesTexture1 = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/SpiderCave/SpiderCaveBGEyes1").Value;
                            Texture2D EyesTexture2 = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/SpiderCave/SpiderCaveBGEyes2").Value;
                            Texture2D EyesTexture3 = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/SpiderCave/SpiderCaveBGEyes3").Value;

                            Main.spriteBatch.Draw(EyesTexture1, drawPosition, frame, Color.Red * intensity1 * Transparency, 0f, zero, Scale, SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(EyesTexture2, drawPosition, frame, Color.Red * intensity2 * Transparency, 0f, zero, Scale, SpriteEffects.None, 0f);
                            Main.spriteBatch.Draw(EyesTexture3, drawPosition, frame, Color.Red * intensity1 * Transparency, 0f, zero, Scale, SpriteEffects.None, 0f);
                        }
                        */

						//reused raveyard sky beams, lazy but it looks good anyways
						if (SpiderWarWorld.SpiderWarActive)
						{
                            if (WarLightBeamOpacity < 1f)
                            {
                                WarLightBeamOpacity += 0.02f;
                            }
                        }
                        else
                        {
                            if (WarLightBeamOpacity > 0f)
                            {
                                WarLightBeamOpacity -= 0.02f;
                            }
                        }

                        if (WarLightBeamOpacity > 0f && Layers != 0)
                        {
							float Rotation = Main.GlobalTimeWrappedHourly * 0.12f * (Layers % 2 == 0 ? -1 : 1);

							Vector2 drawPositionBeam = (new Vector2(j * Scale * (rectangle.Width / vector3.X), ((Main.LocalPlayer.Center.Y / 16f) - 90) * 16f) + vector2 - vector) * vector3 + vector - Main.screenPosition - vector2;

							Texture2D BeamTextureTop = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Cemetery/RaveyardSkyBeam").Value;

							Main.spriteBatch.Draw(BeamTextureTop, drawPositionBeam + new Vector2(0, 1620 - zero.Y), null, Color.White * WarLightBeamOpacity * 0.25f,
							MathF.Sin(Rotation), new Vector2(BeamTextureTop.Width / 2, BeamTextureTop.Height), 1f, SpriteEffects.None, 0f);
						}
					}
                }
            }
        }
    }
}