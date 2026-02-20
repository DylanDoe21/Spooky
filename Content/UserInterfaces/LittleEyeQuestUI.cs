using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.UI.Chat;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.Quest;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Sentient;
using Spooky.Content.Tiles.Painting;

namespace Spooky.Content.UserInterfaces
{
    public class LittleEyeQuestUI
    {
		public static int Delay = 0;
        public static int LittleEye = -1;
        public static bool UIOpen = false;
        public static bool IsHoveringOverAnyButton = false;

		public static Mod Mod = Spooky.mod;

		public static Vector2 modifier = new(-200, -75);
        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

		public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/LittleEye/Talk", SoundType.Sound) { Volume = 2f, PitchVariance = 0.75f };

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

		private static Asset<Texture2D> UITexture;

		//check if little eye is close enough
		public static bool InRangeOfNPC()
        {
            if (!Main.npc.IndexInRange(LittleEye) || !Main.npc[LittleEye].active)
            {
                return false;
            }

            Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);
            
            return validTalkArea.Intersects(Main.npc[LittleEye].Hitbox);
        }

		public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Condition, string Accept, string Warning, Color ConditionColor)
		{
			Vector2 scale = new Vector2(1f, 1.025f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			//first draw the condition text for the biome you find the miniboss in
			foreach (string TextLine in Utils.WordwrapString(Condition, FontAssets.MouseText.Value, 600, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, ConditionColor, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//draw the text to tell players they have to click the button to accept the bounty
			foreach (string TextLine in Utils.WordwrapString(Accept, FontAssets.MouseText.Value, 600, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, Color.Lime, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}

			//finally display the warning that you cant accept another bounty until the selected one is done
			foreach (string TextLine in Utils.WordwrapString(Warning, FontAssets.MouseText.Value, 600, 16, out _))
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

			if (DialogueUI.Visible && DialogueUI.Dialogue.Count > 0)
            {
                return;
            }

            //stop the UI from being open if the player is doing other stuff
            if (player.chest != -1 || player.sign != -1 || player.talkNPC == -1 || !InRangeOfNPC() || Main.InGuideCraftMenu)
            {
                UIOpen = false;
                return;
            }

			if (player.controlInv)
			{
				LittleEye = -1;
				UIOpen = false;
			}

			Main.LocalPlayer.mouseInterface = true;
			Main.LocalPlayer.GetModPlayer<SpookyPlayer>().DisablePlayerControls = true;

            Main.instance.CameraModifiers.Add(new CameraPanning(Main.npc[LittleEye].Center, 20));

            Texture2D UIBoxTexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeQuestUIBar").Value;
            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            //draw the main UI box
            spriteBatch.Draw(UIBoxTexture, UITopLeft, null, Color.White, 0f, UIBoxTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            //prevent any mouse interactions while the mouse is hovering over this UI
            if (IsMouseOverUIBox(UITopLeft, UIBoxTexture, UIBoxScale))
            {
                IsHoveringOverAnyButton = false;

                player.mouseInterface = true;
            }

            Point ButtonTopLeft = (UITopLeft + new Vector2(-525f, -110f) * UIBoxScale).ToPoint();

			//define all the string values needed for the UI
			string Quest1ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty1Condition");
			string Quest2ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty2Condition");
			string Quest3ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty3Condition");
			string Quest4ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty4Condition");
			string QuestIcon5LockedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Locked");
			string Quest5ConditionText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Condition");

			string QuestAcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccept");
			string QuestWarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyWarning");
			string Quest5AcceptText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Accept");
			string Quest5WarningText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.Bounty5Warning");

            string QuestAcceptedText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyAccepted");

			string QuestCompleteText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompleted");
            string QuestCompleteRematchText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyCompletedItem");
			string QuestNewItemText = Language.GetTextValue("Mods.Spooky.UI.LittleEyeBounties.BountyNewItem");

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

			UITexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye");

			if (Delay <= 20 && !Main.mouseLeft)
			{
				Delay++;
			}

			bool BountyLocked1 = !Flags.BountyInProgress1 && (Flags.BountyInProgress2 || Flags.BountyInProgress3 || Flags.BountyInProgress4);
			bool BountyLocked2 = !Flags.BountyInProgress2 && (Flags.BountyInProgress1 || Flags.BountyInProgress3 || Flags.BountyInProgress4);
			bool BountyLocked3 = !Flags.BountyInProgress3 && (Flags.BountyInProgress1 || Flags.BountyInProgress2 || Flags.BountyInProgress4);
			bool BountyLocked4 = !Flags.BountyInProgress4 && (Flags.BountyInProgress1 || Flags.BountyInProgress2 || Flags.BountyInProgress3);

			//frank the goblin
			Vector2 Icon1TopLeft = ButtonTopLeft.ToVector2() + new Vector2(315f, -24f) * Main.UIScale;

            DrawIcon(spriteBatch, Icon1TopLeft, BountyLocked1 ? BountyIconLocked.Value : (Flags.LittleEyeBounty1 ? BountyIcon1Done.Value : BountyIcon1NotDone.Value));

            if (IsMouseOverUI(Icon1TopLeft, BountyIcon1Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon1TopLeft, BountyIconSelectedOutline.Value);

                if (BountyLocked1)
                {
                    DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
                }
				else if (Flags.LittleEyeBounty1 || Flags.PokedLittleEye)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<SummonItem1>()))
					{
						DialogueChain chain = new();
						chain.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
						chain.OnPlayerResponseTrigger += PlayerResponse;
						chain.OnEndTrigger += EndDialogueQuestAccept1;
						DialogueUI.Visible = true;
						DialogueUI.Add(chain);

						UIOpen = false;
					}
				}
                else
                {
					if (!Flags.BountyInProgress1)
					{
				    	DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, Quest1ConditionText, QuestAcceptText, QuestWarningText, Color.OrangeRed);
					}
					else
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestNewItemText, string.Empty, string.Empty, Color.White);
					}

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
                    {
						//quest accept dialogue
						if (!Flags.BountyInProgress1)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest1-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest1-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept1;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						//if the player needs a new item
						else
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem1-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem1-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept1;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}

						UIOpen = false;
                    }
                }
            }

            //tome of spirits
            Vector2 Icon2TopLeft = ButtonTopLeft.ToVector2() + new Vector2(400f, -24f) * Main.UIScale;

			DrawIcon(spriteBatch, Icon2TopLeft, BountyLocked2 ? BountyIconLocked.Value : (Flags.LittleEyeBounty2 ? BountyIcon2Done.Value : BountyIcon2NotDone.Value));

			if (IsMouseOverUI(Icon2TopLeft, BountyIcon2Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon2TopLeft, BountyIconSelectedOutline.Value);

				if (BountyLocked2)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
				}
				else if (Flags.LittleEyeBounty2 || Flags.PokedLittleEye)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<SummonItem2>()))
					{
						DialogueChain chain = new();
						chain.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
						chain.OnPlayerResponseTrigger += PlayerResponse;
						chain.OnEndTrigger += EndDialogueQuestAccept2;
						DialogueUI.Visible = true;
						DialogueUI.Add(chain);

						UIOpen = false;
					}
				}
				else
                {
					if (!Flags.BountyInProgress2)
					{
				    	DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, Quest2ConditionText, QuestAcceptText, QuestWarningText, Color.SeaGreen);
					}
					else
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestNewItemText, string.Empty, string.Empty, Color.White);
					}

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
                    {
						//quest accept dialogue
						if (!Flags.BountyInProgress2)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest2-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest2-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept2;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						//if the player needs a new item
						else
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem2-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem2-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept2;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}

                        UIOpen = false;
                    }
                }
            }

            //spider grotto display stuff
            Vector2 Icon3TopLeft = ButtonTopLeft.ToVector2() + new Vector2(485f, -24f) * Main.UIScale;

			DrawIcon(spriteBatch, Icon3TopLeft, BountyLocked3 ? BountyIconLocked.Value : (Flags.LittleEyeBounty3 ? BountyIcon3Done.Value : BountyIcon3NotDone.Value));

			if (IsMouseOverUI(Icon3TopLeft, BountyIcon3Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon3TopLeft, BountyIconSelectedOutline.Value);

				if (BountyLocked3)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
				}
				else if (Flags.LittleEyeBounty3 || Flags.PokedLittleEye)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<SummonItem3>()))
					{
						DialogueChain chain = new();
						chain.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
						chain.OnPlayerResponseTrigger += PlayerResponse;
						chain.OnEndTrigger += EndDialogueQuestAccept3;
						DialogueUI.Visible = true;
						DialogueUI.Add(chain);

						UIOpen = false;
					}
				}
				else
                {
					if (!Flags.BountyInProgress3)
					{
				    	DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, Quest3ConditionText, QuestAcceptText, QuestWarningText, Color.Chocolate);
					}
					else
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestNewItemText, string.Empty, string.Empty, Color.White);
					}

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
                    {
						//quest accept dialogue
						if (!Flags.BountyInProgress3)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest3-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest3-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest3-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest3-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest3-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest3-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept3;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						//if the player needs a new item
						else
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem3-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem3-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem3-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem3-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem3-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem3-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept3;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}

                        UIOpen = false;
                    }
                }
            }

            //eye wizard display stuff
            Vector2 Icon4TopLeft = ButtonTopLeft.ToVector2() + new Vector2(570f, -24f) * Main.UIScale;

			DrawIcon(spriteBatch, Icon4TopLeft, BountyLocked4 ? BountyIconLocked.Value : (Flags.LittleEyeBounty4 ? BountyIcon4Done.Value : BountyIcon4NotDone.Value));

			if (IsMouseOverUI(Icon4TopLeft, BountyIcon4Done.Value, UIBoxScale))
            {
                IsHoveringOverAnyButton = true;

                DrawIcon(spriteBatch, Icon4TopLeft, BountyIconSelectedOutline.Value);

				if (BountyLocked4)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestAcceptedText, string.Empty, string.Empty, Color.Red);
				}
				else if (Flags.LittleEyeBounty4 || Flags.PokedLittleEye)
				{
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, Color.Lime);

					//give the player the item again if they wish to rematch the miniboss
					if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<SummonItem3>()))
					{
						DialogueChain chain = new();
						chain.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
						chain.OnPlayerResponseTrigger += PlayerResponse;
						chain.OnEndTrigger += EndDialogueQuestAccept4;
						DialogueUI.Visible = true;
						DialogueUI.Add(chain);

						UIOpen = false;
					}
				}
				else
                {
					if (!Flags.BountyInProgress4)
					{
				    	DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, Quest4ConditionText, QuestAcceptText, QuestWarningText, Color.HotPink);
					}
					else
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestNewItemText, string.Empty, string.Empty, Color.White);
					}

                    //accept bounty
                    if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
                    {
                        //quest accept dialogue
						if (!Flags.BountyInProgress4)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest4-5"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest4-5"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept4;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						//if the player needs a new item
						else
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestNewItem4-5"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestNewItem4-5"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept4;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
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
					DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestIcon5LockedText, string.Empty, string.Empty, Color.Red);
				}
				else
				{
					//if you have killed orro-boro display the quest as complete
					if (Flags.downedOrroboro)
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, QuestCompleteText, QuestCompleteRematchText, string.Empty, Color.Lime);

						if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20 && !player.HasItem(ModContent.ItemType<Concoction>()))
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueQuestAccept5;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);

							UIOpen = false;
						}
					}
					//display the actual quest text if you havent killed orro-boro but you killed the mechs
					else
					{
						DrawTextDescription(spriteBatch, UITopLeft + new Vector2(-257f, -30f) * UIBoxScale, Quest5ConditionText, Quest5AcceptText, Quest5WarningText, Color.Magenta);

						//accept bounty (this specific bounty does not need to set the bounty accepted bool to true)
						if (Main.mouseLeftRelease && Main.mouseLeft && Delay > 20)
						{
							//quest accept dialogue
							if (!Flags.downedOrroboro)
							{
								DialogueChain chain = new();
								chain.Add(new(UITexture.Value, Main.npc[LittleEye],
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-1"),
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-1"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
								.Add(new(UITexture.Value, Main.npc[LittleEye],
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-2"),
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-2"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
								.Add(new(UITexture.Value, Main.npc[LittleEye],
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-3"),
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-3"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
								.Add(new(UITexture.Value, Main.npc[LittleEye],
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-4"),
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-4"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
								.Add(new(UITexture.Value, Main.npc[LittleEye],
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Quest5-5"),
								Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuest5-5"),
								TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
								.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
								chain.OnPlayerResponseTrigger += PlayerResponse;
								chain.OnEndTrigger += EndDialogueQuestAccept5;
								DialogueUI.Visible = true;
								DialogueUI.Add(chain);
							}

							UIOpen = false;
						}
					}
				}
            }
        }

		public static void EndDialogueQuestAccept1(Dialogue dialogue, int ID)
		{
			int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, ModContent.ItemType<SummonItem1>());
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);

			if (!Flags.LittleEyeBounty1)
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.BountyAccepted1);
					packet.Send();
				}
				else
				{
					Flags.BountyInProgress1 = true;
				}
			}

			DialogueUI.Visible = false;
		}

		public static void EndDialogueQuestAccept2(Dialogue dialogue, int ID)
		{
			int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, ModContent.ItemType<SummonItem2>());
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);

			if (!Flags.LittleEyeBounty2)
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.BountyAccepted2);
					packet.Send();
				}
				else
				{
					Flags.BountyInProgress2 = true;
				}
			}

			DialogueUI.Visible = false;
		}

		public static void EndDialogueQuestAccept3(Dialogue dialogue, int ID)
		{
			int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, ModContent.ItemType<SummonItem3>());
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);

			if (!Flags.LittleEyeBounty3)
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.BountyAccepted3);
					packet.Send();
				}
				else
				{
					Flags.BountyInProgress3 = true;
				}
			}

			DialogueUI.Visible = false;
		}

		public static void EndDialogueQuestAccept4(Dialogue dialogue, int ID)
		{
			int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, ModContent.ItemType<SummonItem4>());
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);

			if (!Flags.LittleEyeBounty4)
			{
				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.BountyAccepted4);
					packet.Send();
				}
				else
				{
					Flags.BountyInProgress4 = true;
				}
			}

			DialogueUI.Visible = false;
		}

		public static void EndDialogueQuestAccept5(Dialogue dialogue, int ID)
		{
			int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, ModContent.ItemType<Concoction>());
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);

			DialogueUI.Visible = false;
		}

		public static void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer,
			Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}
    }
}