using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TortumorGiant : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/TumorScreech2", SoundType.Sound);

		public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Tortumor");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 750;
            NPC.damage = 65;
            NPC.defense = 20;
            NPC.width = 90;
            NPC.height = 92;
            NPC.npcSlots = 5f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 50, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("If regular tortumors weren't gross enough, these mutated blobs of flesh can fly around, making smaller clones of themselves to attack it's prey."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()) && !NPC.AnyNPCs(ModContent.NPCType<TortumorGiant>()))
            {
                if (!Main.hardMode)
                {
                    return 2f;
                }
                else
                {
                    return 5f;
                }
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            //regular anims
            if (NPC.ai[0] <= 480)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            //screaming animation
            if (NPC.ai[0] == 480)
            {
                NPC.frame.Y = 4 * frameHeight;
            }
            if (NPC.ai[0] > 480)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
			Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.rotation = NPC.velocity.X * 0.04f;

            int Damage = Main.expertMode ? 35 : 50;
			
			NPC.ai[0]++;  

            if (NPC.ai[0] <= 480)
            {
                int MaxSpeed = 30;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 8) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 8)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X = MoveSpeedX * 0.1f;
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y = MoveSpeedY * 0.1f;
            }

            if (NPC.ai[0] >= 480)
            {
                if (NPC.ai[0] == 480)
                {
                    SoundEngine.PlaySound(ScreechSound, NPC.Center);
                }

                NPC.velocity *= 0.95f;

                //only summon tortumors if no other tortumors exist
                if (!NPC.AnyNPCs(ModContent.NPCType<Tortumor>()))
                {
                    if (NPC.ai[0] == 555)
                    {
                        for (int numSpawns = 0; numSpawns < 2; numSpawns++)
                        {
                            int TumorSummon = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + Main.rand.Next(-150, 150), 
                            (int)NPC.Center.Y + Main.rand.Next(-150, 150), ModContent.NPCType<Tortumor>());

                            //set tortumor ai to -2 so the dust effect happens
                            Main.npc[TumorSummon].ai[0] = -2;

                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, TumorSummon);
                            }
                        }

                        //use npc.ai[1] to prevent projectile shooting attack from running after summoning
                        NPC.ai[1] = 1;
                    }
                }
                else
                {
                    if (NPC.ai[0] >= 550 && NPC.ai[1] == 0)
                    {
                        int[] Projectiles = new int[] { ModContent.ProjectileType<TumorOrb1>(), 
                        ModContent.ProjectileType<TumorOrb2>(), ModContent.ProjectileType<TumorOrb3>() };

                        if (Main.rand.Next(3) == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item87, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {   
                                int TumorOrb = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                                0, 0, Main.rand.Next(Projectiles), Damage, 0f, Main.myPlayer, 0f, (float)NPC.whoAmI);
                                Main.projectile[TumorOrb].ai[0] = 179;
                            }   
                        }
                    }
                }

                if (NPC.ai[0] >= 580)
                {
                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                }
            }
        }

        public override bool CheckDead() 
		{
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TortumorGiantGore" + numGores).Type);
                }
            }

            for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), 
                NPC.width / 2, NPC.height / 2, DustID.Blood, 0f, 0f, 100, default, 2f);

                Main.dust[DustGore].scale *= Main.rand.NextFloat(1f, 2f);
                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            return true;
		}
    }
}