using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class Cockroach : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) 
            {
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 26;
			NPC.height = 20;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 66;
			AIType = NPCID.Buggy;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Cockroach"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CockroachGore" + numGores).Type);
                    }
                }
            }
        }
	}
}