using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Backgrounds.Cemetery
{
    public class CemeterySky : CustomSky
    {
        public bool Active;
        public float Intensity;

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                Intensity = Math.Min(1f, 0.01f + Intensity);
            }
            else
            {
                Intensity = Math.Max(0f, Intensity - 0.01f);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Cemetery/CemeterySky").Value;
            
            if (maxDepth >= 3E+38f && minDepth < 3E+38f && !Main.gameMenu)
            {
                //Draw the sky box texture
                spriteBatch.Draw(SkyTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), 
                Main.ColorOfTheSkies * 0.95f * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * this.Intensity));
            }

            //deactivate the sky if in the menu
            if (Main.gameMenu || !Main.LocalPlayer.active)  
            {
                Active = false;
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            Intensity = 0.002f;
            Active = true;
        }

        public override void Deactivate(params object[] args)
        {
            Active = false;
        }

        public override void Reset()
        {
            Active = false;
        }

        public override bool IsActive()
        {
            return Active || Intensity > 0.001f;
        }
    }
}