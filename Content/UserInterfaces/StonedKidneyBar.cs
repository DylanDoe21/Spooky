using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Microsoft.Xna.Framework.Input;

namespace Spooky.Content.UserInterfaces
{
    public class StonedKidneyBar
    {
        public static bool IsDragging = false;

        private static Asset<Texture2D> BarTexture;
        private static Asset<Texture2D> BarFillTexture;
        
        public static void Draw(SpriteBatch spriteBatch)
        {
			Player player = Main.LocalPlayer;

			if (player.GetModPlayer<SpookyPlayer>().StonedKidney)
			{
                BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/StonedKidneyBar", AssetRequestMode.ImmediateLoad);
                BarFillTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/StonedKidneyBarFill", AssetRequestMode.ImmediateLoad);

				Vector2 UIBoxScale = Vector2.One * Main.UIScale;

				//UI dragging 
				MouseState mouse = Mouse.GetState();

				//only allow UI dragging if the config option is on and the player is not in the inventory
				if (ModContent.GetInstance<SpookyConfig>().DraggableUI && !Main.playerInventory)
				{
					//if the player is hovering over the UI panel and presses left click then allow dragging
					if (IsMouseOverUI(player.GetModPlayer<SpookyPlayer>().KidneyUITopLeft, BarTexture.Value, UIBoxScale) && !IsDragging && mouse.LeftButton == ButtonState.Pressed)
					{
						IsDragging = true;
					}

					//if the player is dragging and continues to hold mouse left, then move the ui to the center of the mouse
					if (IsDragging && mouse.LeftButton == ButtonState.Pressed)
					{
						player.mouseInterface = true;
						player.GetModPlayer<SpookyPlayer>().KidneyUITopLeft = Main.MouseScreen - (BarTexture.Size() / 2) * UIBoxScale;
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
				spriteBatch.Draw(BarTexture.Value, player.GetModPlayer<SpookyPlayer>().KidneyUITopLeft, null, Color.White, 0f, Vector2.Zero, UIBoxScale, SpriteEffects.None, 0f);

				float completionRatio = player.GetModPlayer<SpookyPlayer>().StonedKidneyCharge / 10f;
				Rectangle barRectangle = new Rectangle(0, 0, BarTexture.Width(), (int)(BarFillTexture.Width() * completionRatio));
				spriteBatch.Draw(BarFillTexture.Value, player.GetModPlayer<SpookyPlayer>().KidneyUITopLeft, barRectangle, Color.White, 0f, Vector2.Zero, UIBoxScale, SpriteEffects.None, 0f);
			}
		}

        //check if the mouse is hovering over this UI
        public static bool IsMouseOverUI(Vector2 TopLeft, Texture2D texture, Vector2 backgroundScale)
        {
            Rectangle backgroundArea = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, (int)(texture.Width * backgroundScale.X), (int)(texture.Width * backgroundScale.Y));

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