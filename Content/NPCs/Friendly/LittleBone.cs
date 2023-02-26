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
		int AdviceSwitch = 0;

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
				if (!NPC.downedBoss1 && Main.LocalPlayer.statDefense < 10)
				{
					Main.npcChatText = "It seems you are still beginning your adventure. Just search around the spooky forest and see what you can discover, and maybe get some good ores underground.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//rot gourd
				else if ((NPC.downedBoss1 || Main.LocalPlayer.statDefense >= 10) && !Flags.downedRotGourd)
				{
					if (AdviceSwitch == 0)
					{
						AdviceSwitch++;
						Main.npcChatText = "Now that you have some decent gear, I have heard stories that an old rotting gourd the size of a house once looked over this biome! Breaking the pumpkins that grow here might allow you to find him.";
					}
					else if (AdviceSwitch == 1)
					{
						AdviceSwitch--;
						Main.npcChatText = "I have heard that past the tropical jungle, is a dark and foggy cemetery. You can probably search for some loot there!";
					}
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//underground spooky forest chests
				else if (Flags.downedRotGourd && !NPC.downedBoss2)
				{
					Main.npcChatText = "Now that you have that key from the giant gourd, you should be able to open those underground chests. I'm sure there is probably some useful goodies in them!";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//catacombs/valley of eyes
				else if (NPC.downedBoss2 && !Main.hardMode)
				{
					if (AdviceSwitch == 0)
					{
						AdviceSwitch++;
						Main.npcChatText = "Now that you have access into the catacombs, you should explore it. I've heard there is a lot of lost loot down there! You can find it towards one of the oceans in this world.";
					}
					else if (AdviceSwitch == 1)
					{
						AdviceSwitch--;
						Main.npcChatText = "Somewhere down in the underworld, you can find a really creepy biome filled with eyes! I do not like it there because it feels like I am being watched, but you look strong enough to explore it.";
					}
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//catacombs second layer
				else if (Main.hardMode && !NPC.downedMechBossAny)
				{
					Main.npcChatText = "Now that you have access deeper into the catacombs, you should return there. maybe the deeper parts will have some better treasure?";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//orroboro
				else if (Main.hardMode && NPC.downedMechBossAny && !Flags.downedOrroboro)
				{
					Main.npcChatText = "Now that you have defeated one of those giant robots, you can probably make a special flask using their souls to see what's inside that giant egg in the eye valley I have been hearing rumors about. Apparently there is a skilled alchemist who reisdes in the creepy eye place who can create such a substance.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//big bone
				else if (NPC.downedGolemBoss && !Flags.downedBigBone)
				{
					Main.npcChatText = "Since you have access into the catacombs arena, you can finally confront the one causing all the chaos down there.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//no advice dialogue
				else
				{
					Main.npcChatText = "Sorry, I do not have any advice to offer right now!";
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
			//three default dialogue options
			List<string> Dialogue = new List<string>
			{
				"Why am I dancing, you ask? Because it's fun, You should try it sometime.",
				"What do you mean, when is spooky season? It's always spooky season! Well, from my perspective anyway.",
			};

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()))
			{
				Dialogue.Add("Sometimes I like to talk to the other skull plants that pop up around here, but they don't seem to respond back.");
				Dialogue.Add("The spooky forest is such a wonderful place to live! Well, aside from those weird zomboids at night.");
				Dialogue.Add("I have heard rumors of secret loot found underneath this forest, but the key to open those chests has been lost ages ago.");
			}

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				Dialogue.Add("You know, for a place that exists in the underworld, it is oddly cold here.");
				Dialogue.Add("This place gives me the creeps! Somehow I feel like I am being stalked even when nothing is nearby.");
				Dialogue.Add("There is so many nasty organs and tentacles growing in this biome, it's grossing me out.");
				Dialogue.Add("The creatures that live here are so much different than anywhere else, and how are some of them even alive?");
			}

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<CemeteryBiome>()))
			{
				Dialogue.Add("This place is really foggy and dreary, and I absolutely love it!");
				Dialogue.Add("Be aware of your surroundings, I have heard there are ghosts here that can potentially possess unsuspecting humans.");
				Dialogue.Add("The cemetery is a strange area, where zombies and other undead monsters come to life. Be careful!");
			}

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<CatacombBiome>()))
			{
				Dialogue.Add("This giant underground catacomb was once used as a burial to house the deceased, but it looks like something very strange has happened here...");
				Dialogue.Add("I wonder how there is so much plant life down here, with absolutely no sunlight. Perhaps something magical is keeping them alive?");
				Dialogue.Add("Something about this place is strange. These plant creatures seem weirdly familiar.");
			}

			if (Main.LocalPlayer.ZonePurity)
            {
				Dialogue.Add("This is a nice area you've found! Very calm and peaceful.");
				Dialogue.Add("I like this place. This nice forest makes me feel at home, even outside of the spooky forest.");
            }

			if (Main.LocalPlayer.ZoneSnow)
            {
				Dialogue.Add("Brrrr, this is a very cold forest! Could also use some more spookiness, in my opinion.");
				Dialogue.Add("This place looks a bit spooky, but the temperature here is awful!");
            }

			if (Main.LocalPlayer.ZoneDesert)
            {
				Dialogue.Add("What is this horrible place? Its nothing but sand and awfully hot weather. I rate this area a zero out of ten.");
				Dialogue.Add("Ouch! Why are there so many painful prickly plants here? I thought plants were supposed to be nice and friendly!");
            }

			if (Main.LocalPlayer.ZoneJungle)
			{
				Dialogue.Add("This is a really dense and humid forest. Not the kind of weather I was expecting.");
				Dialogue.Add("Why is everything here so unfriendly? Some of these creatures are a bit too territorial, even by nature's standards.");
			}

			if (Main.LocalPlayer.ZoneGlowshroom)
            {
				Dialogue.Add("This place looks really nice, but why are you looking at me like that?");
				Dialogue.Add("I have heard of such strange places before, but I never thought you would actually bring me to one!");
				Dialogue.Add("I wonder if these mushrooms have any sentience like me... probably not though.");
            }

			if (Main.LocalPlayer.ZoneDungeon)
            {
				Dialogue.Add("This dungeon is so spooky! I wonder if those other skeletons in the distance are coming to us to spread halloween cheer?");
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