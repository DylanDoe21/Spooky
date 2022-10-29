using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb
{
    public class FlowerSentry : ModNPC  
    {
        public static List<int> BuffableNPCs = new List<int>() 
        {
            ModContent.NPCType<FloatyFlower>(),
            ModContent.NPCType<HauntedSkull>(),
            ModContent.NPCType<HauntedSkull>(),
            ModContent.NPCType<HoppingFlower>(),
            ModContent.NPCType<RollFlower>()
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunny");
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.hardMode ? 250 : 120;
            NPC.damage = Main.hardMode ? 75 : 50;
            NPC.defense = Main.hardMode ? 40 : 20;
            NPC.width = 60;
            NPC.height = 84;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("These giant flowers watch over the catacombs, buffing nearby allies to prevent unwanted guests from exploring."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome>()))
            {
                return 15f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            if (NPC.frameCounter > 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

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

                        for (int k = 0; k < 10; k++)
                        {
                            Dust dust = Main.dust[Dust.NewDust(NPC.Center + (other.Center - NPC.Center) * Main.rand.NextFloat() - new Vector2(4, 4), 0, 0, DustID.YellowTorch)];
                            dust.noGravity = true;
                            dust.velocity *= 0.04f;
                            dust.scale *= 1.5f;
                        }
                    }

                    if (other.whoAmI != NPC.whoAmI && other.type == NPC.type && Vector2.Distance(NPC.Center, other.Center) < maxDist * 1.75 && NPC.ai[1] < other.ai[1])
                    {
                        NPC.active = false;
                    }
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 50));
        }

        public override bool CheckDead() 
		{
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FlowerSentryGore" + numGores).Type);
                }
            }

            return true;
		}
    }
}