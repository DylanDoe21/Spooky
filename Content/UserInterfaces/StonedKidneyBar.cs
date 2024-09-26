using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.UserInterfaces
{
    public class StonedKidneyBar
    {
        internal const float DefaultXPosition = 50.103603f;
        internal const float DefaultYPosition = 55.765408f;

        private static Asset<Texture2D> BarTexture;
        private static Asset<Texture2D> BarFillTexture;
        
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            Vector2 ScreenPosition = new Vector2(DefaultXPosition, DefaultYPosition);

            Vector2 screenPos = ScreenPosition;
            screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            if (player.GetModPlayer<SpookyPlayer>().StonedKidney)
			{
                BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/StonedKidneyBar", AssetRequestMode.ImmediateLoad);
                BarFillTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/StonedKidneyBarFill", AssetRequestMode.ImmediateLoad);

                float offset = (BarTexture.Width() - BarTexture.Width()) * 0.5f;
                spriteBatch.Draw(BarTexture.Value, screenPos, null, Color.White, 0f, BarTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);

                float completionRatio = (float)player.GetModPlayer<SpookyPlayer>().StonedKidneyCharge / 10f;
                Rectangle barRectangle = new Rectangle(0, 0, BarTexture.Width(), (int)(BarFillTexture.Width() * completionRatio));
                spriteBatch.Draw(BarFillTexture.Value, screenPos + new Vector2(offset, 0), barRectangle, Color.White, 0f, BarFillTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
		}
    }
}