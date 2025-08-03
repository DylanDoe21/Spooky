using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.UserInterfaces
{
	public class KrampusDialogueUI
	{
		public static int CurrentText = 0;
		public static int CurrentTextTimer = 0;
		public static bool SetPeriodDelay = false;
		public static string RealTextToDisplay = string.Empty;
		public static int Krampus = -1;
		public static bool UIOpen = false;
		public static bool IsHoveringOverAnyButton = false;

		public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

		public static void Draw(SpriteBatch spriteBatch)
		{
			Player player = Main.LocalPlayer;
			Mod Mod = Spooky.mod;

			//dont draw at all if the UI isnt open
			if (!UIOpen)
			{
				Krampus = -1;
				CurrentText = 0;
				CurrentTextTimer = 0;
				RealTextToDisplay = string.Empty;
				return;
			}

			//stop the UI from being open if the player is doing other stuff
			if (player.chest != -1 || player.sign != -1 || player.talkNPC == -1 || !InRangeOfNPC() || Main.InGuideCraftMenu)
			{
				UIOpen = false;
				return;
			}

			Main.npcChatText = string.Empty;

			Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/KrampusDialogueBox").Value;
			Vector2 UIBoxScale = Vector2.One * Main.UIScale;

			//draw the main UI box
			spriteBatch.Draw(UIBoxTexture, UITopLeft, null, Color.White, 0f, UIBoxTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

			DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale,
			"YOU ARE IN THE MUSHBLIGHT. GO DEEPER AS THERE MAY BE NEW CONTENT DEEPER. I learned this off of the internet along with the phrase 'Gurt games ain't no game'", Color.White);

			//prevent any mouse interactions while the mouse is hovering over this UI
			if (IsMouseOverUIBox(UITopLeft, UIBoxTexture, UIBoxScale))
			{
				IsHoveringOverAnyButton = false;

				player.mouseInterface = true;
			}
		}

		public static bool InRangeOfNPC()
		{
			if (!Main.npc.IndexInRange(Krampus) || !Main.npc[Krampus].active)
			{
				return false;
			}

			Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);

			return validTalkArea.Intersects(Main.npc[Krampus].Hitbox);
		}

		public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Text, Color color)
		{
			string RealTextToDisplay = string.Empty;

			Vector2 scale = new Vector2(0.9f, 0.925f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			if (CurrentText < Text.Length)
			{
				CurrentTextTimer++;

				if (CurrentTextTimer >= 2)
				{
					CurrentText++;
					CurrentTextTimer = 0;
				}
			}
			for (int i = 0; i < CurrentText; i++)
			{
				RealTextToDisplay += Text[i];
				if (Text[CurrentText - 1].ToString() == "." && !SetPeriodDelay)
				{
					CurrentTextTimer = -20;
					SetPeriodDelay = true;
				}
				else if (Text[CurrentText - 1].ToString() != "." && SetPeriodDelay)
				{
					SetPeriodDelay = false;
				}
			}

			//first draw the mission description
			foreach (string TextLine in Utils.WordwrapString(RealTextToDisplay, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, color, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}
		}

		//used to draw individual icons over the main UI box
		public static void DrawIcon(SpriteBatch spriteBatch, Vector2 drawPositionTopLeft, Texture2D texture)
		{
			spriteBatch.Draw(texture, drawPositionTopLeft, null, Color.White, 0f, Vector2.Zero, Main.UIScale, SpriteEffects.None, 0f);
		}

		//check if the mouse is hovering over a specific button or UI box
		public static bool IsMouseOverUI(Vector2 TopLeft, Texture2D texture, Vector2 backgroundScale)
		{
			Rectangle backgroundArea = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, (int)(texture.Width * backgroundScale.X), (int)(texture.Height * backgroundScale.Y));

			if (backgroundArea.Contains(Main.mouseX, Main.mouseY))
			{
				return true;
			}
			else
			{
				return false;
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