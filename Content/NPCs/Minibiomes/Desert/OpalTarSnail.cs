using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Minibiomes.Desert
{
    public class OpalTarSnail : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 300;
            NPC.damage = 40;
            NPC.defense = 25;
            NPC.width = 60;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1f };
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OpalTarSnail"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            //moving animation
            NPC.frameCounter++;
            if (NPC.localAI[0] == 0)
            {
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //freaky animation
            else
            {
                if (NPC.frame.Y < frameHeight * 6)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 12)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
            if (NPC.Distance(player.Center) <= 350f && lineOfSight)
            {
                NPC.localAI[0] = 1;

                if (NPC.wet)
                {
                    NPC.aiStyle = 1;
                    AIType = NPCID.Crab;
                }
                else
                {
                    NPC.aiStyle = 3;
                    AIType = NPCID.GoblinScout;
                }
            }
            else
            {
                NPC.localAI[0] = 0;

                if (NPC.wet)
                {
                    NPC.aiStyle = 1;
                    AIType = NPCID.Crab;
                }
                else
                {
                    NPC.aiStyle = 3;
                    AIType = NPCID.Crab;
                }
            }
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

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OpalTarSnailGore").Type);
                }
            }
        }
    }
}