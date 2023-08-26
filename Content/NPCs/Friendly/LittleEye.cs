using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Flask;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.Tiles.Pylon;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
	public class LittleEye : ModNPC
	{
		public static int ChosenQuestForToday = 0;

        int Quest1Timer = 0;
		int Quest2Timer = 0;
		int Quest3Timer = 0;
		int Quest4Timer = 0;
		int Quest5Timer = 0;
		int RandomQuestTimer = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
		{
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[NPC.type] = 5;

            NPCProfile = new Profiles.StackedNPCProfile(
                new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)),
                new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex)
            );
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
            NPC.aiStyle = 7;
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
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0;
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
				if (!Flags.EyeQuest1)
				{
					if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<Flask1>()))
					{
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest1Rewards");

						Quest1Timer++;
					}
					else
                    {
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest1") 
						+ $"\n[i:{ItemID.Daybloom}]x5, [i:{ItemID.Blinkroot}]x5, [i:{ItemID.Moonglow}]x5, [i:{ItemID.PurificationPowder}]x10, [i:{ItemID.SuspiciousLookingEye}]x1, [i:{ItemID.Bottle}]x1";;
					}
				}
				if (Flags.EyeQuest1 && !Flags.EyeQuest2)
				{
					if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<Flask2>()))
					{
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest2Rewards");

						Quest2Timer++;
					}
					else
                    {	
						if (!WorldGen.crimson)
						{
							Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest2")
							+ $"\n[i:{ItemID.Shiverthorn}]x5, [i:{ItemID.GlowingMushroom}]x12, [i:{ItemID.RottenChunk}]x20, [i:{ItemID.IceBlock}]x10 [i:{ItemID.WormFood}]x1, [i:{ItemID.Bottle}]x1";
						}
						else
						{
							Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest2")
							+ $"\n[i:{ItemID.Shiverthorn}]x5, [i:{ItemID.GlowingMushroom}]x12, [i:{ItemID.Vertebrae}]x20, [i:{ItemID.IceBlock}]x10 [i:{ItemID.BloodySpine}]x1, [i:{ItemID.Bottle}]x1";
						}
					}
				}
				if (Flags.EyeQuest2 && !Flags.EyeQuest3)
				{
					if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<Flask3>()))
					{
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest3Rewards");

						Quest3Timer++;
					}
					else
                    {
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest3")
						+ $"\n[i:{ItemID.HoneyBlock}]x35, [i:{ItemID.Pumpkin}]x35, [i:{ItemID.Sluggy}]x5, [i:{ItemID.Cobweb}]x15, [i:{ItemID.Abeemination}]x1, [i:{ItemID.Bottle}]x1";
					}
				}
				if (Flags.EyeQuest3 && !Flags.EyeQuest4)
				{
					if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<Flask4>()))
					{
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest4Rewards");

						Quest4Timer++;
					}
					else
                    {
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest4")
						+ $"\n[i:{ItemID.Fireblossom}]x5, [i:{ItemID.Deathweed}]x5, [i:{ItemID.Ruby}]x12, [i:{ItemID.AshBlock}]x35, [i:{ItemID.DeerThing}]x1, [i:{ItemID.Bottle}]x1";
					}
				}
				if (Flags.EyeQuest4 && !Flags.EyeQuest5)
                {
					if (Main.player[Main.myPlayer].ConsumeItem(ItemID.SoulofSight) && Main.player[Main.myPlayer].ConsumeItem(ItemID.SoulofMight) && Main.player[Main.myPlayer].ConsumeItem(ItemID.SoulofFright))
					{
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest5Rewards");

						Quest5Timer++;
					}
					else
                    {
						if (!Main.hardMode)
						{
							Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest5NotHardmode");
						}
						else
						{
							Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Quest5Hardmode")
							+ $"\n[i:{ItemID.SoulofSight}]x1 [i:{ItemID.SoulofMight}]x1 [i:{ItemID.SoulofFright}]x1";
						}
					}
				}
				if (Flags.EyeQuest5)
				{
                    if (!Flags.DailyQuest)
                    {
						//flask of night
						if (ChosenQuestForToday == 0)
						{
							if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<FlaskMisc1>()))
							{
								Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuestReward");

								RandomQuestTimer++;
							}
							else
							{
								if (!WorldGen.crimson)
								{
									Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuest")
									+ $"\n[i:{ItemID.SoulofNight}]x10 [i:{ItemID.CursedFlame}]x15 [i:{ItemID.Bottle}]x1";
								}
								else
								{
                                    Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuest")
                                    + $"\n[i:{ItemID.SoulofNight}]x10 [i:{ItemID.Ichor}]x15 [i:{ItemID.Bottle}]x1";
                                }
                            }
						}
						//flask of light
						if (ChosenQuestForToday == 1)
						{
                            if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<FlaskMisc2>()))
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuestReward");

                                RandomQuestTimer++;
                            }
                            else
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuest")
                                + $"\n[i:{ItemID.SoulofLight}]x10 [i:{ItemID.CrystalShard}]x15 [i:{ItemID.UnicornHorn}]x3 [i:{ItemID.Bottle}]x1";
                            }
                        }
						//spooky flask
						if (ChosenQuestForToday == 2)
						{
                            if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<FlaskMisc3>()))
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuestReward");

                                RandomQuestTimer++;
                            }
                            else
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuest")
                                + $"\n[i:{ItemID.Pumpkin}]x35 [i:{ItemID.Hay}]x25 [i:{ItemID.RottenEgg}]x5 [i:{ItemID.Bottle}]x1";
                            }
                        }
						//aquatic flask
						if (ChosenQuestForToday == 3)
						{
                            if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<FlaskMisc4>()))
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuestReward");

                                RandomQuestTimer++;
                            }
                            else
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuest")
                                + $"\n[i:{ItemID.SharkFin}]x5 [i:{ItemID.Coral}]x15 [i:{ItemID.Glowstick}]x20 [i:{ItemID.Bottle}]x1";
                            }
                        }
						//sandstorm flask
						if (ChosenQuestForToday == 4)
						{
                            if (Main.player[Main.myPlayer].ConsumeItem(ModContent.ItemType<FlaskMisc5>()))
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuestReward");

                                RandomQuestTimer++;
                            }
                            else
                            {
                                Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.RandomQuest")
                                + $"\n[i:{ItemID.SandBlock}]x35 [i:{ItemID.FossilOre}]x15 [i:{ItemID.AncientBattleArmorMaterial}]x1 [i:{ItemID.Bottle}]x1";
                            }
                        }
					}
					//no quest available
					else
					{
						List<string> Dialogue = new List<string>
						{
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.NoQuests1"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.NoQuests2"),
							Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.NoQuests3"),
						};

						Main.npcChatText = Main.rand.Next(Dialogue);
					}
				}
			}
			//upgrade button
			else
			{
				Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Upgrades");
			}
		}

		public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default5"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default6"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default7"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default8"),
			};

			if (Main.player[Main.myPlayer].HasItem(ModContent.ItemType<Concoction>()) && !EggEventWorld.EggEventActive && !Flags.downedEggEvent)
            {
				return Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.EggEventAdvice");
			}

			if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
            {
				return Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.OrroBoro");
			}

			return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.spriteDirection = NPC.direction;

            NPC.velocity *= 0;
			NPC.homeless = true;

            if (Quest1Timer > 0)
			{
				QuestRewards();

				Flags.EyeQuest1 = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}

				Quest1Timer = 0;
			}

			if (Quest2Timer > 0)
			{
				QuestRewards();

				Flags.EyeQuest2 = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}

				Quest2Timer = 0;
			}

			if (Quest3Timer > 0)
			{
				QuestRewards();

				Flags.EyeQuest3 = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}

				Quest3Timer = 0;
			}

			if (Quest4Timer > 0)
			{
				QuestRewards();

				Flags.EyeQuest4 = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}

				Quest4Timer = 0;
			}

			if (Quest5Timer > 0)
			{
				QuestRewards();

				Flags.EyeQuest5 = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}

				Quest5Timer = 0;
			}

			if (RandomQuestTimer > 0)
			{
                QuestRewards();

                Flags.DailyQuest = true;
				RandomQuestTimer = 0;
            }
		}

		private void QuestRewards()
		{
			//different potions
			int[] Potions1 = new int[] { ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion, ItemID.ArcheryPotion };
			int[] Potions2 = new int[] { ItemID.BattlePotion, ItemID.CalmingPotion, ItemID.TitanPotion, ItemID.EndurancePotion };
			int[] Potions3 = new int[] { ItemID.LuckPotion, ItemID.ManaRegenerationPotion, ItemID.SummoningPotion, ItemID.ThornsPotion };

			Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), Main.rand.Next(Potions1), Main.rand.Next(1, 3));
			Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), Main.rand.Next(Potions2), Main.rand.Next(1, 3));
			Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), Main.rand.Next(Potions3), Main.rand.Next(1, 3));

			//second quest
			if (Flags.EyeQuest1 && !Flags.EyeQuest2)
			{
				Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<LittleEyeHat>());
			}
			//third quest
			if (Flags.EyeQuest2 && !Flags.EyeQuest3)
			{
				Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<CottonSwab>());
			}
			//fourth quest
			if (Flags.EyeQuest3 && !Flags.EyeQuest4)
			{
				Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<SpookyHellPylonItem>());
			}
			//final quest
			if (Flags.EyeQuest4 && !Flags.EyeQuest5)
			{
				Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<Concoction>());
			}

            //monster chest food
            if (!Flags.EyeQuest4)
            {
                Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<ChestFood>());
            }

			//drop sentient hearts after each random quest
			if (Flags.EyeQuest5)
			{
                Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<SentientHeart>());
            }

			//columbo pet item
			if (Main.rand.NextBool(50))
			{
				Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<SuspiciousBrownie>());
			}

            //money
            Main.player[Main.myPlayer].QuickSpawnItem(NPC.GetSource_GiftOrReward(), ItemID.GoldCoin, 5);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}