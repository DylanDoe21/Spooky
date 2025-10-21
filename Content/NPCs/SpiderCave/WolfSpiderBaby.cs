using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class WolfSpiderBaby : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 4;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 40;
            NPC.defense = 10;
			NPC.width = 28;
			NPC.height = 24;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = 0.5f };
			NPC.DeathSound = SoundID.NPCDeath41 with { Pitch = 0.5f };
            NPC.aiStyle = 26;
			AIType = NPCID.WalkingAntlion;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.WolfSpiderBaby"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 4)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
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
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TinySpiderBrownGore" + numGores).Type);
                    }
                }
            }
        }
	}
}