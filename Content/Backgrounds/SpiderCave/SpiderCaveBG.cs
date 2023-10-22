using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Biomes;

namespace Spooky.Content.Backgrounds.SpookyHell
{
    public class SpiderCaveBG : UndergroundBGType
    {
        public override string TexturePath => "Spooky/Content/Backgrounds/SpiderCave/SpiderCaveBG";

        public override bool IsActive()
        {
            return Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>());
        }

        public override Color DrawColor => new Color(100, 100, 100);
    }

    internal class UndergroundBGManager
    {
        public static int CurrentBGID = -1;
        public static UndergroundBGType[] UndergroundBGs;

        public static void Load()
        {
            if (Main.dedServ)
                return;

            CurrentBGID = -1;
            UndergroundBGs = new UndergroundBGType[0];

            On_Main.DrawBackgroundBlackFill += DrawUndergroundBG;
        }

        public static void Unload()
        {
            UndergroundBGs = null;
        }

        private static void DrawUndergroundBG(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            int oldID = CurrentBGID;
            CurrentBGID = -1;
            
            if (Main.gameMenu || Main.screenPosition.Y + Main.screenHeight < ((int)Main.worldSurface) * 16f)
            {
                orig(self);

                return;
            }
            for (int i = 0; i < UndergroundBGs.Length; i++)
            {
                if (UndergroundBGs[i].IsActive())
                {
                    CurrentBGID = i;
                    break;
                }
            }
            if (oldID != CurrentBGID && oldID != -1)
            {
                CurrentBGID = oldID;
                oldID = -2;
                UndergroundBGs[CurrentBGID].Transparency -= UndergroundBGs[CurrentBGID].TransitionSpeed;
            }
            else if (CurrentBGID == -1)
            {
                orig(self);
                return;
            }

            var currentBG = UndergroundBGs[CurrentBGID];
            float transparency = 1f;

            if (currentBG.Transparency < 1f)
            {
                orig(self);
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
            float num = (1f) * 0.5f * 200f;
            int bg0Height = ModContent.Request<Texture2D>(currentBG.TexturePath + "0").Height();
            bg0Height += bg0Height >> 1;
            float Scale = 1.2f;
            
            for (int Layers = 3; Layers >= 0; Layers--)
            {
                //get each background texture
                Texture2D BGTexture = ModContent.Request<Texture2D>(currentBG.TexturePath + Layers).Value;

                Vector2 vector2 = new Vector2(BGTexture.Width, BGTexture.Height) * 0.5f;
                float num2 = (Layers * 2 + 3f);
                Vector2 vector3 = new Vector2(1f / num2);
                Rectangle rectangle = new Rectangle(0, 0, BGTexture.Width, BGTexture.Height);
                Vector2 zero = Vector2.Zero;

                switch (Layers)
                {
                    case 0:
                    {
                        zero.Y -= 10f;
                        break;
                    }
                    case 1:
                    {
                        zero.Y += 30f;
                        break;
                    }
                    case 2:
                    {
                        zero.Y += 35f;
                        break;
                    }
                    case 3:
                    {
                        zero.Y -= 20f;
                        break;
                    }
                }

                vector2 *= Scale;

                zero.Y -= num;
                float num5 = Scale * rectangle.Width;
                int num6 = (int)((vector.X * vector3.X - vector2.X + zero.X - (Main.screenWidth >> 1)) / num5);

                for (int j = num6 - 2; j < num6 + 4 + (int)(Main.screenWidth / num5); j++)
                {
                    Vector2 drawPosition = (new Vector2(j * Scale * (rectangle.Width / vector3.X), (Main.maxTilesY - 585) * 16f) + vector2 - vector) * vector3 + vector - Main.screenPosition - vector2 + zero;
                    var frame = rectangle;
                    var clr = currentBG.DrawColor * transparency;

                    if (currentBG.PreDraw(BGTexture, ref drawPosition, ref frame, ref clr, Scale, Layers))
                    {
                        Main.spriteBatch.Draw(BGTexture, drawPosition, frame, clr, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public static void AddUndergroundBG(UndergroundBGType hellBG)
        {
            int id = UndergroundBGs.Length;
            Array.Resize(ref UndergroundBGs, id + 1);
            hellBG.ID = id;
            UndergroundBGs[id] = hellBG;
        }
    }

    public abstract class UndergroundBGType : IAutoload
    {
        public int ID;
        public float Transparency;
        void IAutoload.Load()
        {
            if (Main.dedServ)
            {
                return;
            }
            UndergroundBGManager.AddUndergroundBG(this);
            OnLoad();
        }

        void IAutoload.Unload()
        {
            OnUnload();
        }

        public abstract bool IsActive();
        public abstract string TexturePath { get; }

        public virtual float TransitionSpeed => 0.1f;
        public virtual Color DrawColor => new Color(255, 255, 255, 255);

        protected virtual void OnLoad()
        {
        }

        protected virtual void OnUnload()
        {
        }

        public virtual bool PreDraw(Texture2D texture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, float Scale, int Layer)
        {
            return true;
        }

        public virtual void PostDraw(Texture2D texture, Vector2 drawPosition, Rectangle frame, Color drawColor, float Scale, int Layer)
        {
        }
    }
}