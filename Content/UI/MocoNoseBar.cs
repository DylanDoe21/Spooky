using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using Spooky.Core;

namespace Spooky.Content.UI
{
    public class MocoNoseBar
    {
        internal const float DefaultXPosition = 50.103603f;
        internal const float DefaultYPosition = 55.765408f;
        
        private static Texture2D BarTexture;
        private static Texture2D BarFillTexture;
        private static Texture2D BarFullTexture;

        internal static void Load()
        {
            BarTexture = ModContent.Request<Texture2D>("Spooky/Content/UI/MocoNoseBar", AssetRequestMode.ImmediateLoad).Value;
            BarFillTexture = ModContent.Request<Texture2D>("Spooky/Content/UI/MocoNoseBarFill", AssetRequestMode.ImmediateLoad).Value;
            BarFullTexture = ModContent.Request<Texture2D>("Spooky/Content/UI/MocoNoseBarFull", AssetRequestMode.ImmediateLoad).Value;
        }

        internal static void Unload()
        {
            BarTexture = null;
            BarFillTexture = null;
            BarFullTexture = null;
        }
        
        public static void Draw(SpriteBatch spriteBatch, Player player)
        {
            // Sanity check the planned position before drawing
            Vector2 screenRatioPosition = new Vector2(DefaultXPosition, DefaultYPosition);

            // Convert the screen ratio position to an absolute position in pixels
            // Cast to integer to prevent blurriness which results from decimal pixel positions
            float uiScale = Main.UIScale;
            Vector2 screenPos = screenRatioPosition;
            screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

            if (player.GetModPlayer<SpookyPlayer>().MocoNose)
			{
				DrawMocoBar(spriteBatch, player, screenPos);
			}
		}

        private static void DrawMocoBar(SpriteBatch spriteBatch, Player player, Vector2 screenPos)
		{
            float uiScale = Main.UIScale;
            float offset = (BarTexture.Width - BarTexture.Width) * 0.5f;
            spriteBatch.Draw(BarTexture, screenPos, null, Color.White, 0f, BarTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

            float completionRatio = (float)player.GetModPlayer<SpookyPlayer>().MocoBoogerCharge / 15f;
            Rectangle barRectangle = new Rectangle(0, 0, (int)(BarFillTexture.Width * completionRatio), BarTexture.Width);
            spriteBatch.Draw(BarFillTexture, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White, 0f, BarFillTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
        
            if (player.GetModPlayer<SpookyPlayer>().MocoBoogerCharge >= 15)
            {
                spriteBatch.Draw(BarFullTexture, screenPos, null, Color.White, 0f, BarFullTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
            }
        }
    }
}