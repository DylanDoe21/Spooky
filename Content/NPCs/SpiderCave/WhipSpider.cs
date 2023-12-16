using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.NPCs.SpiderCave.Projectiles;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class WhipSpider : ModNPC  
    {
        int SaveDirection;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 700;
            NPC.damage = 45;
            NPC.defense = 25;
            NPC.width = 92;
			NPC.height = 60;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit45;
			NPC.DeathSound = SoundID.NPCDeath34;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.WhipSpider"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter++;

            if (NPC.localAI[1] == 0)
            {
                if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping frames
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
                if (NPC.velocity.Y > 0)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = 6 * frameHeight;
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] >= 600 && player.Distance(NPC.Center) <= 300f)
            {
                NPC.localAI[1] = 1;
            }

            if (NPC.localAI[1] == 0)
            {
                NPC.aiStyle = 26;
			    AIType = NPCID.Unicorn;

                SaveDirection = NPC.direction;
            }
            else
            {
                NPC.aiStyle = 0;

                NPC.direction = SaveDirection;

                NPC.localAI[2]++;

                if (NPC.localAI[2] == 100 || NPC.localAI[2] == 120 || NPC.localAI[2] == 140)
                {
                    int Tongue = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.direction == -1 ? -45 : 45), 
                    (int)NPC.Center.Y, ModContent.NPCType<WhipSpiderTongue>(), ai3: NPC.whoAmI);
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: Tongue);
                    }
                }

                if (NPC.localAI[2] >= 180)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;

                    NPC.netUpdate = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WhipSpiderGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WhipSpiderLegGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WhipSpiderClawGore").Type);
                    }
                }
            }
        }
    }
}