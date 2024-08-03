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

        public static Rectangle MouseScreenArea => Utils.CenteredRectangle(Main.MouseScreen, Vector2.One * 2f);

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

            Texture2D ButtonSelectedTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIconSelectedOutline").Value;


			//draw each bounty icon and display text when hovering over it

			//First bounty
			Vector2 Icon1TopLeft = ButtonTopLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;

            Texture2D Icon1Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon1TopLeft, Icon1Texture);

            if (IsMouseOverUI(Icon1TopLeft, Icon1Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon1TopLeft, ButtonSelectedTexture);

                if (Flags.BountyInProgress)
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
                }
                else
                {
				    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon1Text, Quest1ConditionText, QuestAcceptText, QuestWarningText, Color.OrangeRed);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        //TODO: spawn item on the player that attracts the miniboss

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

            //chalupo display stuff
            Vector2 Icon2TopLeft = ButtonTopLeft.ToVector2() + new Vector2(400f, -24f) * Main.UIScale;

            Texture2D Icon2Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon1NotDone").Value;
            DrawIcon(spriteBatch, Icon2TopLeft, Icon2Texture);

            if (IsMouseOverUI(Icon2TopLeft, Icon2Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon2TopLeft, ButtonSelectedTexture);

                if (Flags.BountyInProgress)
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
                }
                else
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon2Text, Quest2ConditionText, QuestAcceptText, QuestWarningText, Color.SeaGreen);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        //TODO: spawn item on the player that attracts the miniboss

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

            Texture2D Icon3Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon3NotDone").Value;
            DrawIcon(spriteBatch, Icon3TopLeft, Icon3Texture);

            if (IsMouseOverUI(Icon3TopLeft, Icon3Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon3TopLeft, ButtonSelectedTexture);

                if (Flags.BountyInProgress)
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
                }
                else
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon3Text, Quest3ConditionText, QuestAcceptText, QuestWarningText, Color.Chocolate);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        //TODO: spawn item on the player that attracts the miniboss

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

            Texture2D Icon4Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon4NotDone").Value;
            DrawIcon(spriteBatch, Icon4TopLeft, Icon4Texture);

            if (IsMouseOverUI(Icon4TopLeft, Icon4Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon4TopLeft, ButtonSelectedTexture);

                if (Flags.BountyInProgress)
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
                }
                else
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon4Text, Quest4ConditionText, QuestAcceptText, QuestWarningText, Color.HotPink);

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        //TODO: spawn item on the player that attracts the miniboss

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
            Texture2D Icon5Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5Locked").Value;

            bool downedAllMechs = NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;

            if (downedAllMechs)
            {
                if (!Flags.downedOrroboro)
                {
                    Icon5Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5NotDone").Value;
                }
                else
                {
                    Icon5Texture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestIcons/BountyIcon5Done").Value;
                }
            }

            DrawIcon(spriteBatch, Icon5TopLeft, Icon5Texture);

            if (IsMouseOverUI(Icon5TopLeft, Icon5Texture, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon5TopLeft, ButtonSelectedTexture);
                
				//quest text
                if (downedAllMechs)
                {
					//display the actual quest text if you havent killed orro-boro but you killed the mechs
                    if (!Flags.downedOrroboro)
                    {
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon5Text, Quest5ConditionText, Quest5AcceptText, Quest5WarningText, Color.Magenta);
					}
					//if you have killed orro-boro display the quest as complete
                    else
                    {
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, string.Empty, string.Empty, string.Empty, Color.White);
					}

                    //accept bounty (this specific bounty does not need to set the bounty accepted bool to true)
                    if (Main.mouseLeftRelease && Main.mouseLeft)
                    {
                        //TODO: spawn item on the player that attracts the miniboss

                        UIOpen = false;
                    }
                }
				//if you havent killed all 3 mechs, then display the quest as locked
				else
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, string.Empty, QuestIcon5LockedText, string.Empty, string.Empty, Color.Red);
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
            Rectangle backgroundArea = new Rectangle((int)TopLeft.X, (int)TopLeft.Y, (int)(texture.Width * backgroundScale.X), (int)(texture.Width * backgroundScale.Y));

            if (MouseScreenArea.Intersects(backgroundArea))
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