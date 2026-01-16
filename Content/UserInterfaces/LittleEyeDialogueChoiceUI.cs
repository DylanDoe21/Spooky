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
using Spooky.Content.Achievements;
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
    public class LittleEyeDialogueChoiceUI
    {
        public static int LittleEye = -1;
        public static bool UIOpen = false;

        public static Mod Mod = Spooky.mod;

        public static Vector2 modifier = new(-200, -75);
        public static readonly Vector2 UITopLeft = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

        public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/LittleEye/Talk", SoundType.Sound) { PitchVariance = 0.75f };

        private static Asset<Texture2D> BarTexture;
		private static Asset<Texture2D> BarHoverTexture;
        private static Asset<Texture2D> UITexture;

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

			if (player.controlInv)
			{
				LittleEye = -1;
				UIOpen = false;
			}

			Main.LocalPlayer.mouseInterface = true;
			Main.LocalPlayer.GetModPlayer<SpookyPlayer>().DisablePlayerControls = true;

            Main.instance.CameraModifiers.Add(new CameraPanning(Main.npc[LittleEye].Center, 20));

            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            string Choice1 = Language.GetTextValue("Mods.Spooky.UI.LittleEyeDialogueChoice.TalkChoice");
			string Choice2 = Language.GetTextValue("Mods.Spooky.UI.LittleEyeDialogueChoice.BountyChoice");
            string Choice3 = Language.GetTextValue("Mods.Spooky.UI.LittleEyeDialogueChoice.CauldronChoice");

            BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeDialogueChoiceUIBar");
			BarHoverTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/LittleEyeDialogueChoiceUIBarHover");
            UITexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUILittleEye");

            Vector2 UITopLeft1 = UITopLeft + new Vector2(0, -200);
            Vector2 UITopLeft2 = UITopLeft + new Vector2(0, -150 + (BarTexture.Height() / 2 * UIBoxScale.Y));
            Vector2 UITopLeft3 = UITopLeft + new Vector2(0, -100 + (BarTexture.Height() / 2 * UIBoxScale.Y) * 2);

            spriteBatch.Draw(BarTexture.Value, UITopLeft1, null, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(BarTexture.Value, UITopLeft2, null, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(BarTexture.Value, UITopLeft3, null, Color.White, 0f, BarTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

            Color textColor1 = Color.White;
            Color textColor2 = Color.White;
            Color textColor3 = Color.White;

            //regular talk dialogue
            if (IsMouseOverUI(UITopLeft1, BarTexture.Value, UIBoxScale))
            {
				spriteBatch.Draw(BarHoverTexture.Value, UITopLeft1, null, Color.White, 0f, BarHoverTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

                textColor1 = Color.Gold;
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
					if (Flags.downedOrroboro)
					{
						if (Flags.PokedLittleEye)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.LittleEyePoked"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerLittleEyePoked"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogue;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
					}
					else
					{

					}

                    LittleEye = -1;
                    UIOpen = false;
                }
            }

            //bounty mission dialogue and UI
			if (IsMouseOverUI(UITopLeft2, BarTexture.Value, UIBoxScale))
            {
				spriteBatch.Draw(BarHoverTexture.Value, UITopLeft2, null, Color.White, 0f, BarHoverTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

                textColor2 = Color.Gold;
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
					//first bounty completed
                    if (player.HasItem(ModContent.ItemType<BountyItem1>()))
                    {
						if (!Flags.LittleEyeBounty1 && !Flags.PokedLittleEye)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete1-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete1-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete1-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete1-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete1-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete1-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete1-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete1-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueMissionEnd;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						else
						{
							GiveRewardAndSetComplete();
						}
                    }
					//second bounty completed
                    else if (player.HasItem(ModContent.ItemType<BountyItem2>()))
                    {
						if (!Flags.LittleEyeBounty2 && !Flags.PokedLittleEye)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete2-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete2-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete2-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete2-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete2-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete2-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete2-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete2-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete2-5"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete2-5"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete2-6"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete2-6"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueMissionEnd;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						else
						{
							GiveRewardAndSetComplete();
						}
                    }
					//third bounty completed
                    else if (player.HasItem(ModContent.ItemType<BountyItem3>()))
                    {
						if (!Flags.LittleEyeBounty3 && !Flags.PokedLittleEye)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete3-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete3-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete3-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete3-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete3-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete3-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete3-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete3-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete3-5"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete3-5"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueMissionEnd;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						else
						{
							GiveRewardAndSetComplete();
						}
                    }
					//fourth bounty completed
					else if (player.HasItem(ModContent.ItemType<BountyItem4>()))
                    {
						if (!Flags.LittleEyeBounty4 && !Flags.PokedLittleEye)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete4-1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete4-1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete4-2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete4-2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete4-3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete4-3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete4-4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete4-4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueMissionEnd;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						else
						{
							GiveRewardAndSetComplete();
						}
                    }
					//if little eye is not poked and you have finished the orroboro fight
					else if (Flags.downedOrroboro && !Flags.PokedLittleEye)
					{
						DialogueChain chain = new();
						chain.Add(new(UITexture.Value, Main.npc[LittleEye],
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete5-1"),
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete5-1"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
						.Add(new(UITexture.Value, Main.npc[LittleEye],
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete5-2"),
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete5-2"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
						.Add(new(UITexture.Value, Main.npc[LittleEye],
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete5-3"),
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete5-3"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
						.Add(new(UITexture.Value, Main.npc[LittleEye],
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete5-4"),
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete5-4"),
						TalkSound, 2f, 0f, modifier, Expression: 1, NPCID: Main.npc[LittleEye].type))
						.Add(new(UITexture.Value, Main.npc[LittleEye],
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete5-5"),
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete5-5"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
						.Add(new(UITexture.Value, Main.npc[LittleEye],
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete5-6"),
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete5-6"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
						.Add(new(UITexture.Value, Main.npc[LittleEye],
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.QuestComplete5-7"),
						Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.PlayerQuestComplete5-7"),
						TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
						.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
						chain.OnPlayerResponseTrigger += PlayerResponse;
						chain.OnEndTrigger += EndDialoguePokedLittleEye;
						DialogueUI.Visible = true;
						DialogueUI.Add(chain);
					}
                    else
                    {
						if (!Flags.BountyIntro)
						{
							DialogueChain chain = new();
							chain.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntro1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntroPlayerResponse1"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntro2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntroPlayerResponse2"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntro3"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntroPlayerResponse3"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntro4"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntroPlayerResponse4"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntro5"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntroPlayerResponse5"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye],
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntro6"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.BountyIntroPlayerResponse6"),
							TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
							.Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
							chain.OnPlayerResponseTrigger += PlayerResponse;
							chain.OnEndTrigger += EndDialogueBountyIntro;
							DialogueUI.Visible = true;
							DialogueUI.Add(chain);
						}
						
						LittleEyeQuestUI.LittleEye = LittleEye;
						LittleEyeQuestUI.Delay = 0;
						LittleEyeQuestUI.UIOpen = true;
                    }

                    LittleEye = -1;
                    UIOpen = false;
                }
            }

            //cauldron advice
			if (IsMouseOverUI(UITopLeft3, BarTexture.Value, UIBoxScale))
            {
				spriteBatch.Draw(BarHoverTexture.Value, UITopLeft3, null, Color.White, 0f, BarHoverTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

                textColor3 = Color.Gold;
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    DialogueChain chain = new();
                    chain.Add(new(UITexture.Value, Main.npc[LittleEye],
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Cauldron1"),
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.CauldronPlayerResponse1"),
                    TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
                    .Add(new(UITexture.Value, Main.npc[LittleEye],
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Cauldron2"),
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.CauldronPlayerResponse2"),
                    TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
                    .Add(new(UITexture.Value, Main.npc[LittleEye],
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Cauldron3"),
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.CauldronPlayerResponse3"),
                    TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
                    .Add(new(UITexture.Value, Main.npc[LittleEye],
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.Cauldron4"),
                    Language.GetTextValue("Mods.Spooky.Dialogue.LittleEyeDialogue.CauldronPlayerResponse4"),
                    TalkSound, 2f, 0f, modifier, NPCID: Main.npc[LittleEye].type))
                    .Add(new(UITexture.Value, Main.npc[LittleEye], null, null, TalkSound, 2f, 0f, modifier, true));
                    chain.OnPlayerResponseTrigger += PlayerResponse;
                    chain.OnEndTrigger += EndDialogue;
                    DialogueUI.Visible = true;
                    DialogueUI.Add(chain);

                    LittleEye = -1;
                    UIOpen = false;
                }
            }

            DrawTextDescription(spriteBatch, UITopLeft1 + new Vector2(-50f, -10f), Choice1, textColor1);
            DrawTextDescription(spriteBatch, UITopLeft2 + new Vector2(-50f, -10f), Choice2, textColor2);
            DrawTextDescription(spriteBatch, UITopLeft3 + new Vector2(-50f, -10f), Choice3, textColor3);
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

		public static void DrawTextDescription(SpriteBatch spriteBatch, Vector2 TextTopLeft, string Text, Color color)
		{
			Vector2 scale = new Vector2(0.9f, 0.925f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f) * Main.UIScale;

			foreach (string TextLine in Utils.WordwrapString(Text, FontAssets.MouseText.Value, 700, 16, out _))
			{
				if (string.IsNullOrEmpty(TextLine))
				{
					continue;
				}

				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, TextLine, TextTopLeft, color, 0f, Vector2.Zero, scale);
				TextTopLeft.Y += Main.UIScale * 16f;
			}
        }

        //check if the mouse is hovering over a specific button
        public static bool IsMouseOverUI(Vector2 TopLeft, Texture2D texture, Vector2 backgroundScale)
        {
            Rectangle backgroundArea = new Rectangle((int)TopLeft.X - (int)(texture.Width * backgroundScale.X) / 2, 
            (int)TopLeft.Y - (int)(texture.Height * backgroundScale.Y) / 2, 
            (int)(texture.Width * backgroundScale.X), (int)(texture.Height * backgroundScale.Y));

            if (backgroundArea.Contains(Main.mouseX, Main.mouseY))
			{
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer,
			Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

        public static void EndDialogue(Dialogue dialogue, int ID)
		{
			DialogueUI.Visible = false;
		}

		public static void EndDialogueBountyIntro(Dialogue dialogue, int ID)
		{
			if (Main.netMode != NetmodeID.SinglePlayer)
			{
				ModPacket packet = Mod.GetPacket();
				packet.Write((byte)SpookyMessageType.BountyIntro);
				packet.Send();
			}
			else
			{
				Flags.BountyIntro = true;
			}

			DialogueUI.Visible = false;
		}

		public static void EndDialoguePokedLittleEye(Dialogue dialogue, int ID)
		{
			ModContent.GetInstance<MiscAchievementLittleEyeQuest>().LittleEyeQuestCondition.Complete();

			DialogueUI.Visible = false;
		}

        public static void SpawnItem(int Type, int Amount)
        {
            int newItem = Item.NewItem(Main.LocalPlayer.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, Type, Amount);
			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
        }

		public static void GiveRewardAndSetComplete()
		{
			Player player = Main.LocalPlayer;

			if (player.ConsumeItem(ModContent.ItemType<BountyItem1>()))
			{
				bool IsLastQuest = !Flags.LittleEyeBounty1 && Flags.LittleEyeBounty2 && Flags.LittleEyeBounty3 && Flags.LittleEyeBounty4;

				if (IsLastQuest)
				{
					SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
				}

				SpawnItem(ModContent.ItemType<SewingThread>(), 1);

				SpawnItem(ModContent.ItemType<IconPainting1Item>(), 1);

				if (Main.rand.NextBool(20))
				{
					SpawnItem(ModContent.ItemType<BrownieOrange>(), 1);
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.Bounty1Complete);
					packet.Send();
				}
				else
				{
					Flags.LittleEyeBounty1 = true;
					Flags.BountyInProgress1 = false;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<BountyItem2>()))
			{
				bool IsLastQuest = !Flags.LittleEyeBounty2 && Flags.LittleEyeBounty1 && Flags.LittleEyeBounty3 && Flags.LittleEyeBounty4;

				if (IsLastQuest)
				{
					SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
				}

				SpawnItem(ModContent.ItemType<GhostBook>(), 1);

				SpawnItem(ModContent.ItemType<IconPainting2Item>(), 1);

				if (Main.rand.NextBool(20))
				{
					SpawnItem(ModContent.ItemType<BrownieGhost>(), 1);
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.Bounty2Complete);
					packet.Send();
				}
				else
				{
					Flags.LittleEyeBounty2 = true;
					Flags.BountyInProgress2 = false;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<BountyItem3>()))
			{
				bool IsLastQuest = !Flags.LittleEyeBounty3 && Flags.LittleEyeBounty1 && Flags.LittleEyeBounty2 && Flags.LittleEyeBounty4;

				if (IsLastQuest)
				{
					SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
				}

				SpawnItem(ModContent.ItemType<StitchedCloak>(), 1);

				SpawnItem(ModContent.ItemType<IconPainting3Item>(), 1);

				if (Main.rand.NextBool(20))
				{
					SpawnItem(ModContent.ItemType<BrownieBone>(), 1);
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.Bounty3Complete);
					packet.Send();
				}
				else
				{
					Flags.LittleEyeBounty3 = true;
					Flags.BountyInProgress3 = false;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<BountyItem4>()))
			{
				bool IsLastQuest = !Flags.LittleEyeBounty4 && Flags.LittleEyeBounty1 && Flags.LittleEyeBounty2 && Flags.LittleEyeBounty3;

				if (IsLastQuest)
				{
					SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
				}

				SpawnItem(ModContent.ItemType<MagicEyeOrb>(), 1);

				SpawnItem(ModContent.ItemType<LittleEyeHat>(), 1);
				SpawnItem(ModContent.ItemType<IconPainting4Item>(), 1);

				if (Main.rand.NextBool(20))
				{
					SpawnItem(ModContent.ItemType<BrownieOrganic>(), 1);
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					ModPacket packet = Mod.GetPacket();
					packet.Write((byte)SpookyMessageType.Bounty4Complete);
					packet.Send();
				}
				else
				{
					Flags.LittleEyeBounty4 = true;
					Flags.BountyInProgress4 = false;
				}
			}

			//"Qol" feature where little eye can give you items in vanilla that are obnoxiously rare and have sentient upgrades
			if (Main.rand.NextBool(5))
			{
				int[] RareItemsForSentientStuff = new int[] { ItemID.Katana, ItemID.ChainKnife, ItemID.BlackLens };

				SpawnItem(Main.rand.Next(RareItemsForSentientStuff), 1);
			}

			if (Main.rand.NextBool())
			{
				SpawnItem(ModContent.ItemType<SnotMedication>(), 1);
			}

			SpawnItem(ItemID.GoodieBag, Main.rand.Next(1, 5));

			if (Main.rand.NextBool())
			{
				int[] Foods = new int[] { ModContent.ItemType<BlackLicorice>(), ModContent.ItemType<EyeChocolate>(), ModContent.ItemType<GoofyPretzel>() };

				SpawnItem(Main.rand.Next(Foods), Main.rand.Next(1, 3));
			}

			if (Main.rand.NextBool(3))
			{
				SpawnItem(ItemID.ObsidianLockbox, 1);
			}

			if (Main.rand.NextBool(3))
			{
				SpawnItem(ItemID.BloodMoonStarter, 1);
			}

			SpawnItem(ItemID.GoldCoin, 10);
		}

        public static  void EndDialogueMissionEnd(Dialogue dialogue, int ID)
		{
			GiveRewardAndSetComplete();

			DialogueUI.Visible = false;
		}
    }
}