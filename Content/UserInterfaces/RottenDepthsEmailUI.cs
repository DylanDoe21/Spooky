using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Quest;

namespace Spooky.Content.UserInterfaces
{
    public class RottenDepthsEmailUI
    {
        public static bool UIOpen = false;
        public static bool IsHoveringOverAnyButton = false;

        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

		public static Vector2 UIScale = Vector2.Zero;
		public static Vector2 ComputerPos = Vector2.Zero;

        //actual icon textures
        private static Asset<Texture2D> UIBoxTexture;
		private static Asset<Texture2D> UIBoxScanLineTexture;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

			//dont draw at all if the UI isnt open
			if (!UIOpen)
            {
				UIScale = Vector2.Zero;
				ComputerPos = Vector2.Zero;
                return;
            }
		
            //stop the UI from being open if the player is doing other stuff
            if (player.chest != -1 || player.sign != -1 || player.talkNPC == -1 || !InRangeOfComputer() || Main.InGuideCraftMenu)
            {
				ComputerPos = Vector2.Zero;
                UIOpen = false;
                return;
            }

            Main.npcChatText = string.Empty;

            UIBoxTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/RottenDepthsEmailUIBox");
			UIBoxScanLineTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/RottenDepthsEmailUIBoxFilter");
            Vector2 UIBoxScale = Vector2.One * Main.UIScale * UIScale;

			if (UIScale.X < 1f)
			{
				UIScale.X += 0.05f;
			}
			if (UIScale.X > 0.5f)
			{
				if (UIScale.Y < 1f)
				{
					UIScale.Y += 0.05f;
				}
			}

            //draw the main UI box
            spriteBatch.Draw(UIBoxTexture.Value, UITopLeft, null, new Color(124, 177, 99), 0f, UIBoxTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            //prevent any mouse interactions while the mouse is hovering over this UI
            if (IsMouseOverUIBox(UITopLeft, UIBoxTexture.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = false;

                player.mouseInterface = true;
            }

			if (UIScale.X >= 1f && UIScale.Y >= 1f)
			{
				DrawText(spriteBatch, UITopLeft + new Vector2(-480f, -280f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Inbox"), 1000, 1f);
				DrawText(spriteBatch, UITopLeft + new Vector2(-475f, -205f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Sender"), 250, 0.9f);
				DrawText(spriteBatch, UITopLeft + new Vector2(-165f, -205f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Message"), 250, 0.9f);

				DrawText(spriteBatch, UITopLeft + new Vector2(-475f, -165f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailSender1"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-270f, -165f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailTime1"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-165f, -165f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Email1"), 1000);

				DrawText(spriteBatch, UITopLeft + new Vector2(-475f, -100f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailSender2"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-270f, -100f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailTime2"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-165f, -100f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Email2"), 1000);

				DrawText(spriteBatch, UITopLeft + new Vector2(-475f, -35f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailSender3"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-270f, -35f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailTime3"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-165f, -35f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Email3"), 1000);

				DrawText(spriteBatch, UITopLeft + new Vector2(-475f, 30f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailSender4"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-270f, 30f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailTime4"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-165f, 30f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Email4"), 1000);

				DrawText(spriteBatch, UITopLeft + new Vector2(-475f, 115f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailSender5"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-270f, 115f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.EmailTime5"), 250);
				DrawText(spriteBatch, UITopLeft + new Vector2(-165f, 115f) * UIBoxScale, Language.GetTextValue("Mods.Spooky.UI.RottenDepthsTerminal.Email5"), 1000);
			}

			//draw the main UI box
            spriteBatch.Draw(UIBoxScanLineTexture.Value, UITopLeft, null, Color.Lime * 0.1f, 0f, UIBoxTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
		}

        public static bool InRangeOfComputer()
        {
			if (ComputerPos == Vector2.Zero)
			{
				return false;
			}

            return Main.LocalPlayer.Distance(ComputerPos) <= 150f;
        }

		public static void DrawText(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Text, int HorizontalAmount, float scale = 0.75f)
		{
			Vector2 RealScale = new Vector2(scale, scale)  * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			foreach (string TextLine in Utils.WordwrapString(Text, FontAssets.CombatText[1].Value, HorizontalAmount, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}
				
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.CombatText[1].Value, TextLine, TextTopLeft, Color.Lime, 0f, Vector2.Zero, RealScale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}
		}

		//check if the mouse is hovering over the UI
		public static bool IsMouseOverUIBox(Vector2 TopLeft, Texture2D texture, Vector2 scale)
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