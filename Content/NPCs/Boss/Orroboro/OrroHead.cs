using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
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
        private bool segmentsSpawned;
        public bool Enraged = false;
        public bool Chomp = false;
        public bool OpenMouth = false;
        
        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> NPCTexture;

        public static readonly SoundStyle HissSound1 = new("Spooky/Content/Sounds/Orroboro/HissShort", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle HissSound2 = new("Spooky/Content/Sounds/Orroboro/HissLong", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SpitSound = new("Spooky/Content/Sounds/Orroboro/VenomSpit", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/Orroboro/OrroboroCrunch", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/OrroBestiary",
                Position = new Vector2(0f, 85f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 65f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.CursedInferno] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.ShadowFlame] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePlayerPosition);

            //bools
            writer.Write(Enraged);
            writer.Write(Chomp);
            writer.Write(OpenMouth);
            writer.Write(segmentsSpawned);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
            SavePlayerPosition = reader.ReadVector2();

            //bools
            Enraged = reader.ReadBoolean();
            Chomp = reader.ReadBoolean();
            OpenMouth = reader.ReadBoolean();
            segmentsSpawned = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 15000;
            NPC.damage = 55;
            NPC.defense = 15;
            NPC.width = 30;
            NPC.height = 30;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 3, 0, 0);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.Zombie40;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.OrroHead"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
            });
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
                NPC.frameCounter++;
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    SoundEngine.PlaySound(CrunchSound, NPC.Center);
                    NPC.frame.Y = frameHeight * 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			if (Enraged)
            {
				for (int i = 0; i < 360; i += 30)
				{
                    Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

					Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
					spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.2f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.2f, SpriteEffects.None, 0);
				}
			}

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            int Damage = Main.masterMode ? 70 / 3 : Main.expertMode ? 55 / 2 : 40;

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            Enraged = !NPC.AnyNPCs(ModContent.NPCType<BoroHead>());

            //despawn if the player dies or leaves the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.ai[0] = -1;
            }

            //Make the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int Segment = 0; Segment < 7; Segment++)
                    {
                        int Type = Segment == 2 ? ModContent.NPCType<OrroBodyWings>() : ModContent.NPCType<OrroBody>();

                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), Type, NPC.whoAmI, 0, latestNPC);   
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        if (Type == ModContent.NPCType<OrroBody>()) Main.npc[latestNPC].frame.Y = 38 * Main.rand.Next(0, 3); //38 is the segments actual sprite height on the png
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<OrroTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;         
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    //spawn boro manually because funny shennanigans
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        //if any boro exists and it is active
                        if (Main.npc[i].type == ModContent.NPCType<BoroBodyConnect>() && Main.npc[i].active)
                        {
                            NPC.ai[1] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)Main.npc[i].Center.X, (int)Main.npc[i].Center.Y, ModContent.NPCType<BoroHead>(), ai1: NPC.whoAmI);
                            NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[1]);
                        }
                    }

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            //attacks
            switch ((int)NPC.ai[0])
            {
                //despawning
                case -1:
                {
                    Chomp = false;
                    OpenMouth = false;

                    NPC.localAI[3]++;
                    if (NPC.localAI[3] >= 45)
                    {
                        NPC.velocity.Y = 35;
                    }

                    if (NPC.localAI[3] >= 120)
                    {
                        NPC.active = false;
                    }

                    break;
                }

                //chase the player while chomping
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 3)
                    {
                        Chomp = true;

                        float speed = Enraged ? 10f : 7.5f;

                        //chase movement
                        Vector2 GoTo = player.Center;
                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1f, speed);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

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

                            NPC.position.X = player.Center.X - 50;
                            NPC.position.Y = (NPC.Center.Y < player.Center.Y) ? player.Center.Y - 750 : player.Center.Y + 750;

                            if (NPC.Center.Y < player.Center.Y)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 170, 0, 0, ModContent.ProjectileType<TelegraphRedDown>(), 0, 0f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 250, 0, 0, ModContent.ProjectileType<TelegraphRedDown>(), 0, 0f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y - 330, 0, 0, ModContent.ProjectileType<TelegraphRedDown>(), 0, 0f);
                            }
                            else
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y + 170, 0, 0, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y + 250, 0, 0, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y + 330, 0, 0, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                            }
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
                        NPC.position.Y = player.Center.Y - 30;

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X - 450, player.Center.Y, 0, 0, ModContent.ProjectileType<TelegraphRedLeft>(), 0, 0f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X - 550, player.Center.Y, 0, 0, ModContent.ProjectileType<TelegraphRedLeft>(), 0, 0f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X - 650, player.Center.Y, 0, 0, ModContent.ProjectileType<TelegraphRedLeft>(), 0, 0f);
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
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= Enraged ? 4.5f : 3f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y,
                                ModContent.ProjectileType<OrroBiomatter>(), Damage, 1, Main.myPlayer);
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
                        //chase movement
                        Vector2 GoTo = player.Center;
                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1f, 6f);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                        //Shoot spit when nearby the player
                        if (NPC.localAI[0] >= 140 && NPC.localAI[0] <= 200)
                        {
                            NPC.velocity *= 0.95f;

                            if (NPC.localAI[0] == 160 || NPC.localAI[0] == 180 || Enraged && NPC.localAI[0] == 200)
                            {
                                OpenMouth = true;
                                    
                                SoundEngine.PlaySound(SpitSound, NPC.Center);

                                for (int numProjectiles = 0; numProjectiles <= 2; numProjectiles++)
                                {
                                    Vector2 ShootSpeed = new Vector2(player.Center.X, player.Center.Y + Main.rand.Next(-50, 50)) - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed.X *= Main.rand.Next(10, 18);
                                    ShootSpeed.Y *= Main.rand.Next(10, 18);

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + NPC.velocity.X * 0.5f, NPC.Center.Y + NPC.velocity.Y * 0.5f, 
                                        ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<EyeSpit>(), Damage, 0f, Main.myPlayer, 0, 1);
                                    }
                                }
                            }
                        }

                        if (NPC.localAI[0] == 200)
                        {
                            SavePlayerPosition = player.Center;

                            NPC.netUpdate = true;
                        }

                        if (NPC.localAI[0] == 210)
                        {
                            SoundEngine.PlaySound(HissSound2, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 35;
                            NPC.velocity = ChargeDirection;
                        }

                        if (NPC.localAI[0] >= 240)
                        {
                            OpenMouth = false;
                            NPC.velocity *= 0.98f;
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

                        int Delay = Enraged ? 10 : 20;
                        int Time = Enraged ? 0 : 2;

                        //shoot projectiles only when close enough to the player's x-position
                        if (NPC.localAI[0] % Delay == Time && (NPC.Center.X > player.Center.X - 750 && NPC.Center.X < player.Center.X + 750) && NPC.localAI[1] > 0)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, Main.rand.Next(-2, 3), Main.rand.Next(4, 12), 
                                ModContent.ProjectileType<EyeSpit2>(), Damage, 0f, Main.myPlayer);
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
                                ShootSpeed.X *= Enraged ? 3f : 4f;
                                ShootSpeed.Y *= Enraged ? 3f : 4f;

                                int ProjectileType = Enraged ? ModContent.ProjectileType<OrroBiomatter>() : ModContent.ProjectileType<EyeSpit>();

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, ProjectileType, Damage, 1, Main.myPlayer, 0, 0);
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
            for (int numGores = 1; numGores <= 2; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
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
                            int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<SentientHeart>());

                            if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                            {
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                            }
                        }
                    }

                    Flags.GuaranteedRaveyard = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }
                }

                NPC.SetEventFlagCleared(ref Flags.downedOrroboro, -1);

                if (!MenuSaveSystem.hasDefeatedOrroboro)
                {
                    MenuSaveSystem.hasDefeatedOrroboro = true;
                }
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