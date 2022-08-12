using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Core
{
    internal class SpookyMenu : ModMenu
    {
        public override string DisplayName => "Spooky Mod";

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
}
