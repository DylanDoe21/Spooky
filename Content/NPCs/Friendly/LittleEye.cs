using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.Quest;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Sentient;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.Tiles.SpookyHell.Painting;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
	public class LittleEye : ModNPC
	{
		public static int ChosenQuestForToday = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
		{
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[NPC.type] = 5;

            NPCProfile = new Profiles.StackedNPCProfile(new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)), new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex));
        }

        public override void Load()
        {
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            TownNPCStayingHomeless = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LittleEye"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Button1");
			button2 = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Button2");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			//quest button
			if (firstButton) 
            {
				Player player = Main.LocalPlayer;

				if (player.ConsumeItem(ModContent.ItemType<BountyItem1>()))
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest1Complete");

					bool IsLastQuest = !Flags.LittleEyeBounty1 && Flags.LittleEyeBounty2 && Flags.LittleEyeBounty3 && Flags.LittleEyeBounty4;

					if (IsLastQuest)
					{
						SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
					}

					SpawnItem(ModContent.ItemType<SewingThread>(), 1);

					if (Main.rand.NextBool())
					{
						SpawnItem(ModContent.ItemType<SnotMedication>(), 1);
					}

					SpawnItem(ModContent.ItemType<IconPainting1Item>(), 1);

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

					if (Main.rand.NextBool(50))
					{
						SpawnItem(ModContent.ItemType<SuspiciousBrownie>(), 1);
					}

					SpawnItem(ItemID.GoldCoin, 10);

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						ModPacket packet = Mod.GetPacket();
						packet.Write((byte)SpookyMessageType.Bounty1Complete);
						packet.Send();
					}
					else
					{
						Flags.LittleEyeBounty1 = true;
						Flags.BountyInProgress = false;
					}
				}
				else if (player.ConsumeItem(ModContent.ItemType<BountyItem2>()))
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest2Complete");

					bool IsLastQuest = !Flags.LittleEyeBounty2 && Flags.LittleEyeBounty1 && Flags.LittleEyeBounty3 && Flags.LittleEyeBounty4;

					if (IsLastQuest)
					{
						SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
					}

					SpawnItem(ModContent.ItemType<GhostBook>(), 1);

					if (Main.rand.NextBool())
					{
						SpawnItem(ModContent.ItemType<SnotMedication>(), 1);
					}

					SpawnItem(ModContent.ItemType<IconPainting2Item>(), 1);

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

					if (Main.rand.NextBool(50))
					{
						SpawnItem(ModContent.ItemType<SuspiciousBrownie>(), 1);
					}

					SpawnItem(ItemID.GoldCoin, 10);

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						ModPacket packet = Mod.GetPacket();
						packet.Write((byte)SpookyMessageType.Bounty2Complete);
						packet.Send();
					}
					else
					{
						Flags.LittleEyeBounty2 = true;
						Flags.BountyInProgress = false;
					}
				}
				else if (player.ConsumeItem(ModContent.ItemType<BountyItem3>()))
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest3Complete");

					bool IsLastQuest = !Flags.LittleEyeBounty3 && Flags.LittleEyeBounty1 && Flags.LittleEyeBounty2 && Flags.LittleEyeBounty4;

					if (IsLastQuest)
					{
						SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
					}

					SpawnItem(ModContent.ItemType<StitchedCloak>(), 1);

					if (Main.rand.NextBool())
					{
						SpawnItem(ModContent.ItemType<SnotMedication>(), 1);
					}

					SpawnItem(ModContent.ItemType<IconPainting3Item>(), 1);

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

					if (Main.rand.NextBool(50))
					{
						SpawnItem(ModContent.ItemType<SuspiciousBrownie>(), 1);
					}

					SpawnItem(ItemID.GoldCoin, 10);

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						ModPacket packet = Mod.GetPacket();
						packet.Write((byte)SpookyMessageType.Bounty3Complete);
						packet.Send();
					}
					else
					{
						Flags.LittleEyeBounty3 = true;
						Flags.BountyInProgress = false;
					}
				}
				else if (player.ConsumeItem(ModContent.ItemType<BountyItem4>()))
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest4Complete");

					bool IsLastQuest = !Flags.LittleEyeBounty4 && Flags.LittleEyeBounty1 && Flags.LittleEyeBounty2 && Flags.LittleEyeBounty3;

					if (IsLastQuest)
					{
						SpawnItem(ModContent.ItemType<SentientChumCaster>(), 1);
					}

					SpawnItem(ModContent.ItemType<MagicEyeOrb>(), 1);

					if (Main.rand.NextBool())
					{
						SpawnItem(ModContent.ItemType<SnotMedication>(), 1);
					}

					SpawnItem(ModContent.ItemType<LittleEyeHat>(), 1);
					SpawnItem(ModContent.ItemType<IconPainting4Item>(), 1);

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

					if (Main.rand.NextBool(50))
					{
						SpawnItem(ModContent.ItemType<SuspiciousBrownie>(), 1);
					}

					SpawnItem(ItemID.GoldCoin, 10);

					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						ModPacket packet = Mod.GetPacket();
						packet.Write((byte)SpookyMessageType.Bounty4Complete);
						packet.Send();
					}
					else
					{
						Flags.LittleEyeBounty4 = true;
						Flags.BountyInProgress = false;
					}
				}
				else
				{
					LittleEyeQuestUI.LittleEye = NPC.whoAmI;
					LittleEyeQuestUI.UIOpen = true;
				}
			}
			//cauldron button
			else
			{
				Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Upgrades");
			}
		}

        public void SpawnItem(int Type, int Amount)
        {
            int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), Main.LocalPlayer.Hitbox, Type, Amount);

			if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
			{
				NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
			}
        }

		public override string GetChat()
		{
			if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
            {
				return Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.OrroBoro");
			}

			return Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default" + Main.rand.Next(1, 9));
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];
			
			NPC.spriteDirection = NPC.direction;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}