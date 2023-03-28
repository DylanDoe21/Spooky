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
using Spooky.Content.Items.Cemetery;
using Spooky.Content.Items.Pets;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles;

namespace Spooky.Content.NPCs.Boss.SpookySpirit
{
    [AutoloadBossHead]
    public class SpookySpirit : ModNPC
    {
        Vector2 SavePlayerPosition;
        public int SaveDirection = 0;
        public int SaveNPCDamage;

        public bool ShouldDamagePlayer = true;
        public bool EyeSprite = false;
        public bool BothEyes = false;
        public bool StopSpinning = false;
        public bool Phase2 = false;
        
        public int MoveSpeedX = 0;
		public int MoveSpeedY = 0;
        public int Spin = 0;

        public float rotate = 0;
		public float SpinX = 0;
		public float SpinY = 0;
        public float alpha;

        //good spawn noise: DD2_EtherianPortalOpen
        //DD2_DefeatScene and DD2_WinScene are cool too
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Spirit");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/SpookySpirit/SpookySpiritBC",
                Position = new Vector2(30f, -10f),
                PortraitPositionXOverride = 10f,
                PortraitPositionYOverride = 0f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused, BuffID.Poisoned, BuffID.OnFire
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(SaveDirection);
            writer.Write(SaveNPCDamage);
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);
            writer.Write(Spin);

            //bools
            writer.Write(ShouldDamagePlayer);
            writer.Write(EyeSprite);
            writer.Write(BothEyes);
            writer.Write(StopSpinning);
            writer.Write(Phase2);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            SaveDirection = reader.ReadInt32();
            SaveNPCDamage = reader.ReadInt32();
            MoveSpeedX = reader.ReadInt32();
            MoveSpeedY = reader.ReadInt32();
            Spin = reader.ReadInt32();

