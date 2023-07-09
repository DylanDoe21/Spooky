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
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    [AutoloadBossHead]
    public class OrroHead : ModNPC
    {
        Vector2 SavePlayerPosition;
        
        public bool Enraged = false;
        public bool Chomp = false;
        public bool OpenMouth = false;
        private bool segmentsSpawned;

        public static readonly SoundStyle HissSound1 = new("Spooky/Content/Sounds/Orroboro/HissShort", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle HissSound2 = new("Spooky/Content/Sounds/Orroboro/HissLong", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SpitSound = new("Spooky/Content/Sounds/Orroboro/VenomSpit", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/Orroboro/OrroboroCrunch", SoundType.Sound);
        public static readonly SoundStyle HitSound = new("Spooky/Content/Sounds/EggEvent/EnemyHit", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Orroboro/OrroBestiary",
                Position = new Vector2(2f, -35f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = -24f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused, 
                    BuffID.Poisoned, 
                    BuffID.Venom,
                    BuffID.OnFire, 
                    BuffID.OnFire3,
                    BuffID.Frostburn,
                    BuffID.Frostburn2,
                    BuffID.CursedInferno,
                    BuffID.Ichor, 
                    BuffID.ShadowFlame
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(Enraged);
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
            Enraged = reader.ReadBoolean();
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
            NPC.lifeMax = 15000;
            NPC.damage = 55;
            NPC.defense = 35;
            NPC.width = 98;
            NPC.height = 88;
            NPC.npcSlots = 25f;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.netAlways = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = SoundID.Zombie40;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OrroHead"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
            });
        }

        //rotate the bosses map icon to the NPCs direction
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = NPC.rotation;
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

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Enraged)
            {
                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6f)) / 2f + 0.5f;

                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Red) * 0.5f;

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    Color newColor = color;
                    newColor = NPC.GetAlpha(newColor);
                    newColor *= 1f - fade;
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (numEffect / 4 * 6 + NPC.rotation + 0f).ToRotationVector2() * (4f * fade + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY) - NPC.velocity * numEffect;
                    Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.5f, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 80 / 3 : Main.expertMode ? 60 / 2 : 40;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            Enraged = !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());

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

            //Make the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int Segment = 0; Segment < 3; Segment++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<OrroBody>(), NPC.whoAmI, 0, latestNPC);   
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;

                        if (Main.netMode != NetmodeID.SinglePlayer)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                        }
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<OrroTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;         
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }

                    //spawn boro manually because funny shennanigans
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        //if any boro exists and it is active
                        if (Main.npc[i].type == ModContent.NPCType<BoroBodyConnect>() && Main.npc[i].active)
                        {
                            NPC.ai[1] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)Main.npc[i].Center.X, (int)Main.npc[i].Center.Y, ModContent.NPCType<BoroHead>(), ai1: NPC.whoAmI);

                            //net update so it doesnt vanish on multiplayer
                            if (Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[1]);
                            }
                        }
                    }

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.localAI[3] < 75)
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
                            Chomp = true;

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
                            Chomp = false;

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

                        if (NPC.localAI[1] < 3)
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

                                NPC.position.X = player.Center.X - 40;
                                NPC.position.Y = (NPC.Center.Y < player.Center.Y) ? player.Center.Y - 750 : player.Center.Y + 750;

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), player.Center.X, (NPC.Center.Y < player.Center.Y) ? player.Center.Y - 250 : player.Center.Y + 250,
                                0, 0, ModContent.ProjectileType<TelegraphPurple>(), 0, 0f, 0);
                            }

                            int chargeTime = Enraged ? 65 : 75;
                            if (NPC.localAI[0] == chargeTime)
                            {
                                OpenMouth = true;

                                SoundEngine.PlaySound(HissSound1, NPC.Center);

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();

                                ChargeDirection.X *= 0;
                                ChargeDirection.Y *= Enraged ? 45 : 40;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            int stopTime = Enraged ? 80 : 90;
                            if (NPC.localAI[0] > stopTime)
                            {
                                NPC.velocity *= 0.98f;
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
                                OpenMouth = false;

                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }

                    //go to players side, charge and then curve and spit biomass in sync with boro
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
                            OpenMouth = true;

                            SoundEngine.PlaySound(HissSound1, NPC.Center);

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
                                    ModContent.ProjectileType<OrroBiomatter>(), Damage, 1, Main.myPlayer, 0, 0);
                                }
                            }
                        }

                        if (NPC.localAI[0] == 170)
                        {
                            NPC.velocity *= 0.25f;
                        }

                        if (NPC.localAI[0] > 270)
                        {
                            OpenMouth = false;

                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //spit acid bolt spreads and charge while boro summons tentacle pillars
                    case 3:
                    {
                        NPC.localAI[0]++;

                        int repeats = Enraged ? 2 : 3;
                        if (NPC.localAI[1] < repeats)
                        {
                            //use chase movement
                            ChaseMovement(player, 8.5f, 0.18f);

                            //Shoot toxic spit when nearby the player
                            if (NPC.localAI[0] >= 140 && NPC.localAI[0] <= 200)
                            {
                                NPC.velocity *= 0.95f;

                                if (NPC.localAI[0] == 160 || NPC.localAI[0] == 180 || Enraged && NPC.localAI[0] == 200)
                                {
                                    OpenMouth = true;
                                        
                                    SoundEngine.PlaySound(SpitSound, NPC.Center);

                                    int MaxProjectiles = Main.rand.Next(1, 3);

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

                            if (NPC.localAI[0] == 200)
                            {
                                SavePlayerPosition = player.Center;
                            }

                            if (NPC.localAI[0] == 210)
                            {
                                SoundEngine.PlaySound(HissSound2, NPC.Center);

                                Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 25;
                                ChargeDirection.Y *= 25;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] == 230)
                            {
                                OpenMouth = false;

                                NPC.velocity *= 0.65f;
                            }

                            if (NPC.localAI[0] >= 260)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] > 60)
                            {
                                OpenMouth = false;

                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
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
                            GoTo.Y -= 750;

                            //go from side to side
                            if (NPC.localAI[0] < 138)
                            {
                                GoTo.X += -750;
                            }
                            if (NPC.localAI[0] > 138)
                            {
                                GoTo.X += 750;
                            }

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 17, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                            if (NPC.localAI[0] % 20 == 5 && NPC.localAI[1] > 0)
                            {
                                SoundEngine.PlaySound(SpitSound, NPC.Center);

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
                                NPC.localAI[0] = 20;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            if (NPC.localAI[0] <= 60)
                            {
                                Vector2 GoTo = player.Center;
                                GoTo.Y -= 750;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }
                            if (NPC.localAI[0] > 60)
                            {
                                NPC.velocity *= 0.25f;
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.ai[0]++;
                                NPC.netUpdate = true;
                            }
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
                            SoundEngine.PlaySound(HissSound1, NPC.Center);

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

            //treasure bag
            npcLoot.Add(ItemDropRule.BossBagByCondition(new DropConditions.ShouldOrroDropLootExpert(), ModContent.ItemType<BossBagOrroboro>()));
            
            //master relic and pet
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.ShouldOrroDropLootMaster(), ModContent.ItemType<OrroboroRelicItem>()));
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.ShouldOrroDropLootMaster(), ModContent.ItemType<OrroboroEye>(), 4));

            //weapon drops
            int[] MainItem = new int[] 
            { 
                ModContent.ItemType<EyeFlail>(), 
                ModContent.ItemType<Scycler>(),
                ModContent.ItemType<EyeRocketLauncher>(), 
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //material
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.ShouldOrroDropLoot(), ModContent.ItemType<ArteryPiece>(), 1, 12, 25));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<OrroMask>(), 7));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<OrroTrophyItem>(), 10));

            npcLoot.Add(notExpertRule);
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 2, ModContent.Find<ModGore>("Spooky/OrroHeadGore" + numGores).Type);
                }
            }

            return true;
        }

        public override void OnKill()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
            {
                //drop a sentient heart for each active player in the world
                if (!Flags.downedOrroboro)
                {
                    for (int numPlayer = 0; numPlayer <= Main.maxPlayers; numPlayer++)
                    {
                        if (Main.player[numPlayer].active)
                        {
                            Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<SentientHeart>());
                        }
                    }
                }

                NPC.SetEventFlagCleared(ref Flags.downedOrroboro, -1);
            }
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