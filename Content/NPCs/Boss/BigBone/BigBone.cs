using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.BigBone.Projectiles;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    [AutoloadBossHead]
    public class BigBone : ModNPC
    {
        int[] AttackPattern = new int[] { 0, 1, 2, 3, 4 };

        public int ScaleTimerLimit = 12;
        public int SaveDirection;
        
        public float SaveRotation;
        public float ScaleAmount = 0f;
        public float RealScaleAmount = 0f;
        
        public bool Phase2 = false;
        public bool Transition = false;
        public bool FlowersSpawned = false;
        public bool ActuallyDead = false;
        public bool DeathAnimation = false;

        Vector2[] SavePoint = new Vector2[5];
        Vector2 SavePlayerPosition;
        Vector2 SaveNPCPosition;

        private static Asset<Texture2D> NeckTexture;
        private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> GlowTexture1;
        private static Asset<Texture2D> GlowTexture2;
        private static Asset<Texture2D> GlowTexture3;
        private static Asset<Texture2D> GlowTexture4;
        private static Asset<Texture2D> ShieldTexture;

        public static readonly SoundStyle GrowlSound1 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl1", SoundType.Sound);
        public static readonly SoundStyle GrowlSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl2", SoundType.Sound);
        public static readonly SoundStyle GrowlSound3 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl3", SoundType.Sound);
        public static readonly SoundStyle LaughSound = new("Spooky/Content/Sounds/BigBone/BigBoneLaugh", SoundType.Sound);
        public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBone/BigBoneMagic", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle MagicCastSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneMagic2", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/BigBone/BigBoneDeath", SoundType.Sound);
        public static readonly SoundStyle DeathSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneDeath2", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/BigBoneBestiary",
                Position = new Vector2(48f, -12f),
                PortraitPositionXOverride = 12f,
                PortraitPositionYOverride = -12f
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
            //attack pattern
            writer.Write(AttackPattern[0]);
            writer.Write(AttackPattern[1]);
            writer.Write(AttackPattern[2]);
            writer.Write(AttackPattern[3]);
            writer.Write(AttackPattern[4]);
            writer.Write(AttackPattern[5]);

            //vector2
            writer.WriteVector2(SavePlayerPosition);
            writer.WriteVector2(SaveNPCPosition);

            //ints
            writer.Write(ScaleTimerLimit);
            writer.Write(SaveDirection);

            //bools
            writer.Write(Phase2);
            writer.Write(Transition);
            writer.Write(FlowersSpawned);
            writer.Write(ActuallyDead);
            writer.Write(DeathAnimation);

            //floats
            writer.Write(SaveRotation);
            writer.Write(ScaleAmount);
            writer.Write(RealScaleAmount);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //attack pattern
            AttackPattern[0] = reader.ReadInt32();
            AttackPattern[1] = reader.ReadInt32();
            AttackPattern[2] = reader.ReadInt32();
            AttackPattern[3] = reader.ReadInt32();
            AttackPattern[4] = reader.ReadInt32();
            AttackPattern[5] = reader.ReadInt32();

            //vector2
            SavePlayerPosition = reader.ReadVector2();
            SaveNPCPosition = reader.ReadVector2();

            //ints
            ScaleTimerLimit = reader.ReadInt32();
            SaveDirection = reader.ReadInt32();

            //bools
            Phase2 = reader.ReadBoolean();
            Transition = reader.ReadBoolean();
            FlowersSpawned = reader.ReadBoolean();
            ActuallyDead = reader.ReadBoolean();
            DeathAnimation = reader.ReadBoolean();

            //floats
            SaveRotation = reader.ReadSingle();
            ScaleAmount = reader.ReadSingle();
            RealScaleAmount = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 72000;
            NPC.damage = 70;
            NPC.defense = 50;
            NPC.width = 134;
            NPC.height = 170;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 30, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = DeathSound2;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/BigBone");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BigBone"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public void DrawBody(bool SpawnGore)
		{
			NPC Parent = Main.npc[(int)NPC.ai[3]];

			if (Parent.active && Parent.type == ModContent.NPCType<BigFlowerPot>() && !SpawnGore)
			{
				NeckTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneStem");

				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, NeckTexture.Height() / 2);
				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 32;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = drawPosNext - drawPos2;
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

					Main.spriteBatch.Draw(NeckTexture.Value, drawPos2 - Main.screenPosition, null, NPC.GetAlpha(color), rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)NeckTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

			if (SpawnGore)
			{
				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 32;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/EarWormSegmentGore").Type);
					}
				}
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawBody(false);

            AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneAura");

            var effects = SpriteEffects.None;

            //while charging, use the saved direction and not the actual npc direction
            if (NPC.ai[0] == 7 && NPC.localAI[0] > 85 && NPC.localAI[0] <= 160)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            Vector2 frameOrigin = NPC.frame.Size() / 2f;
            Vector2 offset = new Vector2(NPC.width / 2 - frameOrigin.X, NPC.height - NPC.frame.Height);
            Vector2 drawPos = NPC.position - Main.screenPosition + frameOrigin + offset;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            float time = Main.GlobalTimeWrappedHourly;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            //draw aura during death animation
            if (DeathAnimation)
            {
                Color color1 = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.OrangeRed);
                Color color2 = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Orange);

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(AuraTexture.Value, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time, NPC.frame, color1, NPC.rotation, frameOrigin, NPC.scale + RealScaleAmount * 1.2f, effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(AuraTexture.Value, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time, NPC.frame, color2, NPC.rotation, frameOrigin, NPC.scale + RealScaleAmount * 0.9f, effects, 0);
                }
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow1");
            GlowTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow2");
            GlowTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow3");
            GlowTexture4 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow4");

            var effects = SpriteEffects.None;

            //while charging, use the saved direction and not the actual npc direction
            if (NPC.ai[0] == 7 && NPC.localAI[0] > 85 && NPC.localAI[0] <= 160)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Transparent, Color.White, fade);

            //glowmask
            Main.EntitySpriteDraw(GlowTexture3.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            //draw solar forcefield during his phase transition
            if (NPC.ai[0] == -1 && NPC.AnyNPCs(ModContent.NPCType<BigFlower>()))
            {
                ShieldTexture ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/BigBoneShield");

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var center = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                float intensity = fade;
                DrawData drawData = new DrawData(ShieldTexture.Value, center + new Vector2(0, 55), new Rectangle(0, 0, 500, 400), Color.Lerp(Color.OrangeRed, Color.Tomato, fade), 0, new Vector2(250f, 250f), NPC.scale * (1f + intensity * 0.05f), SpriteEffects.None, 0);
                GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + intensity * 0.5f));
                GameShaders.Misc["ForceField"].Apply(drawData);
                drawData.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                return;
            }
        }

        public override bool CheckDead()
        {
            //death animation stuff
            if (!ActuallyDead)
            {
                SoundEngine.PlaySound(DeathSound, NPC.Center);

                NPC.localAI[0] = 0;
                NPC.localAI[1] = 0;
                NPC.localAI[2] = 0;
                DeathAnimation = true;
                NPC.immortal = true;
                NPC.dontTakeDamage = true;
                NPC.life = 1;

                NPC.netUpdate = true;

                return false;
            }

            return true;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = NPC.direction;

            //EoC rotation
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //despawn if all players are dead
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome2>()))
            {
                GoAboveFlowerPot(100);
                NPC.EncourageDespawn(10);

                if (NPC.Hitbox.Intersects(Parent.Hitbox))
                {
                    NPC.active = false;
                }

                for (int k = 0; k < Main.projectile.Length; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].hostile) 
                    {
                        Main.projectile[k].Kill();
                    }
                }

                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    if (Main.npc[k].type == ModContent.NPCType<BigFlower>()) 
                    {
                        Main.npc[k].active = false;
                    }
                }

                return;
            }
            else
            {
                //set ai based on current state
                if (Transition)
                {
                    NPC.ai[0] = -1;
                }
                if (DeathAnimation)
                {
                    NPC.ai[0] = -2;
                }
                if (!Transition && !DeathAnimation)
                {
                    NPC.ai[0] = AttackPattern[(int)NPC.ai[1]];
                }
            }

            //immediately vanish if not attached to its flower pot
            if (Main.npc[(int)NPC.ai[3]].type != ModContent.NPCType<BigFlowerPot>())
            {
                NPC.active = false;
            }

            //make sure big bone cannot heal over his max life
            if (NPC.life > NPC.lifeMax)
            {
                NPC.life = NPC.lifeMax;
            }

            //reset and randomize attack pattern list
            if (NPC.ai[2] == 0)
            {
                //play laughing sound
                SoundEngine.PlaySound(LaughSound, NPC.Center);

                if (Phase2 && AttackPattern.Length < 6)
                {
                    AttackPattern = AttackPattern.Append(5).ToArray();
                }
                    
                //shuffle the attack pattern list
                AttackPattern = AttackPattern.OrderBy(x => Main.rand.Next()).ToArray();

                NPC.ai[2] = 1;
                NPC.netUpdate = true;
            }

            //transition to phase 2
            if (NPC.life < (NPC.lifeMax / 2) && !Phase2)
            {
                Transition = true;
            }

            switch ((int)NPC.ai[0])
            {
                //death animation
                case -2:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(350);

                    Screenshake.ShakeScreenWithIntensity(NPC.Center, NPC.localAI[0] / 10, 300f);

                    //kill every single hostile projectile to prevent unfair hits or deaths during the death animation
                    if (NPC.localAI[0] <= 5)
                    {
                        for (int k = 0; k < Main.projectile.Length; k++)
                        {
                            if (Main.projectile[k].active && Main.projectile[k].hostile) 
                            {
                                Main.projectile[k].Kill();
                            }
                        }
                    }
                    
                    //save where big bone is
                    if (NPC.localAI[0] == 60)
                    {
                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

                    //shake, and cause a circle of dusts around big bone that becomes bigger over time
                    if (NPC.localAI[0] >= 60)
                    {
                        int Shake = (int)NPC.localAI[0] / 10;

                        NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-Shake, Shake);

                        int MaxDusts = Main.rand.Next(10, 15);
                        float distance = 1.5f + (NPC.localAI[0] / 85);

                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2(NPC.width / 3f, NPC.height / 3f) * distance).RotatedBy((double)((numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / MaxDusts), default) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;

                            if (Main.rand.NextBool(2))
                            {
                                int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.05f + (NPC.localAI[0] / 450));
                                Main.dust[dustEffect].color = Color.Orange;
                                Main.dust[dustEffect].noGravity = true;
                                Main.dust[dustEffect].noLight = false;
                                Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * (-18f + -NPC.localAI[0] / 10);
                                Main.dust[dustEffect].fadeIn = 1.2f;
                            }
                        }
                    }

                    //increase the scaling amounts
                    if (NPC.localAI[0] == 60 || NPC.localAI[0] == 120 || NPC.localAI[0] == 180 || NPC.localAI[0] == 240)
                    {
                        ScaleAmount += 0.02f;
                        ScaleTimerLimit -= 2;
                    }

                    NPC.localAI[1]++;

                    //more scaling stuff
                    if (NPC.localAI[1] < ScaleTimerLimit)
                    {
                        RealScaleAmount += ScaleAmount;
                    }
                    if (NPC.localAI[1] >= ScaleTimerLimit)
                    {
                        RealScaleAmount -= ScaleAmount / 2;
                    }
                    if (NPC.localAI[1] > ScaleTimerLimit * 2)
                    {
                        NPC.localAI[1] = 0;
                    }

                    //kill big bone and spawn gores
                    if (NPC.localAI[0] >= 360)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);

                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 3f, 300f);

                        //spawn gores
                        for (int numGores = 1; numGores <= 7; numGores++)
                        {
                            if (Main.netMode != NetmodeID.Server) 
                            {
                                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-5, -2)), ModContent.Find<ModGore>("Spooky/BigBoneGore" + numGores).Type);
                            }
                        }

                        //flame dusts
                        for (int numDust = 0; numDust < 50; numDust++)
                        {                                                                                  
                            int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default, Main.rand.NextFloat(3f, 5f));
                            Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-20f, 20f);
                            Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-10f, 10f);
                            Main.dust[dustGore].noGravity = true;
                        }

                        //explosion smoke
                        for (int numExplosion = 0; numExplosion < 25; numExplosion++)
                        {
                            int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 
                            0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));
                            Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                            Main.dust[DustGore].noGravity = true;
                        }

                        //kill big bone
                        ActuallyDead = true;
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                        player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                    }

                    break;
                }
                
                //phase 2 transition
                case -1:
                {
                    GoAboveFlowerPot(350);

                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;

                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.ai[1] = 0;

                    NPC.localAI[2]++;

                    //spawn solar flowers
                    if (NPC.localAI[2] >= 60 && !FlowersSpawned)
                    {
                        int maxFlowers = 5;
                        for (int numFlowers = 0; numFlowers < maxFlowers; numFlowers++)
                        {
                            Vector2 flowerPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * 5f).RotatedBy((double)((float)(numFlowers - (maxFlowers / 2 - 1)) * 6.28318548f / (float)maxFlowers), default(Vector2)) + NPC.Center;

                            int solarFlower = NPC.NewNPC(NPC.GetSource_FromAI(), (int)flowerPos.X, (int)flowerPos.Y, ModContent.NPCType<BigFlower>(), ai0: NPC.whoAmI);

                            if (Main.netMode != NetmodeID.SinglePlayer)
                            {
                                NetMessage.SendData(MessageID.SyncNPC, number: solarFlower);
                            }
                        }

                        FlowersSpawned = true;
                    }

                    if (NPC.localAI[2] >= 180 && NPC.localAI[2] < 200)
                    {
                        SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100)), 
                        new Vector2(Main.rand.Next(-2, 2), Main.rand.Next(-2, 2)), ModContent.ProjectileType<FlowerSpore>(), NPC.damage - 10, 4.5f);
                    }

                    if (NPC.localAI[2] >= 560)
                    {
                        //if theres 3 or less flowers left, shoot a homing fire ball at the end of the attack
                        if (NPC.CountNPCS(ModContent.NPCType<BigFlower>()) <= 3)
			            {
                            SoundEngine.PlaySound(MagicCastSound2, NPC.Center);

                            //recoil
                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize();
                                    
                            Recoil.X *= -20;
                            Recoil.Y *= -20;
                            NPC.velocity.X = Recoil.X;
                            NPC.velocity.Y = Recoil.Y;

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 5;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<MassiveFlameBall>(), NPC.damage + 20, 4.5f);
                        }

                        NPC.localAI[2] = 0;
                        NPC.netUpdate = true;
                    }

                    if (NPC.CountNPCS(ModContent.NPCType<BigFlower>()) <= 0 && FlowersSpawned)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;

                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;
                        Phase2 = true;
                        Transition = false;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //shoot bouncing flowers
                case 0:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(150);

                    if (NPC.localAI[0] == 100)
                    {
                        int Type = Phase2 ? 1 : 0;

                        for (int numProjectiles = -6; numProjectiles <= 6; numProjectiles++)
                        {
                            Vector2 ParentTopPos = new Vector2(Parent.Top.X + Main.rand.Next(-(Parent.width / 2) + 15, (Parent.width / 2) - 15), Parent.Top.Y + 2);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, ParentTopPos, 10f * Parent.DirectionTo(new Vector2(Parent.Center.X, Parent.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(12) * numProjectiles),
                            ModContent.ProjectileType<VineBase>(), NPC.damage, 4.5f, ai2: Type);
                        }
                    }

                    if (NPC.localAI[0] >= 490)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                        }

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //skull wisp attack
                case 1:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(500);

                    if (NPC.localAI[0] >= 75 && NPC.localAI[0] <= 195)
                    {
                        int WispChance = Phase2 ? 3 : 5;

                        if (Main.rand.NextBool(WispChance))
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                                    
                            ShootSpeed.X *= Main.rand.Next(-12, 12);
                            ShootSpeed.Y *= Main.rand.Next(-12, 12);

                            int ProjType = Phase2 ? ModContent.ProjectileType<FlamingWisp>() : ModContent.ProjectileType<BoneWisp>();

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100)), 
                            ShootSpeed, ProjType, NPC.damage, 4.5f, ai1: NPC.whoAmI);
                        }
                    }

                    if (NPC.localAI[0] >= 240)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                        }

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
                        NPC.localAI[0] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //thorn attacks
                case 2:
                {
                    NPC.localAI[0]++;

                    //in phase 1, spawn multiple telegraphs around the player and shoot thorns at each telegraph
                    if (!Phase2)
                    {
                        GoAboveFlowerPot(100);

                        if (NPC.localAI[1] < 4)
                        {
                            if (NPC.localAI[0] == 75)
                            {
                                for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                                {
                                    float distance = Main.rand.NextFloat(8f, 12f);

                                    Vector2 Position = (Vector2.One * new Vector2((float)player.width / 3f, (float)player.height / 3f) * distance).RotatedBy((double)((float)(numProjectiles - (5 / 2 - 1)) * 6f / 5f), default(Vector2)) + player.Center;

                                    NPCGlobalHelper.ShootHostileProjectile(NPC, Position, Vector2.Zero, ModContent.ProjectileType<BigBoneThornTelegraph>(), 0, 0f);

                                    SavePoint[numProjectiles] = new Vector2(Position.X, Position.Y);
                                }

                                for (int numPoints = 0; numPoints < SavePoint.Length; numPoints++)
                                {
                                    Vector2 Direction = NPC.Center - SavePoint[numPoints];
                                    Direction.Normalize();

                                    Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

                                    Vector2 ShootSpeed = player.Center - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed *= 15;

                                    Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                                    Vector2 position = NPC.Center;

                                    if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                                    {
                                        position += Offset;
                                    }
                                
                                    NPCGlobalHelper.ShootHostileProjectile(NPC, position, Vector2.Zero, ModContent.ProjectileType<BigBoneThorn>(), NPC.damage + 20, 4.5f, ai0: lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                                }
                            }

                            if (NPC.localAI[0] == 120)
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

                            if (NPC.localAI[0] >= 125)
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
                            if (NPC.localAI[0] >= 75)
                            {
                                if (Main.rand.NextBool(3))
                                {
                                    SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                                }

                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.localAI[2] = 0;
                                NPC.ai[1]++;

                                if (NPC.ai[1] >= AttackPattern.Length)
                                {
                                    NPC.localAI[0] = 0;
                                    NPC.localAI[1] = 0;
                                    NPC.localAI[2] = 0;
                                    NPC.ai[1] = 0;
                                    NPC.ai[2] = 0;
                                }

                                NPC.netUpdate = true;
                            }
                        }
                    }

                    //in phase 2, make a circle of thorns with a random gap that the player has to get into before they are launched out
                    if (Phase2)
                    {
                        GoAboveFlowerPot(350);

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(MagicCastSound, NPC.Center);

                            //set this to a random number without going too low or too high
                            NPC.localAI[1] = Main.rand.Next(4, 44);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                for (float numProjectiles = 0; numProjectiles < 45; numProjectiles++)
                                {
                                    //this is how the gap in the circle of thorns is created
                                    //this check basically makes it so every thorn in the loop is spawned, except for the randomly chosen number and multiple thorns behind the chosen number
                                    if (numProjectiles != NPC.localAI[1] && numProjectiles != NPC.localAI[1] - 1 && numProjectiles != NPC.localAI[1] - 2 && 
                                    numProjectiles != NPC.localAI[1] - 3 && numProjectiles != NPC.localAI[1] - 4)
                                    {
                                        Vector2 projPos = NPC.Center + new Vector2(0, 2).RotatedBy(numProjectiles * (Math.PI * 2f / 45));

                                        Vector2 Direction = NPC.Center - projPos;
                                        Direction.Normalize();

                                        Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

                                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<SolarThorn>(), NPC.damage + 20, 4.5f, ai0: lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                                    }
                                }
                            }
                        }

                        if (NPC.localAI[0] >= 310)
                        {
                            if (Main.rand.NextBool(3))
                            {
                                SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                            }

                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[1]++;

                            if (NPC.ai[1] >= AttackPattern.Length)
                            {
                                NPC.localAI[0] = 0;
                                NPC.localAI[1] = 0;
                                NPC.localAI[2] = 0;
                                NPC.ai[1] = 0;
                                NPC.ai[2] = 0;
                            }

                            NPC.netUpdate = true;
                        }
                    }
                    
                    break;
                }

                //grow pitcher plants and spit lingering poison
                case 3:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(150);

                    int ShootFrequency = Phase2 ? 7 : 15;

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 180 && NPC.localAI[0] % ShootFrequency == 0)
                    {
                        int VelocityX = Main.rand.Next(-12, 13);
                        int VelocityY = Main.rand.Next(-15, -10);

                        Vector2 RandomPosition = new Vector2(Parent.Top.X + Main.rand.Next(-(Parent.width / 2) + 15, (Parent.width / 2) - 15), Parent.Top.Y + 2);
                        NPCGlobalHelper.ShootHostileProjectile(NPC, RandomPosition, new Vector2(VelocityX, VelocityY), ModContent.ProjectileType<VineBase>(), NPC.damage, 4.5f, ai2: 2);
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                        }

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //homing fire ball attack
                case 4:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(400);

                    //in phase 1, shoot 3 fast fire balls
                    if (!Phase2)
                    {
                        if (NPC.localAI[0] == 75 || NPC.localAI[0] == 105 || NPC.localAI[0] == 135)
                        {
                            SoundEngine.PlaySound(MagicCastSound2, NPC.Center);

                            //recoil
                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize();
                                    
                            Recoil.X *= -15;
                            Recoil.Y *= -15;
                            NPC.velocity.X = Recoil.X;
                            NPC.velocity.Y = Recoil.Y;

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 1.5f;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<GiantFlameBall>(), NPC.damage + 30, 4.5f);
                        }
                    }

                    //in phase 2, shoot one fireball that chases for longer and explodes into other fire bolts
                    if (Phase2)
                    {
                        if (NPC.localAI[0] == 75)
                        {
                            SoundEngine.PlaySound(MagicCastSound2, NPC.Center);

                            //recoil
                            Vector2 Recoil = player.Center - NPC.Center;
                            Recoil.Normalize();
                            Recoil *= -20;
                            NPC.velocity = Recoil;

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 4.5f;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<MassiveFlameBall>(), NPC.damage + 30, 4.5f);
                        }
                    }

                    if (NPC.localAI[0] >= 175)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                        }

                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.localAI[2] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //stay still, then charge and crash into a wall
                case 5:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1)
                    {
                        SoundEngine.PlaySound(GrowlSound2, NPC.Center);
                    }

                    if (NPC.localAI[0] <= 85)
                    {
                        GoAboveFlowerPot(300);

                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                            Main.dust[dustEffect].color = Color.Yellow;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-18f, -5f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] == 85)
                    {
                        SoundEngine.PlaySound(GrowlSound3, NPC.Center);

                        //predict the players movement in phase 2
                        SavePlayerPosition = Phase2 ? player.Center + player.velocity * 40f : player.Center;

                        //in phase 2, modify big bone's rotation and direction right before saving it so it is correct with the player movement prediction
                        if (Phase2)
                        {
                            //set big bone's rotation to the predicted spot so it is correct when charging
                            Vector2 newVector = new Vector2(NPC.Center.X, NPC.Center.Y);
                            float newRotateX = player.Center.X + player.velocity.X * 40f - newVector.X;
                            float newRotateY = player.Center.Y + player.velocity.Y * 40f - newVector.Y;
                            NPC.rotation = (float)Math.Atan2((double)newRotateY, (double)newRotateX) + 4.71f;

                            //set big bone's direction based on the saved location so his sprite doesnt flip backwards
                            NPC.direction = SavePlayerPosition.X > NPC.Center.X ? 1 : -1;
                        }

                        SaveRotation = NPC.rotation;
                        SaveDirection = NPC.direction;

                        Vector2 Recoil = SavePlayerPosition - NPC.Center;
                        Recoil.Normalize();
                                
                        Recoil *= -5; 
                        NPC.velocity = Recoil;

                        NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] > 85 && NPC.localAI[0] <= 160)
                    {
                        NPC.spriteDirection = SaveDirection;
                        NPC.rotation = SaveRotation;
                    }

                    if (NPC.localAI[0] == 95)
                    {
                        NPC.velocity *= 0.5f;
                    }

                    //charge to the destination
                    if (NPC.localAI[0] == 105)
                    {
                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                        ChargeDirection *= 50;
                        NPC.velocity = ChargeDirection;
                    }

                    //stop and play slamming sound when big bone impacts a solid surface
                    if (NPC.localAI[0] > 105 && NPC.localAI[1] == 0)
                    {
                        if (NPC.velocity.X <= 0.1f && NPC.velocity.X >= -0.1f)
                        {
                            NPC.velocity = Vector2.Zero;
                        }

                        if (NPC.velocity.Y <= 0.1f && NPC.velocity.Y >= -0.1f)
                        {
                            NPC.velocity = Vector2.Zero;
                        }

                        if (NPC.velocity == Vector2.Zero)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath43, NPC.Center);
                            
                            Screenshake.ShakeScreenWithIntensity(NPC.Center, 25f, 1000f);

                            NPC.velocity = Vector2.Zero;

                            NPC.localAI[1] = 1;
                        }
                    }

                    //go back above the flower pot before switching attacks
                    if (NPC.localAI[0] >= 160 && NPC.localAI[1] == 1)
                    {
                        GoAboveFlowerPot(300);
                        NPC.velocity *= 0.98f;
                    }

                    if (NPC.localAI[0] >= 260)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[1]++;

                        if (NPC.ai[1] >= AttackPattern.Length)
                        {
                            NPC.localAI[0] = 0;
                            NPC.localAI[1] = 0;
                            NPC.ai[1] = 0;
                            NPC.ai[2] = 0;
                        }

                        NPC.netUpdate = true;
                    }

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

        //Loot and stuff
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            LeadingConditionRule notExpertRule = new(new Conditions.NotExpert());

            //treasure bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagBigBone>()));
            
            //relic and master pet
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BigBoneRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SkullSeed>(), 4));

            //weapon drops
            int[] MainItem = new int[] 
            { 
                ModContent.ItemType<BigBoneHammer>(), 
                ModContent.ItemType<BigBoneBow>(), 
                ModContent.ItemType<BigBoneStaff>(), 
                ModContent.ItemType<BigBoneScepter>() 
            };

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<BigBoneMask>(), 7));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FlowerPotHead>(), 20));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BigBoneTrophyItem>(), 10));

            //sunflower bloom seed, drop directly from the boss
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SummerSeed>()));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            for (int numPlayer = 0; numPlayer <= Main.maxPlayers; numPlayer++)
            {
                if (Main.player[numPlayer].active && !Main.player[numPlayer].GetModPlayer<BloomBuffsPlayer>().UnlockedSlot4)
                {
                    int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<Slot4Unlocker>());

                    if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                    }
                }
            }

            if (!Flags.downedBigBone)
            {
                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.BigBoneDefeat");

                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(text, 171, 64, 255);
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                }

                Flags.GuaranteedRaveyard = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }

            NPC.SetEventFlagCleared(ref Flags.downedBigBone, -1);

            if (!MenuSaveSystem.hasDefeatedBigBone)
            {
                MenuSaveSystem.hasDefeatedBigBone = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<CranberryJuice>();
		}
    }
}