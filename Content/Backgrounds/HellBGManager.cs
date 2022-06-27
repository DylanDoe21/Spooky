﻿using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Backgrounds
{
    internal static class HellBGManager
    {
        public static int CurrentBGID = -1;
        public static HellBGType[] HellBGs;

        public static void Load()
        {
            CurrentBGID = -1;
            HellBGs = new HellBGType[0];

            On.Terraria.Main.DrawUnderworldBackground += DrawHellBG;
        }

        public static void Unload()
        {
            HellBGs = null;
        }

        private static void DrawHellBG(On.Terraria.Main.orig_DrawUnderworldBackground orig, Main self, bool flat)
        {
            int oldID = CurrentBGID;
            CurrentBGID = -1;
            
            if (Main.gameMenu || Main.screenPosition.Y + Main.screenHeight < (Main.maxTilesY - 220) * 16f)
            {
                orig(self, flat);
                return;
            }
            for (int i = 0; i < HellBGs.Length; i++)
            {
                if (HellBGs[i].IsActive())
                {
                    CurrentBGID = i;
                    break;
                }
            }
            if (oldID != CurrentBGID && oldID != -1)
            {
                CurrentBGID = oldID;
                oldID = -2;
                HellBGs[CurrentBGID].Transparency -= HellBGs[CurrentBGID].TransitionSpeed;
            }
            else if (CurrentBGID == -1)
            {
                orig(self, flat);
                return;
            }

            var currentBG = HellBGs[CurrentBGID];
            float transparency = 1f;

            if (currentBG.Transparency < 1f)
            {
                orig(self, flat);
                if (oldID != -2)
                {
                    currentBG.Transparency += currentBG.TransitionSpeed;

                    if (currentBG.Transparency > 1f)
                    {
                        currentBG.Transparency = 1f;
                    }
                }
                else
                {
                    currentBG.Transparency -= currentBG.TransitionSpeed;

                    if (currentBG.Transparency < 0f)
                    {
                        currentBG.Transparency = 0f;
                    }
                }

                transparency = currentBG.Transparency;
            }

            Vector2 vector = Main.screenPosition + new Vector2((Main.screenWidth >> 1), (Main.screenHeight >> 1));
            float num = (Main.GameViewMatrix.Zoom.Y - 1f) * 0.5f * 200f;
            int bg0Height = ModContent.Request<Texture2D>(currentBG.TexturePath + "0").Height();
            bg0Height += bg0Height >> 1;
            float Scale = 1.4f;
            for (int Layers = 4; Layers >= 0; Layers--)
            {
                Texture2D BGTexture = ModContent.Request<Texture2D>(currentBG.TexturePath + Layers).Value;
                Vector2 vector2 = new Vector2(BGTexture.Width, BGTexture.Height) * 0.5f;
                float num2 = flat ? 1f : (Layers * 2 + 3f);
                Vector2 vector3 = new Vector2(1f / num2);
                Rectangle rectangle = new Rectangle(0, 0, BGTexture.Width, BGTexture.Height);
                Vector2 zero = Vector2.Zero;

                switch (Layers)
                {
                    case 0:
                    {
                        zero.Y += 180f;
                        break;
                    }
                    case 1:
                    {
                        zero.Y += 100f;
                        break;
                    }
                    case 2:
                    {
                        zero.Y += 85f;
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

                if (flat)
                {
                    zero.Y += bg0Height * 1.3f - vector2.Y;
                }

                vector2 *= Scale;

                zero.Y -= num;
                float num5 = Scale * rectangle.Width;
                int num6 = (int)((vector.X * vector3.X - vector2.X + zero.X - (Main.screenWidth >> 1)) / num5);

                for (int j = num6 - 2; j < num6 + 4 + (int)(Main.screenWidth / num5); j++)
                {
                    Vector2 drawPosition = (new Vector2(j * Scale * (rectangle.Width / vector3.X), (Main.maxTilesY - 200) * 16f) + vector2 - vector) * vector3 + vector - Main.screenPosition - vector2 + zero;
                    var frame = rectangle;
                    var clr = currentBG.DrawColor * transparency;

                    if (currentBG.PreDraw(BGTexture, ref drawPosition, ref frame, ref clr, Scale, Layers))
                    {
                        Main.spriteBatch.Draw(BGTexture, drawPosition, frame, clr, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                    }

                    currentBG.PostDraw(BGTexture, drawPosition, frame, clr, Scale, Layers);
                }
            }
        }

        public static void AddHellBG(HellBGType hellBG)
        {
            int id = HellBGs.Length;
            Array.Resize(ref HellBGs, id + 1);
            hellBG.ID = id;
            HellBGs[id] = hellBG;
        }
    }
}