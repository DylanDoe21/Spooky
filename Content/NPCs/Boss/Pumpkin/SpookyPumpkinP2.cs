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
using Spooky.Content.Items.BossBags.Pets;
using Spooky.Content.Items.SpookyBiome.Boss;
using Spooky.Content.NPCs.Boss.Pumpkin.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Boss.Pumpkin
{
    [AutoloadBossHead]
    public class SpookyPumpkinP2 : ModNPC
    {
        public bool Transition = true;
        public bool Enraged = false;
        public bool Left = false;

        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public static readonly SoundStyle FlyBuzz = new("Spooky/Content/Sounds/FlyBuzzing", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rot Gourd");
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Pumpkin/SpookyPumpkinBestiary",
                Position = new Vector2(20f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 0f
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
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 2400 / 3 : Main.expertMode ? 1850 / 2 : 1420;
            NPC.damage = 30;
            NPC.defense = 5;
            NPC.width = 200;
            NPC.height = 110;
            NPC.npcSlots = 15f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 2, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/PumpkinBoss");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("This giant pumpkin was once a guardian of the spooky forest. However, after the being controlling it left, it became a rotten husk of it's former self.")
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.velocity != Vector2.Zero) 
			{
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
                Vector2 drawOrigin = new(tex.Width / 2, (NPC.height / 2 + 4));

				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Brown) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(tex, drawPos, NPC.frame, color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}

            return true;
		}

        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity != Vector2.Zero)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 3)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
            }
            else
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 7)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
            }

            if (NPC.frame.Y == frameHeight * 3)
            {
                SoundEngine.PlaySound(SoundID.Item32, NPC.Center);
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 50 / 3 : Main.expertMode ? 40 / 2 : 25;

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.04f;

            //fade away and despawn when all players die or if you leave the biome
            if (Main.player[NPC.target].dead || !player.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyBiome>()))
            {
                NPC.alpha += 2;
                
                if (NPC.alpha >= 255)
                {
                    NPC.active = false;
                }
            }
            else
            {
                if (NPC.alpha >= 0)
                {
                    NPC.alpha -= 2;
                }
            }

            //spawn swarm of flies when spawned
            if (NPC.ai[2] <= 0)
            {
                for (int numFlies = 0; numFlies < 20; numFlies++)
                {
                    Vector2 vector = Vector2.UnitY.RotatedByRandom(1.57079637050629f) * new Vector2(5f, 3f);
                        
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, vector.X, vector.Y, 
                        ModContent.ProjectileType<Fly>(), 0, 0.0f, Main.myPlayer, 0.0f, (float)NPC.whoAmI);
                    }
                }

                NPC.netUpdate = true;

                NPC.ai[2] = 1;
            }

            //transition end from phase 1
            if (Transition)
            {
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;

                NPC.ai[3]++;

                if (NPC.ai[3] == 180)
                {
                    NPC.ai[3] = 0;

                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.immortal = false;
                    NPC.dontTakeDamage = false;

                    Transition = false;
                }
            }

            if (NPC.life <= NPC.lifeMax / 2)
            {
                Enraged = true;
            }

            //dont run attack ai during transition
            if (!Transition)
            {
                switch ((int)NPC.ai[0])
                {
                    //fly at the player for a bit
                    case 0:
                    {
                        NPC.localAI[0]++;

                        int MaxSpeed = Enraged ? 60 : 45;

                        //flies to players X position
                        if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed) 
                        {
                            MoveSpeedX--;
                        }
                        else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed)
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

                        if (NPC.localAI[0] >= 300)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                        }

                        break;
                    }

                    //go to the side and charge 3 times
                    case 1:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] < 3)
                        {
                            //Go to the side of the player to prepare for dash
                            if (NPC.localAI[0] >= 0 && NPC.localAI[0] < 80) 
                            {	
                                Vector2 GoTo = player.Center;
                                GoTo.X += (NPC.Center.X < player.Center.X) ? -335 : 335;
                                GoTo.Y -= 20;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }

                            //slow down right before charging
                            if (NPC.localAI[0] == 85)
                            {
                                NPC.velocity *= 0;
                            }

                            //actual dash attack
                            if (NPC.localAI[0] == 90)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                                int ChargeSpeed = Enraged ? 25 : 20;
                                
                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= ChargeSpeed;
                                ChargeDirection.Y *= 0;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] >= 110)
                            {
                                NPC.velocity *= 0.98f;
                            }

                            //loop charge attack
                            if (NPC.localAI[0] == 120)
                            {
                                NPC.localAI[1]++;
                                NPC.localAI[0] = 0;
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

                    //fall, stay still, and randomly summon flies
                    case 2:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] > 0 && NPC.localAI[0] < 180)
                        {   
                            NPC.velocity.X *= 0.95f;

                            NPC.noGravity = false;
                            NPC.noTileCollide = false;

                            if (Main.rand.Next(12) == 0)
                            {
                                SoundEngine.PlaySound(SoundID.NPCHit45, NPC.Center);

                                if (NPC.CountNPCS(ModContent.NPCType<BigFly>()) < 10)
                                {
                                    int Fly = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-60, 60), 
                                    (int)NPC.Center.Y + Main.rand.Next(-60, 60), ModContent.NPCType<BigFly>());

                                    if (Main.netMode == NetmodeID.Server)
                                    {
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, Fly);
                                    }
                                }
                            }
                        }

                        if (NPC.localAI[0] >= 270)
                        {
                            NPC.noGravity = true;
                            NPC.noTileCollide = true;
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //fly around the player, fire bolt spread, repeat 5 times
                    case 3:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[1] <= 5)
                        {
                            //go to random position above player
                            if (NPC.localAI[0] > 1 && NPC.localAI[0] < 80)
                            {
                                Vector2 GoTo = player.Center;
                                GoTo.X += Main.rand.Next(-300, 300);
                                GoTo.Y -= Main.rand.Next(300, 400);

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 30);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }
                            
                            //use bolt spread the first 4 times
                            if (NPC.localAI[1] <= 4)
                            {
                                if (NPC.localAI[0] == 70)
                                {
                                    if (player.Center.X < NPC.Center.X)
                                    {
                                        Left = true;
                                    }
                                    else
                                    {
                                        Left = false;
                                    }
                                }

                                if (NPC.localAI[0] >= 80 && NPC.localAI[0] <= 105 && NPC.localAI[2] <= 6)
                                {
                                    NPC.localAI[2]++;

                                    NPC.velocity *= 0.5f;

                                    SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

                                    float storeRot = (float)Math.Atan2(NPC.Center.Y - player.Center.Y, NPC.Center.X - player.Center.X);

                                    Vector2 projSpeed = new((float)((Math.Cos(storeRot) * 10) * -1), (float)((Math.Sin(storeRot) * 10) * -1));
                                    float rotation = MathHelper.ToRadians(5);
                                    float amount = Left ? NPC.localAI[2] - 7.2f / 2 : -(NPC.localAI[2] - 8.8f / 2);
                                    Vector2 perturbedSpeed = new Vector2(projSpeed.X, projSpeed.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, amount));

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X, 
                                        perturbedSpeed.Y, ModContent.ProjectileType<PumpkinSeed2>(), Damage, 0f, Main.myPlayer);
                                    }
                                }

                                if (NPC.localAI[0] > 180)
                                {
                                    NPC.localAI[1]++;
                                    NPC.localAI[2] = 0;
                                    NPC.localAI[0] = 20;
                                    NPC.netUpdate = true;
                                }
                            }
                            //summon flies on the 5th time
                            else 
                            {
                                if (NPC.localAI[0] == 80)
                                {
                                    SoundEngine.PlaySound(FlyBuzz, NPC.Center);
                                }

                                if (NPC.localAI[0] >= 80 && NPC.localAI[0] <= 140)
                                {
                                    NPC.velocity *= 0.5f;

                                    if (Main.rand.Next(5) == 0)
                                    {
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-60, 60), NPC.Center.Y + Main.rand.Next(-60, 60), 
                                            Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<FlyProj>(), Damage, 1, Main.myPlayer, 0, 0);	
                                        }
                                    }
                                }

                                if (NPC.localAI[0] >= 260)
                                {
                                    NPC.localAI[1]++;
                                    NPC.netUpdate = true;
                                }
                            }
                        }
                        else
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //fly above the player, shoot root thorns at the player
                    case 4:
                    {
                        NPC.localAI[0]++;

                        //fly the corner of the player
                        if (NPC.localAI[0] >= 20 && NPC.localAI[0] < 70)
                        {
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -500 : 500;
                            GoTo.Y -= 320;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 25);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        //stop before charge to prevent weird slowness issue
                        if (NPC.localAI[0] == 70)
                        {
                            NPC.velocity *= 0;
                        }

                        //charge
                        if (NPC.localAI[0] == 80)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);
                            
                            Vector2 ChargeDirection = player.Center - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            ChargeDirection.X *= 25;
                            ChargeDirection.Y *= 0;  
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        //shoot spreads thorns at the player
                        if (NPC.localAI[0] == 80 || NPC.localAI[0] == 100 || NPC.localAI[0] == 120) 
                        {
                            SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
                            
                            for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                            {
                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= 6f;
                                ShootSpeed.Y *= 6f;
                                
                                float Spread = Main.rand.Next(-2, 2);

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X + Spread, 
                                ShootSpeed.Y + Spread, ModContent.ProjectileType<RootThorn2>(), Damage, 1, NPC.target, 0, 0);
                            }
                        }

                        if (NPC.localAI[0] >= 120)
                        {
                            NPC.noGravity = false;
                            NPC.noTileCollide = false;
                            NPC.velocity.X *= 0.92f;
                        }

                        if (NPC.localAI[0] >= 320)
                        {
                            NPC.noGravity = true;
                            NPC.noTileCollide = true;
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //fly above player, smash down, repeat 3 times
                    case 5:
                    {
                        NPC.localAI[0]++;

                        //repeat 3 times
                        if (NPC.localAI[1] < 3)
                        {
                            //fly above the player for a second
                            if (NPC.localAI[0] >= 0 && NPC.localAI[0] < 60)
                            {
                                NPC.noTileCollide = true;

                                NPC.direction = Math.Sign(player.Center.X - NPC.Center.X);	
                                Vector2 GoTo = player.Center;
                                NPC.spriteDirection = NPC.direction;
                                GoTo.X += 0f;
                                GoTo.Y -= 450;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 25);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }

                            if (NPC.localAI[0] == 60)
                            {
                                NPC.velocity.X *= 0;
                            }

                            //attempt to crush the player
                            if (NPC.localAI[0] == 70)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                                NPC.noGravity = true;

                                int Speed = Enraged ? 30 : 25;

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= 0;
                                ChargeDirection.Y *= Speed;  
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] > 70)
                            {
                                if (NPC.position.Y >= player.Center.Y - 150)
                                {
                                    NPC.noTileCollide = false;
                                }
                            }

                            //make pumpkin collide with tiles after flying
                            if (NPC.localAI[0] > 70 && NPC.velocity.Y <= 0.1f && NPC.localAI[2] == 0)
                            {
                                NPC.noGravity = false;

                                NPC.velocity.X *= 0;
                                
                                if (player.velocity.Y == 0)
                                {
                                    player.velocity.Y -= 10f;
                                }

                                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);
                                
                                //make cool dust effect when slamming the ground
                                for (int i = 0; i < 65; i++)
                                {                                                                                  
                                    int slamDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 5, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
                                    Main.dust[slamDust].noGravity = true;
                                    Main.dust[slamDust].position.X -= Main.rand.Next(-50, 51) * .05f - 1.5f;
                                    Main.dust[slamDust].position.Y += 104;
                                    Main.dust[slamDust].scale = 3f;
                                    
                                    if (Main.dust[slamDust].position != NPC.Center)
                                    {
                                        Main.dust[slamDust].velocity = NPC.DirectionTo(Main.dust[slamDust].position) * 2f;
                                    }
                                }

                                SoundEngine.PlaySound(SoundID.Item69, NPC.Center);

                                //shoot upward spreads of root spikes
                                int NumProjectiles = Main.rand.Next(10, 15);
                                for (int i = 0; i < NumProjectiles; ++i)
                                {
                                    float Spread = (float)Main.rand.Next(-1500, 1500) * 0.01f;

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + 20, 0 + Spread, Main.rand.Next(-18, -13), 
                                        ModContent.ProjectileType<RootThorn>(), Damage, 1, Main.myPlayer, 0, 0);
                                    }
                                }

                                //on the final slam, make root spikes all over the place
                                if (NPC.localAI[1] >= 2)
                                {
                                    for (int j = 1; j <= 10; j++)
                                    {
                                        for (int i = -1; i <= 1; i += 2) 
                                        {
                                            Vector2 center = new(NPC.Center.X, NPC.Center.Y + NPC.height / 4);
                                            center.X += j * Main.rand.Next(150, 250) * i; //Main.rand.Next(150, 250) is the distance between each one
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
                                                Projectile.NewProjectile(NPC.GetSource_FromThis(), center.X, center.Y, 0, 0, 
                                                ModContent.ProjectileType<RootTelegraph>(), Damage, 1, Main.myPlayer, 0, 0);
                                            }
                                        }
                                    }
                                }

                                //"complete" the slam attack
                                NPC.localAI[2] = 1;
                            }

                            //loop the slam attack
                            if (NPC.localAI[0] > 130 && NPC.localAI[2] > 0)
                            {
                                NPC.localAI[1]++;
                                NPC.localAI[0] = 0;
                                NPC.localAI[2] = 0;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            //loop ai here
                            if (NPC.localAI[0] > 200)
                            {
                                NPC.noGravity = true;
                                NPC.noTileCollide = true;
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.localAI[2] = 0;
                                NPC.ai[0] = 0;
                                NPC.netUpdate = true;
                            }
                        }

                        break;
                    }
                }
            }
		}

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PumpkinGore" + numGores).Type);
                }
            }

            for (int numDust = 0; numDust < 50; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), 
                NPC.width / 2, NPC.height / 2, 288, 0f, 0f, 100, default, 2f);

                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].scale *= Main.rand.NextFloat(1f, 2.5f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            return true;
        }

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagPumpkin>()));
            
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<RottenGourd>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<SpookyPumpkinRelicItem>()));

            //normal drops
            int[] MainItem = new int[] { ModContent.ItemType<PumpkinAxe>(), ModContent.ItemType<PumpkinSpear>(), 
            ModContent.ItemType<PumpkinSlingshot>(), ModContent.ItemType<PumpkinShuriken>(), ModContent.ItemType<PumpkinStaff>(), 
			ModContent.ItemType<PumpkinTome>(), ModContent.ItemType<FlyScroll>(), ModContent.ItemType<PumpkinWhip>() };

            notExpertRule.OnSuccess(ItemDropRule.Common(Main.rand.Next(MainItem)));

            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<RottenChunk>(), 1, 12, 25));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedRotGourd, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.LesserHealingPotion;
		}
    }
}