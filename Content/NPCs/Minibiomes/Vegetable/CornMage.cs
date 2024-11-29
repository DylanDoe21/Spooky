/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
    public class CornMage1 : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
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
            NPC.lifeMax = 100;
            NPC.damage = 20;
            NPC.defense = 5;
            NPC.width = 46;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidWarlock"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeNight_Background", Color.White)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            //walking animation
            NPC.frameCounter++;
            if (NPC.localAI[0] <= 420)
            {
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping/falling frame
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
            }
            //casting animation
            if (NPC.localAI[0] > 420)
            {
                if (NPC.frame.Y < frameHeight * 6)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 420)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.Crab;
            }

            if (NPC.localAI[0] > 420)
            {
                NPC.aiStyle = 0;

                if (NPC.localAI[0] == 480 || NPC.localAI[0] == 500 || NPC.localAI[0] == 520)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 4.5f;
                    
                    int Skull = NPC.NewNPC(NPC.GetSource_OnHit(NPC), (int)NPC.Center.X + Main.rand.Next(-20, 20), (int)NPC.Center.Y + Main.rand.Next(-10, 5), ModContent.NPCType<WarlockSkull>(), ai1: NPC.whoAmI);

					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						NetMessage.SendData(MessageID.SyncNPC, number: Skull);
					}
                }
            }

            if (NPC.localAI[0] >= 560)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidWarlockGore" + numGores).Type);
                    }
                }
            }
        }
    }
}
*/