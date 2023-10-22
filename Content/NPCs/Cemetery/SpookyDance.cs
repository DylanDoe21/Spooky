using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Cemetery
{
    public class SpookyDance : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 40;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
            AnimationType = NPCID.Ghost;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SpookyDance"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
        {
            if (NPC.localAI[0] == 0)
            {
                NPC.ai[0] = NPC.position.Y;
                NPC.localAI[0]++;
            }

            NPC.ai[1]++;
            NPC.position.Y = NPC.ai[0] + (float)Math.Sin(NPC.ai[1] / 30) * 30;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int GhostDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GemDiamond, 0f, 0f, 100, default, 2f);
                    Main.dust[GhostDust].velocity *= 1.2f;
                    Main.dust[GhostDust].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[GhostDust].scale = 0.5f;
                        Main.dust[GhostDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
		}
    }
}