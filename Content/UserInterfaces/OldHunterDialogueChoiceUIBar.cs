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
    public class OldHunterDialogueChoiceUI
    {
        public static int OldHunter = -1;
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
                OldHunter = -1;
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
				OldHunter = -1;
				UIOpen = false;
			}

			Main.LocalPlayer.mouseInterface = true;
			Main.LocalPlayer.GetModPlayer<SpookyPlayer>().DisablePlayerControls = true;

            Main.instance.CameraModifiers.Add(new CameraPanning(Main.npc[OldHunter].Center, 20));

            Vector2 UIBoxScale = Vector2.One * Main.UIScale;

            string Choice1 = Language.GetTextValue("Mods.Spooky.UI.OldHunterDialogueChoice.TalkChoice");
			string Choice2 = Language.GetTextValue("Mods.Spooky.UI.OldHunterDialogueChoice.RareItemChoice");
			string Choice3 = Language.GetTextValue("Mods.Spooky.UI.OldHunterDialogueChoice.RematchChoice");

            BarTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/OldHunterDialogueChoiceUIBar");
			BarHoverTexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/OldHunterDialogueChoiceUIBarHover");
            UITexture ??= ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIOldHunter");

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
					DialogueChain chain = new();
					chain.Add(new(UITexture.Value, Main.npc[OldHunter],
					Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.OldHunterTalk1-1"),
					Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerOldHunterTalk1-1"),
					TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
					.Add(new(UITexture.Value, Main.npc[OldHunter], null, null, TalkSound, 2f, 0f, modifier, true));
					chain.OnPlayerResponseTrigger += PlayerResponse;
					chain.OnEndTrigger += EndDialogue;
					DialogueUI.Visible = true;
					DialogueUI.Add(chain);

                    OldHunter = -1;
                    UIOpen = false;
                }
            }

			//dialogue and rewards for rare enemy items
			if (IsMouseOverUI(UITopLeft2, BarTexture.Value, UIBoxScale))
            {
				spriteBatch.Draw(BarHoverTexture.Value, UITopLeft2, null, Color.White, 0f, BarHoverTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

                textColor2 = Color.Gold;
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
					DialogueChain chain = new();
					chain.Add(new(UITexture.Value, Main.npc[OldHunter],
					Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.OldHunterNoItem"),
					Language.GetTextValue("Mods.Spooky.Dialogue.OldHunterDialogue.PlayerOldHunterNoItem"),
					TalkSound, 2f, 0f, modifier, NPCID: Main.npc[OldHunter].type))
					.Add(new(UITexture.Value, Main.npc[OldHunter], null, null, TalkSound, 2f, 0f, modifier, true));
					chain.OnPlayerResponseTrigger += PlayerResponse;
					chain.OnEndTrigger += EndDialogue;
					DialogueUI.Visible = true;
					DialogueUI.Add(chain);

                    OldHunter = -1;
                    UIOpen = false;
                }
            }

			//old hunter boss rematch
			if (IsMouseOverUI(UITopLeft3, BarTexture.Value, UIBoxScale))
            {
				spriteBatch.Draw(BarHoverTexture.Value, UITopLeft3, null, Color.White, 0f, BarHoverTexture.Size() / 2, UIBoxScale, SpriteEffects.None, 0f);

                textColor3 = Color.Gold;
                player.mouseInterface = true;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
					if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.SpawnOldHunter);
                        packet.Send();
                    }
                    else
                    {
                        Flags.SpawnOldHunter = true;
                    }

                    OldHunter = -1;
                    UIOpen = false;
                }
            }

            DrawTextDescription(spriteBatch, UITopLeft1 + new Vector2(-50f, -10f), Choice1, textColor1);
            DrawTextDescription(spriteBatch, UITopLeft2 + new Vector2(-50f, -10f), Choice2, textColor2);
			DrawTextDescription(spriteBatch, UITopLeft3 + new Vector2(-50f, -10f), Choice3, textColor3);
        }

        public static bool InRangeOfNPC()
        {
            if (!Main.npc.IndexInRange(OldHunter) || !Main.npc[OldHunter].active)
            {
                return false;
            }

            Rectangle validTalkArea = Utils.CenteredRectangle(Main.LocalPlayer.Center, new Vector2(Player.tileRangeX * 3f, Player.tileRangeY * 2f) * 16f);
            
            return validTalkArea.Intersects(Main.npc[OldHunter].Hitbox);
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

		/*
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
				bool IsLastQuest = !Flags.OldHunterBounty1 && Flags.OldHunterBounty2 && Flags.OldHunterBounty3 && Flags.OldHunterBounty4;

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
					Flags.OldHunterBounty1 = true;
					Flags.BountyInProgress1 = false;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<BountyItem2>()))
			{
				bool IsLastQuest = !Flags.OldHunterBounty2 && Flags.OldHunterBounty1 && Flags.OldHunterBounty3 && Flags.OldHunterBounty4;

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
					Flags.OldHunterBounty2 = true;
					Flags.BountyInProgress2 = false;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<BountyItem3>()))
			{
				bool IsLastQuest = !Flags.OldHunterBounty3 && Flags.OldHunterBounty1 && Flags.OldHunterBounty2 && Flags.OldHunterBounty4;

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
					Flags.OldHunterBounty3 = true;
					Flags.BountyInProgress3 = false;
				}
			}
			else if (player.ConsumeItem(ModContent.ItemType<BountyItem4>()))
			{
				bool IsLastQuest = !Flags.OldHunterBounty4 && Flags.OldHunterBounty1 && Flags.OldHunterBounty2 && Flags.OldHunterBounty3;

				if (IsLastQuest)
				{
					SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
				}

				SpawnItem(ModContent.ItemType<MagicEyeOrb>(), 1);

				SpawnItem(ModContent.ItemType<OldHunterHat>(), 1);
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
					Flags.OldHunterBounty4 = true;
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
		*/
    }
}