            //bools
            ShouldDamagePlayer = reader.ReadBoolean();
            EyeSprite = reader.ReadBoolean();
            BothEyes = reader.ReadBoolean();
            StopSpinning = reader.ReadBoolean();
            Phase2 = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 3400;
            NPC.damage = 40;
            NPC.defense = 8;
            NPC.width = 116;
            NPC.height = 112;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath52;
            Music = MusicID.Eclipse;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * 0.85f);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A giant, mischevious spirit. As a result of becoming too powerful, it abandoned the giant gourd it once possessed and took over the swampy cemetery."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //draw aura
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookySpirit/SpookySpiritAura").Value;

            Vector2 drawOrigin = new(tex.Width * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (SaveDirection != 0)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            for (int numEffect = 0; numEffect < 4; numEffect++)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.White, (EyeSprite ? Color.OrangeRed : Color.BlueViolet), numEffect));

                Color newColor = color;
                newColor = NPC.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(NPC.Center.X - 1, NPC.Center.Y) + (numEffect / 2 * 6.28318548f + NPC.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4) * numEffect;
                Main.EntitySpriteDraw(tex, vector, NPC.frame, newColor, NPC.rotation, drawOrigin, NPC.scale * 1.035f, effects, 0);
            }
            
            return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //make alpha fade in and out properly
            if (NPC.alpha < 5 && alpha < 1f)
            {
                alpha += 0.01f;
            }
            if (NPC.alpha >= 5 && alpha > 0f)
            {
                alpha -= 0.05f;
            }

            Texture2D eyeTex1 = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookySpirit/SpookySpiritEye1").Value;
            Texture2D eyeTex2 = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookySpirit/SpookySpiritEye2").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (SaveDirection != 0)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            
            if (!EyeSprite || BothEyes)
            {
                Main.EntitySpriteDraw(eyeTex1, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, 
                new Color(255, 255, 255) * Math.Min(1f, (Main.screenPosition.Y - 500f) / 1000f * alpha), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
            if (EyeSprite || BothEyes)
            {
                Main.EntitySpriteDraw(eyeTex2, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), NPC.frame, 
                new Color(255, 255, 255) * Math.Min(1f, (Main.screenPosition.Y - 500f) / 1000f * alpha), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return ShouldDamagePlayer;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 42 / 3 : Main.expertMode ? 30 / 2 : 22;

            if (SaveDirection != 0)
            {
                NPC.spriteDirection = SaveDirection;
            }
            else
            {
                NPC.spriteDirection = NPC.direction;
            }

            NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.02f : -0.02f);

            Phase2 = NPC.life <= (NPC.lifeMax / 2);

            //despawn when all players die or if it is day time
            if (Main.player[NPC.target].dead || Main.dayTime)
            {
                NPC.velocity.Y = -45;

                NPC.ai[1]++;
                if (NPC.ai[1] >= 75)
                {
                    NPC.active = false;
                }
            }
                
            switch ((int)NPC.ai[0])
            {
                //fly at the player for a bit
                case 0:
                {
                    NPC.localAI[0]++;

                    //flies to players X position
                    if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -55) 
                    {
                        MoveSpeedX -= 2;
                    }
                    else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 55)
                    {
                        MoveSpeedX += 2;
                    }

                    NPC.velocity.X = MoveSpeedX * 0.1f;
                    
                    //flies to players Y position
                    if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -35)
                    {
                        MoveSpeedY--;
                    }
                    else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= 35)
                    {
                        MoveSpeedY++;
                    }

                    NPC.velocity.Y = MoveSpeedY * 0.1f;

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //go to the side and charge 3 times
                case 1:
                {
                    NPC.localAI[0]++;

                    //repeat three times
                    if (NPC.localAI[1] < 3)
                    {
                        //Go to the side of the player to prepare for dash
                        if (NPC.localAI[0] >= 0 && NPC.localAI[0] < 60) 
                        {	
                            Vector2 GoTo = player.Center;
                            GoTo.X += (NPC.Center.X < player.Center.X) ? -400 : 400;
                            GoTo.Y += 0;

                            float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 30);
                            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                        }

                        if (NPC.localAI[0] == 60)
                        {
                            NPC.velocity *= 0;
                            SavePlayerPosition = player.Center;
                            SaveDirection = NPC.direction;
                        }

                        //actual dash attack
                        if (NPC.localAI[0] == 75)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath51, NPC.Center);

                            Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                            ChargeDirection.Normalize();
                                    
                            int Speed = Phase2 ? 50 : 45;
                            ChargeDirection.X = ChargeDirection.X * Speed;
                            ChargeDirection.Y = ChargeDirection.Y * 0;
                            NPC.velocity.X = ChargeDirection.X;
                            NPC.velocity.Y = ChargeDirection.Y;
                        }

                        if (NPC.localAI[0] >= 90)
                        {
                            NPC.velocity *= 0.65f;
                        }

                        //loop charge attack
                        if (NPC.localAI[0] == 105)
                        {
                            SaveDirection = 0;
                            NPC.localAI[1]++;
                            NPC.localAI[0] = 0;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    { 
                        SaveDirection = 0;
                        NPC.velocity *= 0.65f;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //stay still and randomly fire off phantom bolts
                case 2:
                {
                    NPC.localAI[0]++;

                    //fly to the player very slowly
                    if (NPC.localAI[0] > 30 && NPC.localAI[0] < 210)
                    {   
                        //flies to players X position
                        if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -5) 
                        {
                            MoveSpeedX -= 2;
                        }
                        else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= 5)
                        {
                            MoveSpeedX += 2;
                        }

                        NPC.velocity.X = MoveSpeedX * 0.1f;
                        
                        //flies to players Y position
                        if (NPC.Center.Y >= player.Center.Y - 60f && MoveSpeedY >= -5)
                        {
                            MoveSpeedY--;
                        }
                        else if (NPC.Center.Y <= player.Center.Y - 60f && MoveSpeedY <= 5)
                        {
                            MoveSpeedY++;
                        }

                        NPC.velocity.Y = MoveSpeedY * 0.1f;

                        //fire out homing seeds
                        if (Main.rand.Next(12) == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-60, 60), NPC.Center.Y + Main.rand.Next(-60, 60), 
                                Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-8f, 8f), ModContent.ProjectileType<PhantomSeed>(), Damage, 1, Main.myPlayer, 0, 0);	
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = Phase2 ? 3 : 4;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //go above player and then fire off homing skull circles
                case 3:
                {
                    NPC.localAI[0]++;

                    Vector2 GoTo = player.Center;
                    GoTo.X += 0;
                    GoTo.Y -= 350;

                    float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                    if (NPC.localAI[0] > 75)
                    {
                        NPC.velocity *= 0.75f;
                    }

                    if (NPC.localAI[0] == 120 || NPC.localAI[0] == 165 || NPC.localAI[0] == 210)
                    {
                        SoundEngine.PlaySound(SoundID.Item84, NPC.position);

                        for (int numSkulls = 0; numSkulls < 6; numSkulls++)
                        {
                            int distance = 360 / 6;
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PhantomSkull>(), NPC.whoAmI, NPC.whoAmI, numSkulls * distance);
                        }
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //teleport around the player, fire bolt spread, repeat 4 times
                case 4:
                {
                    NPC.localAI[0]++;

                    int repeats = Phase2 ? 6 : 5;
                    if (NPC.localAI[1] < repeats)
                    {
                        NPC.velocity *= 0;

                        if (NPC.localAI[0] == 60)
                        {
                            EyeSprite = true;
                        }

                        //make glowmask fade before teleporting
                        if (NPC.localAI[0] > 60 && NPC.localAI[0] < 90)
                        {
                            if (alpha > 0f)
                            {
                                alpha -= 0.025f;
                            }
                        }

                        //shrink and fade out before teleport
                        if (NPC.localAI[0] >= 90 && NPC.localAI[0] < 120)
                        {
                            NPC.immortal = true;
                            NPC.dontTakeDamage = true;

                            NPC.scale -= 0.01f;
                            NPC.alpha += 10;

                            //make the spirit itself shrink and fade out
                            if (NPC.alpha >= 255)
                            {
                                NPC.alpha = 255;
                            }
                            if (NPC.scale <= 0)
                            {
                                NPC.scale = 0;
                            }
                        }

                        //teleport
                        if (NPC.localAI[0] == 120)
                        {
                            NPC.position.X = player.direction == -1 ? player.Center.X + 550 : player.Center.X - 650;
                            NPC.position.Y = player.Center.Y - 350;

                            NPC.netUpdate = true;
                        }

                        //grow and fade in after teleport
                        if (NPC.localAI[0] > 120 && NPC.localAI[0] <= 150)
                        {
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;

                            NPC.scale += 0.01f;
                            NPC.alpha -= 10;
                                
                            //make the spirit itself grow and fade back in
                            if (NPC.alpha <= 0)
                            {
                                NPC.alpha = 0;
                            }
                            if (NPC.scale >= 1)
                            {
                                NPC.scale = 1;
                            }

                            //set glowmask alpha back correctly
                            if (alpha < 1f)
                            {
                                alpha += 0.01f;
                            }
                        }

                        if (NPC.localAI[0] == 150)
                        {
                            SaveDirection = NPC.direction;
                        }
                        
                        //after the first 4 teleports use bolt spread
                        if (NPC.localAI[1] <= 4)
                        {
                            if (NPC.localAI[0] >= 155 && NPC.localAI[0] <= 180 && NPC.localAI[2] <= 6)
                            {
                                NPC.localAI[2]++;

                                SoundEngine.PlaySound(SoundID.DD2_GhastlyGlaiveImpactGhost with { Volume = SoundID.DD2_GhastlyGlaiveImpactGhost.Volume * 3.5f }, NPC.Center);

                                float storeRotation = (float)Math.Atan2(NPC.Center.Y - player.Center.Y, NPC.Center.X - player.Center.X);

                                Vector2 projSpeed = new Vector2((float)((Math.Cos(storeRotation) * 10) * -1), (float)((Math.Sin(storeRotation) * 10) * -1));
                                float rotation = MathHelper.ToRadians(5);
                                float amount = NPC.direction == -1 ? NPC.localAI[2] - 7.2f / 2 : -(NPC.localAI[2] - 8.8f / 2);
                                Vector2 ShootSpeed = new Vector2(projSpeed.X, projSpeed.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, amount));

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                    ModContent.ProjectileType<EyeBolt>(), Damage, 0f, Main.myPlayer);
                                }
                            }

                            if (NPC.localAI[0] > 180)
                            {
                                SaveDirection = 0;
                                NPC.localAI[0] = 50;
                                NPC.localAI[2] = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                        
                        //shoot laser out of his eye
                        if (NPC.localAI[1] == 5)
                        {
                            if (NPC.localAI[0] == 155)
                            {
                                SaveDirection = NPC.direction;
                                SavePlayerPosition = new Vector2(player.Center.X, player.Center.Y + 10);

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X, player.Center.Y, 0, 0, 
                                ModContent.ProjectileType<EyeBeamTelegraph>(), 0, 0f, Main.myPlayer);
                            }

                            if (NPC.localAI[0] == 200)
                            {
                                SpookyPlayer.ScreenShakeAmount = 8;
                                
                                SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
                            }

                            if (NPC.localAI[0] >= 200 && NPC.localAI[0] <= 270)
                            {
                                NPC.direction = SaveDirection;

                                Vector2 ShootSpeed = SavePlayerPosition - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed.X *= 5f;
                                ShootSpeed.Y *= 5f;

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + (NPC.direction == -1 ? -48 : 48), NPC.Center.Y + 3,
                                ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<EyeBeam>(), Damage * 2, 0f, Main.myPlayer);
                            }

                            if (NPC.localAI[0] >= 350)
                            {
                                SaveDirection = 0;
                                NPC.localAI[1]++;
                                NPC.netUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        int EndTime = Phase2 ? 0 : 180;
                        if (NPC.localAI[0] >= EndTime)
                        {
                            EyeSprite = false;
                            SaveDirection = 0;
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                    }
                    
                    break;
                }

                //fly above the player, shoot skulls upward
                case 5:
                {
                    NPC.localAI[0]++;

                    //fly the corner of the player
                    if (NPC.localAI[0] >= 0 && NPC.localAI[0] < 70)
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
                        SaveDirection = NPC.direction;

                        NPC.velocity *= 0f;
                    }

                    //charge
                    if (NPC.localAI[0] == 80)
                    {   
                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X = ChargeDirection.X * 25;
                        ChargeDirection.Y = ChargeDirection.Y * 0;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    //shoot spreads of skulls upward
                    if (NPC.localAI[0] == 80 || NPC.localAI[0] == 100 || NPC.localAI[0] == 120) 
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { Volume = SoundID.DD2_DarkMageSummonSkeleton.Volume * 3.5f }, NPC.Center);

                        int NumProjectiles = Main.rand.Next(5, 8);
                        for (int i = 0; i < NumProjectiles; i++)
                        {
                            float Spread = (float)Main.rand.Next(-1000, 1000) * 0.01f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0 + Spread, Main.rand.Next(-15, -10), 
                                ModContent.ProjectileType<PhantomBomb>(), Damage, 1, Main.myPlayer, 0, 0);
                            }
                        }
                    }

                    if (NPC.localAI[0] >= 120)
                    {
                        SaveDirection = 0;

                        NPC.velocity *= 0.95f;
                    }

                    if (NPC.localAI[0] >= 320)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //spin around the player and go slightly invisible, shoot pumpkin seeds, and then charge
                case 6:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 120)
                    {
                        Vector2 GoTo = player.Center;
                        GoTo.X += (NPC.Center.X < player.Center.X) ? -400 : 400;
                        GoTo.Y += 0;

                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 18, 25);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

                        //use localAI[1] to save which direction he should spin in
                        if (NPC.Center.X < player.Center.X)
                        {
                            NPC.localAI[1] = 0;
                        }
                        else
                        {
                            NPC.localAI[1] = 1;
                        }
                    }

                    //actual spin attack
                    if (NPC.localAI[0] > 120 && NPC.localAI[0] < 300)
                    {
                        ShouldDamagePlayer = false;
                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;

                        //make the spirit mostly invisible
                        NPC.alpha += 8;
                        if (NPC.alpha >= 200)
                        {
                            NPC.alpha = 200;
                        }

                        NPC.velocity = new Vector2(NPC.velocity.X, NPC.velocity.Y).RotatedBy(MathHelper.ToRadians(Spin - 30));
                        NPC.TargetClosest(false);
                        
                        rotate += NPC.localAI[1] > 0 ? -4f : 4f;

                        Vector2 SpinTo = new Vector2(500, 500).RotatedBy(MathHelper.ToRadians(rotate * 1.57f));
                        
                        SpinX = player.Center.X + SpinTo.X - NPC.Center.X;
                        SpinY = player.Center.Y + SpinTo.Y - NPC.Center.Y;
                            
                        float distance = (float)System.Math.Sqrt((double)(SpinX * SpinX + SpinY * SpinY));

                        if (distance > 55)
                        {
                            distance = 6.5f / distance;
                                                
                            SpinX *= distance * 7;
                            SpinY *= distance * 7;
                                
                            NPC.velocity.X = SpinX;
                            NPC.velocity.Y = SpinY;
                        }
                        else
                        {
                            NPC.position.X = player.Center.X + SpinTo.X - NPC.height / 2;
                            NPC.position.Y = player.Center.Y + SpinTo.Y - NPC.width / 2;
                                
                            distance = 6.5f / distance;
                                                
                            SpinX *= distance * 7;
                            SpinY *= distance * 7;
                            NPC.velocity.X = 0;
                            NPC.velocity.Y = 0;
                        }

                        if (Main.rand.Next(15) == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Item20, NPC.position);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-60, 60), NPC.Center.Y + Main.rand.Next(-60, 60), 
                                Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f), ModContent.ProjectileType<PhantomSeed>(), Damage, 1, Main.myPlayer, 0, 0);
                            }
                        }
                    }
                    //make spirit visible again after spin attack
                    else
                    {
                        ShouldDamagePlayer = true;
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;

                        NPC.alpha -= 8;
                        if (NPC.alpha <= 0)
                        {
                            NPC.alpha = 0;
                        }
                    }

                    //teleport right above the player
                    if (NPC.localAI[0] == 300)
                    {
                        NPC.position.X = player.position.X + Main.rand.Next(-250, 250);
                        NPC.position.Y = player.position.Y - Main.rand.Next(300, 350);
                    }

                    //slow down right before charging
                    if (NPC.localAI[0] >= 300 && NPC.localAI[0] < 350)
                    {
                        NPC.velocity *= 0.1f;
                    }

                    //charge at the player
                    if (NPC.localAI[0] == 350)
                    {
                        SaveDirection = NPC.direction;

                        SoundEngine.PlaySound(SoundID.NPCDeath51, NPC.position);

                        Vector2 ChargeDirection = player.Center - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X = ChargeDirection.X * 25;
                        ChargeDirection.Y = ChargeDirection.Y * 25;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] >= 350)
                    {
                        SaveDirection = 0;

                        NPC.velocity *= 0.97f;
                    }

                    if (NPC.localAI[0] >= 530)
                    {
                        rotate = 0;
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0] = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
		}

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

			//treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagSpookySpirit>()));
            
			//master relic and pet
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<SpookySpiritRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SpiritLamp>(), 4));

            //weapon drops
            int[] MainItem = new int[] 
            { 
                ModContent.ItemType<SpiritSword>(), 
                ModContent.ItemType<SpiritSlingshot>(), 
                ModContent.ItemType<SpiritHandStaff>(), 
                ModContent.ItemType<SpiritScroll>()
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

			//material
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SpookyPlasma>(), 1, 12, 18));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedSpookySpirit, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.LesserHealingPotion;
		}
    }
}