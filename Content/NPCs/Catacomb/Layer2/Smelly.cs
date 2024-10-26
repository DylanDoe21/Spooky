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
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.Catacomb.Layer2.Projectiles;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
	public class Smelly : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 1200;
            NPC.damage = 50;
            NPC.defense = 0;
			NPC.width = 92;
			NPC.height = 42;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Smelly"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
			if (NPC.frameCounter > 7)
			{
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y >= frameHeight * 8)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
		}

        public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			if (Main.rand.NextBool())
			{
				Color color = new Color(114, 103, 42); 

				switch (Main.rand.Next(3))
				{
					//brown
					case 0:
					{
						color = new Color(114, 103, 42);
						break;
					}
					//dark orange
					case 1:
					{
						color = new Color(145, 100, 29);
						break;
					}
					//reddish orange
					case 2:
					{
						color = new Color(178, 67, 46);
						break;
					}
				}

				int DustEffect = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, color * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
				Main.dust[DustEffect].position.X += Main.rand.Next(-65, 65);
				Main.dust[DustEffect].position.Y += Main.rand.Next(-50, -10);
				Main.dust[DustEffect].velocity *= 0;
				Main.dust[DustEffect].alpha = 100;
			}

			//spawn harmful flies
			if (Main.rand.NextBool(20))
			{
				NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-75, 75), NPC.Center.Y + Main.rand.Next(-50, -10)), Vector2.Zero, ModContent.ProjectileType<SmellyFly>(), NPC.damage, 4.5f);
			}
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantChunk>(), 1, 6, 12));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SmellyPetalGore" + Main.rand.Next(1, 4)).Type);
                    }
                }

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SmellyGore" + numGores).Type);
                    }
                }
            }
        }
    }
}