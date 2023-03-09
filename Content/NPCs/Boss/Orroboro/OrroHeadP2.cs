using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    //[AutoloadBossHead]
    public class OrroHeadP2 : ModNPC
    {
        public bool Enraged = false;
        private bool spawned;

        public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/SpookyHell/OrroboroCrunch", SoundType.Sound);
        public static readonly SoundStyle GrowlSound = new("Spooky/Content/Sounds/SpookyHell/OrroboroGrowl1", SoundType.Sound);
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/SpookyHell/OrroboroDeath", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orro");

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Orroboro/OrroBestiary",
                Position = new Vector2(2f, -24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -24f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(Enraged);
            writer.Write(spawned);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            Enraged = reader.ReadBoolean();
            spawned = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 18000 / 3 : Main.expertMode ? 14500 / 2 : 10000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 75;
            NPC.height = 75;
            NPC.npcSlots = 25f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }
        
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A fast and calculated serpent that works together with Boro to defend it's territory. These serpents are constantly growing, and constantly consuming the other's flesh."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			float Divide = 1.8f;

			if (projectile.penetrate <= -1)
			{
				damage /= (int)Divide;
			}
			else if (projectile.penetrate >= 3)
			{
				damage /= (int)Divide;
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //if orro is in its enraged state
            if (Enraged)
            {
                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (6.28318548f + NPC.rotation + 0f).ToRotationVector2() * (4f * fade + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity;
                Main.EntitySpriteDraw(tex, vector, NPC.frame, Color.Red * 0.75f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.5f, SpriteEffects.None, 0);
            }

            Texture2D texture =  ModContent.Request<Texture2D>(Texture).Value;
            Rectangle frame = new Rectangle(0, NPC.frame.Y, texture.Width, texture.Height / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
            Main.spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {   
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 100 / 3 : Main.expertMode ? 80 / 2 : 50;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            Enraged = !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());

            //despawn if all players are dead or not in the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.localAI[3]++;
                if (NPC.localAI[3] >= 75)
                {
                    NPC.velocity.Y = 25;
                    NPC.EncourageDespawn(10);
                }
            }
            else
            {
                NPC.localAI[3] = 0;
            }

            //Make the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!spawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int LatestNPC = NPC.whoAmI;

                    //spawn the 3 orro segments
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroBody2>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroBody1>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    //spawn tail
                    LatestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroTail>(), NPC.whoAmI, 0, LatestNPC);                   
                    Main.npc[LatestNPC].realLife = NPC.whoAmI;
                    Main.npc[LatestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[LatestNPC].netUpdate = true;

                    //spawn boro manually because funny shennanigans
                    NPC.ai[1] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BoroHead>(), ai1: NPC.whoAmI);
                    Enraged = !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());
                    
                    NPC.netUpdate = true;
                    spawned = true;
                }
            }

            if (!player.dead && player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()) && NPC.localAI[3] < 75)
            {
                //attacks
                switch ((int)NPC.ai[0])
                {
                    //chase the player while chomping
                    case 0:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            float speed = Enraged ? 13f : 10f;
                            float acceleration = Enraged ? 0.16f : 0.18f;
                            ChaseMovement(player, speed, acceleration);
                            
                            if (NPC.localAI[0] >= 145)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            //sync boros ai to prevent being slightly off sync
                            for (int i = 0; i < Main.maxNPCs; i++)
                            {
                                //if any boro exists and it is active
                                if (Main.npc[i].type == ModContent.NPCType<BoroHead>() && Main.npc[i].active)
                                {
                                    Main.npc[i].localAI[0] = 0;
                                    Main.npc[i].localAI[1] = 0;
                                    Main.npc[i].ai[0] = 1;
                                    Main.npc[i].netUpdate = true;
                                }
                            }

                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //charge from the top/bottom while boro dashes
                    case 1:
                    {
                        NPC.localAI[0]++;

                        int repeats = Enraged ? 4 : 3;
                        if (NPC.localAI[1] < repeats)
                        {
                            int positionTime = Enraged ? 50 : 60;
                            if (NPC.localAI[0] < positionTime)
                            {
                                Vector2 GoTo = player.Center;
                                GoTo.X += 0;
                                GoTo.Y += (NPC.Center.Y < player.Center.Y) ? -750 : 750;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 30);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }

                            if (NPC.localAI[0] == positionTime)
                            {
                                NPC.velocity *= 0;

                                NPC.position.X = player.Center.X - 20;
                                NPC.position.Y = (NPC.Center.Y < player.Center.Y) ? player.Center.Y - 750 : player.Center.Y + 750;

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, (NPC.Center.Y < player.Center.Y) ? player.Center.Y - 250 : player.Center.Y + 250, 
                                0, 0, ModContent.ProjectileType<TelegraphPurple>(), 0, 0f, 0);
                            }

                            int chargeTime = Enraged ? 65 : 75;
                            if (NPC.localAI[0] == chargeTime)
                            {
                                SoundEngine.PlaySound(GrowlSound, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 0;
                                ChargeDirection.Y *= Enraged ? 45 : 40;  
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            int stopTime = Enraged ? 85 : 100;
                            if (NPC.localAI[0] > stopTime)
                            {
                                NPC.velocity *= 0.99f;
                            }

                            int extraTime = Enraged ? 15 : 45;
                            if (NPC.localAI[0] >= stopTime + extraTime)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 60)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }

                    //go to players side, charge and then curve and spit projectiles in sync with boro
                    case 2:
                    {
                        NPC.localAI[0]++;
                        
                        if (NPC.localAI[0] < 75)
                        {
                            //this is slightly offset so its even with the other worm in game
                            Vector2 GoTo = player.Center;
                            GoTo.X -= 1250;
                            GoTo.Y += 0;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        
                        //set exact position right before so the curling up is always even
                        if (NPC.localAI[0] == 75)
                        {
                            NPC.velocity *= 0;

                            //this is slightly offset so its even with the other worm in game
                            NPC.position.X = player.Center.X - 1250;
                            NPC.position.Y = player.Center.Y + 0;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X - 550, player.Center.Y, 0, 0,
                            ModContent.ProjectileType<TelegraphPurple>(), 0, 0f, 0);
                        }

                        if (NPC.localAI[0] == 90)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.Center);

                            NPC.velocity.X = Enraged ? 48 : 42;
                            NPC.velocity.Y *= 0;
                        }

                        if (NPC.localAI[0] >= 125 && NPC.localAI[0] <= 170)
                        {
                            double angle = NPC.DirectionTo(player.Center).ToRotation() - NPC.velocity.ToRotation();
                            while (angle > Math.PI)
                            {
                                angle -= 2.0 * Math.PI;
                            }
                            while (angle < -Math.PI)
                            {
                                angle += 2.0 * Math.PI;
                            }

                            NPC.localAI[1] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 32;

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4f) * NPC.localAI[1]);

                            if (NPC.localAI[0] == 130 || NPC.localAI[0] == 140 || NPC.localAI[0] == 150 || NPC.localAI[0] == 160 || NPC.localAI[0] == 170)
                            {
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= Enraged ? 4.5f : 3f;
                                ShootSpeed.Y *= Enraged ? 4.5f : 3f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                    ModContent.ProjectileType<EyeSpit>(), Damage, 1, Main.myPlayer, 0, 0);  
                                }
                            }
                        }

                        if (NPC.localAI[0] == 170)
                        {
                            NPC.velocity *= 0.25f;
                        }

                        if (NPC.localAI[0] > 270)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //spit acid bolt spreads and chase player while boro summons tentacle pillars
                    case 3:
                    {
                        NPC.localAI[0]++;

                        int repeats = Enraged ? 5 : 3;
                        if (NPC.localAI[1] < repeats)
                        {
                            //use chase movement
                            ChaseMovement(player, 8.5f, 0.18f);

                            //Shoot toxic spit when nearby the player
                            if (NPC.localAI[0] >= 140 && NPC.localAI[0] < 200) 
                            {
                                NPC.velocity *= 0.95f;

                                if (NPC.localAI[0] == 160 || NPC.localAI[0] == 180 || (Enraged && NPC.localAI[0] == 200)) 
                                {
                                    SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                                    int MaxProjectiles = Main.rand.Next(2, 4);

                                    float speed = Enraged ? 4.8f : 4.2f;

                                    for (int numProjectiles = -MaxProjectiles; numProjectiles <= MaxProjectiles; numProjectiles++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center,
                                            speed * NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(10) * numProjectiles),
                                            ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }
                            
                            if (NPC.localAI[0] >= 240)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0]++; 
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //fly above player and drop projectiles down while boro uses acid breath
                    case 4:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y -= 750;

                            //go from side to side
                            if (NPC.localAI[0] < 120)
                            {
                                GoTo.X += -1000;
                            }
                            if (NPC.localAI[0] > 120)
                            {
                                GoTo.X += 1000;
                            }
                            
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 17, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                            if (NPC.localAI[0] % 20 == 5 && NPC.localAI[1] > 0)
                            {
                                //if enraged shoot spreads of fangs downward
                                if (Enraged)
                                {
                                    Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y + 50);

                                    for (int numProjectiles = -1; numProjectiles <= 1; numProjectiles++)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center,
                                            4.5f * NPC.DirectionTo(position).RotatedBy(MathHelper.ToRadians(10) * numProjectiles),
                                            ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                                else
                                {
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0, 5, 
                                        ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer, 0, 0);
                                    }
                                }
                            }

                            if (NPC.localAI[0] >= 275)
                            {
                                NPC.velocity *= 0.25f;
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            NPC.velocity *= 0.25f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
                            NPC.ai[0]++; 
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //circle and chase other worm
                    case 5:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] < 119)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X -= 1000;
                            GoTo.Y += 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        
                        //set exact position right before so it is even
                        if (NPC.localAI[0] == 119)
                        {
                            NPC.velocity *= 0;

                            NPC.position.X = player.Center.X - 1000;
                            NPC.position.Y = player.Center.Y + 750;
                        }

                        if (NPC.localAI[0] == 120)
                        {
                            SoundEngine.PlaySound(GrowlSound, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= Enraged ? 45 : 40;
                            ChargeDirection.Y *= 0;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] >= 151 && NPC.localAI[0] <= 500)
                        {
                            if (NPC.localAI[0] >= 250 && NPC.localAI[0] <= 380)
                            {
                                int frequency = Enraged ? 5 : 8;

                                if (NPC.localAI[0] % 20 == frequency)
                                {
                                    Vector2 ShootSpeed = player.Center - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed.X *= Enraged ? 3f : 2f;
                                    ShootSpeed.Y *= Enraged ? 3f : 2f;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                        ModContent.ProjectileType<EyeSpit>(), Damage, 1, Main.myPlayer, 0, 0);  
                                    }
                                }

                                NPC.localAI[2] += 0.025f;
                            }

                            double angle = NPC.velocity.ToRotation();

                            //always subtract angle so it goes in a set circle
                            angle -= 3.5 * Math.PI;

                            NPC.localAI[1] = Math.Sign(angle);
                            NPC.velocity = Vector2.Normalize(NPC.velocity) * 50;

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(3.5f + NPC.localAI[2]) * NPC.localAI[1]);
                        }

                        if (NPC.localAI[0] >= 500)
                        {
                            NPC.velocity *= 0.25f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[0] = 0;
                            NPC.netUpdate = true;
                        }

                        break;
                    }
                }
            }
        }

        private void ChaseMovement(Player player, float maxSpeed, float accel)
        {
            bool collision = false;
            float speed = maxSpeed;
            float acceleration = accel;

            if (!collision)
            {
                int maxDistance = 12;
                bool playerCollision = true;
                Rectangle rectangle1 = new((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                
                if (player.active)
                {
                    Rectangle rectangle2 = new((int)player.position.X - maxDistance, 
                    (int)player.position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                    if (rectangle1.Intersects(rectangle2))
                    {
                        playerCollision = false;
                    }
                }

                if (playerCollision)
                {
                    collision = true;
                }
            }

            Vector2 NPCCenter = new(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float targetXPos = player.position.X + (player.width / 2);
            float targetYPos = player.position.Y + (player.height / 2);

            float targetRoundedPosX = (int)(targetXPos / 16.0) * 16;
            float targetRoundedPosY = (int)(targetYPos / 16.0) * 16;
            NPCCenter.X = (int)(NPCCenter.X / 16.0) * 16;
            NPCCenter.Y = (int)(NPCCenter.Y / 16.0) * 16;
            float dirX = targetRoundedPosX - NPCCenter.X;
            float dirY = targetRoundedPosY - NPCCenter.Y;
            float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            
            if (!collision)
            {
                NPC.TargetClosest(true);

                if (NPC.velocity.Y > speed)
                {
                    NPC.velocity.Y = speed;
                }
                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                }

                else if (NPC.velocity.Y == speed)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                }
                else if (NPC.velocity.Y > 4.0)
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 1f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 1f; 
                    }
                }
            }

            if (!collision)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y = NPC.velocity.Y + 0.11f;

                if (NPC.velocity.Y > speed)
                { 
                    NPC.velocity.Y = speed;
                }

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 1.0) //was 0.5
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X += acceleration * 1.4f;
                    }
                }
                
                if (NPC.velocity.Y > 5.0) 
                {
                    if (NPC.velocity.X < 0.0)
                    {
                        NPC.velocity.X += acceleration * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X -= acceleration * 0.9f;
                    }
                }
            }
            else if (collision)
            {
                float absDirX = Math.Abs(dirX);
                float absDirY = Math.Abs(dirY);
                float newSpeed = speed / length;
                dirX *= newSpeed;
                dirY *= newSpeed;

                if (NPC.velocity.X > 0.0 && dirX > 0.0 || NPC.velocity.X < 0.0 && dirX < 0.0 || (NPC.velocity.Y > 0.0 && dirY > 0.0 || NPC.velocity.Y < 0.0 && dirY < 0.0))
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration;
                    }
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration;
                    }

                    if (Math.Abs(dirY) < speed * 0.2 && (NPC.velocity.X > 0.0 && dirX < 0.0 || NPC.velocity.X < 0.0 && dirX > 0.0))
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration * 2f;
                        }
                    }
                    if (Math.Abs(dirX) < speed * 0.2 && (NPC.velocity.Y > 0.0 && dirY < 0.0 || NPC.velocity.Y < 0.0 && dirY > 0.0))
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration * 2f;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration * 2f;
                        }
                    }
                }
                else if (absDirX > absDirY)
                {
                    if (NPC.velocity.X < dirX)
                    {
                        NPC.velocity.X += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.X > dirX)
                    {
                        NPC.velocity.X -= acceleration * 1.1f;
                    }

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.Y > 0.0)
                        {
                            NPC.velocity.Y += acceleration;
                        }
                        else
                        {
                            NPC.velocity.Y -= acceleration;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < dirY)
                    {
                        NPC.velocity.Y += acceleration * 1.1f;
                    }
                    else if (NPC.velocity.Y > dirY)
                    {
                        NPC.velocity.Y -= acceleration * 1.1f;
                    }

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                    {
                        if (NPC.velocity.X > 0.0)
                        {
                            NPC.velocity.X += acceleration;
                        }
                        else
                        {
                            NPC.velocity.X -= acceleration;
                        }
                    }
                }
            }

            //Some netupdate stuff
            if (collision)
            {
                if (NPC.ai[2] != 1)
                {
                    NPC.netUpdate = true;
                }
                NPC.ai[2] = 1f;
            }
            else
            {
                if (NPC.ai[2] != 0.0)
                {
                    NPC.netUpdate = true;
                }
                NPC.ai[2] = 0.0f;
            }

            if ((NPC.velocity.X > 0.0 && NPC.oldVelocity.X < 0.0 || NPC.velocity.X < 0.0 && NPC.oldVelocity.X > 0.0 || 
            (NPC.velocity.Y > 0.0 && NPC.oldVelocity.Y < 0.0 || NPC.velocity.Y < 0.0 && NPC.oldVelocity.Y > 0.0)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());
            
            if (Enraged)
            {
                npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagOrro>()));

                npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<OrroboroEye>(), 4));
                npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<OrroboroRelicItem>()));

                int[] MainItem = new int[] { ModContent.ItemType<EyeFlail>(), ModContent.ItemType<Scycler>(), 
                ModContent.ItemType<EyeRocketLauncher>(), ModContent.ItemType<MouthFlamethrower>(), 
                ModContent.ItemType<LeechStaff>(), ModContent.ItemType<LeechWhip>() };

                notExpertRule.OnSuccess(ItemDropRule.Common(Main.rand.Next(MainItem)));

                notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OrroboroChunk>(), 1, 12, 25));

                npcLoot.Add(notExpertRule);
            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedOrroboro, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}