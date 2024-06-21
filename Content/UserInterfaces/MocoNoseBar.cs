using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.UserInterfaces
{
    public class MocoNoseBar
    {
        internal const float DefaultXPosition = 50.103603f;
        internal const float DefaultYPosition = 55.765408f;

        private static Asset<Texture2D> BarTexture;
        private static Asset<Texture2D> BarFillTexture;
        private static Asset<Texture2D> BarFullTexture;
        
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            Vector2 ScreenPosition = new Vector2(DefaultXPosition, DefaultYPosition);

            float uiScale = Main.UIScale;
            Vector2 screenPos = ScreenPosition;
            screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            if (player.GetModPlayer<SpookyPlayer>().MocoNose)
			{
                BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/MocoNoseBar", AssetRequestMode.ImmediateLoad);
                BarFillTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/MocoNoseBarFill", AssetRequestMode.ImmediateLoad);
                BarFullTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/MocoNoseBarFull", AssetRequestMode.ImmediateLoad);

                float offset = (BarTexture.Width() - BarTexture.Width()) * 0.5f;
                spriteBatch.Draw(BarTexture.Value, screenPos, null, Color.White, 0f, BarTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

                float completionRatio = (float)player.GetModPlayer<SpookyPlayer>().MocoBoogerCharge / 15f;
                Rectangle barRectangle = new Rectangle(0, 0, (int)(BarFillTexture.Width() * completionRatio), BarTexture.Width());
                spriteBatch.Draw(BarFillTexture.Value, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White, 0f, BarFillTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
        
                if (player.GetModPlayer<SpookyPlayer>().MocoBoogerCharge >= 15)
                {
                    spriteBatch.Draw(BarFullTexture.Value, screenPos, null, Color.White, 0f, BarFullTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
                }
			}
		}
    }
}