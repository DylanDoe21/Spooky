using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using Spooky.Core;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Spooky.Content.UserInterfaces
{
    public class BloomBuffUI
    {
        public static bool UIOpen = false;
        public static bool IsDragging = false;

        private static Asset<Texture2D> BarTexture;
		private static Asset<Texture2D> DaffodilLockTexture;
		private static Asset<Texture2D> BigBoneLockTexture;

		public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

            //dont draw this ui if its transparent
            if (player.GetModPlayer<BloomBuffsPlayer>().UITransparency <= 0f || player.ghost || player.dead)
            {
                return;
            }

			//set textures
            BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomBuffUI");
			DaffodilLockTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomIcons/BloomBuffsSlotDaffodilLock");
			BigBoneLockTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomIcons/BloomBuffsSlotBigBoneLock");

			//UI box scaling
			Vector2 UIBoxScale = Vector2.One * Main.UIScale;

			//UI dragging 
			MouseState mouse = Mouse.GetState();

            //only allow UI dragging if the config option is on and the player is not in the inventory
            if (ModContent.GetInstance<SpookyConfig>().DraggableUI && !Main.playerInventory)
            {
                //if the player is hovering over the UI panel and presses left click then allow dragging
                if (IsMouseOverUI(player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos, BarTexture.Value, UIBoxScale) && !IsDragging && mouse.LeftButton == ButtonState.Pressed)
                {
                    IsDragging = true;
                }

                //if the player is dragging and continues to hold mouse left, then move the ui to the center of the mouse
                if (IsDragging && mouse.LeftButton == ButtonState.Pressed)
                {
                    player.mouseInterface = true;
                    player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos = Main.MouseScreen;
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
            spriteBatch.Draw(BarTexture.Value, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos, null, Color.White * player.GetModPlayer<BloomBuffsPlayer>().UITransparency, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            //bloom buff icon drawing for each slot
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[0] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos - new Vector2(66f, 0f) * Main.UIScale, 0);
				DisplayTimeText(spriteBatch, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos - new Vector2(82f, -12f) * Main.UIScale, player.GetModPlayer<BloomBuffsPlayer>().Duration1);
			}
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[1] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos - new Vector2(22f, 0f) * Main.UIScale, 1);
				DisplayTimeText(spriteBatch, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos - new Vector2(38f, -12f) * Main.UIScale, player.GetModPlayer<BloomBuffsPlayer>().Duration2);
			}
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[2] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos + new Vector2(22.5f, 0f) * Main.UIScale, 2);
				DisplayTimeText(spriteBatch, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos + new Vector2(6.5f, 12f) * Main.UIScale, player.GetModPlayer<BloomBuffsPlayer>().Duration3);
			}
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[3] != string.Empty)
            {
                DrawIcon(spriteBatch, player, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos + new Vector2(66.5f, 0f) * Main.UIScale, 3);
				DisplayTimeText(spriteBatch, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos + new Vector2(50.5f, 12f) * Main.UIScale, player.GetModPlayer<BloomBuffsPlayer>().Duration4);
			}

            //draw locked overlays if the player doesnt have those respective slots unlocked yet
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[2] == string.Empty && !player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot3)
            {
				spriteBatch.Draw(DaffodilLockTexture.Value, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos, null, Color.White * player.GetModPlayer<BloomBuffsPlayer>().UITransparency, 0f, DaffodilLockTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
			}
            if (player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[3] == string.Empty && !player.GetModPlayer<BloomBuffsPlayer>().UnlockedSlot4)
            {
				spriteBatch.Draw(BigBoneLockTexture.Value, player.GetModPlayer<BloomBuffsPlayer>().BloomUIPos, null, Color.White * player.GetModPlayer<BloomBuffsPlayer>().UITransparency, 0f, BigBoneLockTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
			}
        }

        //used to draw individual buff icons over the main UI box
        public static void DrawIcon(SpriteBatch spriteBatch, Player player, Vector2 IconPos, int SlotToCheckFor)
        {
			Vector2 UIBoxScale = Vector2.One * Main.UIScale;

			//get the texture and display name based on the name of the bloom string in the slot to check for
			//REMINDER: this is not the best way to implement it, but this always assumes that the string in the slot to check for matches the exact display name of the bloom item
			//the same goes for that blooms corresponding icon, where its file name is the exact file name as the bloom item + "Icon"
			Texture2D IconTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomIcons/" + player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[SlotToCheckFor] + "Icon").Value;
			string BuffDisplayName = Language.GetTextValue("Mods.Spooky.Items." + player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[SlotToCheckFor] + ".DisplayName");

            spriteBatch.Draw(IconTexture, IconPos, null, Color.White * player.GetModPlayer<BloomBuffsPlayer>().UITransparency, 0f, IconTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            //only display text if the player is hovering over the UI, and the inventory is not open
            if (IsMouseOverUI(IconPos, IconTexture, Vector2.One * Main.UIScale * 0.9f) && !Main.playerInventory)
            {
                //if the player has the dragon fruit buff, then also display the dragon fruit buff stacks as part of the description
                if (IconTexture == ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/BloomIcons/DragonfruitIcon").Value)
                {
                    Main.instance.MouseText(BuffDisplayName + "\n" + 
                    Language.GetTextValue("Mods.Spooky.UI.BloomBuffs.DivaStacks") + " " + player.GetModPlayer<BloomBuffsPlayer>().DragonfruitStacks + "/10" + "\n" +
                    Language.GetTextValue("Mods.Spooky.UI.BloomBuffs.RightClick"));
                }
                else
                {
					Main.instance.MouseText(BuffDisplayName + "\n" + 
                    Language.GetTextValue("Mods.Spooky.UI.BloomBuffs.RightClick"));
                }

				//remove the buff if the player right clicks the icon on the ui
				if (Main.mouseRightRelease && Main.mouseRight)
				{
					player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots[SlotToCheckFor] = string.Empty;
				}
            }
        }

		public static void DisplayTimeText(SpriteBatch spriteBatch, Vector2 TextTopLeft, int DurationToCheckFor)
		{
			Vector2 scale = new Vector2(0.85f, 0.825f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			TimeSpan time = TimeSpan.FromSeconds(DurationToCheckFor / 60);
			string actualTime = string.Format("{0:D2}:{1:D2}", time.Minutes, time.Seconds);

			//bloom buff timeleft text
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, actualTime, TextTopLeft, 
			Color.White * Main.LocalPlayer.GetModPlayer<BloomBuffsPlayer>().UITransparency, 0f, Vector2.Zero, scale);
			TextTopLeft.Y += Main.UIScale * 16f;
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