using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
	public class LittleBone : ModNPC
	{
		private static int ButtonMode = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Plant");
			NPCID.Sets.TownCritter[NPC.type] = true;
            Main.npcFrameCount[NPC.type] = 8;

			NPC.Happiness
			.SetBiomeAffection<Biomes.SpookyBiome>(AffectionLevel.Love)
			.SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
			.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
			.SetBiomeAffection<SpookyHellBiome>(AffectionLevel.Hate)
			.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
			.SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Like)
			.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) 
			.SetNPCAffection(NPCID.TaxCollector, AffectionLevel.Hate);
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
			NPC.friendly = true;
			NPC.townNPC = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.rarity = 1;
            NPC.aiStyle = -1;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A cute and friendly little skull creature who hangs around in his flower pot. He likes to give advice to passerbys."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
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
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0;
            }
        }

		public override List<string> SetNPCNameList()
        {
            string[] names = { "Little Bone" };

            return new List<string>(names);
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "Advice";
			button2 = "Transport";
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton) 
            {
				//spooky biome
				if (!NPC.downedBoss1)
				{
					Main.npcChatText = "It seems you are still beginning your adventure, but thats ok! As a matter of fact, I have heard that underground beneath the spooky forest, you can find special loot! I am not too sure who left it there, though.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//rot gourd
				else if (NPC.downedBoss1 && !Flags.downedRotGourd)
				{
					Main.npcChatText = "You are progressing pretty fast! I remember hearing of a giant, hostile pumpkin who used to guard this biome. Maybe you can try breaking the pumpkins in this spooky forest?";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//spooky hell
				else if (NPC.downedBoss2 && !Main.hardMode)
				{
					Main.npcChatText = "Somewhere down in the underworld, you can find a really creepy biome filled with eyes! I do not like it there because it feels like I am being watched, but you look strong enough to explore it.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//orroboro
				else if (NPC.downedMechBossAny && Main.hardMode)
				{
					Main.npcChatText = "Now that you have defeated one of those giant robots, you can probably make a special flask to see what's inside that giant egg in the living hell.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				else
				{
					Main.npcChatText = "Sorry, I do not have any advice to offer right now, come back later!";
					SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
				}
            }
			else
			{
				if (!Main.LocalPlayer.HasItem(ModContent.ItemType<LittleBonePot>()))
				{
					Main.npcChatText = "Oh, here! Take my magical transportation pot! When you use this, it will allow you to bring me anywhere on your adventures. I was hoping I could tag along and help you when you need.";
					Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, ModContent.ItemType<LittleBonePot>(), 1);
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				else
				{
					Main.npcChatText = "Oh, I already gave you my magical pot silly!";
					SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
				}
			}
		}

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{	
				/*
				//spooky forest exlusive dialogue
				"I wish I could go outside to play in the leaves, but I also can't get out of this pot! Oh well...",
				"I am not sure how these weird houses got here, but maybe you can fix them up? I would do it myself, but...",
                "Sometimes I like to talk to the other skull plants if they pop up nearby this house, but they don't seem to respond to me.",
				*/
				"Why am I always dancing you ask? I like to vibe with the season. You should try it sometime!",
			};

			if (Main.LocalPlayer.ZoneGlowshroom)
            {
				Dialogue.Add("This place looks really trippy. Why are you looking at me like that?");
            }

			if (Main.LocalPlayer.ZoneJungle)
			{
				Dialogue.Add("This is the most dense forest I have ever seen! very groovy too!");
			}

			if (Main.LocalPlayer.ZoneDungeon)
            {
				Dialogue.Add("This dungeon reminds me of that other giant dungeon where all my relatives were locked away in. I dunno why they got locked there though.");
			}

			if (Main.LocalPlayer.ZoneUnderworldHeight)
			{
				Dialogue.Add("Oh my goodness It's way too hot down here! How does anything even come down here?!");
			}

			return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
			NPC.homeless = false;
			NPC.homeTileX = -1;
			NPC.homeTileY = -1;
		}

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}