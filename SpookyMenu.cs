using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Core
{
    internal class SpookyForestMenu : ModMenu
    {
        public override string DisplayName => "Spooky Forest";

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("Spooky/SpookyMenuLogo");
        
        public override int Music => MusicID.ConsoleMenu;
        
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<Content.Backgrounds.SpookyBiome.SpookyForestMenuBG>();

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoDrawCenter -= new Vector2(0, 0);
            logoScale = 0.8f;
            return true;
        }
    }

    internal class SpookyHellMenu : ModMenu
    {
        public override string DisplayName => "Living Hell";

        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("Spooky/SpookyMenuLogo");
        
        public override int Music => MusicID.ConsoleMenu;
        
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<Content.Backgrounds.SpookyHell.SpookyHellMenuBG>();

        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoDrawCenter -= new Vector2(0, 0);
            logoScale = 0.8f;
            return true;
        }

        public override void Update(bool isOnTitleScreen)
        {
            Main.dayTime = true;
            Main.time = 40000;
        }
    }
}
