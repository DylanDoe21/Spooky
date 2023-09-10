using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
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
using Spooky.Content.NPCs.Boss.Moco.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Moco
{
    [AutoloadBossHead]
    public class Moco : ModNPC
    {
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;

        public bool Phase2 = false;
        public bool Transition = false;
        public bool Sneezing = false;
        public bool SwitchedSides = false;
        public bool AfterImages = false;

        Vector2 SaveNPCPosition;

        public static readonly SoundStyle SneezeSound1 = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound);
        public static readonly SoundStyle SneezeSound2 = new("Spooky/Content/Sounds/Moco/MocoSneeze2", SoundType.Sound);
        public static readonly SoundStyle SneezeSound3 = new("Spooky/Content/Sounds/Moco/MocoSneeze3", SoundType.Sound);
        public static readonly SoundStyle AngrySound = new("Spooky/Content/Sounds/Moco/MocoAngry", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.TrailCacheLength[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused, 
                    BuffID.Poisoned, 
                    BuffID.OnFire
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);

            //bools
            writer.Write(Phase2);
            writer.Write(Transition);
            writer.Write(Sneezing);
            writer.Write(SwitchedSides);
            writer.Write(AfterImages);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();

            //bools
            Phase2 = reader.ReadBoolean();
            Transition = reader.ReadBoolean();
            Sneezing = reader.ReadBoolean();
            SwitchedSides = reader.ReadBoolean();
            AfterImages = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 4200;
            NPC.damage = 40;
            NPC.defense = 12;
            NPC.width = 130;
            NPC.height = 112;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit22;
			NPC.DeathSound = SoundID.NPCDeath60;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Moco");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Moco"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (AfterImages) 
			{
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
				Vector2 drawOrigin = new(tex.Width * 0.5f, (NPC.height * 0.5f));

				for (int oldPos = 0; oldPos < NPC.oldPos.Length; oldPos++)
				{
					var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
					Vector2 drawPos = NPC.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, NPC.gfxOffY + 4);
					Color color = NPC.GetAlpha(Color.Purple) * (float)(((float)(NPC.oldPos.Length - oldPos) / (float)NPC.oldPos.Length) / 2);
					spriteBatch.Draw(tex, drawPos, new Microsoft.Xna.Framework.Rectangle?(NPC.frame), color, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
				}
			}
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //angry phase transition
            if (NPC.ai[0] == -1 && (NPC.localAI[0] > 120 && NPC.localAI[0] < 240))
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
                Texture2D angerTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoAngry").Value;
                Vector2 drawOrigin1 = new(tex.Width * 0.5f, (NPC.height * 0.5f));
                Vector2 drawOrigin2 = new(angerTex.Width * 0.5f, (NPC.height * 0.5f));

                //draw moco but red
                spriteBatch.Draw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame,
				Color.Red, NPC.rotation, drawOrigin1, NPC.scale, SpriteEffects.None, 0.99f);

                //draw angry symbol thingie
                spriteBatch.Draw(angerTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame,
				Color.Red, NPC.rotation, drawOrigin2, NPC.scale, SpriteEffects.None, 0);
            }

            //eye glow textures
            Texture2D glowTex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Moco/MocoGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(glowTex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void FindFrame(int frameHeight)
        {
            //side facing animations
            if (NPC.ai[0] <= 2 && NPC.ai[0] >= 0)
            {
                //normal animation
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 3)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = frameHeight * 0;
                }

                //sneezing frame
                if (Sneezing)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
            }

            //front facing animations
            if (NPC.ai[0] >= 3 || NPC.ai[0] == -1)
            {
                //normal animation
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 3)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 9)
                {
                    NPC.frame.Y = frameHeight * 5;
                }

                //sneezing frame
                if (Sneezing)
                {
                    NPC.frame.Y = frameHeight * 9;
                }
            }
        }

        public override void AI()
        {   
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 50 / 3 : Main.expertMode ? 40 / 2 : 30;

            NPC.spriteDirection = NPC.direction;

            //despawn if all players are dead
            if (player.dead)
            {
                AfterImages = true;
                NPC.velocity.Y = -25;

                NPC.localAI[2]++;

                if (NPC.localAI[2] >= 120)
                {
                    NPC.active = false;
                }
            }

            //set to transition
            if (NPC.life < (NPC.lifeMax / 2) && !Phase2 && NPC.ai[0] != -1)
            {
                NPC.ai[0] = -1;
                NPC.localAI[0] = 0;
            }

            if (!player.dead)
            {
                switch ((int)NPC.ai[0])
                {
                    case -1:
                    {
                        NPC.velocity *= 0;
                        NPC.rotation = 0;

                        AfterImages = false;
                        Sneezing = false;
                        Transition = true;
                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;
                        NPC.noGravity = true;

                        NPC.localAI[0]++;

                        if (NPC.localAI[0] == 60)
                        {
                            SaveNPCPosition = NPC.Center;
                        }

                        if (NPC.localAI[0] == 120)
                        {
                            SoundEngine.PlaySound(AngrySound, NPC.Center);
                        }

                        if (NPC.localAI[0] > 120 && NPC.localAI[0] < 240)
                        {
                            Sneezing = true;

                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-8, 8);

                            int Steam1 = Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X + 5, NPC.Center.Y + 35), default, Main.rand.Next(61, 64), 1f);
                            Main.gore[Steam1].velocity.X *= 2f;
                            Main.gore[Steam1].velocity.Y *= -2f;
                            Main.gore[Steam1].alpha = 125;

                            int Steam2 = Gore.NewGore(NPC.GetSource_FromThis(), new Vector2(NPC.Center.X - 50, NPC.Center.Y + 35), default, Main.rand.Next(61, 64), 1f);
                            Main.gore[Steam2].velocity.X *= -2f;
                            Main.gore[Steam2].velocity.Y *= -2f;
                            Main.gore[Steam2].alpha = 125;
                        }

                        if (NPC.localAI[0] == 250)
                        {
                            Sneezing = false;
                        }

                        if (NPC.localAI[0] >= 280)
                        {
                            Phase2 = true;
                            Transition = false;
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;

                            NPC.ai[0] = 0;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;

                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //fly at player
                    case 0:
                    {
                        NPC.localAI[0]++;

                        NPC.rotation = NPC.velocity.X * 0.04f;

                        //flies to players X position
                        if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -70) 
                        {
                            MoveSpeedX -= 2;
                        }
                        else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 70)
                        {
                            MoveSpeedX += 2;
                        }

                        NPC.velocity.X = MoveSpeedX * 0.1f;
                        
                        //flies to players Y position
                        if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -40)
                        {
                            MoveSpeedY--;
                        }
                        else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= 40)
                        {
                            MoveSpeedY++;
                        }

                        NPC.velocity.Y = MoveSpeedY * 0.1f;

                        if (NPC.localAI[0] >= 300)
                        {
                            MoveSpeedX = 0;
                            MoveSpeedY = 0;

                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //zip to the players side, then charge towards them really fast
                    case 1:
                    {
                        NPC.localAI[0]++;

                        NPC.rotation = NPC.velocity.X * 0.04f;

                        int numCharges = Phase2 ? 2 : 1;

                        if (NPC.localAI[1] < numCharges)
                        {
                            if (NPC.localAI[0] < 40) 
                            {	
                                Vector2 GoTo = player.Center;
                                GoTo.X += (NPC.Center.X < player.Center.X) ? -420 : 420;
                                GoTo.Y -= 20;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }

                            if (NPC.localAI[0] == 40)
                            {
                                NPC.velocity *= 0f;
                            }

                            if (NPC.localAI[0] == 45)
                            {
                                AfterImages = true;

                                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                                int ChargeSpeed = Phase2 ? 23 : 22;

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= ChargeSpeed;
                                ChargeDirection.Y *= ChargeSpeed / 1.5f;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] >= 75)
                            {
                                AfterImages = false;
                                NPC.localAI[0] = 20;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            AfterImages = false;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //shoot giant snot glob
                    case 2:
                    {
                        NPC.localAI[0]++;

                        NPC.velocity *= 0.9f;

                        Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                        float RotateX = player.Center.X - vector.X;
                        float RotateY = player.Center.Y - vector.Y;
                        NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

                        if (NPC.localAI[0] == 60)
                        {
                            SaveNPCPosition = NPC.Center;
                        }

                        if (NPC.localAI[0] > 60 && NPC.localAI[0] < 100) 
                        {
                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-5, 5);
                        }

                        if (NPC.localAI[0] == 100)
                        {
                            Sneezing = true;

                            SoundEngine.PlaySound(SneezeSound1, NPC.Center);

                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize(); 

                            Recoil *= -8;
                            NPC.velocity = Recoil;

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 15f;

                            int snotBall = Phase2 ? ModContent.ProjectileType<GiantSnot2>() : ModContent.ProjectileType<GiantSnot>();
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, snotBall, Damage, 1, NPC.target, 0, 0);
                        }

                        if (NPC.localAI[0] >= 120)
                        {
                            Sneezing = false;
                        }

                        if (NPC.localAI[0] >= 170)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //zip above the player and shoot lingering snot globs down
                    case 3:
                    {
                        NPC.localAI[0]++;

                        NPC.rotation = NPC.velocity.X * 0.04f;

                        if (NPC.localAI[0] < 65) 
                        {	
                            Vector2 GoTo = player.Center;
                            GoTo.X += 0;
                            GoTo.Y -= 350;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 65)
                        {
                            SaveNPCPosition = NPC.Center;

                            NPC.velocity *= 0f;
                        }

                        if (NPC.localAI[0] > 65 && NPC.localAI[0] < 100) 
                        {
                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-5, 5);
                        }

                        if (NPC.localAI[0] == 100 || NPC.localAI[0] == 120 || NPC.localAI[0] == 140) 
                        {
                            Sneezing = true;
                            NPC.noGravity = false;

                            SoundEngine.PlaySound(SneezeSound2, NPC.Center);

                            int MaxProjectiles = Phase2 ? 15 : 8;

                            for (int numProjectiles = 0; numProjectiles < MaxProjectiles; numProjectiles++)
                            {
                                //recoil upward
                                NPC.velocity.X = 0;
                                NPC.velocity.Y = -6;
                                
                                float Spread = Main.rand.Next(-15, 15);
                                
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + 35, 0 + Spread, 
                                Main.rand.Next(2, 4), ModContent.ProjectileType<SnotBall2>(), Damage, 1, NPC.target, 0, 0);
                            }
                        }

                        if (NPC.localAI[0] >= 160)
                        {
                            Sneezing = false;
                            NPC.noGravity = true;
                        }

                        if (NPC.localAI[0] >= 220)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //go above player and charge down, repeat 3 times in phase 2
                    case 4:
                    {
                        NPC.localAI[0]++;

                        int numCharges = Phase2 ? 3 : 1;

                        if (NPC.localAI[1] < numCharges)
                        {
                            if (NPC.localAI[0] < 45) 
                            {	
                                Vector2 GoTo = player.Center;
                                GoTo.X += 0;
                                GoTo.Y -= 420;

                                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                            }

                            if (NPC.localAI[0] == 45)
                            {
                                NPC.velocity *= 0f;
                            }

                            if (NPC.localAI[0] == 55)
                            {
                                AfterImages = true;

                                SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);

                                int ChargeSpeed = Phase2 ? 30 : 25;

                                Vector2 ChargeDirection = player.Center - NPC.Center;
                                ChargeDirection.Normalize();
                                        
                                ChargeDirection.X *= ChargeSpeed / 1.5f;
                                ChargeDirection.Y *= ChargeSpeed;
                                NPC.velocity.X = ChargeDirection.X;
                                NPC.velocity.Y = ChargeDirection.Y;
                            }

                            if (NPC.localAI[0] >= 55)
                            {
                                NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;
			                    NPC.rotation += 0f * (float)NPC.direction;
                            }
                            else
                            {
                                NPC.rotation = NPC.velocity.X * 0.04f;
                            }

                            if (NPC.localAI[0] >= 85)
                            {
                                AfterImages = false;
                                NPC.velocity *= 0.98f;
                                NPC.localAI[0] = 45;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        else
                        {
                            AfterImages = false;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                        
                        break;
                    }

                    //go close to the player and shoot a stream of snot balls
                    case 5:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] < 40) 
                        {	
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -420 : 420;
                            GoTo.Y -= 20;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] < 40)
                        {
                            NPC.rotation = NPC.velocity.X * 0.04f;
                        }
                        else
                        {
                            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
                            float RotateX = player.Center.X - vector.X;
                            float RotateY = player.Center.Y - vector.Y;
                            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
                        }

                        if (NPC.localAI[0] == 40) 
                        {
                            NPC.velocity *= 0f;
                        }

                        //use attack for longer in phase 2
                        int MaxTime = Phase2 ? 300 : 200;

                        if (NPC.localAI[0] == 60)
                        {
                            SaveNPCPosition = NPC.Center;
                        }

                        if (NPC.localAI[0] > 60 && NPC.localAI[0] < MaxTime)
                        {
                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-5, 5);
                        }

                        if (NPC.localAI[0] > 100 && NPC.localAI[0] < MaxTime) 
                        {
                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 12f;

                            float Spread = Main.rand.Next(-1, 1);

                            if (NPC.localAI[0] % 8 == 2)
                            {
                                Sneezing = true;

                                SoundEngine.PlaySound(SneezeSound1, NPC.Center);

                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X + Spread, 
                                ShootSpeed.Y + Spread, ModContent.ProjectileType<SnotBall>(), Damage, 1, NPC.target, 0, 0);
                            }
                        }

                        if (NPC.localAI[0] > MaxTime + 10)
                        {
                            Sneezing = false;
                        }

                        if (NPC.localAI[0] >= MaxTime + 60)
                        {
                            NPC.localAI[0] = 0;
                            
                            if (!Phase2)
                            {
                                NPC.ai[0] = 0;
                            }
                            else
                            {
                                NPC.ai[0]++;
                            }

                            NPC.netUpdate = true;
                        }

                        break;
                    }

                    //in phase 2, shoot a spread sideways and fly offscreen, then come back on the other side
                    case 6:
                    {
                        NPC.localAI[0]++;

                        if (NPC.localAI[0] < 60) 
                        {	
                            NPC.rotation = NPC.velocity.X * 0.04f;

                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -550 : 550;
                            GoTo.Y += 0;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 25, 50);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 60) 
                        {
                            SaveNPCPosition = NPC.Center;

                            NPC.velocity *= 0;
                        }

                        if (NPC.localAI[0] > 60 && NPC.localAI[0] < 90)
                        {
                            NPC.rotation = (NPC.Center.X < player.Center.X) ? 80 : -80;

                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-5, 5);
                        }

                        if (NPC.localAI[0] == 90)
                        {
                            AfterImages = true;

                            SoundEngine.PlaySound(SneezeSound3, NPC.Center);

                            NPC.velocity.X = (NPC.Center.X < player.Center.X) ? -25 : 25;
                            NPC.velocity.Y *= 0;

                            int ShootTowards = (NPC.Center.X < player.Center.X) ? 100 : -100;

                            for (int numProjectiles = -6; numProjectiles <= 6; numProjectiles++)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, 10f * NPC.DirectionTo(new Vector2(NPC.Center.X + ShootTowards, NPC.Center.Y)).RotatedBy(MathHelper.ToRadians(8) * numProjectiles),
                                ModContent.ProjectileType<SnotBall>(), Damage, 0f, Main.myPlayer);
                            }
                        }

                        if (NPC.localAI[0] >= 90)
                        {
                            Sneezing = true;

                            NPC.rotation = (NPC.velocity.X < 0) ? 80 : -80;

                            if (NPC.Distance(player.Center) >= 1400f && !SwitchedSides) 
                            {
                                NPC.position.X = (NPC.Center.X < player.Center.X) ? player.Center.X + 2000 : player.Center.X - 2000;
                                SwitchedSides = true;
                            }
                        }

                        if (NPC.localAI[0] >= 160)
                        {
                            NPC.velocity *= 0.98f;
                        }

                        if (NPC.localAI[0] >= 230)
                        {
                            SwitchedSides = false;
                            AfterImages = false;
                            Sneezing = false;
                            NPC.localAI[0] = 0;
                            NPC.ai[0] = 0;
                            NPC.netUpdate = true;
                        }

                        break;
                    }
                }
            }
		}

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            //treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagMoco>()));

            //master relic and pet
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<MocoRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MocoTissue>(), 4));

            //weapon drops
            int[] MainItem = new int[]
            { 
                ModContent.ItemType<BoogerFlail>(), 
                ModContent.ItemType<BoogerBlaster>(), 
                ModContent.ItemType<BoogerBook>(),
                ModContent.ItemType<BoogerStaff>()
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MocoMask>(), 7));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MocoTrophyItem>(), 10));

            npcLoot.Add(notExpertRule);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore" + numGores).Type);
                    }
                }
            }
        }

        public override void OnKill()
        {
            //drop a sentient heart for each active player in the world
            if (!Flags.downedMoco)
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
            }

            NPC.SetEventFlagCleared(ref Flags.downedMoco, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }
    }
}