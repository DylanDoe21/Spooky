using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.NPCs.Boss.BigBone.Projectiles;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    //[AutoloadBossHead]
    public class BigBone : ModNPC
    {
        int[] AttackPattern = new int[] { 0, 1, 2, 3, 4, 5 };
        int[] SpecialAttack = new int[] { 6, 7 };

        public bool Phase2 = false;
        public bool Transition = false;
        public bool shieldSpawned = false;

        Vector2[] SavePoint = new Vector2[5];
        Vector2 SavePlayerPosition;

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
            NPC.lifeMax = Main.masterMode ? 80000 / 3 : Main.expertMode ? 72000 / 2 : 55000;
            NPC.damage = 100;
            NPC.defense = 55;
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

            /*
            if (NPC.ai[0] == -1)
            {
                var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.4f / 2.4f * 6.28318548f)) / 2f + 0.5f;

                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Orange);

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    Color newColor = color;
                    newColor = NPC.GetAlpha(newColor);
                    newColor *= 1f - fade;
                    Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) + (numEffect / 4 * 6.28318548f + NPC.rotation + 0f).ToRotationVector2() * (4f * fade + 2f) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4) - NPC.velocity * numEffect;
                    Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 1.05f + fade / 2, effects, 0);
                }
            }
            */

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow1").Value;

            if (NPC.ai[0] == 0 || NPC.ai[0] == 7)
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow1").Value;
            }

            if (NPC.ai[0] == 2 || NPC.ai[0] == 3 || NPC.ai[0] == 6)
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow2").Value;
            }

            if (NPC.ai[0] == 4 || NPC.ai[0] == 5 || NPC.ai[0] == 8)
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow3").Value;
            }

            if (NPC.ai[0] == 1)
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow4").Value;
            }

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Transparent, Color.White, fade);

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            if (NPC.ai[0] == -1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                var center = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                float intensity = fade;
                DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Spooky/Content/Effects/SkullEffect").Value, center - new Vector2(0, -55), 
                new Rectangle(0, 0, 500, 400), Color.Orange, 0, new Vector2(250f, 250f), NPC.scale * (1f + intensity * 0.05f), SpriteEffects.None, 0);
                GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + intensity * 0.5f));
                GameShaders.Misc["ForceField"].Apply(drawData);
                drawData.Draw(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
                return;
            }
            //Filters.Scene["Nebula"].GetShader().UseIntensity(0f).UseProgress(0f); // why is this here
        }

        public override void AI()
        {   
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 85 / 3 : Main.expertMode ? 50 / 2 : 50;

            NPC.spriteDirection = NPC.direction;

            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //despawn if all players are dead or not in the biome
            if (player.dead)
            {
                NPC.localAI[2]++;
                if (NPC.localAI[2] >= 180)
                {
                    NPC.active = false;
                }
            }

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
                    AttackPattern = AttackPattern.Where(i => i != 6).ToArray();
                    AttackPattern = AttackPattern.Where(i => i != 7).ToArray();
                }
                    
                //shuffle the attack pattern array
                AttackPattern = AttackPattern.OrderBy(x => Main.rand.Next()).ToArray();

                //add one of the special attacks to the end of the array
                if (NPC.CountNPCS(ModContent.NPCType<HealingFlower>()) <= 0)
			    {
                    AttackPattern = AttackPattern.Append(Main.rand.Next(SpecialAttack)).ToArray();
                    Main.NewText("flowers dont exist", 125, 125, 125);
                }
                else
                {
                    AttackPattern = AttackPattern.Append(7).ToArray();
                    Main.NewText("Flowers do exist", 125, 125, 125);
                }

                //debug text
                foreach (var i in AttackPattern)
                {
                    Main.NewText("" + i, 125, 125, 125);
                }

                SoundEngine.PlaySound(LaughSound, NPC.Center);
                NPC.ai[2] = 1;
            }

            if (NPC.life < (NPC.lifeMax / 2) && !Phase2)
            {
                Transition = true;
            }

            if (Transition)
            {
                NPC.ai[0] = -1;
            }
            else
            {
                NPC.ai[0] = AttackPattern[(int)NPC.ai[1]];
            }

            switch ((int)NPC.ai[0])
            {
                //phase 2 transition
                case -1:
                {
                    GoAboveFlowerPot(350);

                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;

                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.ai[1] = 0;

                    if (!shieldSpawned)
                    {
                        for (int numOrbiter = 0; numOrbiter < 12; numOrbiter++)
                        {
                            int distance = 360 / 12;

                            int orbiter = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, 
                            ModContent.NPCType<FlameSkull>(), NPC.whoAmI, numOrbiter * distance, NPC.whoAmI);
                        }

                        shieldSpawned = true;
                    }

                    if (NPC.CountNPCS(ModContent.NPCType<FlameSkull>()) <= 0 && shieldSpawned)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;

                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;
                        Phase2 = true;
                        Transition = false;
                    }

                    break;
                }
                //shoot flowers that bounce around off walls
                case 0:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(300);

                    if (NPC.localAI[0] == 75 || NPC.localAI[0] == 105 || NPC.localAI[0] == 135)
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

                    if (NPC.localAI[0] >= 180)
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
                case 1:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(500);

                    if (NPC.localAI[0] >= 75 && NPC.localAI[0] <= 195)
                    {
                        int WispChance = Phase2 ? 2 : 6;

                        if (Main.rand.Next(WispChance) == 0)
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

                    if (NPC.localAI[0] >= 270)
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
                case 2:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(100);

                    if (NPC.localAI[1] < 4)
                    {
                        if (NPC.localAI[0] == 75)
                        {
                            for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                            {
                                float distance = Phase2 ? Main.rand.NextFloat(8f, 12f) : Main.rand.NextFloat(5f, 8f);

                                Vector2 Position = (Vector2.One * new Vector2((float)player.width / 3f, (float)player.height / 3f) * distance).RotatedBy((double)((float)(numProjectiles - (5 / 2 - 1)) * 6.28318548f / 5f), default(Vector2)) + player.Center;
                            
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), Position.X, Position.Y, 0, 0,
                                    ModContent.ProjectileType<BigBoneThornTelegraph>(), 0, 0f, Main.myPlayer, 0, 0);
                                }

                                SavePoint[numProjectiles] = new Vector2(Position.X, Position.Y);
                            }

                            /*
                            for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                            {   
                                int PosX = Phase2 ? (int)player.Center.X + Main.rand.Next(-400, 400) : (int)player.Center.X + Main.rand.Next(-120, 120);
                                int PosY = Phase2 ? (int)player.Center.X + Main.rand.Next(-150, 150) : (int)player.Center.Y + Main.rand.Next(-120, 120);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromThis(), PosX, PosY, 0, 0,
                                    ModContent.ProjectileType<BigBoneThornTelegraph>(), 0, 0f, Main.myPlayer, 0, 0);
                                }

                                SavePoint[numProjectiles] = new Vector2(PosX, PosY);
                            }
                            */

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

                        if (NPC.localAI[0] == 115)
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

                        if (NPC.localAI[0] >= 135)
                        {
                            if (NPC.localAI[1] == 3)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1]++;
                            }
                            else
                            {
                                NPC.localAI[0] = 74;
                                NPC.localAI[1]++;
                            }
                        }
                    }
                    else
                    {
                        if (NPC.localAI[0] >= 65)
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
                case 3:
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
                                    
                            ShootSpeed.X *= Phase2 ? 18 : 15;
                            ShootSpeed.Y *= Phase2 ? 18 : 15;

                            SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                                ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<RazorLeaf>(), Damage, 0f, Main.myPlayer, 0, 0);
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
                //shoot greek fire around
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 120)
                    {
                        GoAboveFlowerPot(300);
                    }

                    if (NPC.localAI[0] > 120 && NPC.localAI[0] <= 150)
                    {
                        GoAboveFlowerPot(400);
                    }

                    if (NPC.localAI[0] > 150)
                    {
                        GoAboveFlowerPot(500);
                    }

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 150 || NPC.localAI[0] == 180)
                    {
                        int NumProjectiles = Main.rand.Next(10, 15);
                        for (int i = 0; i < NumProjectiles; ++i)
                        {
                            float Spread = (float)Main.rand.Next(-1500, 1500) * 0.01f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int GreekFire = Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, 0 + Spread, 
                                Main.rand.Next(-5, -2), ProjectileID.GreekFire1, Damage, 1, Main.myPlayer, 0, 0);
                                Main.projectile[GreekFire].timeLeft = 1200;
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 210)
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
                case 5:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(400);

                    if ((!Phase2 && (NPC.localAI[0] == 75 || NPC.localAI[0] == 105 || NPC.localAI[0] == 135)) ||
                    (Phase2 && (NPC.localAI[0] == 55 || NPC.localAI[0] == 80 || NPC.localAI[0] == 105 || NPC.localAI[0] == 130 || NPC.localAI[0] == 155)))
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

                    if (NPC.localAI[0] >= 315)
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

                //special attacks

                //shoot seeds that turn into flowers that heal big bone
                case 6:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(300);

                    if (NPC.localAI[0] <= 120)
                    {
                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(0.75f, 1.2f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.RedTorch, velocity.X * 2f, velocity.Y * 2f, 100, default, 1.4f);
                            Main.dust[dustEffect].scale = 2.5f;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-18f, -5f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 130 || NPC.localAI[0] == 140)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                        Vector2 Speed = new Vector2(Main.rand.NextFloat(5f, 8.5f), 0f).RotatedByRandom(2 * Math.PI);

                        for (int numProjectiles = 0; numProjectiles < 4; numProjectiles++)
                        {
                            Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, speed, 
                                ModContent.ProjectileType<HealingFlowerSeed>(), 0, 0f, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                    }

                    break;
                }
                //stay still, then charge and crash into the wall
                case 7:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 120)
                    {
                        GoAboveFlowerPot(300);

                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(0.75f, 1.2f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.YellowTorch, velocity.X * 2f, velocity.Y * 2f, 100, default, 1.4f);
                            Main.dust[dustEffect].scale = 2.5f;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-18f, -5f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] == 120)
                    {
                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize();
                                
                        Recoil.X *= -5;
                        Recoil.Y *= -5;  
                        NPC.velocity.X = Recoil.X;
                        NPC.velocity.Y = Recoil.Y;

                        SavePlayerPosition = player.Center;
                    }

                    if (NPC.localAI[0] == 130)
                    {
                        NPC.velocity *= 0.5f;
                    }

                    if (NPC.localAI[0] == 140)
                    {
                        SoundEngine.PlaySound(GrowlSound2, NPC.Center);

                        //charge
                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 45;
                        ChargeDirection.Y *= 45;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] > 140 && NPC.localAI[1] == 0)
                    {
                        if (NPC.velocity.X <= 0.1f && NPC.velocity.X >= -0.1f)
                        {
                            NPC.velocity *= 0;
                        }

                        if (NPC.velocity.Y <= 0.1f && NPC.velocity.Y >= -0.1f)
                        {
                            NPC.velocity *= 0;
                        }

                        if (NPC.velocity == Vector2.Zero)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath43, NPC.Center);
                            
                            SpookyPlayer.ScreenShakeAmount = 25;

                            NPC.velocity *= 0;

                            NPC.localAI[1] = 1;
                        }
                    }

                    if (NPC.localAI[0] >= 200)
                    {
                        GoAboveFlowerPot(300);
                    }

                    if (NPC.localAI[0] >= 240)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
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
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(300);

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

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            //LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagBigBone>()));

            /*
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MocoTissue>(), 4));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<MocoRelicItem>()));

            int[] MainItem = new int[] { ModContent.ItemType<BoogerFlail>(), ModContent.ItemType<BoogerBlaster>() };
            
            notExpertRule.OnSuccess(ItemDropRule.Common(Main.rand.Next(MainItem)));

            npcLoot.Add(notExpertRule);
            */
        }

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