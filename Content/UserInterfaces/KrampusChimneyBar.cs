using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Microsoft.Xna.Framework.Input;

namespace Spooky.Content.UserInterfaces
{
    public class KrampusChimneyBar
    {
        public static bool IsDragging = false;

        private static Asset<Texture2D> BarTexture;
        private static Asset<Texture2D> BarFillTexture;
        
        public static void Draw(SpriteBatch spriteBatch)
        {
			Player player = Main.LocalPlayer;

			if (player.GetModPlayer<SpookyPlayer>().KrampusChimney)
			{
                BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/KrampusChimneyBar", AssetRequestMode.ImmediateLoad);
                BarFillTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/KrampusChimneyBarFill", AssetRequestMode.ImmediateLoad);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				Vector2 UIBoxScale = (Vector2.One * 0.85f) * Main.UIScale;

				//UI dragging 
				MouseState mouse = Mouse.GetState();

				//only allow UI dragging if the config option is on and the player is not in the inventory
				if (ModContent.GetInstance<SpookyConfig>().DraggableUI && !Main.playerInventory)
				{
					//if the player is hovering over the UI panel and presses left click then allow dragging
					if (IsMouseOverUI(player.GetModPlayer<SpookyPlayer>().ChimneyUIPos, BarTexture.Value, UIBoxScale) && !IsDragging && mouse.LeftButton == ButtonState.Pressed)
					{
						IsDragging = true;
					}

					//if the player is dragging and continues to hold mouse left, then move the ui to the center of the mouse
					if (IsDragging && mouse.LeftButton == ButtonState.Pressed)
					{
						player.mouseInterface = true;
						player.GetModPlayer<SpookyPlayer>().ChimneyUIPos = Main.MouseScreen;
					}

					//if the player lets go of mouse left, stop dragging the UI panel
					if (IsDragging && mouse.LeftButton == ButtonState.Released)
					{
						IsDragging = false;
					}
				}
				else
				{
					//set dragging to false here just to be safe
					IsDragging = false;
				}

				//draw the main UI box
				spriteBatch.Draw(BarTexture.Value, player.GetModPlayer<SpookyPlayer>().ChimneyUIPos, null, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

				float completionRatio = player.GetModPlayer<SpookyPlayer>().KrampusChimneyCharge / 10f;
				Rectangle barRectangle = new Rectangle(0, 0, BarTexture.Width(), (int)(BarFillTexture.Width() * completionRatio));
				spriteBatch.Draw(BarFillTexture.Value, player.GetModPlayer<SpookyPlayer>().ChimneyUIPos, barRectangle, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

				if (player.GetModPlayer<SpookyPlayer>().KrampusChimneyCharge >= 10.5f || player.GetModPlayer<SpookyPlayer>().KrampusChimneyProjTimer > 0)
				{
					spriteBatch.Draw(BarFillTexture.Value, player.GetModPlayer<SpookyPlayer>().ChimneyUIPos, barRectangle, Color.Orange, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
				}

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			}
		}

        //check if the mouse is hovering over the UI
        public static bool IsMouseOverUI(Vector2 TopLeft, Texture2D texture, Vector2 scale)
        {
            Rectangle backgroundArea = new Rectangle((int)TopLeft.X - (int)(texture.Width / 2 * scale.X), 
			(int)TopLeft.Y - (int)(texture.Height / 2 * scale.Y), 
			(int)(texture.Width * scale.X), (int)(texture.Height * scale.Y));

            if (backgroundArea.Contains(Main.mouseX, Main.mouseY))
			{
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}