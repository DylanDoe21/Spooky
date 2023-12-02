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
        public bool skyActive;
        public float opacity;

        public override void Update(GameTime gameTime)
        {
            if (skyActive && opacity < 1f)
            {
                opacity += 0.01f;
            }
            else if (!skyActive && opacity > 0f)
            {
                opacity -= 0.1f;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D SkyTexture = ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/Cemetery/CemeterySky").Value;
            
            if (maxDepth >= 3E+38f && minDepth < 3E+38f && !Main.gameMenu)
            {
                //Draw the sky box texture
                spriteBatch.Draw(SkyTexture, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), 
                Main.ColorOfTheSkies * 0.95f * Math.Min(1f, (Main.screenPosition.Y - 800f) / 1000f * opacity));
            }

            //deactivate the sky if in the menu
            if (Main.gameMenu || !Main.LocalPlayer.active)  
            {
                skyActive = false;
            }
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            opacity = 0.002f;
            skyActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            skyActive = false;
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0.001f;
        }
    }
}