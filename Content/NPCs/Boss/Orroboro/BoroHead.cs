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
using Spooky.Content.Tiles.SpookyHell;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Orroboro
{
    [AutoloadBossHead]
    public class BoroHead : ModNPC
    {
        private bool segmentsSpawned;
        public bool DefaultRotation = true;

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

			if (NPC.localAI[3] > 0)
			{
				for (int i = 0; i < 360; i += 30)
				{
                    Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

					Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
					spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, NPC.GetAlpha(color * 0.2f), NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.2f, SpriteEffects.None, 0);
				}
			}

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

            NPC.localAI[3] = !NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) ? 1 : 0;
            bool Enraged = NPC.localAI[3] > 0;

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

                    int repeats = Enraged ? 4 : 3;
                    if (NPC.localAI[1] < repeats)
                    {
                        int chargeTime = Enraged ? 65 : 75;

                        if (NPC.localAI[0] == 2)
                        {
                            SavePlayerPosition = new Vector2(NPC.Center.X < player.Center.X ? -600 : 600, Main.rand.Next(-200, 200));
                            NPC.netUpdate = true;
                        }
                        
                        if (NPC.localAI[0] > 2 && NPC.localAI[0] < chargeTime - 20)
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

                        if (NPC.localAI[0] == chargeTime - 20)
                        {
                            NPC.velocity *= 0.5f;
                            SavePlayerPosition = player.Center;
                            SaveNPCPosition = NPC.Center;
                        }

                        if (NPC.localAI[0] > chargeTime - 20 && NPC.localAI[0] < chargeTime)
                        {
                            DefaultRotation = false;

                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-10, 10);
                        }

                        if (NPC.localAI[0] == chargeTime)
                        {
                            DefaultRotation = true;

                            SoundEngine.PlaySound(HissSound1, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();

                            ChargeDirection *= Enraged ? 45 : 40;
                            NPC.velocity = ChargeDirection;
                        }

                        int stopTime = Enraged ? 80 : 90;
                        if (NPC.localAI[0] > stopTime)
                        {
                            NPC.velocity *= 0.95f;
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
                        NPC.velocity *= 0.5f;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        //NPC.ai[0]++;
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
                        int time1 = Enraged ? 45 : 45;
                        int time2 = Enraged ? 80 : 90;
                        int time3 = Enraged ? 115 : 130;
                        
                        if (NPC.localAI[0] == time1 || NPC.localAI[0] == time2 || NPC.localAI[0] == time3)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= (Enraged ? 33 : 25) + Main.rand.Next(-5, 5);
                            ChargeDirection.Y *= (Enraged ? 33 : 25) + Main.rand.Next(-5, 5);
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;

                            NPC.netUpdate = true;
                        }
                        else
                        {
                            NPC.velocity *= 0.95f;
                        }

                        if (NPC.localAI[0] > time3)
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
                        SavePlayerPosition = new Vector2(NPC.Center.X < player.Center.X ? -600 : 600, Main.rand.Next(-200, 200));
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
                        ChargeDirection *= Enraged ? 48 : 42;
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
                            ShootSpeed.X *= Enraged ? 5f : 3f;
                            ShootSpeed.Y *= Enraged ? 5f : 3f;

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

                    int repeats = Enraged ? 2 : 3;
                    if (NPC.localAI[1] < repeats)
                    {
                        if (NPC.localAI[0] > 30)
                        {
                            if (NPC.alpha < 255)
                            {
                                NPC.alpha += 5;
                            }
                        }

                        Vector2 GoTo = player.Center;
                        GoTo.Y += 550;

                        if (NPC.Distance(GoTo) > 50f)
                        {
                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 20, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }
                        else
                        {
                            NPC.velocity *= 0.95f;
                        }

                        if (NPC.localAI[1] > 0 && ((!Enraged && NPC.localAI[0] % 60 == 20) || (Enraged && NPC.localAI[0] % 20 == 5)))
                        {
                            Vector2 center = new(NPC.Center.X, player.Center.Y);
                            center.X += Main.rand.Next(-500, 501);
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
                        NPC.alpha = 0;

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
                        int time1 = Enraged ? 60 : 80;
                        int time2 = Enraged ? 120 : 160;
                        int time3 = Enraged ? 180 : 240;
                        
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
                            ChargeDirection *= Enraged ? 25 : 22;
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
                        NPC.velocity *= 0;

                        NPC.position.X = player.Center.X - (NPC.width / 2) + 1000;
                        NPC.position.Y = player.Center.Y - (NPC.height / 2) - 750;
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(HissSound1, NPC.position);

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
                                SoundEngine.PlaySound(SpitSound, NPC.Center);

                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= Enraged ? 3f : 4f;
                                ShootSpeed.Y *= Enraged ? 3f : 4f;

                                int ProjectileType = Enraged ? ModContent.ProjectileType<BoroBiomatter>() : ModContent.ProjectileType<EyeSpit>();

                                NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ProjectileType, NPC.damage, 4.5f);
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