using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Core
{
    internal class SpookyMenu : ModMenu
    {
        public override string DisplayName => "Spooky Mod";

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyMenu");

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("Spooky/SpookyMenuLogo");

        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blank");

        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blank");
        
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => null;

        public override void OnDeselected()
        {
            //un-hide the sun when this menu is switched
            Main.sunModY = 0;
        }

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            drawColor = Color.White;

            //hides the sun offscreen so you cant click it
            Main.sunModY = -300;

            //set daytime to true 
            Main.time = 27000;
            Main.dayTime = true;

            logoDrawCenter -= new Vector2(0, 0);
            logoScale = 0.8f;

            Texture2D texture = ModContent.Request<Texture2D>("Spooky/SpookyMenu").Value;

            Vector2 drawOffset = Vector2.Zero;
            float xScale = (float)Main.screenWidth / texture.Width;
            float yScale = (float)Main.screenHeight / texture.Height;
            float scale = xScale;

            if (xScale != yScale)
            {
                if (yScale > xScale)
                {
                    scale = yScale;
                    drawOffset.X -= (texture.Width * scale - Main.screenWidth - 10) * 0.5f;
                }
                else
                {
                    drawOffset.Y -= (texture.Height * scale - Main.screenHeight) * 0.5f;
                }
            }

            spriteBatch.Draw(texture, drawOffset, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            return true;
        }
    }
}
