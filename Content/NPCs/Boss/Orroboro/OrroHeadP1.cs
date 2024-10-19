using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    [AutoloadBossHead]
    public class OrroHeadP1 : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroHead";

        private bool segmentsSpawned;
        public bool Chomp = false;
        public bool OpenMouth = false;

        Vector2 SavePlayerPosition;

        public static readonly SoundStyle HissSound1 = new("Spooky/Content/Sounds/Orroboro/HissShort", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle HissSound2 = new("Spooky/Content/Sounds/Orroboro/HissLong", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SpitSound = new("Spooky/Content/Sounds/Orroboro/VenomSpit", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle CrunchSound = new("Spooky/Content/Sounds/Orroboro/OrroboroCrunch", SoundType.Sound);
        public static readonly SoundStyle SplitSound = new("Spooky/Content/Sounds/Orroboro/OrroboroSplit", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };

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
            NPC.lifeMax = 32000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 30;
            NPC.height = 30;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 12, 0, 0);
            NPC.boss = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
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
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            
            return false;
        }
        
        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if the player dies or leaves the biome
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.ai[0] = -3;
            }

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int Segment1 = 0; Segment1 < 7; Segment1++)
                    {
                        int Type = Segment1 == 2 ? ModContent.NPCType<OrroBodyWingsP1>() : ModContent.NPCType<OrroBodyP1>();

                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), Type, NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        if (Type == ModContent.NPCType<OrroBodyP1>()) Main.npc[latestNPC].frame.Y = 38 * Main.rand.Next(0, 3); //38 is the segments actual sprite height on the png
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<BoroBodyConnect>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    for (int Segment2 = 0; Segment2 < 7; Segment2++)
                    {
                        int Type = Segment2 == 2 ? ModContent.NPCType<BoroBodyWingsP1>() : ModContent.NPCType<BoroBodyP1>();

                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), Type, NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        if (Type == ModContent.NPCType<BoroBodyP1>()) Main.npc[latestNPC].frame.Y = 38 * Main.rand.Next(0, 3); //38 is the segments actual sprite height on the png
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<BoroTailP1>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            //set to phase transition
            if (NPC.life <= NPC.lifeMax / 2)
			{
                NPC.ai[0] = -2;
            }

            //attacks
            switch ((int)NPC.ai[0])
            {
                //despawning
                case -3:
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

                        if (Main.netMode != NetmodeID.SinglePlayer)
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
                        NPC.velocity.Y = -22;
                    }
                        
                    if (NPC.localAI[0] > 25)
                    {
                        OpenMouth = false;

                        NPC.velocity *= 0.3f;
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

                    if (NPC.localAI[1] < 2)
                    {
                        //chase the player
                        if (NPC.localAI[0] < 170)
                        {
                            Chomp = true;

                            //chase movement
                            Vector2 GoTo = player.Center;
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1f, 7f);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //save the players location
                        if (NPC.localAI[0] == 180)
                        {
                            SavePlayerPosition = player.Center;

                            NPC.netUpdate = true;
                        }

                        //slow down before charging
                        if ((NPC.localAI[0] >= 170 && NPC.localAI[0] < 190))
                        {
                            Chomp = false;
                            NPC.velocity *= 0.9f;
                        }

                        //charge at the saved location
                        if (NPC.localAI[0] == 190)
                        {
                            OpenMouth = true;

                            SoundEngine.PlaySound(HissSound1, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection.X *= 28;
                            ChargeDirection.Y *= 20;
                            NPC.velocity = ChargeDirection;
                        }

                        //slow down after charging
                        if (NPC.localAI[0] == 220)
                        {
                            OpenMouth = false;
                            Chomp = false;

                            NPC.velocity *= 0.2f;
                        }

                        if (NPC.localAI[0] > 250)
                        {
                            OpenMouth = false;
                            Chomp = false;

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

                        NPC.position.X = player.Center.X - (NPC.width / 2);
                        NPC.position.Y = player.Center.Y + (NPC.height / 2) + 850;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X, player.Center.Y + 170), Vector2.Zero, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X, player.Center.Y + 250), Vector2.Zero, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X, player.Center.Y + 330), Vector2.Zero, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
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
                        NPC.velocity *= 0.8f;
                    }

                    if (NPC.localAI[0] > 160)
                    {
                        OpenMouth = false;

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
                        if (NPC.localAI[0] < 100)
                        {
                            //chase movement
                            Vector2 GoTo = player.Center;
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1, 5);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //slow down
                        if (NPC.localAI[0] == 70)
                        {
                            NPC.velocity *= 0.95f;
                        }

                        //charge at the player
                        if (NPC.localAI[0] == 100)
                        {
                            OpenMouth = true;

                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 13;
                            NPC.velocity = ChargeDirection;
                        }

                        //shoot a spread of venom spit
                        if (NPC.localAI[0] == 110)
                        {
                            for (int numProjectiles = 0; numProjectiles <= 7; numProjectiles++)
                            {
                                Vector2 ShootSpeed = new Vector2(player.Center.X, player.Center.Y + Main.rand.Next(-50, 50)) - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= Main.rand.Next(8, 21);
                                ShootSpeed.Y *= Main.rand.Next(8, 21);

                                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + NPC.velocity.X * 0.5f, NPC.Center.Y + NPC.velocity.Y * 0.5f), 
                                ShootSpeed, ModContent.ProjectileType<EyeSpit>(), NPC.damage, 4.5f);
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
                        SavePlayerPosition = player.Center;

                        NPC.velocity *= 0.02f;

                        NPC.position.X = player.Center.X - (NPC.width / 2) + (NPC.Center.X < player.Center.X ? -1300 : 1300);
                        NPC.position.Y = player.Center.Y - (NPC.height / 2) - 400;

                        if (NPC.Center.X < player.Center.X)
                        {
                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X - 100, player.Center.Y - 400), Vector2.Zero, ModContent.ProjectileType<TelegraphRedLeft>(), 0, 0f);
                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X - 200, player.Center.Y - 400), Vector2.Zero, ModContent.ProjectileType<TelegraphRedLeft>(), 0, 0f);
                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X - 300, player.Center.Y - 400), Vector2.Zero, ModContent.ProjectileType<TelegraphRedLeft>(), 0, 0f);
                        }
                        else
                        {
                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X + 100, player.Center.Y - 400), Vector2.Zero, ModContent.ProjectileType<TelegraphRedRight>(), 0, 0f);
                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X + 200, player.Center.Y - 400), Vector2.Zero, ModContent.ProjectileType<TelegraphRedRight>(), 0, 0f);
                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X + 300, player.Center.Y - 400), Vector2.Zero, ModContent.ProjectileType<TelegraphRedRight>(), 0, 0f);
                        }
                    }

                    //charge horizontally toward the player, but not vertically
                    if (NPC.localAI[0] == 110)
                    {
                        SoundEngine.PlaySound(HissSound1, NPC.position);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
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
                            ShootSpeed *= 7f;

                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<EyeSpit>(), NPC.damage, 4.5f);
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

                        NPC.position.X = player.Center.X - (NPC.width / 2);
                        NPC.position.Y = player.Center.Y - (NPC.height / 2) + 750;

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X, player.Center.Y + 170), Vector2.Zero, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X, player.Center.Y + 250), Vector2.Zero, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(player.Center.X, player.Center.Y + 330), Vector2.Zero, ModContent.ProjectileType<TelegraphRedUp>(), 0, 0f);
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        OpenMouth = true;

                        SoundEngine.PlaySound(HissSound1, NPC.Center);

                        NPC.velocity.X *= 0;
                        NPC.velocity.Y = -32;
                    }

                    if (NPC.localAI[0] >= 150 && NPC.localAI[0] <= 200)
                    {
                        if (NPC.localAI[0] % 2 == 0)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + NPC.velocity.X * 0.5f, NPC.Center.Y + NPC.velocity.Y * 0.5f), 
                            new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-8f, -5f)), ModContent.ProjectileType<EyeSpit2>(), NPC.damage, 4.5f);
                        }
                    }

                    if (NPC.localAI[0] > 120)
                    {
                        NPC.velocity *= 0.95f;
                    }

                    if (NPC.localAI[0] > 200)
                    {
                        OpenMouth = false;
                    }

                    if (NPC.localAI[0] > 250)
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

                        if (NPC.localAI[0] % 30 == 10 && NPC.localAI[1] > 0)
                        {
                            Vector2 center = new(NPC.Center.X, player.Center.Y + player.height / 4);
                            center.X += Main.rand.Next(150, 220);
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

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(center.X, center.Y + 20), Vector2.Zero, ModContent.ProjectileType<FleshPillarTelegraph>(), NPC.damage, 4.5f);

                            //sometimes spawn a pillar at the players position
                            if (Main.rand.NextBool(15))
                            {
                                Vector2 PlayerCenter = new(player.Center.X, player.Center.Y + player.height / 4);
                                PlayerCenter.X += Main.rand.Next(150, 220);
                                int numPlayerTries = 0;
                                int playerX = (int)(PlayerCenter.X / 16);
                                int playerY = (int)(PlayerCenter.Y / 16);
                                while (playerY < Main.maxTilesY - 10 && Main.tile[playerX, playerY] != null && !WorldGen.SolidTile2(playerX, playerY) && 
                                Main.tile[playerX - 1, playerY] != null && !WorldGen.SolidTile2(playerX - 1, playerY) && Main.tile[playerX + 1, playerY] != null && !WorldGen.SolidTile2(playerX + 1, playerY)) 
                                {
                                    playerY++;
                                    PlayerCenter.Y = playerY * 16;
                                }
                                while ((WorldGen.SolidOrSlopedTile(playerX, playerY) || WorldGen.SolidTile2(playerX, playerY)) && numPlayerTries < 10) 
                                {
                                    numPlayerTries++;
                                    playerY--;
                                    PlayerCenter.Y = playerY * 16;
                                }

                                if (numPlayerTries >= 10)
                                {
                                    break;
                                }

                                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(PlayerCenter.X, PlayerCenter.Y + 20), Vector2.Zero, ModContent.ProjectileType<FleshPillarTelegraph>(), NPC.damage, 4.5f);
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
                        //go up to the player before looping its attack pattern
                        if (NPC.localAI[0] <= 75)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += NPC.Center.X < player.Center.X ? -475 : 475;
                            GoTo.Y -= 350;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 10, 15);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] >= 75)
                        {
                            NPC.velocity *= 0.2f;

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
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}