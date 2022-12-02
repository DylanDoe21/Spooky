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
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Plant");
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
            Main.npcFrameCount[NPC.type] = 8;
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
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
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
					Main.npcChatText = "It seems you are still beginning your adventure. Just search around the spooky forest and see what you can discover, and maybe get some good ores underground.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//catacombs
				else if (NPC.downedBoss1 && !NPC.downedBoss2 && !Flags.downedRotGourd)
				{
					Main.npcChatText = "Now that you have access into the catacombs, you should explore it. I've heard there is a lot of lost loot down there!";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//underground spooky forest chests
				else if (Flags.downedRotGourd && !NPC.downedBoss2)
				{
					Main.npcChatText = "Now that you have better gear, why not try finding the valley of eyes? I've heard it's a super creepy eye biome somewhere in the depths of the underworld. You look strong enough to explore it.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				/*
				//this is where the ghost circus advice will be
				else if (NPC.downedBoss2 && !Main.hardMode)
				{
					Main.npcChatText = "Theres not too much spooky stuff to do right now";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				*/
				//catacombs second layer
				else if (Main.hardMode && !NPC.downedMechBossAny)
				{
					Main.npcChatText = "Now that you have access deeper into the catacombs, you should return there. maybe the deeper parts will have some better treasure?";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//orroboro
				else if (NPC.downedMechBossAny && Main.hardMode && !Flags.downedOrroboro)
				{
					Main.npcChatText = "Now that you have defeated one of those giant robots, you can probably make a special flask to see what's inside that giant egg in the eye valley.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//big bone
				else if (NPC.downedGolemBoss)
				{
					Main.npcChatText = "Since you have access into the catacombs arena, you can finally confront the one causing all the chaos down there.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//no advice dialogue
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
					Main.npcChatText = "Oh here, Take my magical transportation pot. When you use this, it will allow you to bring me anywhere on your adventures! I was hoping I could tag along and help you when you need.";
					Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, ModContent.ItemType<LittleBonePot>(), 1);
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				else
				{
					Main.npcChatText = "Oh, it looks like I already gave you my magical pot.";
					SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
				}
			}
		}

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                "Sometimes in the spooky forest I like to talk to the other skull plants if they pop up nearby this house, but they don't seem to respond to me.",
				"Why am I dancing, you ask? Because it's fun! You should try it sometime.",
			};

			if (Main.LocalPlayer.ZoneGlowshroom)
            {
				Dialogue.Add("This place looks really trippy. Why are you looking at me like that?");
            }

			if (Main.LocalPlayer.ZoneJungle)
			{
				Dialogue.Add("This is the most dense forest I have ever seen!");
			}

			if (Main.LocalPlayer.ZoneDungeon)
            {
				Dialogue.Add("This dungeon reminds me of those catacombs where all my relatives are locked away in. I don't really remember why they got put there though.");
			}

			return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.homeless = false;
				NPC.homeTileX = -1;
				NPC.homeTileY = -1;
				NPC.netUpdate = true;
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}