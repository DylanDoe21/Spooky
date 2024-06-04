using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
    public class NoseCultistGunner : ModNPC  
    {
        public static readonly SoundStyle GunPumpSound = new("Spooky/Content/Sounds/ScarecrowReload", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 200;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 60;
			NPC.height = 54;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.25f;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.3f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.Crab;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistGunner"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            NPC.frameCounter++;

            if (NPC.localAI[1] <= 0)
            {
                //walking animation
                if (NPC.velocity.Y == 0)
                {
                    if (NPC.frameCounter > 9)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                //falling animation
                else
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }

                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 13)
                {
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;

                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] == 1)
            {
                NPC.localAI[3] = Main.rand.Next(300, 420);
            }

            if (NPC.localAI[0] >= NPC.localAI[3] && NPC.velocity.Y == 0)
            {
                NPC.localAI[1] = 1;
            }

            if (NPC.localAI[1] > 0)
            {
                NPC.velocity *= 0;

                if (NPC.frame.Y == 7 * NPC.height && NPC.localAI[2] == 0)
                {
                    SoundEngine.PlaySound(GunPumpSound, NPC.Center);

                    NPC.localAI[2]++;
                }

                if (NPC.frame.Y == 8 * NPC.height)
                {
                    NPC.localAI[2] = 0;
                }

                if (NPC.frame.Y == 10 * NPC.height && NPC.localAI[2] == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item167, NPC.Center);

                    for (int numProjectiles = 0; numProjectiles <= 5; numProjectiles++)
                    {
                        int ShootSpeedX = NPC.direction == -1 ? Main.rand.Next(-7, -4) : Main.rand.Next(5, 8);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + 6, ShootSpeedX, Main.rand.Next(-4, 0), ModContent.ProjectileType<CultistGruntSnot>(), NPC.damage / 4, 0, NPC.target);
                    }

                    NPC.localAI[2]++;

                    NPC.netUpdate = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistGunnerGore" + numGores).Type);
                    }
                }
            }
        }
    }
}