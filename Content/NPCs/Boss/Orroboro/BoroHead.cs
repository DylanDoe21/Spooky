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
using Spooky.Content.Achievements;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.Boss.Orroboro.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    [AutoloadBossHead]
    public class BoroHead : ModNPC
    {
		int Tongue = 0;

        private bool segmentsSpawned;
        public bool DefaultRotation = true;

		public float SaveRotation = 0;

        int timer = 0;

        Vector2 SaveNPCPosition;
        Vector2 SavePlayerPosition;

		int[] BlockTypes = new int[]
		{
			ModContent.TileType<SpookyMush>(),
			ModContent.TileType<SpookyMushGrass>(),
			ModContent.TileType<LivingFlesh>()
		};

		private static Asset<Texture2D> NPCTexture;

        public static readonly SoundStyle LickSound = new("Spooky/Content/Sounds/Orroboro/BoroLick", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle HissSound1 = new("Spooky/Content/Sounds/Orroboro/HissShort", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle HissSound2 = new("Spooky/Content/Sounds/Orroboro/HissLong", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle SpitSound = new("Spooky/Content/Sounds/Orroboro/VenomSpit", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/BoroBestiary",
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
            writer.WriteVector2(SaveNPCPosition);
            writer.WriteVector2(SavePlayerPosition);

            //bools
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
            NPC.lifeMax = 15000;
            NPC.damage = 55;
            NPC.defense = 30;
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
            NPC.DeathSound = SoundID.Zombie38;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Orroboro");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}
        
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BoroHead"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 8)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = frameHeight * 0;
            }
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
        {
            NPC Orro = Main.npc[(int)NPC.ai[1]];

            //always sync the necessary ai timers if orro is active
            if (Orro.active && Orro.type == ModContent.NPCType<OrroHead>())
            {
                NPC.ai[0] = Orro.ai[0];
                NPC.localAI[0] = Orro.localAI[0];
            }

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

            //enraged behavior
            if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()))
            {
                if (NPC.ai[0] != 6)
                {
                    NPC.ai[0] = 6;
                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.localAI[2] = 0;
                }
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
                        int Type = Segment == 2 ? ModContent.NPCType<BoroBodyWings>() : ModContent.NPCType<BoroBody>();

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

            //attacks
            switch ((int)NPC.ai[0])
            {
                //charge from the sides while orro chases
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 3)
                    {
                        if (NPC.localAI[0] == 2)
                        {
                            SavePlayerPosition = new Vector2(NPC.Center.X < player.Center.X ? -600 : 600, Main.rand.Next(-200, 200));
                            NPC.netUpdate = true;
                        }
                        
                        if (NPC.localAI[0] > 2 && NPC.localAI[0] < 55)
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

                        if (NPC.localAI[0] == 55)
                        {
                            NPC.velocity *= 0.5f;
                            SavePlayerPosition = player.Center;
                            SaveNPCPosition = NPC.Center;
                        }

                        if (NPC.localAI[0] > 55 && NPC.localAI[0] < 75)
                        {
                            DefaultRotation = false;

                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-10, 10);
                        }

                        if (NPC.localAI[0] == 75)
                        {
                            DefaultRotation = true;

                            SoundEngine.PlaySound(HissSound1, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 40;
                            NPC.velocity = ChargeDirection;
                        }

                        if (NPC.localAI[0] > 90)
                        {
                            NPC.velocity *= 0.95f;
                        }

                        if (NPC.localAI[0] >= 135)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.velocity *= 0.5f;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }
                    
                    break;
                }

                //charge at the player 
                case 1:
                {
                    NPC.localAI[0]++;
                    
                    NPC.velocity *= 0.975f;

                    if (NPC.localAI[1] < 3)
                    {
                        if (NPC.localAI[0] == 45 || NPC.localAI[0] == 90 || NPC.localAI[0] == 130)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 25 + Main.rand.Next(-5, 5);
                            ChargeDirection.Y *= 25 + Main.rand.Next(-5, 5);
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;

                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity *= 0.95f;
                        }

                        if (NPC.localAI[0] > 130)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1]++;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        NPC.velocity *= 0.95f;

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

                //go to players side, charge and then curve and spit biomass in sync with orro
                case 2:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 2)
                    {
                        SavePlayerPosition = new Vector2(NPC.Center.X > player.Center.X ? -600 : 600, Main.rand.Next(-200, 200));
                        NPC.netUpdate = true;
                    }
                    
                    if (NPC.localAI[0] > 2 && NPC.localAI[0] < 80)
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

                    if (NPC.localAI[0] == 80)
                    {
                        NPC.velocity *= 0.5f;
                        SavePlayerPosition = player.Center;
                        SaveNPCPosition = NPC.Center;
                    }

                    if (NPC.localAI[0] > 80 && NPC.localAI[0] < 110)
                    {
                        DefaultRotation = false;

                        NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-10, 10);
                    }

                    if (NPC.localAI[0] == 110)
                    {
                        DefaultRotation = true;

                        SoundEngine.PlaySound(HissSound1, NPC.Center);

                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                        ChargeDirection *= 42;
                        NPC.velocity = ChargeDirection;
                    }

                    if (NPC.localAI[0] >= 125 && NPC.localAI[0] <= 195)
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

                        if (NPC.localAI[0] % 12 == 0)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 3f;

                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<BoroBiomatter>(), NPC.damage, 0f);
                        }
                    }

                    if (NPC.localAI[0] >= 195)
                    {
                        NPC.velocity *= 0.95f;
                    }

                    if (NPC.localAI[0] > 290)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //use tentacle pillars while orro spits acid and chases
                case 3:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[1] < 3)
                    {
						if (NPC.localAI[0] <= 50)
						{
							Vector2 GoTo = player.Center;
							GoTo.Y -= 400;

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 45);
							NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
						}

						if (NPC.localAI[0] == 50)
						{
							DefaultRotation = false;
							NPC.velocity = Vector2.Zero;

                            SaveNPCPosition = NPC.Center;
                        }

						if (NPC.localAI[0] == 60)
                        {
                            SavePlayerPosition = new Vector2(player.Center.X, NPC.Center.Y + 500);
                        }

						if (NPC.localAI[0] > 50 && NPC.localAI[0] <= 78)
						{
							NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
							NPC.Center += Main.rand.NextVector2Square(-10, 10);
						}

						if (NPC.localAI[0] >= 80)
						{
							if (NPC.localAI[2] == 0)
							{
								NPC.behindTiles = true;
								DefaultRotation = true;

								if (NPC.localAI[0] == 80)
                                {
                                    Vector2 ChargeSpeed = SavePlayerPosition - NPC.Center;
                                    ChargeSpeed.Normalize();
                                    ChargeSpeed *= 50;
                                    NPC.velocity = ChargeSpeed;
                                }

                                if (!NPCGlobalHelper.IsColliding(NPC))
                                {
                                    NPC.localAI[3]++;
                                }

                                if (NPCGlobalHelper.IsColliding(NPC) && NPC.localAI[3] >= 2)
                                {
                                    SaveRotation = NPC.rotation + (NPC.velocity.X > 0 ? -1 : 1);

                                    NPC.velocity = Vector2.Zero;

                                    Screenshake.ShakeScreenWithIntensity(NPC.Center, 10f, 500f);

                                    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

                                    for (int j = 1; j <= 5; j += 2)
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

                                    NPC.localAI[2] = 1;
                                }
							}
							else
							{
								DefaultRotation = false;
							}
						}

						if (NPC.localAI[0] >= 260)
						{
							NPC.behindTiles = false;
							DefaultRotation = true;

							SaveRotation = 0;

							NPC.localAI[0] = 0;
							NPC.localAI[2] = 0;
							NPC.localAI[3] = 0;
							NPC.localAI[1]++;
							NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        //go to the players side before doing the tongue attack so it doesnt unfairly hit the player from off screen
                        if (NPC.localAI[0] <= 60)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += NPC.Center.X < player.Center.X ? -475 : 475;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 17, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        else
                        {
                            NPC.velocity *= 0.5f;

                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0; 
							NPC.localAI[2] = 0;
							NPC.localAI[3] = 0;
                            NPC.ai[0]++; 
                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //charge and shoot out tongue while orro drops venom spit
                case 4:
                {
                    NPC.localAI[0]++;
                    NPC.velocity *= 0.97f;

                    if (NPC.localAI[1] < 3)
                    {
                        int time1 = 80;
                        int time2 = 160;
                        int time3 = 240;
                        
                        if (NPC.localAI[0] == time1 - 10 || NPC.localAI[0] == time2 - 10 || NPC.localAI[0] == time3 - 10)
                        {
                            Vector2 CenterPoint = player.Center;
                        
                            SavePlayerPosition = CenterPoint;
                            SaveNPCPosition = NPC.Center;

                            NPC.netUpdate = true;
                        }

                        if ((NPC.localAI[0] > time1 - 10 && NPC.localAI[0] < time1 + 15) || (NPC.localAI[0] > time2 - 10 && NPC.localAI[0] < time2 + 15) || (NPC.localAI[0] > time3 - 10 && NPC.localAI[0] < time3 + 15))
                        {
                            DefaultRotation = false;

                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-10, 10);
                        }

                        if (NPC.localAI[0] == time1 + 15 || NPC.localAI[0] == time2 + 15 || NPC.localAI[0] == time3 + 15)
                        {
                            DefaultRotation = true;

                            SoundEngine.PlaySound(LickSound, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                            ChargeDirection *= 22;
                            NPC.velocity = ChargeDirection;

                            int Tongue = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + NPC.height / 2, ModContent.NPCType<BoroTongue>(), ai1: SavePlayerPosition.X, ai2: SavePlayerPosition.Y, ai3: NPC.whoAmI);
                    
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: Tongue);
                            }
                        }

                        if (NPC.localAI[0] >= time3 + 40)
                        {
							NPC.velocity *= 0.92f;

                            NPC.localAI[0] = 20;
                            NPC.localAI[1]++;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
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
                        GoTo.X += 1000;
                        GoTo.Y -= 750;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 42);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }
                    
                    //set exact position right before so it is even
                    if (NPC.localAI[0] == 119)
                    {
                        NPC.velocity = Vector2.Zero;

                        NPC.position.X = player.Center.X - (NPC.width / 2) + 1000;
                        NPC.position.Y = player.Center.Y - (NPC.height / 2) - 750;
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(HissSound1, NPC.position);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 40;
                        ChargeDirection.Y *= 0;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= 151 && NPC.localAI[0] <= 500)
                    {
                        if (NPC.localAI[0] >= 250 && NPC.localAI[0] <= 380)
                        {
                            int frequency = 8;

                            if (NPC.localAI[0] % 20 == frequency)
                            {
                                SoundEngine.PlaySound(SpitSound, NPC.Center);

                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= 4f;

                                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<EyeSpit>(), NPC.damage, 4.5f);
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

                //enraged behavior
                case 6:
                {
                    NPC.localAI[0]++;

                    //chase movement
                    if (NPC.localAI[0] < 140)
                    {
                        Vector2 GoTo = player.Center;
                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 1f, 6f);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                        if (NPC.localAI[0] == 75 || NPC.localAI[0] == 85 || NPC.localAI[0] == 95)
                        {
                            SoundEngine.PlaySound(SpitSound, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed.X *= Main.rand.Next(4, 13);
                            ShootSpeed.Y *= Main.rand.Next(4, 13);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + NPC.velocity.X, NPC.Center.Y + NPC.velocity.Y),
                            ShootSpeed, ModContent.ProjectileType<BoroBiomatter>(), NPC.damage, 4.5f);
                        }
                    }

                    //charge a bunch of times
                    if (NPC.localAI[0] >= 140 && NPC.localAI[0] <= 260)
                    {
                        if (NPC.localAI[0] % 30 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 28 + Main.rand.Next(-4, 5);
                            ChargeDirection.Y *= 28 + Main.rand.Next(-4, 5);
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;

                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity *= 0.92f;
                        }
                    }

                    if (NPC.localAI[0] >= 280)
                    {
                        NPC.localAI[0] = 0;
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
            npcLoot.Add(ItemDropRule.BossBagByCondition(new DropConditions.ShouldBoroDropLootExpert(), ModContent.ItemType<BossBagOrroboro>()));
            
            //master relic and pet
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.ShouldBoroDropLootMaster(), ModContent.ItemType<OrroboroRelicItem>()));
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.ShouldBoroDropLootMaster(), ModContent.ItemType<OrroboroRing>(), 4));

            //weapon drops
            int[] MainItem = new int[] 
            {
                ModContent.ItemType<MouthFlamethrower>(),
                ModContent.ItemType<LeechStaff>(),
                ModContent.ItemType<LeechWhip>()
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //material
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.ShouldBoroDropLoot(), ModContent.ItemType<ArteryPiece>(), 1, 12, 25));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BoroMask>(), 7));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BoroTrophyItem>(), 10));

            npcLoot.Add(notExpertRule);
        }

        public override bool CheckDead()
        {
            for (int numGores = 1; numGores <= 2; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 2, ModContent.Find<ModGore>("Spooky/BoroHeadGore" + numGores).Type);
                }
            }

            return true;
        }

        public override void OnKill()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<OrroHead>()))
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

                ModContent.GetInstance<BossAchievementOrroboro>().EitherOrroBoroDead.Complete();
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<CranberryJuice>();
		}
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}