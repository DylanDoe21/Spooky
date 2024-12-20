using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class Tarblimp : ModNPC  
    {
		bool HasJumped = false;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 120;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 78;
			NPC.height = 40;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1.3f };
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 16;
			AIType = NPCID.AnglerFish;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Tarblimp"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter++;
			if (NPC.frameCounter > 8)
			{
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y >= frameHeight * 12)
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
                for (int numDusts = 0; numDusts < 25; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, -2f, 0, default, 1f);
                    Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TarBlimpGore" + numGores).Type);
                    }
                }
            }
        }
	}
}