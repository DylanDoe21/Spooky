using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
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

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
		{
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;
			Main.npcFrameCount[NPC.type] = 8;

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
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LittleBone"),
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

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Button1");
			button2 = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Button2");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			if (firstButton) 
            {
				//spooky biome
				if (!NPC.downedBoss1 && Main.player[Main.myPlayer].statDefense < 10)
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyBiome");
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//rot gourd and cemetery biome
				else if ((NPC.downedBoss1 || Main.player[Main.myPlayer].statDefense >= 10) && !Flags.downedRotGourd)
				{
					if (AdviceSwitch == 0)
					{
						AdviceSwitch++;
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.RotGourd");
					}
					else if (AdviceSwitch == 1)
					{
						AdviceSwitch--;
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CemeteryBiome");
					}
					
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//underground spooky forest chests
				else if (Flags.downedRotGourd && !NPC.downedBoss2)
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyBiomeChests");
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//spooky spirit
				else if (NPC.downedBoss2 && !Flags.downedSpookySpirit)
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookySpirit");
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//catacombs/valley of eyes
				else if (Flags.downedSpookySpirit && !Main.hardMode)
				{
					if (AdviceSwitch == 0)
					{
						AdviceSwitch++;
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombLayer1");
					}
					else if (AdviceSwitch == 1)
					{
						AdviceSwitch--;
						Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.EyeValley");
					}
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//catacombs second layer
				else if (Main.hardMode && !NPC.downedMechBossAny)
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombLayer2");
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//orroboro
				else if (Main.hardMode && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3 && !Flags.downedOrroboro)
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.OrroBoro");
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//big bone
				else if (NPC.downedGolemBoss && !Flags.downedBigBone)
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.BigBone");
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//no advice dialogue
				else
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.NoAdvice");
					SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
				}
            }
			else
			{
				if (!Main.player[Main.myPlayer].HasItem(ModContent.ItemType<LittleBonePot>()))
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Transport");
					Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, ModContent.ItemType<LittleBonePot>(), 1);
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				else
				{
					Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.NoTransport");
					SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
				}
			}
		}

        public override string GetChat()
		{
			//default dialogue options
			List<string> Dialogue = new List<string>
			{
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Default1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Default2"),
			};

			if (Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()))
			{
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyBiome1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyBiome2"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyBiome3"));
			}

			if (Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyHell1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyHell2"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyHell3"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.SpookyHell4"));
			}

			if (Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<CemeteryBiome>()))
			{
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Cemetery1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Cemetery2"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Cemetery3"));
			}

			if (Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<CatacombBiome>()))
			{
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombFirstLayer1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombFirstLayer2"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombFirstLayer3"));
			}

			if (Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<CatacombBiome2>()))
			{
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombSecondLayer1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombSecondLayer2"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.CatacombSecondLayer3"));
			}

			if (Main.player[Main.myPlayer].ZonePurity)
            {
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Forest1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Forest2"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Forest3"));
            }

			if (Main.player[Main.myPlayer].ZoneSnow)
            {
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Snow1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Snow2"));
            }

			if (Main.player[Main.myPlayer].ZoneDesert)
            {
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Desert1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Desert2"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Desert3"));
            }

			if (Main.player[Main.myPlayer].ZoneJungle)
			{
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Jungle1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Jungle2"));
			}

			if (Main.player[Main.myPlayer].ZoneGlowshroom)
            {
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Mushroom1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Mushroom2"));
            }

			if (Main.player[Main.myPlayer].ZoneDungeon)
            {
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Dungeon"));
			}

			if (Main.player[Main.myPlayer].ZoneShimmer)
			{
                Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Shimmer1"));
				Dialogue.Add(Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Shimmer2"));
            }

			return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
			NPC.velocity.X *= 0;
			NPC.homeless = true;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}