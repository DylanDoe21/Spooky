using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Minibiomes.TarPits.Projectiles;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class Hydroraptor : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
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
            NPC.lifeMax = 80;
            NPC.damage = 30;
            NPC.defense = 12;
            NPC.width = 60;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.Item95 with { Volume = 0.8f, Pitch = 1.3f };
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Hydroraptor"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            //walking animation
            NPC.frameCounter++;
            if (NPC.localAI[0] <= 420)
            {
                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping/falling frame
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                if (NPC.velocity.Y > 0)
                {
                    NPC.frame.Y = 3 * frameHeight;
                }
            }
            //spitting animation
            if (NPC.localAI[0] > 420)
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
                if (NPC.frame.Y >= frameHeight * 16)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (NPC.velocity.Y == 0)
            {
                NPC.localAI[0]++;
            }

            if (NPC.localAI[0] <= 420)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.GoblinScout;
            }
            
            if (NPC.localAI[0] > 420)
            {
                NPC.aiStyle = 0;

				NPC.velocity.Y = 0;

                if (NPC.localAI[0] == 460)
                {
                    SoundEngine.PlaySound(SoundID.Item54 with { Pitch = -1.2f }, NPC.Center);

                    Vector2 ShootSpeed = new Vector2(player.Center.X, player.Center.Y - 30) - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 10;

                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<HydroraptorSpit>(), NPC.damage, 4.5f);
                }
            }

            if (NPC.localAI[0] >= 490)
            {
                NPC.localAI[0] = 0;
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

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/HydroraptorGore" + numGores).Type);
                    }
                }
            }
        }
    }
}