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
        int[] AttackPattern = new int[] { 0, 1, 2, 3, 4, 5 };

        public int ScaleTimerLimit = 12;
        public int SaveDirection;
        
        public float SaveRotation;
        public float ScaleAmount = 0f;
        public float RealScaleAmount = 0f;
        public float HeatMaskAlpha = 0f;
        
        public bool Phase2 = false;
        public bool Transition = false;
        public bool FlowersSpawned = false;
        public bool ActuallyDead = false;
        public bool DeathAnimation = false;
        public bool DefaultRotation = true;

        Vector2[] SavePoint = new Vector2[8];
        Vector2 SavePlayerPosition;
        Vector2 SaveNPCPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> NeckTexture;
        private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> EyeGlowTexture;
        private static Asset<Texture2D> HeatGlowTexture;

        public static readonly SoundStyle GrowlSound1 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl1", SoundType.Sound) { Pitch = 1.35f, PitchVariance = 0.5f };
        public static readonly SoundStyle GrowlSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl2", SoundType.Sound) { Pitch = 1.35f, PitchVariance = 0.5f };
        public static readonly SoundStyle GrowlSound3 = new("Spooky/Content/Sounds/BigBone/BigBoneGrowl3", SoundType.Sound) { Pitch = 1f, PitchVariance = 0.5f };
        public static readonly SoundStyle LaughSound = new("Spooky/Content/Sounds/BigBone/BigBoneLaugh", SoundType.Sound) { Pitch = 0.4f, PitchVariance = 0.5f };
        public static readonly SoundStyle SteamSound = new("Spooky/Content/Sounds/BigBone/BigBoneHeat", SoundType.Sound);
        public static readonly SoundStyle StunnedSound = new("Spooky/Content/Sounds/GoofyStunned", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/BigBoneBestiary",
                Position = new Vector2(38f, -12f),
                PortraitPositionXOverride = 12f,
                PortraitPositionYOverride = -2f
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
            for (int i = 0; i < AttackPattern.Length; i++)
            {
                writer.Write(AttackPattern[i]);
            }

            //vector2
            for (int i = 0; i < SavePoint.Length; i++)
            {
                writer.WriteVector2(SavePoint[i]);
            }
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
            writer.Write(DefaultRotation);

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
            for (int i = 0; i < AttackPattern.Length; i++)
            {
                AttackPattern[i] = reader.ReadInt32();
            }

            //vector2
            for (int i = 0; i < SavePoint.Length; i++)
            {
                SavePoint[i] = reader.ReadVector2();
            }
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
            DefaultRotation = reader.ReadBoolean();

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
            NPC.lifeMax = 75000;
            NPC.damage = 65;
            NPC.defense = 50;
            NPC.width = 130;
            NPC.height = 130;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 30, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath55 with { Pitch = 1.25f };
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
        
        public void DrawBody(Color drawColor, bool SpawnGore)
		{
			NPC Parent = Main.npc[(int)NPC.ai[3]];

			if (Parent.active && Parent.type == ModContent.NPCType<BigFlowerPot>() && !SpawnGore)
			{
				NeckTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneNeck");

				bool flip = false;
                //if big bones saved direction is currently saving a direction, then use that for sprite effects
                if (SaveDirection != 0)
                {
                    flip = SaveDirection == -1;
                }
                else
                {
                    flip = NPC.direction == -1;
                }

				Vector2 drawOrigin = new Vector2(0, NeckTexture.Height() / 6);
				Vector2 myCenter = NPC.Center - new Vector2(45 * (flip ? -1 : 1), 5).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(0 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 36;

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

                    Rectangle Frame = new Rectangle(0, 0, 40, 46);

                    if (i % 3 == 0)
                    {
                        Frame = new Rectangle(0, NeckTexture.Height() / 3 * 0, 40, 46);
                    }
                    else if (i % 3 == 1)
                    {
                        Frame = new Rectangle(0, NeckTexture.Height() / 3 * 1, 40, 46);
                    }
                    else
                    {
                        Frame = new Rectangle(0, NeckTexture.Height() / 3 * 2, 40, 46);
                    }
                    
                    float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;
                    //Color StemDrawColor = Phase2 ? Color.Lerp(drawColor, Color.Firebrick, fade) : drawColor;

					Main.EntitySpriteDraw(NeckTexture.Value, drawPos2 - Main.screenPosition, Frame, NPC.GetAlpha(drawColor), rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);

                    if (NPC.ai[0] == -2 && NPC.localAI[0] >= 360)
                    {
                        Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, Frame, Color.Black * 0.9f, rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
                    }
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

                    if (i % 3 == 0)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneNeckGore1").Type);
                        }
                    }
                    else if (i % 3 == 1)
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneNeckGore2").Type);
                        }
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/BigBoneNeckGore3").Type);
                        }
                    }
				}
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            DrawBody(drawColor, false);

            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneAura");

            var effects = SpriteEffects.None;

            //if big bones saved direction is currently saving a direction, then use that for sprite effects
            if (SaveDirection != 0)
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
            if (NPC.ai[0] == -2 && NPC.localAI[0] < 360)
            {
                Color color1 = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.OrangeRed);
                Color color2 = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Orange);

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0f, 25f).RotatedBy(radians) * time, 
                    NPC.frame, color1, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale + RealScaleAmount * 1.2f, effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0f, 25f).RotatedBy(radians) * time, 
                    NPC.frame, color2, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale + RealScaleAmount * 0.9f, effects, 0);
				}
			}

			Color HeadDrawColor = Phase2 ? Color.Lerp(drawColor, Color.Firebrick, fade) : drawColor;

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, HeadDrawColor, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

			if (NPC.ai[0] == -2 && NPC.localAI[0] >= 360)
            {
                Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.Black * 0.9f, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

			return false;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow");
            EyeGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneEyesGlow");
            HeatGlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneHeatGlow");

            var effects = SpriteEffects.None;

            //if big bones saved direction is currently saving a direction, then use that for sprite effects
            if (SaveDirection != 0)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            Color color = NPC.ai[0] == -2 && NPC.localAI[0] >= 360 ? Color.White : Color.Lerp(Color.Transparent, Color.White, fade);

            //glowmask
            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(EyeGlowTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            //heat mask when big bone uses fire-related attacks
            if (HeatMaskAlpha > 0)
            {
                Color OuterColor = Phase2 ? Color.OrangeRed : new Color(255, 218, 75);

                Main.EntitySpriteDraw(HeatGlowTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, OuterColor * HeatMaskAlpha, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                Main.EntitySpriteDraw(HeatGlowTexture.Value, NPC.Center - Main.screenPosition, NPC.frame, Color.White * HeatMaskAlpha, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale * 0.82f, effects, 0);
            }
        }

        public override void FindFrame(int frameHeight)
		{
            //determine frame based on direction
            if (SaveDirection != 0)
            {
                if (SaveDirection == -1)
                {
                    NPC.frame.Y = frameHeight * 0;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 1;
                }
            }
            else
            {
                if (NPC.direction == -1)
                {
                    NPC.frame.Y = frameHeight * 0;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 1;
                }
            }
        }

        public override bool CheckDead()
        {
            //death animation stuff
            if (!ActuallyDead)
            {
                //SoundEngine.PlaySound(SoundID.NPCDeath2 with { Pitch = -1f }, NPC.Center);

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

            //rotation stuff
            if (DefaultRotation)
            {
                Vector2 RotateTowards = player.Center - NPC.Center;

                float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
                float RotateSpeed = 0.05f;

                NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);
            }

            if (!Parent.active || Parent.type != ModContent.NPCType<BigFlowerPot>())
            {
                NPC.active = false;
            }

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

            if (Phase2)
            {
                if (Main.rand.NextBool(10))
                {
                    Color[] colors = new Color[] { Color.Gray, Color.DarkGray };

                    int DustEffect = Dust.NewDust(NPC.position, NPC.width, 3, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Main.rand.Next(colors) * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
                    Main.dust[DustEffect].velocity.X = 0;
                    Main.dust[DustEffect].velocity.Y = -2;
                    Main.dust[DustEffect].alpha = 100;
                }
            }

            switch ((int)NPC.ai[0])
            {
                //death animation
                case -2:
                {
                    HeatMaskAlpha = 0;
					SaveDirection = 0;
					SaveRotation = 0;
					DefaultRotation = true;

                    NPC.localAI[0]++;

                    GoAboveFlowerPot(350);

                    if (NPC.localAI[0] < 360)
                    {
                        Screenshake.ShakeScreenWithIntensity(NPC.Center, NPC.localAI[0] / 10, 300f);

                        if (HeatMaskAlpha < 1)
                        {
                            HeatMaskAlpha = NPC.localAI[0] / 360;
                        }

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

                            for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                            {
                                float distance = Main.rand.NextFloat(2f, 5f) + (NPC.localAI[0] / 85);

                                Vector2 dustPos = (Vector2.One * new Vector2(NPC.width / 3f, NPC.height / 3f) * distance).RotatedBy((double)((numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / MaxDusts), default) + NPC.Center;
                                Vector2 velocity = (dustPos - NPC.Center) * 0.5f;

                                if (Main.rand.NextBool())
                                {
                                    int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.InfernoFork, velocity.X * 2f, velocity.Y * 2f, 100, default, 1f + (NPC.localAI[0] / 100));
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
                    }

                    if (NPC.localAI[0] == 260)
                    {
                        SoundEngine.PlaySound(SteamSound, NPC.Center);
                    }

                    //explode
                    if (NPC.localAI[0] == 360)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);
                        SoundEngine.PlaySound(SoundID.NPCDeath2 with { Pitch = -1f }, NPC.Center);

                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 3f, 300f);

                        HeatMaskAlpha = 0;

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
                            int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));
                            Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                            Main.dust[DustGore].noGravity = true;
                        }
                    }

                    //kill big bone
                    if (NPC.localAI[0] >= 480)
                    {
                        for (int numDusts = 0; numDusts < 55; numDusts++)
                        {
                            int CrumbleDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Asphalt, 0f, 0f, 100, default, 2.5f);
                            Main.dust[CrumbleDust].velocity *= 1.2f;
                        }

                        //kill big bone
                        ActuallyDead = true;
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                        DrawBody(Color.Black, true);
                        player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                    }

                    break;
                }
                
                //phase 2 transition
                case -1:
                {
                    GoAboveFlowerPot(350);

                    SaveDirection = 0;
					SaveRotation = 0;
					DefaultRotation = true;

                    NPC.immortal = true;
                    NPC.dontTakeDamage = true;

                    NPC.localAI[0] = 0;
                    NPC.localAI[1] = 0;
                    NPC.ai[1] = 0;

                    NPC.localAI[2]++;

                    if (NPC.localAI[2] == 2)
                    {
                        SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                        
                        HeatMaskAlpha = 0;
                    }

					//kill every single hostile projectile to prevent unfair hits or deaths during the death animation
                    if (NPC.localAI[2] <= 5)
                    {
                        for (int k = 0; k < Main.projectile.Length; k++)
                        {
                            if (Main.projectile[k].active && Main.projectile[k].hostile) 
                            {
                                Main.projectile[k].Kill();
                            }
                        }
                    }

                    //save positions
                    if (NPC.localAI[2] == 60)
                    {
                        SaveNPCPosition = NPC.Center;

                        NPC.netUpdate = true;
                    }

					//shake, and cause a circle of dusts around big bone that becomes bigger over time
                    if (NPC.localAI[2] > 60 && NPC.localAI[2] < 290)
                    {
                        int Shake = (int)NPC.localAI[2] / 10;

                        NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                        NPC.Center += Main.rand.NextVector2Square(-Shake, Shake);

                        int MaxDusts = Main.rand.Next(10, 15);

                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            float distance = Main.rand.NextFloat(2f, 5f);

                            Vector2 dustPos = (Vector2.One * new Vector2(NPC.width / 3f, NPC.height / 3f) * distance).RotatedBy((double)((numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / MaxDusts), default) + NPC.Center;
                            Vector2 velocity = (dustPos - NPC.Center) * 0.5f;

                            if (Main.rand.NextBool())
                            {
                                int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, DustID.InfernoFork, velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                                Main.dust[dustEffect].color = Color.Orange;
                                Main.dust[dustEffect].noGravity = true;
                                Main.dust[dustEffect].noLight = false;
                                Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * -18f;
                                Main.dust[dustEffect].fadeIn = 1.2f;
                            }
                        }
                    }

					if (NPC.localAI[2] == 195)
					{
						SoundEngine.PlaySound(SteamSound, NPC.Center);
					}

                    if (NPC.localAI[2] > 195 && NPC.localAI[2] < 290)
                    {
                        if (HeatMaskAlpha < 1)
                        {
                            HeatMaskAlpha += 0.015f;
                        }
                    }

                    if (NPC.localAI[2] == 290)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);

                        HeatMaskAlpha = 0;

                        //flame dusts
                        for (int numDusts = 0; numDusts < 50; numDusts++)
                        {                                                                                  
                            int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default, Main.rand.NextFloat(3f, 6f));
                            Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-8f, 8f) * 2;
                            Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-8f, 8f);
                            Main.dust[dustGore].noGravity = true;
                        }

                        //explosion smoke
                        for (int numExplosion = 0; numExplosion < 25; numExplosion++)
                        {
                            int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, 
                            ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));

                            Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
                            Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                            Main.dust[DustGore].noGravity = true;

                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[DustGore].scale = 0.5f;
                                Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        //reset all variables
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.localAI[2] = 0;
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

                    GoAboveFlowerPot(250);

                    if (NPC.localAI[0] == 100)
                    {
                        int Type = Phase2 ? ModContent.ProjectileType<HomingFlowerVine>() : ModContent.ProjectileType<BouncingFlowerVine>();

                        for (int numProjectiles = -6; numProjectiles <= 6; numProjectiles++)
                        {
                            Vector2 ParentTopPos = new Vector2(Parent.Top.X + Main.rand.Next(-(Parent.width / 2) + 25, (Parent.width / 2) - 25), Parent.Top.Y + 12);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, ParentTopPos, 10f * Parent.DirectionTo(ParentTopPos).RotatedBy(MathHelper.ToRadians(12) * numProjectiles), Type, NPC.damage, 4.5f);
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

                //shoot out seeking thorns at the player
                case 1:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(420);

					int repeats = Phase2 ? 8 : 5;

					if (NPC.localAI[1] <= repeats)
					{
						if (NPC.localAI[0] == 60)
						{
							SoundEngine.PlaySound(GrowlSound2, NPC.Center);

							DefaultRotation = false;

							SaveNPCPosition = NPC.Center;

							SavePlayerPosition = player.Center;
                            SaveDirection = NPC.direction;
						}

						if (NPC.localAI[0] > 60 && NPC.localAI[0] < 90)
						{
							NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
							NPC.Center += Main.rand.NextVector2Square(-6, 6);
						}

						if (NPC.localAI[0] > 60 && NPC.localAI[0] < 120)
						{
							Vector2 RotateTowards = SavePlayerPosition - NPC.Center;

							float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
							float RotateSpeed = 0.05f;

							NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);
							NPC.spriteDirection = SaveDirection;
						}

						if (NPC.localAI[0] == 90)
						{
							SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

							NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<ThornVine>(), NPC.damage, 4.5f);
						}

						if (NPC.localAI[0] >= 120)
						{
							NPC.localAI[0] = 59;
							NPC.localAI[1]++;

							SaveDirection = 0;

							DefaultRotation = true;

							NPC.netUpdate = true;
						}
					}
					else
					{
						SaveDirection = 0;
						DefaultRotation = true;

						if (NPC.localAI[0] >= 190)
						{
							for (int k = 0; k < Main.projectile.Length; k++)
							{
								if (Main.projectile[k].active && Main.projectile[k].type == ModContent.ProjectileType<ThornVine>()) 
								{
									Main.projectile[k].Kill();
								}
							}

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

                //flame attacks
                case 2:
                {
                    NPC.localAI[0]++;

                    //in phase 1, shoot multiple randomized flamethrowers
                    if (!Phase2)
                    {
                        GoAboveFlowerPot(450);

                        if (NPC.localAI[0] == 60)
                        {
                            SaveNPCPosition = NPC.Center;
                            SaveDirection = NPC.spriteDirection;
                            SaveRotation = NPC.rotation;
                        }

                        if (NPC.localAI[0] > 60)
                        {
                            NPC.spriteDirection = SaveDirection;
                            NPC.rotation = SaveRotation;
                        }

                        //save positions
                        if (NPC.localAI[0] == 75)
                        {
                            SoundEngine.PlaySound(SteamSound, NPC.Center);

                            for (int numProjectiles = 0; numProjectiles < SavePoint.Length; numProjectiles++)
                            {
								SavePoint[numProjectiles] = NPC.Center + new Vector2(650, 0).RotatedByRandom(360);
                            }

                            NPC.netUpdate = true;
                        }

                        if (NPC.localAI[0] > 75 && NPC.localAI[0] < 155)
                        {
                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-6, 6);

                            if (HeatMaskAlpha < 1)
                            {
                                HeatMaskAlpha += 0.015f;
                            }

                            for (int numPoints = 0; numPoints < SavePoint.Length; numPoints++)
                            {
                                if (Main.rand.NextBool(5))
                                {
                                    Vector2 ShootSpeed = SavePoint[numPoints] - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed *= 6;

                                    Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                                    Vector2 position = NPC.Center;

                                    if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                                    {
                                        position += Offset;
                                    }

                                    Color[] colors = new Color[] { Color.Gray, Color.DarkGray };

                                    Dust DustEffect = Dust.NewDustPerfect(position, ModContent.DustType<BigBoneSmokeDust>());
                                    DustEffect.velocity = ShootSpeed;
                                    DustEffect.color = Main.rand.Next(colors);
                                    DustEffect.scale = Main.rand.NextFloat(0.5f, 0.75f);
                                    DustEffect.alpha = 100;
                                }
                            }
                        }

                        if (NPC.localAI[0] == 155)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                        }

						if (NPC.localAI[0] >= 155 && NPC.localAI[0] < 250)
                        {
                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-13, 13);

							if (NPC.localAI[0] % 10 == 0)
							{
								SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
							}
                                
							if (NPC.localAI[0] % 3 == 0)
							{
                                for (int numPoints = 0; numPoints < SavePoint.Length; numPoints++)
                                {
                                    Vector2 ShootSpeed = SavePoint[numPoints] - NPC.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed *= 10f;

                                    Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                                    Vector2 position = NPC.Center;

                                    if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
                                    {
                                        position += Offset;
                                    }
                            
                                    NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<BigBoneFlame>(), NPC.damage + 30, 4.5f);
                                }
                            }
						}

                        if (NPC.localAI[0] >= 250)
                        {
                            if (HeatMaskAlpha > 0)
                            {
                                HeatMaskAlpha -= 0.025f;
                            }
                        }
                        
                        if (NPC.localAI[0] >= 300)
                        {
                            if (Main.rand.NextBool(3))
                            {
                                SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                            }

							SaveDirection = 0;

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

                    //in phase 2, make a circle of flames with a random gap you have to go to
                    if (Phase2)
                    {
                        GoAboveFlowerPot(450);

                        int ProjAmount = 35;

						if (NPC.localAI[0] == 60)
						{
							//set this to a random number without going too low or too high
                            NPC.localAI[1] = Main.rand.Next(4, ProjAmount - 1);

							SaveNPCPosition = NPC.Center;
                            SaveDirection = NPC.spriteDirection;
                            SaveRotation = NPC.rotation;
                        }

                        if (NPC.localAI[0] > 60)
                        {
                            NPC.spriteDirection = SaveDirection;
                            NPC.rotation = SaveRotation;
                        }

						if (NPC.localAI[0] > 75 && NPC.localAI[0] < 200)
                        {
                            if (HeatMaskAlpha < 1)
                            {
                                HeatMaskAlpha += 0.01f;
                            }

                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-6, 6);

							for (float numProjectiles = 0; numProjectiles < ProjAmount; numProjectiles++)
                            {
								if (Main.rand.NextBool(5))
								{
									//this is how the gap in the circle is created
									//this check basically makes it so every thorn in the loop is spawned, except for the randomly chosen number and multiple thorns behind the chosen number
									if (numProjectiles != NPC.localAI[1] && numProjectiles != NPC.localAI[1] - 1 && numProjectiles != NPC.localAI[1] - 2 && 
									numProjectiles != NPC.localAI[1] - 3 && numProjectiles != NPC.localAI[1] - 4)
									{
										Vector2 projPos = NPC.Center + new Vector2(0, 2).RotatedBy(numProjectiles * (Math.PI * 2f / ProjAmount));

										Vector2 ShootSpeed = NPC.Center - projPos;
										ShootSpeed.Normalize();
										ShootSpeed *= 6;

										Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
										Vector2 position = NPC.Center;

										if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
										{
											position += Offset;
										}

										Color[] colors = new Color[] { Color.Gray, Color.DarkGray };

										Dust DustEffect = Dust.NewDustPerfect(position, ModContent.DustType<BigBoneSmokeDust>());
										DustEffect.velocity = ShootSpeed;
										DustEffect.color = Main.rand.Next(colors);
										DustEffect.scale = Main.rand.NextFloat(0.5f, 0.75f);
                                        DustEffect.alpha = 100;
									}
								}
							}
						}

                        if (NPC.localAI[0] == 120)
                        {
                            SoundEngine.PlaySound(SteamSound, NPC.Center);
                        }

                        if (NPC.localAI[0] == 200)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
                        }

                        if (NPC.localAI[0] >= 200 && NPC.localAI[0] < 300)
                        {
                            NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                            NPC.Center += Main.rand.NextVector2Square(-13, 13);

							if (NPC.localAI[0] % 10 == 0)
							{
								SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
							}
                                
							if (NPC.localAI[0] % 3 == 0)
							{
                                for (float numProjectiles = 0; numProjectiles < ProjAmount; numProjectiles++)
                                {
                                    //this is how the gap in the circle of thorns is created
                                    //this check basically makes it so every thorn in the loop is spawned, except for the randomly chosen number and multiple thorns behind the chosen number
                                    if (numProjectiles != NPC.localAI[1] && numProjectiles != NPC.localAI[1] - 1 && numProjectiles != NPC.localAI[1] - 2 && 
                                    numProjectiles != NPC.localAI[1] - 3 && numProjectiles != NPC.localAI[1] - 4)
                                    {
										Vector2 projPos = NPC.Center + new Vector2(0, 2).RotatedBy(numProjectiles * (Math.PI * 2f / ProjAmount));

                                        Vector2 ShootSpeed = NPC.Center - projPos;
										ShootSpeed.Normalize();
										ShootSpeed *= 15;

										Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
										Vector2 position = NPC.Center;

										if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
										{
											position += Offset;
										}

										NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, ShootSpeed, ModContent.ProjectileType<BigBoneFlame2>(), NPC.damage + 60, 4.5f);
									}
                                }
                            }
						}

                        if (NPC.localAI[0] >= 300)
                        {
                            if (HeatMaskAlpha > 0)
                            {
                                HeatMaskAlpha -= 0.025f;
                            }
                        }
                        
                        if (NPC.localAI[0] >= 350)
                        {
                            if (Main.rand.NextBool(3))
                            {
                                SoundEngine.PlaySound(GrowlSound1, NPC.Center);
                            }

							SaveDirection = 0;

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

                    GoAboveFlowerPot(250);

                    int ShootFrequency = Phase2 ? 8 : 15;

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] <= 180 && NPC.localAI[0] % ShootFrequency == 0)
                    {
                        int VelocityX = Main.rand.Next(-12, 13);
                        int VelocityY = Main.rand.Next(-15, -10);

                        Vector2 RandomPosition = new Vector2(Parent.Top.X + Main.rand.Next(-(Parent.width / 2) + 15, (Parent.width / 2) - 15), Parent.Top.Y + 12);
                        NPCGlobalHelper.ShootHostileProjectile(NPC, RandomPosition, new Vector2(VelocityX, VelocityY), ModContent.ProjectileType<PitcherVine>(), NPC.damage, 4.5f);
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

                    GoAboveFlowerPot(420);

                    //in phase 1, shoot 3 fast fire balls
                    if (!Phase2)
                    {
                        if (NPC.localAI[0] == 75 || NPC.localAI[0] == 105 || NPC.localAI[0] == 135)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);

                            NPC.velocity = -Vector2.Normalize(player.Center - NPC.Center) * Main.rand.Next(15, 23);

                            NPC.velocity *= 0.8f;

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
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, NPC.Center);
                            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);

                            NPC.velocity = -Vector2.Normalize(player.Center - NPC.Center) * Main.rand.Next(15, 23);

                            NPC.velocity *= 0.8f;

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

                //big bone slams head on roof and causes debris to fall in phase 1, charges directly at the player in phase 2
                case 5:
                {
                    NPC.localAI[0]++;

                    if (!Phase2)
                    {
                        if (NPC.localAI[1] == 0)
                        {
                            NPC.rotation = NPC.rotation.AngleTowards(NPC.direction == -1 ? (float)MathHelper.PiOver2 : -(float)MathHelper.PiOver2, 0.2f);
                        }

                        if (NPC.localAI[0] == 5)
                        {
                            SoundEngine.PlaySound(GrowlSound2, NPC.Center);
                        }

                        if (NPC.localAI[0] >= 5 && NPC.localAI[0] <= 85)
                        {
                            GoAboveFlowerPot(150);

                            int MaxDusts = Main.rand.Next(5, 15);
                            for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                            {
                                Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                                Vector2 velocity = dustPos - NPC.Center;
                                int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                                Main.dust[dustEffect].color = Color.Gold;
                                Main.dust[dustEffect].noGravity = true;
                                Main.dust[dustEffect].noLight = false;
                                Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-18f, -5f);
                                Main.dust[dustEffect].fadeIn = 1.3f;
                            }
                        }

                        if (NPC.localAI[0] == 85)
                        {
                            SoundEngine.PlaySound(GrowlSound3, NPC.Center);

                            SaveRotation = NPC.rotation;
                            SaveDirection = NPC.direction;

                            NPC.velocity.Y = 5;

                            NPC.netUpdate = true;
                        }

                        if (NPC.localAI[0] > 85)
                        {
                            NPC.spriteDirection = SaveDirection;
                            
                            if (NPC.localAI[1] == 0)
                            {
                                NPC.rotation = SaveRotation;
                            }
                        }

                        if (NPC.localAI[0] == 95)
                        {
                            NPC.velocity *= 0.5f;
                        }

                        //charge to the destination
                        if (NPC.localAI[0] == 100)
                        {
                            NPC.velocity.Y = -50;

                            NPC.netUpdate = true;
                        }

                        //stop and play slamming sound when big bone impacts a solid surface
                        if (NPC.localAI[0] > 100 && NPC.localAI[1] == 0)
                        {
                            if (NPCGlobalHelper.IsColliding(NPC))
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath43, NPC.Center);
                                SoundEngine.PlaySound(StunnedSound, NPC.Center);
                                
                                Screenshake.ShakeScreenWithIntensity(NPC.Center, 25f, 1000f);

                                NPC.velocity = Vector2.Zero;

                                for (int numRubble = -700; numRubble <= 700; numRubble += 100)
                                {
                                    Vector2 center = new Vector2(Parent.Center.X + numRubble, Parent.Center.Y - 500);
                                    center.X += Main.rand.Next(-20, 21); //add some randomness so it isnt always the same even spread

                                    int numtries = 0;
                                    int x = (int)(center.X / 16);
                                    int y = (int)(center.Y / 16);
                                    while (Main.tile[x, y] == null || !Main.tile[x, y].HasTile || Main.tile[x, y].IsActuated ||
                                    !Main.tileSolid[(int)Main.tile[x, y].TileType] || TileID.Sets.Platforms[(int)Main.tile[x, y].TileType])
                                    {
                                        y--;
                                        center.Y = y * 16;
                                    }

                                    NPCGlobalHelper.ShootHostileProjectile(NPC, center, Vector2.Zero, ModContent.ProjectileType<CatacombBrickRubble>(), NPC.damage, 4.5f, ai1: Main.rand.Next(0, 3));
                                }

                                NPC.localAI[1] = 1;
                            }
                        }
                    
                        //stunned behavior, produce star dusts and rotate
                        if (NPC.localAI[1] > 0)
                        {
                            DefaultRotation = false;

                            if (NPC.localAI[0] == 120)
                            {
                                NPC.velocity.Y = 20;
                            }

                            NPC.velocity *= 0.95f;

                            if (NPC.localAI[2] == 0)
                            {
                                NPC.rotation += 0.015f;
                                if (NPC.rotation > SaveRotation + 0.25f)
                                {
                                    NPC.localAI[2] = 1;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                NPC.rotation -= 0.015f;
                                if (NPC.rotation < SaveRotation - 0.25f)
                                {
                                    NPC.localAI[2] = 0;
                                    NPC.netUpdate = true;
                                }
                            }

                            if (NPC.localAI[0] % 5 == 0)
                            {				
                                int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 4, ModContent.DustType<CartoonStar>(), 0f, -2f, 0, default, 1f);
                                Main.dust[newDust].velocity.X = Main.rand.NextFloat(-1.5f, 1.5f);
                                Main.dust[newDust].velocity.Y = Main.rand.NextFloat(-1.5f, -0.2f);
                                Main.dust[newDust].alpha = Main.rand.Next(0, 2);
                                Main.dust[newDust].noGravity = true;
                            }
                        }

                        if (NPC.localAI[0] >= 320)
                        {
                            SaveDirection = 0;
                            DefaultRotation = true;

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
                    else
                    {
                        if (NPC.localAI[0] == 5)
                        {
                            SoundEngine.PlaySound(GrowlSound2, NPC.Center);
                        }

                        if (NPC.localAI[0] >= 5 && NPC.localAI[0] <= 85)
                        {
                            GoAboveFlowerPot(375);

                            int MaxDusts = Main.rand.Next(5, 15);
                            for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                            {
                                Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                                Vector2 velocity = dustPos - NPC.Center;
                                int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                                Main.dust[dustEffect].color = Color.Gold;
                                Main.dust[dustEffect].noGravity = true;
                                Main.dust[dustEffect].noLight = false;
                                Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-18f, -5f);
                                Main.dust[dustEffect].fadeIn = 1.3f;
                            }
                        }

                        if (NPC.localAI[0] == 85)
                        {
                            SoundEngine.PlaySound(GrowlSound3, NPC.Center);

                            SaveNPCPosition = NPC.Center;

                            //predict the players movement in phase 2
                            SavePlayerPosition = player.Center;

                            //set big bone's direction based on the saved location so his sprite doesnt flip backwards
                            NPC.direction = SavePlayerPosition.X > NPC.Center.X ? 1 : -1;

                            SaveRotation = NPC.rotation;
                            SaveDirection = NPC.direction;

                            Vector2 Recoil = SavePlayerPosition - NPC.Center;
                            Recoil.Normalize();
                                    
                            Recoil *= -5; 
                            NPC.velocity = Recoil;

                            NPC.netUpdate = true;
                        }

                        if (NPC.localAI[0] > 85)
                        {
                            NPC.spriteDirection = SaveDirection;

                            if (NPC.localAI[1] == 0)
                            {
                                NPC.rotation = SaveRotation;
                            }
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

                            NPC.netUpdate = true;
                        }

                        //stop and play slamming sound when big bone impacts a solid surface
                        if (NPC.localAI[0] > 105 && NPC.localAI[1] == 0)
                        {
                            if (NPCGlobalHelper.IsColliding(NPC))
                            {
                                SoundEngine.PlaySound(SoundID.NPCDeath43, NPC.Center);
                                SoundEngine.PlaySound(StunnedSound, NPC.Center);
                                
                                Screenshake.ShakeScreenWithIntensity(NPC.Center, 25f, 1000f);

                                NPC.velocity = Vector2.Zero;

                                NPC.localAI[0] = 120;

                                for (int numRubble = -700; numRubble <= 700; numRubble += 100)
                                {
                                    Vector2 center = new Vector2(Parent.Center.X + numRubble, Parent.Center.Y - 500);
                                    center.X += Main.rand.Next(-20, 21); //add some randomness so it isnt always the same even spread

                                    int numtries = 0;
                                    int x = (int)(center.X / 16);
                                    int y = (int)(center.Y / 16);
                                    while (Main.tile[x, y] == null || !Main.tile[x, y].HasTile || Main.tile[x, y].IsActuated ||
                                    !Main.tileSolid[(int)Main.tile[x, y].TileType] || TileID.Sets.Platforms[(int)Main.tile[x, y].TileType])
                                    {
                                        y--;
                                        center.Y = y * 16;
                                    }

                                    int Type = Main.rand.NextBool(4) ? ModContent.ProjectileType<CatacombBrickRubbleGiant>() : ModContent.ProjectileType<CatacombBrickRubble>();

                                    NPCGlobalHelper.ShootHostileProjectile(NPC, center, Vector2.Zero, Type, NPC.damage, 4.5f, ai1: Main.rand.Next(0, 3));
                                }

                                NPC.localAI[1] = 1;
                            }
                        }

                        //stunned behavior, produce star dusts and rotate
                        if (NPC.localAI[1] > 0)
                        {
                            DefaultRotation = false;

                            if (NPC.localAI[0] == 120)
                            {
                                Vector2 ChargeDirection = SaveNPCPosition - NPC.Center;
                                ChargeDirection.Normalize();
                                ChargeDirection *= 20;
                                NPC.velocity = ChargeDirection;
                            }

                            NPC.velocity *= 0.95f;

                            if (NPC.localAI[2] == 0)
                            {
                                NPC.rotation += 0.015f;
                                if (NPC.rotation > SaveRotation + 0.25f)
                                {
                                    NPC.localAI[2] = 1;
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                NPC.rotation -= 0.015f;
                                if (NPC.rotation < SaveRotation - 0.25f)
                                {
                                    NPC.localAI[2] = 0;
                                    NPC.netUpdate = true;
                                }
                            }

                            if (NPC.localAI[0] % 5 == 0)
                            {				
                                int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 4, ModContent.DustType<CartoonStar>(), 0f, -2f, 0, default, 1f);
                                Main.dust[newDust].velocity.X = Main.rand.NextFloat(-1.5f, 1.5f);
                                Main.dust[newDust].velocity.Y = Main.rand.NextFloat(-1.5f, -0.2f);
                                Main.dust[newDust].alpha = Main.rand.Next(0, 2);
                                Main.dust[newDust].noGravity = true;
                            }
                        }

                        if (NPC.localAI[0] >= 320)
                        {
                            SaveDirection = 0;
                            DefaultRotation = true;

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
            }
		}

        public void GoAboveFlowerPot(float DistanceAbove)
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            float goToX = Parent.Center.X;
            float goToY = Parent.Center.Y - DistanceAbove;

            Vector2 GoTo = new Vector2(Parent.Center.X, Parent.Center.Y - DistanceAbove);

            if (Vector2.Distance(NPC.Center, GoTo) >= 50f)
            {
                Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 15;
                NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
            }
            else
            {
                NPC.velocity *= 0.85f;
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