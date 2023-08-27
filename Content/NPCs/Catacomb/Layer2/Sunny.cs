using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class Sunny : ModNPC  
    {
        /*
        public static List<int> BuffableNPCs = new List<int>() 
        {
            ModContent.NPCType<FloatyFlower>(),
            ModContent.NPCType<HoppingFlower>(),
            ModContent.NPCType<RollFlower>()
        };
        */

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused, 
                    BuffID.Poisoned, 
                    BuffID.Venom
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 250;
            NPC.damage = 35;
            NPC.defense = 20;
            NPC.width = 60;
            NPC.height = 84;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Sunny"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 6)
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
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            /*
            float maxDist = 600;
            int maxHealing = 5;
            int numHealing = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC other = Main.npc[i];
                if (other.active)
                {
                    if (BuffableNPCs.Contains(other.type) && Vector2.Distance(NPC.Center, other.Center) < maxDist && 
                    other.type != NPC.type && numHealing < maxHealing)
                    {
                        numHealing++;
                        int healamount = (int)(other.lifeMax * 0.05f);

                        if (other.life <= other.lifeMax - healamount && NPC.ai[1] % 20 == 0) 
                        {
                            other.life += healamount;
                        }

                        for (int numDust = 0; numDust < 3; numDust++)
                        {
                            int magicDust = Dust.NewDust(NPC.Center + (other.Center - NPC.Center) * Main.rand.NextFloat() - new Vector2(4, 4), 0, 0, ModContent.DustType<GlowyDust>());
                            Main.dust[magicDust].color = Color.Yellow;
                            Main.dust[magicDust].velocity.X *= Main.rand.NextFloat(-1.05f, 1.05f);
                            Main.dust[magicDust].velocity.Y *= Main.rand.NextFloat(-1.05f, 1.05f);
                            Main.dust[magicDust].scale = 0.12f;
                            Main.dust[magicDust].noGravity = true;
                        }
                    }

                    if (other.whoAmI != NPC.whoAmI && other.type == NPC.type && Vector2.Distance(NPC.Center, other.Center) < maxDist * 1.75 && NPC.ai[1] < other.ai[1])
                    {
                        NPC.active = false;
                    }
                }
            }
            */
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
                for (int numDust = 0; numDust < 12; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass, 0f, -2f, 0, default, 1.5f);
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SunnyGore" + numGores).Type);
                    }
                }

                //separate flower petals
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SunnyGore4").Type);
                    }
                }
            }
        }
    }
}