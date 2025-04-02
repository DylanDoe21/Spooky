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
    //TODO:
    //make icons display using the locked texture once a bounty has been accepted
    //allow players to click the icon of the quest they accepted in case they loose the item to attract the miniboss (will likely require a separate accepted bool for each quest)
    public class LittleEyeQuestUI
    {
        public static int LittleEye = -1;
        public static bool UIOpen = false;
        public static bool IsHoveringOverAnyButton = false;

        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

        //actual icon textures
        private static Asset<Texture2D> BountyIcon1Done;
        private static Asset<Texture2D> BountyIcon1NotDone;
        private static Asset<Texture2D> BountyIcon2Done;
        private static Asset<Texture2D> BountyIcon2NotDone;
        private static Asset<Texture2D> BountyIcon3Done;
        private static Asset<Texture2D> BountyIcon3NotDone;
        private static Asset<Texture2D> BountyIcon4Done;
        private static Asset<Texture2D> BountyIcon4NotDone;
        private static Asset<Texture2D> BountyIcon5Done;
        private static Asset<Texture2D> BountyIcon5NotDone;
        
        //misc icon textures
        private static Asset<Texture2D> BountyIconSelectedOutline;
        private static Asset<Texture2D> BountyIconLocked;
        private static Asset<Texture2D> BountyIcon5Locked;

        public static void Draw(SpriteBatch spriteBatch)
        {
            Player player = Main.LocalPlayer;

			Mod Mod = Spooky.mod;

			//dont draw at all if the UI isnt open
			if (!UIOpen)
            {
                LittleEye = -1;
                return;
            }

            //stop the UI from being open if the player is doing other stuff
            if (player.chest != -1 || player.sign != -1 || player.talkNPC == -1 || !InRangeOfNPC() || Main.InGuideCraftMenu)
            {
                UIOpen = false;
                return;
            }

            Main.npcChatText = string.Empty;

            Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestUIBar").Value;
            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            //draw the main UI box
            spriteBatch.Draw(UIBoxTexture, UITopLeft, null, Color.White, 0f, UIBoxTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            //prevent any mouse interactions while the mouse is hovering over this UI
            if (IsMouseOverUI(UITopLeft, UIBoxTexture, UIBoxScale))
            {
                IsHoveringOverAnyButton = false;

                player.mouseInterface = false;
                Main.blockMouse = true;
            }

            Point ButtonTopLeft = (UITopLeft + new Vector2(-525f, -110f) * UIBoxScale).ToPoint();

			//define all the string values needed for the UI
			string QuestIcon1Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty1");
			string Quest1ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty1Condition");

			string QuestIcon2Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty2");
			string Quest2ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty2Condition");

			string QuestIcon3Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty3");
			string Quest3ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty3Condition");

			string QuestIcon4Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty4");
			string Quest4ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty4Condition");

			string QuestIcon5LockedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Locked");
			string QuestIcon5Text = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5");
			string Quest5ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Condition");

			string QuestAcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccept");
			string QuestWarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyWarning");
			string Quest5AcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Accept");
			string Quest5WarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Warning");

            string QuestAcceptedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccepted");

			string QuestCompleteText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompleted");
            string QuestCompleteRematchText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompletedItem");

            //actual icon textures
            BountyIcon1Done ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon1Done");
            BountyIcon1NotDone ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon1NotDone");
            BountyIcon2Done ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon2Done");
            BountyIcon2NotDone ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon2NotDone");
            BountyIcon3Done ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon3Done");
            BountyIcon3NotDone ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon3NotDone");
            BountyIcon4Done ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon4Done");
            BountyIcon4NotDone ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon4NotDone");
            BountyIcon5Done ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5Done");
            BountyIcon5NotDone ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5NotDone");
            
            //misc icon textures
            BountyIconSelectedOutline ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIconSelectedOutline");
            BountyIconLocked ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIconLocked");
            BountyIcon5Locked ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5Locked");


			//draw each bounty icon and display text when hovering over it

			//First bounty
			Vector2 Icon1TopLeft = ButtonTopLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;

            DrawIcon(spriteBatch, Icon1TopLeft, Flags.BountyInProgress ? BountyIconLocked.Value : (Flags.LittleEyeBounty1 ? BountyIcon1Done.Value : BountyIcon1NotDone.Value));

            if (IsMouseOverUI(Icon1TopLeft, BountyIcon1Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon1TopLeft, BountyIconSelectedOutline.Value);

                if (Flags.BountyInProgress)
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
                }
				else if (Flags.LittleEyeBounty1)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && !player.HasItem(ModContent.ItemType<SummonItem1>()))
					{
						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem1>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						UIOpen = false;
					}
				}
                else
                {
				    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon1Text, Quest1ConditionText, QuestAcceptText, QuestWarningText, Color.OrangeRed);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest1Accepted");

						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem1>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						if (Main.netMode != NetmodeID.SinglePlayer)
						{
							ModPacket packet = Mod.GetPacket();
							packet.Write((byte)SpookyMessageType.BountyAccepted);
							packet.Send();
						}
						else
						{
							Flags.BountyInProgress = true;
						}

						UIOpen = false;
                    }
                }
            }

            //evil book display stuff
            Vector2 Icon2TopLeft = ButtonTopLeft.ToVector2() + new Vector2(400f, -24f) * Main.UIScale;

			DrawIcon(spriteBatch, Icon2TopLeft, Flags.BountyInProgress ? BountyIconLocked.Value : (Flags.LittleEyeBounty2 ? BountyIcon2Done.Value : BountyIcon2NotDone.Value));

			if (IsMouseOverUI(Icon2TopLeft, BountyIcon2Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon2TopLeft, BountyIconSelectedOutline.Value);

				if (Flags.BountyInProgress)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
				}
				else if (Flags.LittleEyeBounty2)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && !player.HasItem(ModContent.ItemType<SummonItem2>()))
					{
						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem2>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						UIOpen = false;
					}
				}
				else
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon2Text, Quest2ConditionText, QuestAcceptText, QuestWarningText, Color.SeaGreen);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest2Accepted");

						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem2>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            ModPacket packet = Mod.GetPacket();
                            packet.Write((byte)SpookyMessageType.BountyAccepted);
                            packet.Send();
                        }
                        else
                        {
                            Flags.BountyInProgress = true;
                        }

                        UIOpen = false;
                    }
                }
            }

            //spider grotto display stuff
            Vector2 Icon3TopLeft = ButtonTopLeft.ToVector2() + new Vector2(485f, -24f) * Main.UIScale;

			DrawIcon(spriteBatch, Icon3TopLeft, Flags.BountyInProgress ? BountyIconLocked.Value : (Flags.LittleEyeBounty3 ? BountyIcon3Done.Value : BountyIcon3NotDone.Value));

			if (IsMouseOverUI(Icon3TopLeft, BountyIcon3Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon3TopLeft, BountyIconSelectedOutline.Value);

				if (Flags.BountyInProgress)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
				}
				else if (Flags.LittleEyeBounty3)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && !player.HasItem(ModContent.ItemType<SummonItem3>()))
					{
						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem3>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						UIOpen = false;
					}
				}
				else
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon3Text, Quest3ConditionText, QuestAcceptText, QuestWarningText, Color.Chocolate);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest3Accepted");

						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem3>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            ModPacket packet = Mod.GetPacket();
                            packet.Write((byte)SpookyMessageType.BountyAccepted);
                            packet.Send();
                        }
                        else
                        {
                            Flags.BountyInProgress = true;
                        }

                        UIOpen = false;
                    }
                }
            }

            //eye wizard display stuff
            Vector2 Icon4TopLeft = ButtonTopLeft.ToVector2() + new Vector2(570f, -24f) * Main.UIScale;

			DrawIcon(spriteBatch, Icon4TopLeft, Flags.BountyInProgress ? BountyIconLocked.Value : (Flags.LittleEyeBounty4 ? BountyIcon4Done.Value : BountyIcon4NotDone.Value));

			if (IsMouseOverUI(Icon4TopLeft, BountyIcon4Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon4TopLeft, BountyIconSelectedOutline.Value);

				if (Flags.BountyInProgress)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
				}
				else if (Flags.LittleEyeBounty4)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && !player.HasItem(ModContent.ItemType<SummonItem4>()))
					{
						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem4>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						UIOpen = false;
					}
				}
				else
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon4Text, Quest4ConditionText, QuestAcceptText, QuestWarningText, Color.HotPink);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest4Accepted");

						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<SummonItem4>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            ModPacket packet = Mod.GetPacket();
                            packet.Write((byte)SpookyMessageType.BountyAccepted);
                            packet.Send();
                        }
                        else
                        {
                            Flags.BountyInProgress = true;
                        }

                        UIOpen = false;
                    }
                }
            }

            //orroboro display stuff
            Vector2 Icon5TopLeft = ButtonTopLeft.ToVector2() + new Vector2(655f, -24f) * Main.UIScale;

            bool downedAllMechs = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;

			DrawIcon(spriteBatch, Icon5TopLeft, !downedAllMechs ? BountyIcon5Locked.Value : (Flags.downedOrroboro ? BountyIcon5Done.Value : BountyIcon5NotDone.Value));

			if (IsMouseOverUI(Icon5TopLeft, BountyIcon5Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon5TopLeft, BountyIconSelectedOutline.Value);

				if (!downedAllMechs)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestIcon5LockedText, string.Empty, string.Empty, Color.Red);
				}
				else
				{
					//if you have killed orro-boro display the quest as complete
					if (Flags.downedOrroboro)
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, string.Empty, Color.Lime);
					}
					//display the actual quest text if you havent killed orro-boro but you killed the mechs
					else
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon5Text, Quest5ConditionText, Quest5AcceptText, Quest5WarningText, Color.Magenta);
					}

					//accept bounty (this specific bounty does not need to set the bounty accepted bool to true)
					if (Main.mouseLeftRelease && Main.mouseLeft && !player.HasItem(ModContent.ItemType<Concoction>()))
					{
						if (!Flags.downedOrroboro)
						{
							Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest5Accepted");
						}

						int newItem = Item.NewItem(player.GetSource_DropAsItem(), player.Hitbox, ModContent.ItemType<Concoction>());

						if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
						}

						UIOpen = false;
					}
				}
            }
        }

        public static bool InRangeOfNPC()
        {
            if (!Main.npc.IndexInRange(LittleEye) || !Main.npc[LittleEye].active)
            {
                return false;
            }

            Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);
            
            return validTalkArea.Intersects(Main.npc[LittleEye].Hitbox);
        }

		public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Description, string Condition, string Accept, string Warning, Color ConditionColor)
		{
			Vector2 scale = new Vector2(0.9f, 0.925f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			//this probably doesnt look pretty, but since text wrapping didnt work with manually coloring the text in localization I had to do this instead
			//basically it takes all 4 parts of the text for each bounty and just draws each one separately below each other with their own individual colors

			//first draw the mission description
			foreach (string TextLine in Utils.WordwrapString(Description, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}
				
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, new Color(130, 132, 255), 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//then draw the condition text for the biome you find the miniboss in
			foreach (string TextLine in Utils.WordwrapString(Condition, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, ConditionColor, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//draw the text to tell players they have to click the button to accept the bounty
			foreach (string TextLine in Utils.WordwrapString(Accept, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, Color.Lime, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//finally display the warning that you cant accept another bounty until the selected one is done
			foreach (string TextLine in Utils.WordwrapString(Warning, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, Color.Red, 0f, Vector2.Zero, scale);
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
    }
}