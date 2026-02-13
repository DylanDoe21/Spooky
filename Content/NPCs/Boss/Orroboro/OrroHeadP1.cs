using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    [AutoloadBossHead]
    public class OrroHeadP1 : ModNPC
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/Orroboro/OrroHead";

		int Tongue = 0;

		public float SaveRotation = 0;

		private bool segmentsSpawned;
        public bool Chomp = false;
        public bool OpenMouth = false;
        public bool DefaultRotation = true;

        Vector2 SaveNPCPosition;
        Vector2 SavePlayerPosition;

		int[] BlockTypes = new int[]
		{
			ModContent.TileType<SpookyMush>(),
			ModContent.TileType<SpookyMushGrass>(),
			ModContent.TileType<LivingFlesh>()
		};

        private static Asset<Texture2D> GlowTexture;

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
            writer.WriteVector2(SaveNPCPosition);
            writer.WriteVector2(SavePlayerPosition);

            //bools
            writer.Write(Chomp);
            writer.Write(OpenMouth);
            writer.Write(DefaultRotation);
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
            SaveNPCPosition = reader.ReadVector2();
            SavePlayerPosition = reader.ReadVector2();

            //bools
            Chomp = reader.ReadBoolean();
            OpenMouth = reader.ReadBoolean();
            DefaultRotation = reader.ReadBoolean();
            segmentsSpawned = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 50000;
            NPC.damage = 55;
            NPC.defense = 30;
            NPC.width = 30;
            NPC.height = 30;
            NPC.npcSlots = 8f;
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
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Orroboro/OrroHeadGlow");

            Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture).Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White) * 0.5f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
        
        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			if (SaveRotation != 0)
			{
				NPC.rotation = SaveRotation;
			}
			else
			{
				if (DefaultRotation)
				{
					NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;
				}
				else
				{
					Vector2 RotateTowards = player.Center - NPC.Center;

					float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 1.57f;
					float RotateSpeed = 0.1f;

					NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);
				}
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
                        int Type = Segment1 == 2 ? ModContent.NPCType<OrroBodyWings>() : ModContent.NPCType<OrroBody>();

                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), Type, NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        if (Type == ModContent.NPCType<OrroBody>()) Main.npc[latestNPC].frame.Y = 38 * Main.rand.Next(0, 3); //38 is the segments actual sprite height on the png
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<BoroBodyConnect>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    for (int Segment2 = 0; Segment2 < 7; Segment2++)
                    {
                        int Type = Segment2 == 2 ? ModContent.NPCType<BoroBodyWings>() : ModContent.NPCType<BoroBody>();

                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), Type, NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        if (Type == ModContent.NPCType<BoroBody>()) Main.npc[latestNPC].frame.Y = 38 * Main.rand.Next(0, 3); //38 is the segments actual sprite height on the png
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<BoroTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            //despawn if the player dies or leaves the biome
            if (player.dead || !player.active || !player.InModBiome(ModContent.GetInstance<Biomes.SpookyHellBiome>()))
            {
                NPC.velocity.Y += 0.25f;
				NPC.EncourageDespawn(60);
				return;
            }

            //set to phase transition
            if (NPC.life <= NPC.lifeMax / 2)
			{
                NPC.ai[0] = -2;
            }

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

                            NPC.netUpdate = true;
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

                //go to the players side and then dash and curl around
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 2)
                    {
                        SavePlayerPosition = new Vector2(NPC.Center.X < player.Center.X ? -550 : 550, Main.rand.Next(-200, 200));
						NPC.netUpdate = true;
                    }
                    
                    if (NPC.localAI[0] > 2 && NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo += SavePlayerPosition;

						bool IsPositionInTiles = TileGlobal.SolidCollisionWithSpecificTiles(player.Center + SavePlayerPosition - new Vector2(5, 5), 10, 10, BlockTypes);

                        if (NPC.Distance(GoTo + SavePlayerPosition) > 100f)
                        {
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 25);
                            NPC.velocity.X = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f).X;

                            if (!IsPositionInTiles)
                            {
                                NPC.velocity.Y = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f).Y;
                            }
                            else
                            {
                                NPC.velocity.Y *= 0.85f;
                            }
                        }
                        else
                        {
                            NPC.velocity.X *= 0.85f;
                        }
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        OpenMouth = true;

                        NPC.velocity *= 0.5f;
                        SavePlayerPosition = player.Center;
                        SaveNPCPosition = NPC.Center;
                    }

                    if (NPC.localAI[0] > 60 && NPC.localAI[0] <= 90)
                    {
                        DefaultRotation = false;

                        NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
						NPC.Center += Main.rand.NextVector2Square(-10, 10);
                    }

                    //charge
                    if (NPC.localAI[0] == 90)
                    {
                        DefaultRotation = true;

                        SoundEngine.PlaySound(HissSound2, NPC.Center);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                        ChargeDirection *= 40;
                        NPC.velocity = ChargeDirection;

                        NPC.netUpdate = true;
                    }

                    //turn around after passing the player
                    if (NPC.localAI[0] >= 90 && NPC.localAI[0] <= 175)
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

                        NPC.velocity = NPC.velocity.RotatedBy(MathHelper.ToRadians(8f) * NPC.localAI[1]);
                    }

                    if (NPC.localAI[0] > 175)
                    {
                        NPC.velocity *= 0.8f;
                    }

                    if (NPC.localAI[0] > 185)
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
                            OpenMouth = true;

                            NPC.velocity *= 0.95f;
                        }

                        //charge at the player
                        if (NPC.localAI[0] == 100)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 13;
                            NPC.velocity = ChargeDirection;

                            for (int numProjectiles = 0; numProjectiles <= 7; numProjectiles++)
                            {
                                Vector2 ShootSpeed = new Vector2(player.Center.X, player.Center.Y + Main.rand.Next(-50, 50)) - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= Main.rand.Next(8, 21);
                                ShootSpeed.Y *= Main.rand.Next(8, 21);

                                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + NPC.velocity.X * 0.5f, NPC.Center.Y + NPC.velocity.Y * 0.5f), 
                                ShootSpeed, ModContent.ProjectileType<EyeSpit>(), NPC.damage, 4.5f);
                            }

                            NPC.netUpdate = true;
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

                    if (NPC.localAI[0] == 2)
                    {
                        SavePlayerPosition = new Vector2(NPC.Center.X < player.Center.X ? -600 : 600, -100);
						NPC.netUpdate = true;
                    }
                    
                    if (NPC.localAI[0] > 2 && NPC.localAI[0] < 60)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo += SavePlayerPosition;

                        if (NPC.Distance(GoTo + SavePlayerPosition) > 100f)
                        {
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        else
                        {
                            NPC.velocity *= 0.85f;
                        }
                    }

                    if (NPC.localAI[0] == 60)
                    {
                        OpenMouth = true;

                        SavePlayerPosition = player.Center;
                        SaveNPCPosition = NPC.Center;
                    }

                    if (NPC.localAI[0] > 60 && NPC.localAI[0] <= 80)
                    {
                        DefaultRotation = false;

                        NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
						NPC.Center += Main.rand.NextVector2Square(-10, 10);
                    }

                    //charge horizontally toward the player, but not vertically
                    if (NPC.localAI[0] == 80)
                    {
                        DefaultRotation = true;

                        SoundEngine.PlaySound(HissSound1, NPC.position);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection *= 38;
						NPC.velocity = ChargeDirection;

                        NPC.netUpdate = true;
                    }

                    //spin around after horizontally passing the player
                    if (NPC.localAI[0] >= 90 && NPC.localAI[0] <= 220)
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
                    if (NPC.localAI[0] >= 110 && NPC.localAI[0] <= 220)
                    {
                        if (NPC.localAI[0] % 20 == 5)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 7f;

                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<EyeSpit>(), NPC.damage, 4.5f);
                        }
                    }

                    if (NPC.localAI[0] > 220)
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
                        
                    if (NPC.localAI[0] == 2)
                    {
                        SavePlayerPosition = new Vector2(0, 650);
						NPC.netUpdate = true;
                    }
                    
                    if (NPC.localAI[0] > 2 && NPC.localAI[0] < 85)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo += SavePlayerPosition;

                        if (NPC.Distance(GoTo + SavePlayerPosition) > 100f)
                        {
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        else
                        {
                            NPC.velocity *= 0.85f;
                        }
                    }

                    //play warning sound to telegraph the attack
                    if (NPC.localAI[0] == 30)
                    {
                        SoundEngine.PlaySound(HissSound2, NPC.Center);
                    }

                    if (NPC.localAI[0] == 85)
                    {
                        NPC.velocity *= 0.5f;
                    }

                    if (NPC.localAI[0] == 100)
                    {
                        OpenMouth = true;

                        SoundEngine.PlaySound(HissSound1, NPC.Center);

                        NPC.velocity.X = 0;
                        NPC.velocity.Y = -32;
                    }

                    if (NPC.localAI[0] > 100 && NPC.Center.Y < player.Center.Y)
                    {
                        NPC.velocity *= 0.85f;
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

                    if (NPC.localAI[0] > 200)
                    {
                        OpenMouth = false;
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

                //slam down and spawn thorns
                case 5:
                {
                    NPC.localAI[0]++;
                    if (NPC.localAI[0] <= 120)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.Y -= 400;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 16);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

					if (NPC.localAI[0] == 120)
                    {
						OpenMouth = true;
                        DefaultRotation = false;
						NPC.velocity = Vector2.Zero;

                        SaveNPCPosition = NPC.Center;
                    }

                    if (NPC.localAI[0] == 130)
                    {
                        SavePlayerPosition = new Vector2(player.Center.X, NPC.Center.Y + 500);
                    }

                    if (NPC.localAI[0] > 120 && NPC.localAI[0] <= 138)
                    {
                        NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
						NPC.Center += Main.rand.NextVector2Square(-10, 10);
                    }

					if (NPC.localAI[0] >= 140)
                    {
						if (NPC.localAI[1] == 0)
						{
							NPC.behindTiles = true;
							DefaultRotation = true;

                            if (NPC.localAI[0] == 140)
                            {
                                Vector2 ChargeSpeed = SavePlayerPosition - NPC.Center;
                                ChargeSpeed.Normalize();
                                ChargeSpeed *= 50;
                                NPC.velocity = ChargeSpeed;
                            }

                            if (!NPCGlobalHelper.IsColliding(NPC))
                            {
                                NPC.localAI[2]++;
                            }

							if (NPCGlobalHelper.IsColliding(NPC) && NPC.localAI[2] >= 2)
							{
								OpenMouth = false;

                                SaveRotation = NPC.rotation + (NPC.velocity.X > 0 ? -1 : 1);

								NPC.velocity = Vector2.Zero;

								Screenshake.ShakeScreenWithIntensity(NPC.Center, 10f, 500f);

								SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

								for (int j = 1; j <= 10; j += 2)
								{
									for (int i = -1; i <= 1; i += 2)
									{
										Vector2 center = new Vector2(NPC.Center.X, NPC.Center.Y - 100);
                                        center.X += j * 45 * i; //45 is the distance between each one
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

                                        Vector2 lineDirection = new Vector2(-(j * i) * 2, 16);

                                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(center.X, center.Y + 30), Vector2.Zero, ModContent.ProjectileType<FleshPillar>(), NPC.damage, 4.5f, 
                                        ai0: lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
									}
								}

								NPC.localAI[1] = 1;
							}
						}
						else
						{ 
							DefaultRotation = false;
						}
					}

                    if (NPC.localAI[0] > 320)
                    {
						NPC.behindTiles = false;
						DefaultRotation = true;

						SaveRotation = 0;

                        NPC.velocity *= 0.5f;
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
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}