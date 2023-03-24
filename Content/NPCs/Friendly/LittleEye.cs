using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Events;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.Tiles.Pylon;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
	public class LittleEye : ModNPC
	{
		int Quest1Timer = 0;
		int Quest2Timer = 0;
		int Quest3Timer = 0;
		int Quest4Timer = 0;
		int Quest5Timer = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alchemist");
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
            Main.npcFrameCount[NPC.type] = 5;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.rarity = 1;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<SpookyHellBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A strange little eye stalk with a cool wizard hat. A decently experienced alchemist who prefers using his skills to mess with others."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<SpookyHellBiome>().ModBiomeBestiaryInfoElement)
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

		public override List<string> SetNPCNameList()
        {
            string[] names = { "Little Eye" };

            return new List<string>(names);
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "Quest";
			button2 = "Upgrades";
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			//quest button
			if (firstButton) 
            {
				if (!Flags.EyeQuest1)
				{
					if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<Flask1>()))
					{
						Main.npcChatText = "You made that potion faster than expected. I'll be holding onto these for later use, Take this stuff as a reward for your troubles.";

						Quest1Timer++;
					}
					else
                    {
						Main.npcChatText = "So, there are four different flasks you can make that I can combine into something that will be helpful later. However, I am unable to gather the needed ingredients, so I'll need your help to find them. To make this first flask, you'll need the following."
						+ $"\nMaterials: [i:{ItemID.Daybloom}]x5, [i:{ItemID.Blinkroot}]x5, [i:{ItemID.Moonglow}]x5, [i:{ItemID.PurificationPowder}]x10, [i:{ItemID.SuspiciousLookingEye}]x1, [i:{ItemID.Bottle}]x1";
					}
				}
				if (Flags.EyeQuest1 && !Flags.EyeQuest2)
				{
					if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<Flask2>()))
					{
						Main.npcChatText = "You're getting the hang of this! Here, since I know how much you love my magical little hat, you can have this bootleg version!";

						Quest2Timer++;
					}
					else
                    {	
						if (!WorldGen.crimson)
						{
							Main.npcChatText = "This next flask is similar, but again my inability to move prevents me from getting these materials, so I'll need you to craft the next flask with this stuff."
							+ $"\nMaterials: [i:{ItemID.Shiverthorn}]x5, [i:{ItemID.GlowingMushroom}]x12, [i:{ItemID.RottenChunk}]x20, [i:{ItemID.IceBlock}]x10 [i:{ItemID.WormFood}]x1, [i:{ItemID.Bottle}]x1";
						}
						else
						{
							Main.npcChatText = "This next flask is similar, but again my inability to move prevents me from getting these materials, so I'll need you to craft the next flask with this stuff."
							+ $"\nMaterials: [i:{ItemID.Shiverthorn}]x5, [i:{ItemID.GlowingMushroom}]x12, [i:{ItemID.Vertebrae}]x20, [i:{ItemID.IceBlock}]x10 [i:{ItemID.BloodySpine}]x1, [i:{ItemID.Bottle}]x1";
						}
					}
				}
				if (Flags.EyeQuest2 && !Flags.EyeQuest3)
				{
					if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<Flask3>()))
					{
						Main.npcChatText = "Good work! I've heard that this little cotton swab can be used at a shrine somewhere around here to summon a rare creature. Truth is, I just wanna see you fight it because I heard it looks really funny.";

						Quest3Timer++;
					}
					else
                    {
						Main.npcChatText = "At this point I assume you understand what to do. For this next flask, you'll need the following materials."
						+ $"\nMaterials: [i:{ItemID.Fireblossom}]x5, [i:{ItemID.Deathweed}]x5, [i:{ItemID.Ruby}]x12, [i:{ItemID.AshBlock}]x35, [i:{ItemID.DeerThing}]x1, [i:{ItemID.Bottle}]x1";
					}
				}
				if (Flags.EyeQuest3 && !Flags.EyeQuest4)
				{
					if (Main.LocalPlayer.ConsumeItem(ModContent.ItemType<Flask4>()))
					{
						Main.npcChatText = "Alright, this is the final flask! Eventually, I'll need you to bring me a special ingredient and I can make you a special concoction of mine.";

						Quest4Timer++;
					}
					else
                    {
						Main.npcChatText = "So, now that you only have one last substance to make for me, I've got a special reward for you if you can bring me it. As usual you'll need some materials to make it."
						+ $"\nMaterials: [i:{ItemID.HoneyBlock}]x35, [i:{ItemID.Pumpkin}]x35, [i:{ItemID.Sluggy}]x5, [i:{ItemID.Cobweb}]x15, [i:{ItemID.Abeemination}]x1, [i:{ItemID.Bottle}]x1";
					}
				}
				if (Flags.EyeQuest4 && !Flags.EyeQuest5)
                {
					if (Main.LocalPlayer.ConsumeItem(ItemID.SoulofSight) || Main.LocalPlayer.ConsumeItem(ItemID.SoulofMight) || Main.LocalPlayer.ConsumeItem(ItemID.SoulofFright))
					{
						Main.npcChatText = "Here, take this nasty concoction. With it, you should be able to break that giant egg over there. I am not responsible for what happens, or any injuries sustained if something bad does happen.";

						Quest5Timer++;
					}
					else
                    {
						if (!Main.hardMode)
						{
							Main.npcChatText = "In order for me to combine all four flasks, you'll need to bring me a special soul. But, I don't know if you can get them just yet."
							+ $"\nMaterials: [i:{ItemID.SoulofSight}][i:{ItemID.SoulofMight}][i:{ItemID.SoulofFright}]x1 (Any mechanical boss soul)";
						}
						else
						{
							Main.npcChatText = "In order for me to combine all four flasks, you'll need to bring me a special soul. Since you defeated that giant flesh wall, you should be able to get one now."
							+ $"\nMaterials: [i:{ItemID.SoulofSight}][i:{ItemID.SoulofMight}][i:{ItemID.SoulofFright}]x1 (Any mechanical boss soul)";
						}
					}
				}
				if (Flags.EyeQuest5)
				{
					Main.npcChatText = "Sorry, I don't have anything else for you to gather.";
				}
			}
			//upgrade button
			else
			{
				Main.npcChatText = "So, you want me to upgrade your weapons? Well, if you show me one of those peculiar flesh weapons and the heart of a giant beast, I can turn them into something a little crazy.";
			}
		}

		public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                "This place is really funky. I love watching those weird floating flesh creatures crash into stuff!",
				"Did you know there's a strange giant egg somewhere around here? That'd make enough scrambled eggs to last for weeks!",
				"You may call me an alchemist, I call it making nasty concoctions that I give to others for laughs.",
				"You may use my magic cauldron... Under the condition that you say 'The magic word'... Alright fine, you can use it whenever.",
				"My cauldron is pretty crazy. Last guy wanted to make a fire elixir to fight some giant bee, but instead he comically exploded.",
				"You know, the alchemy we do down here is not science. Nothing says alchemy more than throwing random things into a cauldron and saying gibberish afterward!",
				"If your alchemy creation doesn't completely blow up in your face, you're probably doing something wrong."
			};

			if (Main.LocalPlayer.HasItem(ModContent.ItemType<Concoction>()) && !EggEventWorld.EggEventActive && !Flags.downedEggEvent)
            {
				return "So, you must be trying to open that egg, huh? Well then, just be careful and follow my favorite saying: 'Bloody red, you'll be dead, purplish blue, cripple you!'";
			}

			if (EggEventWorld.EggEventActive)
            {
				return "You're trying to start conversation, right now? You might wanna take care of the literal abominations trying to attack you.";
			}

			if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
            {
				return "Hey there's a giant serpent chasing you. Maybe go worry about that?";
			}

			return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.homeless = false;
				NPC.homeTileX = -1;
				NPC.homeTileY = -1;
				NPC.netUpdate = true;
			}

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
		}

		private void QuestRewards()
		{
			int[] Potions1 = new int[] { ItemID.NightOwlPotion, ItemID.ShinePotion, ItemID.SpelunkerPotion, ItemID.ArcheryPotion };
			int[] Potions2 = new int[] { ItemID.BattlePotion, ItemID.CalmingPotion, ItemID.TitanPotion, ItemID.EndurancePotion };
			int[] Potions3 = new int[] { ItemID.LuckPotion, ItemID.ManaRegenerationPotion, ItemID.SummoningPotion, ItemID.ThornsPotion };

			Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), ItemID.GoldCoin, 5);

			Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), Main.rand.Next(Potions1), Main.rand.Next(2, 5));
			Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), Main.rand.Next(Potions2), Main.rand.Next(2, 5));
			Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), Main.rand.Next(Potions3), Main.rand.Next(2, 5));

			//second quest
			if (Flags.EyeQuest1 && !Flags.EyeQuest2)
			{
				Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<LittleEyeHat>());
			}
			//third quest
			if (Flags.EyeQuest2 && !Flags.EyeQuest3)
			{
				Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<CottonSwab>());
			}
			//fourth quest
			if (Flags.EyeQuest3 && !Flags.EyeQuest4)
			{
				Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<SpookyHellPylonItem>());
			}
			if (Flags.EyeQuest4 && !Flags.EyeQuest5)
			{
				Main.LocalPlayer.QuickSpawnItem(NPC.GetSource_GiftOrReward(), ModContent.ItemType<Concoction>());
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}