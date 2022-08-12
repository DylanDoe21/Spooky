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
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Boss.BigBone.Projectiles;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    //[AutoloadBossHead]
    public class BigBone : ModNPC
    {
        //save point for big bones thorn vine attack
        Vector2[] SavePoint = new Vector2[5];

        int[] AttackPattern = new int[] { 0, 1, 2, 3, 4, 5 };
        int[] SpecialAttack = new int[] { 6, 7 };

        public static readonly SoundStyle GrowlSound1 = new("Spooky/Content/Sounds/BigBoneGrowl1", SoundType.Sound);
        public static readonly SoundStyle GrowlSound2 = new("Spooky/Content/Sounds/BigBoneGrowl2", SoundType.Sound);
        public static readonly SoundStyle LaughSound = new("Spooky/Content/Sounds/BigBoneLaugh", SoundType.Sound);
        public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBoneMagic", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/BigBoneDeath", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Bone");
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/BigBone/BigBoneBestiary",
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
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.masterMode ? 78000 / 3 : Main.expertMode ? 65000 / 2 : 50000;
            NPC.damage = 65;
            NPC.defense = 40;
            NPC.width = 134;
            NPC.height = 170;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 10, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = DeathSound;
            NPC.aiStyle = -1;
            Music = MusicID.Dungeon;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.DeepCatacombBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("The massive, long lost brother of little bone who absorbed many souls to grow in power and size. He was banished to the catacombs as a result of this power.")
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //only draw if the parent is active
            if (Main.npc[(int)NPC.ai[3]].active)
			{
                Vector2 rootPosition = Main.npc[(int)NPC.ai[3]].Center;

                Vector2[] bezierPoints = { rootPosition, rootPosition + new Vector2(0, -60), NPC.Center + new Vector2(-60 * NPC.direction, 0).RotatedBy(NPC.rotation), NPC.Center + new Vector2(-14 * NPC.direction, 0).RotatedBy(NPC.rotation) };
                float bezierProgress = 0;
                float bezierIncrement = 27;

                Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneStem").Value;
                Vector2 textureCenter = NPC.spriteDirection == -1 ? new Vector2(16, 16) : new Vector2(16, 16);

                float rotation;

                while (bezierProgress < 1)
                {
                    //draw stuff
                    Vector2 oldPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);

                    //increment progress
                    while ((oldPos - BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement)
                    {
                        bezierProgress += 0.1f / BezierCurveUtil.BezierCurveDerivative(bezierPoints, bezierProgress).Length();
                    }

                    Vector2 newPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);
                    rotation = (newPos - oldPos).ToRotation() + MathHelper.Pi;

                    spriteBatch.Draw(texture, (oldPos + newPos) / 2 - Main.screenPosition, texture.Frame(), drawColor, rotation, textureCenter, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow1").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Transparent, Color.White, fade);

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void AI()
        {   
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 125 / 3 : Main.expertMode ? 85 / 2 : 55;

            NPC.spriteDirection = NPC.direction;

            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            if (NPC.life > NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
            }

            //reset and randomize attack pattern array
            if (NPC.ai[2] == 0)
            {
                //remove special attacks from the attack array
                foreach (var i in AttackPattern)
                {
                    if (i == 6)
                    {
                        AttackPattern = AttackPattern.Where((source, index) => index != i).ToArray();
                    }
                }
                    
                //shuffle the attack pattern array
                AttackPattern = AttackPattern.OrderBy(x => Main.rand.Next()).ToArray();

                //add one of the special attacks to the end of the array
                AttackPattern = AttackPattern.Append(Main.rand.Next(SpecialAttack)).ToArray();

                //debug text
                foreach (var i in AttackPattern)
                {
                    Main.NewText("" + i, 125, 125, 125);
                }

                SoundEngine.PlaySound(LaughSound, NPC.Center);
                NPC.ai[2] = 1;
            }

            NPC.ai[0] = AttackPattern[(int)NPC.ai[1]];

            switch ((int)NPC.ai[0])
            {
                //shoot flowers that bounce around off walls
                case 0:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(300);

                    if (NPC.localAI[0] == 140 || NPC.localAI[0] == 170 || NPC.localAI[0] == 200)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                        //recoil
                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize();
                                
                        Recoil.X *= -10;
                        Recoil.Y *= -10;  
                        NPC.velocity.X = Recoil.X;
                        NPC.velocity.Y = Recoil.Y;

                        NPC.velocity *= 0.98f;

                        int MaxProjectiles = Main.rand.Next(2, 3);

                        for (int numProjectiles = -MaxProjectiles; numProjectiles <= MaxProjectiles; numProjectiles++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center,
                                10f * NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(10) * numProjectiles),
                                ModContent.ProjectileType<BouncingFlower>(), Damage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 320)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }

                    break;
                }
                //make a group of telegraphs around the player, then shoot vine tendrils at all of the telegraphs
                case 1:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(100);

                    if (NPC.localAI[1] < 5)
                    {
                        if (NPC.localAI[0] == 60)
                        {
                            for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                            {   
                                int PosX = (int)player.Center.X + Main.rand.Next(-120, 120);
                                int PosY = (int)player.Center.Y + Main.rand.Next(-120, 120);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), PosX, PosY, 0, 0,
                                    ModContent.ProjectileType<BigBoneThornTelegraph>(), 0, 0f, Main.myPlayer, 0, 0);
                                }

                                //save each point after making telegraphs
                                SavePoint[numProjectiles] = new Vector2(PosX, PosY);
                            }

                            for (int i = 0; i < SavePoint.Length; i++)
                            {
                                Vector2 Direction = NPC.Center - SavePoint[i];
                                Direction.Normalize();

                                Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 0,
                                    ModContent.ProjectileType<BigBoneThorn>(), Damage + 20, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                                }
                            }
                        }

                        if (NPC.localAI[0] == 100)
                        {
                            SoundEngine.PlaySound(MagicCastSound, NPC.Center);

                            //recoil
                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize();
                                    
                            Recoil.X *= -10;
                            Recoil.Y *= -10;
                            NPC.velocity.X = Recoil.X;
                            NPC.velocity.Y = Recoil.Y;
                        }

                        if (NPC.localAI[0] >= 120)
                        {
                            if (NPC.localAI[1] == 4)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                            }
                            else
                            {
                                NPC.localAI[0] = 58;
                                NPC.localAI[1]++;
                            }
                        }
                    }
                    else
                    {
                        if (NPC.localAI[0] >= 85)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[1]++;

                            if (NPC.ai[1] >= AttackPattern.Length)
                            {
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                            }
                        }
                    }
                    
                    break;
                }
                //shoot a continuous stream of leaves that move in a sinewave like pattern
                case 2:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(200);

                    if (NPC.localAI[0] >= 80 && NPC.localAI[0] <= 200)
                    {
                        NPC.velocity *= 0.98f;

                        if (Main.rand.Next(2) == 0)
                        {
                            //recoil
                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize();
                                    
                            Recoil.X *= -2;
                            Recoil.Y *= -2;
                            NPC.velocity.X = Recoil.X;
                            NPC.velocity.Y = Recoil.Y;

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                                    
                            ShootSpeed.X *= 15;
                            ShootSpeed.Y *= 15;

                            SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                                ShootSpeed.X, ShootSpeed.Y,ModContent.ProjectileType<RazorLeaf>(), Damage, 0f, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 240)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }

                    break;
                }
                //shoot greek fire around twice
                case 3:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(500);

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 180)
                    {
                        //recoil
                        NPC.velocity.X = 0;
                        NPC.velocity.Y *= 5;

                        int NumProjectiles = Main.rand.Next(10, 15);
                        for (int i = 0; i < NumProjectiles; ++i)
                        {
                            float Spread = (float)Main.rand.Next(-1500, 1500) * 0.01f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0 + Spread, Main.rand.Next(-5, -2), 
                                ProjectileID.GreekFire1, Damage, 1, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }

                    break;
                }
                //shoot multiple homing, slow fire ball that massively explodes into a spread of smoke and smaller fireballs
                case 4:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(400);

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 150 || NPC.localAI[0] == 180)
                    {
                        SoundEngine.PlaySound(MagicCastSound, NPC.Center);

                        //recoil
                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize();
                                
                        Recoil.X *= -15;
                        Recoil.Y *= -15;
                        NPC.velocity.X = Recoil.X;
                        NPC.velocity.Y = Recoil.Y;

                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                                
                        ShootSpeed.X *= 2;
                        ShootSpeed.Y *= 2;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                        Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                        if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                        {
                            position += muzzleOffset;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromThis(), position.X, position.Y, ShootSpeed.X, 
                            ShootSpeed.Y, ModContent.ProjectileType<GiantFlameBall>(), Damage, 1, Main.myPlayer, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 480)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }

                    break;
                }
                //create skull wisps around itself that launch themselves at the player
                case 5:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(500);

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] <= 240)
                    {
                        if (Main.rand.Next(10) == 0)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                                    
                            ShootSpeed.X *= Main.rand.Next(-12, 12);
                            ShootSpeed.Y *= Main.rand.Next(-12, 12);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100), 
                                ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<BoneWisp>(), Damage, 1, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 420)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }

                    break;
                }

                //Special attacks

                //shoot seeds that turn into flowers that heal big bone
                case 6:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(300);

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 130 || NPC.localAI[0] == 140)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                        Vector2 Speed = new Vector2(Main.rand.NextFloat(5f, 10f), 0f).RotatedByRandom(2 * Math.PI);

                        for (int numProjectiles = 0; numProjectiles < 4; numProjectiles++)
                        {
                            Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, 
                                ModContent.ProjectileType<LingeringFlowerSeed>(), Damage, 0f, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }

                    break;
                }
                //charge attack, stop for a second, flash some kind of visual aura, then charge and crash into the wall and then get stunned for a few seconds
                case 7:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 180)
                    {
                        GoAboveFlowerPot(300);
                        
                        Vector2 Speed = new Vector2(-8f, 0f).RotatedByRandom(2 * Math.PI);

                        for (int num = 0; num < 10; num++)
                        {
                            Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (num + Main.rand.NextDouble() - 0.5));
                            
                            int DustGore = Dust.NewDust(NPC.Center, NPC.width / 3, NPC.height / 3, DustID.YellowTorch, 0f, -2f, 0, default, 1.5f);
                            Main.dust[DustGore].noGravity = true;
                            Main.dust[DustGore].velocity = speed;
                        }
                    }

                    if (NPC.localAI[0] == 180)
                    {
                        SoundEngine.PlaySound(GrowlSound1, NPC.Center);

                        //charge
                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 45;
                        ChargeDirection.Y *= 45;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] > 180 && NPC.localAI[1] == 0)
                    {
                        if (NPC.velocity.Y <= 0.1f && NPC.velocity.X <= 0.1f)
                        {
                            NPC.velocity *= 0;

                            SpookyPlayer.ScreenShakeAmount = 5;

                            NPC.localAI[1] = 1;
                        }
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }
                    }

                    break;
                }
                //aura attack, make an expanding aura of light. once fully expanded, any player within will have a % of their hp drained and heals a % of big bones hp
                //normal mode = 20% of hp drained, 15% of big bone hp healed
                //expert mode = 35% of hp drained, 25% of big bone hp healed
                //master mode = 45% of hp drained, 35% of big bone hp healed
                //note that these values are based on maximum hp, not current hp
                //alternate idea: being in the aura drains your life slowly but heals big bone over time slowly
                //also use an intense orange screenshader that gets more intense as the attack goes on
                case 8:
                {
                    break;
                }
            }
		}

        public void GoAboveFlowerPot(float DistanceAbove)
        {
            float goToX = Main.npc[(int)NPC.ai[3]].Center.X - NPC.Center.X;
            float goToY = (Main.npc[(int)NPC.ai[3]].Center.Y - DistanceAbove) - NPC.Center.Y;

            float speed;

            if (Vector2.Distance(NPC.Center, Main.npc[(int)NPC.ai[3]].Center) >= 400f)
            {
                speed = 0.8f;
            }
            else if (NPC.ai[0] >= 180)
            {
                speed = 0.01f;
            }
            else
            {
                speed = 0.5f;
            }
            
            if (NPC.velocity.X > speed)
            {
                NPC.velocity.X *= 0.98f;
            }
            if (NPC.velocity.Y > speed)
            {
                NPC.velocity.Y *= 0.98f;
            }

            if (NPC.velocity.X < goToX)
            {
                NPC.velocity.X = NPC.velocity.X + speed;
                if (NPC.velocity.X < 0f && goToX > 0f)
                {
                    NPC.velocity.X = NPC.velocity.X + speed;
                }
            }
            else if (NPC.velocity.X > goToX)
            {
                NPC.velocity.X = NPC.velocity.X - speed;
                if (NPC.velocity.X > 0f && goToX < 0f)
                {
                    NPC.velocity.X = NPC.velocity.X - speed;
                }
            }
            if (NPC.velocity.Y < goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y + speed;
                if (NPC.velocity.Y < 0f && goToY > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + speed;
                    return;
                }
            }
            else if (NPC.velocity.Y > goToY)
            {
                NPC.velocity.Y = NPC.velocity.Y - speed;
                if (NPC.velocity.Y > 0f && goToY < 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y - speed;
                    return;
                }
            }
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                /*
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MocoGore" + numGores).Type);
                }
                */
            }

            return true;
        }

        /*
        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagMoco>()));

            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MocoTissue>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<MocoRelicItem>()));

            int[] MainItem = new int[] { ModContent.ItemType<BoogerFlail>(), ModContent.ItemType<BoogerBlaster>() };
            
            notExpertRule.OnSuccess(ItemDropRule.Common(Main.rand.Next(MainItem)));

            npcLoot.Add(notExpertRule);
        }
        */

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedBigBone, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}