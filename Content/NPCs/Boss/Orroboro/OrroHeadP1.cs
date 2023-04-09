using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    [AutoloadBossHead]
    public class OrroHeadP1 : ModNPC
    {
        Vector2 SavePlayerPosition;

        public bool ShouldDamagePlayer = true;
        public bool Chomp = false;
        public bool OpenMouth = false;
        private bool segmentsSpawned;

        public static readonly SoundStyle HissSound1 = new("Spooky/Content/Sounds/Orroboro/HissShort", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle HissSound2 = new("Spooky/Content/Sounds/Orroboro/HissLong", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SpitSound = new("Spooky/Content/Sounds/Orroboro/VenomSpit", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/Orroboro/OrroboroCrunch", SoundType.Sound);
        public static readonly SoundStyle SplitSound = new("Spooky/Content/Sounds/Orroboro/OrroboroSplit", SoundType.Sound);
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/SpookyHell/EnemyHit", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Orro");
            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused, BuffID.Poisoned, BuffID.Venom, BuffID.OnFire, BuffID.CursedInferno, BuffID.Ichor, BuffID.ShadowFlame
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(ShouldDamagePlayer);
            writer.Write(Chomp);
            writer.Write(OpenMouth);
            writer.Write(segmentsSpawned);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            ShouldDamagePlayer = reader.ReadBoolean();
            Chomp = reader.ReadBoolean();
            OpenMouth = reader.ReadBoolean();
            segmentsSpawned = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 32000;
            NPC.damage = 60;
            NPC.defense = 35;
            NPC.width = 75;
            NPC.height = 75;
            NPC.npcSlots = 25f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 12, 0, 0);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = HitSound;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
        }

        public override void FindFrame(int frameHeight)
        {
            if (!Chomp)
            {
                if (!OpenMouth)
                {
                    NPC.frame.Y = frameHeight * 0;
                }
                if (OpenMouth)
                {
                    NPC.frame.Y = frameHeight * 3;
                }
            }
            if (Chomp)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    SoundEngine.PlaySound(CrunchSound, NPC.Center);

                    NPC.frame.Y = 0;
                }
            }
        }

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return ShouldDamagePlayer;
        }
        
        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 80 / 3 : Main.expertMode ? 60 / 2 : 40;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if all players are dead or not in the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.localAI[3]++;
                if (NPC.localAI[3] >= 75)
                {
                    NPC.velocity.Y = 35;
                }

                if (NPC.localAI[3] >= 240)
                {
                    NPC.active = false;
                }
            }
            else
            {
                NPC.localAI[3] = 0;
            }

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int Segment1 = 0; Segment1 < 3; Segment1++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<OrroBodyP1>(), NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        Main.npc[latestNPC].netUpdate = true;
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroBodyConnect>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[latestNPC].netUpdate = true;

                    for (int Segment2 = 0; Segment2 < 3; Segment2++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<BoroBodyP1>(), NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        Main.npc[latestNPC].netUpdate = true;
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<BoroTailP1>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    Main.npc[latestNPC].netUpdate = true;

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            //set to phase transition
            if (NPC.life <= NPC.lifeMax / 2)
			{
                NPC.ai[0] = -2;
            }

            if (NPC.localAI[3] < 75)
            {
                //attacks
                switch ((int)NPC.ai[0])
                {
                    //phase 2 transition
                    case -2:
                    {
                        OpenMouth = true;
                        Chomp = false;

                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;
                        NPC.netUpdate = true;
                        NPC.velocity *= 0.97f;

                        NPC.ai[2]++;
                        
                        //hiss
                        if (NPC.ai[2] == 2)
                        {
                            SoundEngine.PlaySound(HissSound2, NPC.Center);
                        }
                        
                        //shake
                        if (NPC.ai[2] < 180)
                        {
                            NPC.Center = new Vector2(NPC.Center.X, NPC.Center.Y);
                            NPC.Center += Main.rand.NextVector2Square(-2, 2);
                        }

                        //spawn both worms (boro is spawned by orro for ai syncing reasons)
                        if (NPC.ai[2] == 180)
                        {
                            SoundEngine.PlaySound(SplitSound, NPC.Center);

                            int Orro = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<OrroHead>());

                            //net update so it doesnt vanish on multiplayer
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: Orro);
                            }

                            NPC.netUpdate = true;
                        }

                        if (NPC.ai[2] >= 181)
                        {
                            NPC.active = false;
                            NPC.netUpdate = true;
                        }

                        break;
                    }
                    //charge up after spawning from the egg
                    case -1:
                    {
                        NPC.localAI[0]++;

                        //charge up
                        if (NPC.localAI[0] == 2)
                        {
                            OpenMouth = true;

                            SoundEngine.PlaySound(HissSound1, NPC.position);

                            NPC.velocity.X *= 0;
                            NPC.velocity.Y = -30;
                        }
                            
                        if (NPC.localAI[0] > 30)
                        {
                            OpenMouth = false;

                            NPC.velocity *= 0.35f;
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //chase the player directly, charge twice
                    case 0:
                    {
                        NPC.localAI[0]++;

                        //use chase movement
                        if (NPC.localAI[0] < 170 || (NPC.localAI[0] >= 240 && NPC.localAI[0] < 370) || NPC.localAI[0] >= 450)
                        {
                            Chomp = true;

                            Movement(player, 10f, 0.2f, true);
                        }

                        //slow down before charging
                        if (NPC.localAI[0] == 180 || NPC.localAI[0] == 380)
                        {
                            Chomp = false;

                            SavePlayerPosition = player.Center;
                            NPC.velocity *= 0.95f;
                        }

                        //charge at the saved location
                        if (NPC.localAI[0] == 190 || NPC.localAI[0] == 390)
                        {
                            OpenMouth = true;

                            SoundEngine.PlaySound(HissSound1, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 28;
                            ChargeDirection.Y *= 28;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        //slow down after charging
                        if (NPC.localAI[0] == 230 || NPC.localAI[0] == 440)
                        {
                            OpenMouth = false;
                            Chomp = false;

                            NPC.velocity *= 0.2f;
                        }

                        if (NPC.localAI[0] > 470)
                        {
                            OpenMouth = false;
                            Chomp = false;

                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //go below player, dash up, then curve back down
                    case 1:
                    {
                        NPC.localAI[0]++;

                        //go below the player
                        if (NPC.localAI[0] < 75)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += 850;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //teleport below the player, then create telegraph
                        if (NPC.localAI[0] == 75)
                        {
                            NPC.velocity *= 0;

                            NPC.position.X = player.Center.X - 20;
                            NPC.position.Y = player.Center.Y + 850;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }

                        //charge up
                        if (NPC.localAI[0] == 90)
                        {
                            OpenMouth = true;

                            SoundEngine.PlaySound(HissSound2, NPC.Center);

                            NPC.velocity.X *= 0;
                            NPC.velocity.Y = -42;
                        }

                        //turn around after vertically passing the player
                        if (NPC.localAI[0] >= 90 && NPC.localAI[0] <= 150)
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

                            if (Math.Abs(angle) > Math.PI / 2)
                            {
                                NPC.localAI[1] = Math.Sign(angle);
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 30;
                            }

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(4.5f) * NPC.localAI[1]);
                        }

                        if (NPC.localAI[0] > 150)
                        {
                            OpenMouth = false;

                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //charge at the player and shoot spreads of spit, 3 times
                    case 2:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            //chase the player
                            if (NPC.localAI[0] < 70)
                            {
                                Movement(player, 13f, 0.2f, true);
                            }

                            //slow down
                            if (NPC.localAI[0] == 70)
                            {
                                NPC.velocity *= 0.97f;
                            }

                            //charge at the player
                            if (NPC.localAI[0] == 100)
                            {
                                OpenMouth = true;

                                SoundEngine.PlaySound(SpitSound, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 30;
                                ChargeDirection.Y *= 30;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            //shoot spread of venom spit
                            if (NPC.localAI[0] == 110)
                            {
                                for (int numProjectiles = -3; numProjectiles <= 3; numProjectiles++)
                                {
                                    Vector2 velocity = NPC.velocity * 0.25f;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity.RotatedBy(MathHelper.ToRadians(12) * numProjectiles),
                                        ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer);
                                    }
                                }
                            }

                            if (NPC.localAI[0] >= 130)
                            {
                                OpenMouth = false;

                                NPC.velocity *= 0.5f;
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] >= 30)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }

                    //go to the top corner of the player, dash horizontally, and then circle the player
                    case 3:
                    {
                        NPC.localAI[0]++;

                        //go to the top corner of the player
                        if (NPC.localAI[0] <= 100)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -1300 : 1300;
                            GoTo.Y -= 400;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 35);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //teleport to the exact position before using attack
                        if (NPC.localAI[0] == 100)
                        {
                            NPC.velocity *= 0.02f;

                            NPC.position.X = (NPC.Center.X < player.Center.X) ? player.Center.X - 1300 : player.Center.X + 1300;
                            NPC.position.Y = player.Center.Y - 400;
                        }

                        //charge horizontally toward the player, but not vertically
                        if (NPC.localAI[0] == 110)
                        {
                            SoundEngine.PlaySound(HissSound1, NPC.position);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 45;
                            ChargeDirection.Y *= 0;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        //spin around after horizontally passing the player
                        if (NPC.localAI[0] >= 120 && NPC.localAI[0] <= 250)
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

                            if (Math.Abs(angle) > Math.PI / 2)
                            {
                                NPC.localAI[1] = Math.Sign(angle);
                                NPC.velocity = Vector2.Normalize(NPC.velocity) * 35;
                            }

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(5f) * NPC.localAI[1]);
                        }

                        //shoot venom spit at the player while spinning
                        if (NPC.localAI[0] >= 140 && NPC.localAI[0] <= 250)
                        {
                            OpenMouth = true;

                            if (NPC.localAI[0] % 20 == 5)
                            {
                                SoundEngine.PlaySound(SpitSound, NPC.Center);

                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= 3f;
                                ShootSpeed.Y *= 3f;

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                    ModContent.ProjectileType<EyeSpit>(), Damage, 1, Main.myPlayer, 0, 0);  
                                }
                            }
                        }

                        if (NPC.localAI[0] > 250)
                        {
                            OpenMouth = false;

                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //go below player, charge up, and shoot spreads of venom spit upward
                    case 4:
                    {
                        NPC.localAI[0]++;
                            
                        if (NPC.localAI[0] < 80)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y += 750;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 35);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 80)
                        {
                            NPC.velocity *= 0.02f;

                            NPC.position.X = player.Center.X - 20;
                            NPC.position.Y = player.Center.Y + 750;

                            Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, player.Center.Y + 225, 0, 0,
                            ModContent.ProjectileType<TelegraphRed>(), 0, 0f, 0);
                        }

                        if (NPC.localAI[0] == 100)
                        {
                            OpenMouth = true;

                            SoundEngine.PlaySound(HissSound1, NPC.Center);

                            NPC.velocity.X *= 0;
                            NPC.velocity.Y = -32;
                        }

                        if (NPC.localAI[0] == 125 || NPC.localAI[0] == 135)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            int MaxProjectiles = Main.rand.Next(5, 8);

                            for (int numProjectiles = -MaxProjectiles; numProjectiles <= MaxProjectiles; numProjectiles++)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 18f * NPC.DirectionTo(new Vector2(NPC.Center.X, NPC.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(8) * numProjectiles),
                                    ModContent.ProjectileType<EyeSpit2>(), Damage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (NPC.localAI[0] > 135)
                        {
                            OpenMouth = false;

                            NPC.velocity *= 0.97f;
                        }

                        if (NPC.localAI[0] > 300)
                        {
                            NPC.velocity *= 0.5f;
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //summon thorn pillars 2 times, then charge up at the player
                    case 5:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.Y += 750;

                            //go from side to side
                            if (NPC.localAI[0] < 120)
                            {
                                GoTo.X += 550;
                            }
                            if (NPC.localAI[0] > 120)
                            {
                                GoTo.X += -550;
                            }
                            
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                            if (NPC.localAI[0] % 20 == 5 && NPC.localAI[1] > 0)
                            {
                                for (int j = 0; j <= 0; j++) //0 was 1, 1 was 10
                                {
                                    for (int i = 0; i <= 0; i += 1) 
                                    {
                                        Vector2 center = new(NPC.Center.X, player.Center.Y + player.height / 4);
                                        center.X += j * Main.rand.Next(150, 220) * i; //distance between each spike
                                        int numtries = 0;
                                        int x = (int)(center.X / 16);
                                        int y = (int)(center.Y / 16);
                                        while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                                        Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                                        {
                                            y++;
                                            center.Y = y * 16;
                                        }
                                        while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                                        {
                                            numtries++;
                                            y--;
                                            center.Y = y * 16;
                                        }

                                        if (numtries >= 10)
                                        {
                                            break;
                                        }

                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y + 20, 0, 0, 
                                            ModContent.ProjectileType<ThornTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                        }
                                    }

                                    for (int i = -1; i <= 1; i += 2) 
                                    {
                                        Vector2 center = new(player.Center.X, player.Center.Y + player.height / 4);
                                        center.X += j * Main.rand.Next(150, 220) * i; //distance between each spike
                                        int numtries = 0;
                                        int x = (int)(center.X / 16);
                                        int y = (int)(center.Y / 16);
                                        while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                                        Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                                        {
                                            y++;
                                            center.Y = y * 16;
                                        }
                                        while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                                        {
                                            numtries++;
                                            y--;
                                            center.Y = y * 16;
                                        }

                                        if (numtries >= 10)
                                        {
                                            break;
                                        }

                                        if (Main.rand.Next(15) == 0)
                                        {
                                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                            {
                                                Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y + 20, 0, 0, 
                                                ModContent.ProjectileType<ThornTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                            }
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
                            if (NPC.localAI[0] == 30)
                            {
                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 8;
                                ChargeDirection.Y *= 8;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] >= 60)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0; 
                                NPC.ai[0] = 0;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }
                }
            }
        }

        //goofy ahh modified vanilla worm movement code
        private void Movement(Player player, float maxSpeed, float accel, bool ChaseMovement = false)
        {
            bool collision = false;
            bool FastMovement = false;

            if (Vector2.Distance(NPC.Center, player.Center) >= 500)
            {
                FastMovement = true;
            }
            if (Vector2.Distance(NPC.Center, player.Center) <= 500)           
            {
                FastMovement = false;
            }

            float speed = maxSpeed;
            float acceleration = accel;

            if (!collision)
            {
                Rectangle rectangle1 = new((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);

                int maxDistance = 350;

                if (!ChaseMovement)
                {
                    maxDistance = 350;
                }
                if (ChaseMovement)
                {
                    maxDistance = 12;
                }

                bool playerCollision = true;

                for (int index = 0; index < 255; ++index)
                {
                    if (Main.player[index].active)
                    {
                        Rectangle rectangle2 = new((int)Main.player[index].position.X - maxDistance, 
                        (int)Main.player[index].position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
                        if (rectangle1.Intersects(rectangle2))
                        {
                            playerCollision = false;
                        
                            break;
                        }
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

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 1.0)
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
            else if (collision || FastMovement)
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
                if (NPC.localAI[2] != 1)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 1f;
            }
            else
            {
                if (NPC.localAI[2] != 0.0)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[2] = 0.0f;
            }

            if ((NPC.velocity.X > 0.0 && NPC.oldVelocity.X < 0.0 || NPC.velocity.X < 0.0 && NPC.oldVelocity.X > 0.0 || 
            (NPC.velocity.Y > 0.0 && NPC.oldVelocity.Y < 0.0 || NPC.velocity.Y < 0.0 && NPC.oldVelocity.Y > 0.0)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}