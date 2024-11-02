using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
	public class Sunflower1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 9;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Frame = 1
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 200;
            NPC.damage = 35;
            NPC.defense = 15;
			NPC.width = 58;
			NPC.height = 52;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 26;
			AIType = NPCID.Unicorn;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Sunflower1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
		{
			if (NPC.localAI[0] == 0)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
			else
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 7 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            	{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 9)
				{
					NPC.frame.Y = 1 * frameHeight;
				}

				//jumping frame
				if (NPC.velocity.Y < 0)
				{
					NPC.frame.Y = 3 * frameHeight;
				}
				//falling frame
				if (NPC.velocity.Y > 0)
				{
					NPC.frame.Y = 6 * frameHeight;
				}
			}
		}

        public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

			if (NPC.localAI[0] == 0)
			{
				NPC.aiStyle = 0;

				NPC.dontTakeDamage = true;
			}
			else
			{
				NPC.aiStyle = 26;
				AIType = NPCID.Unicorn;

				NPC.dontTakeDamage = false;
			}

			if ((NPC.Distance(player.Center) <= 150f || NPC.life < NPC.lifeMax) && NPC.localAI[0] == 0)
            {
				//SoundEngine.PlaySound(SoundID.Zombie74, NPC.Center);

				NPC.localAI[0] = 1;

				NPC.netUpdate = true;
			}
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantChunk>(), 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/Sunflower1Gore" + numGores).Type);
                    }
                }
            }
        }
    }

	public class Sunflower2 : Sunflower1
	{
		public override void SetDefaults()
		{
			NPC.lifeMax = 275;
            NPC.damage = 40;
            NPC.defense = 15;
			NPC.width = 54;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 26;
			AIType = NPCID.Unicorn;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Sunflower2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/Sunflower2Gore" + numGores).Type);
                    }
                }
            }
        }
	}

	public class Sunflower3 : Sunflower1
	{
		public override void SetDefaults()
		{
			NPC.lifeMax = 350;
            NPC.damage = 50;
            NPC.defense = 20;
			NPC.width = 54;
			NPC.height = 66;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.1f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 26;
			AIType = NPCID.Unicorn;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Sunflower3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/Sunflower3Gore" + numGores).Type);
                    }
                }
            }
        }
	}
}