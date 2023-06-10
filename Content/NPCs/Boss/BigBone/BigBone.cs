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
using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.BigBone.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    [AutoloadBossHead]
    public class BigBone : ModNPC
    {
        int[] AttackPattern = new int[] { 0, 1, 2, 3, 4, 5 };
        int[] SpecialAttack = new int[] { 6, 7 };

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

        public static readonly SoundStyle GrowlSound1 = new("Spooky/Content/Sounds/Catacomb/BigBoneGrowl1", SoundType.Sound);
        public static readonly SoundStyle GrowlSound2 = new("Spooky/Content/Sounds/Catacomb/BigBoneGrowl2", SoundType.Sound);
        public static readonly SoundStyle GrowlSound3 = new("Spooky/Content/Sounds/Catacomb/BigBoneGrowl3", SoundType.Sound);
        public static readonly SoundStyle LaughSound = new("Spooky/Content/Sounds/Catacomb/BigBoneLaugh", SoundType.Sound);
        public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/Catacomb/BigBoneMagic", SoundType.Sound);
        public static readonly SoundStyle MagicCastSound2 = new("Spooky/Content/Sounds/Catacomb/BigBoneMagic2", SoundType.Sound);
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/Catacomb/BigBoneDeath", SoundType.Sound);
        public static readonly SoundStyle DeathSound2 = new("Spooky/Content/Sounds/Catacomb/BigBoneDeath2", SoundType.Sound);
        
        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailCacheLength[NPC.type] = 8;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/BigBone/BigBoneBestiary",
                Position = new Vector2(48f, -12f),
                PortraitPositionXOverride = 12f,
                PortraitPositionYOverride = -12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused, 
                    BuffID.Poisoned, 
                    BuffID.Venom, 
                    BuffID.OnFire,
                    BuffID.OnFire3,
                    BuffID.Frostburn,
                    BuffID.Frostburn2,
                    BuffID.CursedInferno, 
                    BuffID.Ichor, 
                    BuffID.ShadowFlame,
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //attack pattern
            for (int i = 0; i <= AttackPattern.Length; i++)
            {
                writer.Write(AttackPattern[i]);
            }

            //ints
            writer.Write(ScaleTimerLimit);
            writer.Write(SaveDirection);

            //bools
            writer.Write(Phase2);
            writer.Write(Transition);
            writer.Write(FlowersSpawned);
            writer.Write(ActuallyDead);
            writer.Write(DeathAnimation);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //attack pattern
            for (int i = 0; i <= AttackPattern.Length; i++)
            {
                AttackPattern[i] = reader.ReadInt32();
            }

            //ints
            ScaleTimerLimit = reader.ReadInt32();
            SaveDirection = reader.ReadInt32();

            //bools
            Phase2 = reader.ReadBoolean();
            Transition = reader.ReadBoolean();
            FlowersSpawned = reader.ReadBoolean();
            ActuallyDead = reader.ReadBoolean();
            DeathAnimation = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 65000;
            NPC.damage = 65;
            NPC.defense = 65;
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
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BigBone"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
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

            //draw aura when healed or protected by flowers
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneAura").Value;

            var effects = SpriteEffects.None;

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

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = time / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            //draw healing aura if any healing flowers exist
            if (NPC.CountNPCS(ModContent.NPCType<HealingFlower>()) > 0)
			{
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(tex, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time, 
                    NPC.frame, new Color(255, 0, 0, 50), NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(tex, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time,
                    NPC.frame, new Color(255, 0, 0, 50), NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }
            }

            //draw invulnerability aura if defensive flowers exist
            if (NPC.CountNPCS(ModContent.NPCType<DefensiveFlower>()) > 0)
			{
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(tex, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time, 
                    NPC.frame, new Color(225, 70, 0, 50), NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(tex, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time,
                    NPC.frame, new Color(225, 70, 0, 50), NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }
            }

            //draw aura during death animation
            if (DeathAnimation)
            {
                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(tex, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time, 
                    NPC.frame, new Color(255, 165, 0, 50), NPC.rotation, frameOrigin, NPC.scale + RealScaleAmount, effects, 0);
                }

                for (float i = 0f; i < 1f; i += 0.34f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(tex, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time,
                    NPC.frame, new Color(255, 0, 0, 50), NPC.rotation, frameOrigin, NPC.scale + RealScaleAmount, effects, 0);
                }
            }

            return true;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow1").Value;

            if (!Phase2)
            {
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
            }

            if (Phase2)
            {
                tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneGlow3").Value;
            }

            var effects = SpriteEffects.None;

            if (NPC.ai[0] == 7 && NPC.localAI[0] > 85 && NPC.localAI[0] <= 160)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Transparent, Color.White, fade);

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);

            if (NPC.ai[0] == -1 && NPC.CountNPCS(ModContent.NPCType<BigFlower>()) > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var center = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                float intensity = fade;
                DrawData drawData = new DrawData(ModContent.Request<Texture2D>("Spooky/ShaderAssets/SkullEffect").Value, center - new Vector2(0, -55), 
                new Rectangle(0, 0, 500, 400), Color.Orange, 0, new Vector2(250f, 250f), NPC.scale * (1f + intensity * 0.05f), SpriteEffects.None, 0);
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
            //death animation
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
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 100 / 3 : (Main.expertMode ? 70 / 2 : 50);

            NPC.spriteDirection = NPC.direction;

            //EoC rotation
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //despawn if all players are dead
            if (player.dead)
            {
                NPC.ai[2]++;
                if (NPC.ai[2] >= 60)
                {
                    for (int k = 0; k < Main.projectile.Length; k++)
                    {
                        if (Main.projectile[k].active && Main.projectile[k].hostile) 
                        {
                            Main.projectile[k].Kill();
                        }
                    }

                    NPC.active = false;
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

            //set these back to false during phase 2 if no defensive flowers exist
            if (Phase2 && !DeathAnimation && NPC.CountNPCS(ModContent.NPCType<DefensiveFlower>()) <= 0)
			{
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
            }

            //reset and randomize attack pattern list
            if (NPC.ai[2] == 0)
            {
                //play laughing sound
                SoundEngine.PlaySound(LaughSound, NPC.Center);

                //remove his special attacks from the attack list
                foreach (var i in AttackPattern)
                {
                    AttackPattern = AttackPattern.Where(i => i != 6).ToArray();
                    AttackPattern = AttackPattern.Where(i => i != 7).ToArray();
                }
                    
                //shuffle the attack pattern list
                AttackPattern = AttackPattern.OrderBy(x => Main.rand.Next()).ToArray();

                //add one of the special attacks to the end of the list
                //if none of big bones healing/immunity flowers exist, then add one of his two special attacks
                if (NPC.CountNPCS(ModContent.NPCType<HealingFlower>()) <= 0 && NPC.CountNPCS(ModContent.NPCType<DefensiveFlower>()) <= 0)
			    {
                    AttackPattern = AttackPattern.Append(Main.rand.Next(SpecialAttack)).ToArray();
                }
                //if his flowers do exist, only add his charging attack to the list to prevent more healing/immunity flowers from being spammed
                else
                {
                    AttackPattern = AttackPattern.Append(7).ToArray();
                }

                NPC.ai[2] = 1;
                NPC.netUpdate = true;
            }

            //transition to phase 2
            if (NPC.life < (NPC.lifeMax / 2) && !Phase2)
            {
                Transition = true;
            }

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

            switch ((int)NPC.ai[0])
            {
                //death animation
                case -2:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(350);

                    SpookyPlayer.ScreenShakeAmount = NPC.localAI[0] / 8;

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
                    }

                    //shake, and cause a circle of dusts around big bone that become bigger over time
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
                                int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                                Main.dust[dustEffect].color = Color.Orange;
                                Main.dust[dustEffect].scale = 0.05f + (NPC.localAI[0] / 450);
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

                    //kill big bone, spawn gores, ect
                    if (NPC.localAI[0] >= 360)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, NPC.Center);

                        SpookyPlayer.ScreenShakeAmount = 0;

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
                            int dustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
                            Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-20f, 20f);
                            Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-10f, 10f);
                            Main.dust[dustGore].scale = Main.rand.NextFloat(3f, 5f);
                            Main.dust[dustGore].noGravity = true;
                        }

                        //explosion smoke
                        for (int numExplosion = 0; numExplosion < 25; numExplosion++)
                        {
                            int DustGore = Dust.NewDust(NPC.Center, NPC.width / 2, NPC.height / 2, ModContent.DustType<SmokeEffect>(), 
                            0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));

                            Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                            Main.dust[DustGore].noGravity = true;

                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[DustGore].scale = 0.5f;
                                Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        //kill big bone
                        ActuallyDead = true;
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;
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
                            int distance = 360 / 12;

                            int solarFlower = NPC.NewNPC(NPC.GetSource_FromAI(), (int)flowerPos.X, (int)flowerPos.Y, ModContent.NPCType<BigFlower>(), NPC.whoAmI, numFlowers * distance, NPC.whoAmI);

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

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100), 
                        Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), ModContent.ProjectileType<FlowerSpore>(), Damage, 1, Main.myPlayer, 0, 0);
                    }

                    if (NPC.localAI[2] >= 560)
                    {
                        //if theres only 3 flowers left, shoot a homing fire ball at the end of the attack
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
                                    
                            ShootSpeed.X *= 5;
                            ShootSpeed.Y *= 5;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), position.X, position.Y, ShootSpeed.X, 
                            ShootSpeed.Y, ModContent.ProjectileType<MassiveFlameBall>(), Damage, 1, Main.myPlayer, 0, 0);
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

                        int MaxProjectiles = Phase2 ? 1 : 2;

                        for (int numProjectiles = -MaxProjectiles; numProjectiles <= MaxProjectiles; numProjectiles++)
                        {
                            int ProjType = Phase2 ? ModContent.ProjectileType<HomingFlower>() : ModContent.ProjectileType<BouncingFlower>();

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 
                            10f * NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(13) * numProjectiles), 
                            ProjType, Damage, 0f, Main.myPlayer);
                        }
                    }

                    if (NPC.localAI[0] >= 165)
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

                //create skull wisps around itself that launch themselves at the player
                case 1:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(500);

                    if (NPC.localAI[0] >= 75 && NPC.localAI[0] <= 195)
                    {
                        int WispChance = Phase2 ? 5 : 3;

                        if (Main.rand.NextBool(WispChance))
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                                    
                            ShootSpeed.X *= Main.rand.Next(-12, 12);
                            ShootSpeed.Y *= Main.rand.Next(-12, 12);

                            int ProjType = Phase2 ? ModContent.ProjectileType<FlamingWisp>() : ModContent.ProjectileType<BoneWisp>();

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100), 
                            ShootSpeed.X, ShootSpeed.Y, ProjType, Damage, 1, Main.myPlayer, 0, 0);
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

                //use thorn attacks
                case 2:
                {
                    NPC.localAI[0]++;

                    //in phase 1, use the telegraph thorn spread
                    if (!Phase2)
                    {
                        GoAboveFlowerPot(100);

                        if (NPC.localAI[1] < 4)
                        {
                            if (NPC.localAI[0] == 75)
                            {
                                for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                                {
                                    float distance = Phase2 ? Main.rand.NextFloat(8f, 12f) : Main.rand.NextFloat(5f, 8f);

                                    Vector2 Position = (Vector2.One * new Vector2((float)player.width / 3f, (float)player.height / 3f) * distance).RotatedBy((double)((float)(numProjectiles - (5 / 2 - 1)) * 6.28318548f / 5f), default(Vector2)) + player.Center;
                                
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), Position.X, Position.Y, 0, 0,
                                    ModContent.ProjectileType<BigBoneThornTelegraph>(), 0, 0f, Main.myPlayer, 0, 0);

                                    SavePoint[numProjectiles] = new Vector2(Position.X, Position.Y);
                                }

                                for (int i = 0; i < SavePoint.Length; i++)
                                {
                                    Vector2 Direction = NPC.Center - SavePoint[i];
                                    Direction.Normalize();

                                    Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 0,
                                    ModContent.ProjectileType<BigBoneThorn>(), Damage + 20, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
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

                    //in phase 2, make a circle of thorns with a random gap
                    if (Phase2)
                    {
                        GoAboveFlowerPot(350);

                        if (NPC.localAI[0] == 60)
                        {
                            SoundEngine.PlaySound(MagicCastSound, NPC.Center);

                            NPC.localAI[1] = Main.rand.Next(4, 44);

                            for (float numProjectiles = 0; numProjectiles < 45; numProjectiles++)
                            {
                                if (numProjectiles != NPC.localAI[1] && numProjectiles != NPC.localAI[1] - 1 && numProjectiles != NPC.localAI[1] - 2 && 
                                numProjectiles != NPC.localAI[1] - 3 && numProjectiles != NPC.localAI[1] - 4)
                                {
                                    Vector2 projPos = NPC.Center + new Vector2(0, 2).RotatedBy(numProjectiles * (Math.PI * 2f / 45));

                                    Vector2 Direction = NPC.Center - projPos;
                                    Direction.Normalize();

                                    Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 0,
                                    ModContent.ProjectileType<SolarThorn>(), Damage + 20, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                                }
                            }
                        }

                        if (NPC.localAI[0] >= 280)
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

                //shoot a continuous stream of roses that move in a wave
                case 3:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(200);

                    if (NPC.localAI[0] >= 80 && NPC.localAI[0] <= 200)
                    {
                        NPC.velocity *= 0.98f;

                        if (Main.rand.NextBool(2))
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

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                            ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<RazorRose>(), Damage, 0f, Main.myPlayer, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 200)
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

                //shoot greek fire around
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 60)
                    {
                        GoAboveFlowerPot(300);
                    }

                    if (NPC.localAI[0] > 60 && NPC.localAI[0] <= 90)
                    {
                        GoAboveFlowerPot(400);
                    }

                    if (NPC.localAI[0] > 90)
                    {
                        GoAboveFlowerPot(500);
                    }

                    if (NPC.localAI[0] == 60 || NPC.localAI[0] == 90 || NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                        SoundEngine.PlaySound(MagicCastSound, NPC.Center);

                        int NumProjectiles = Phase2 ? Main.rand.Next(15, 25) : Main.rand.Next(10, 15);
                        for (int maxProjectiles = 0; maxProjectiles < NumProjectiles; maxProjectiles++)
                        {
                            float Spread = (float)Main.rand.Next(-1200, 1200) * 0.01f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0 + Spread, 
                            Main.rand.Next(-5, -2), ModContent.ProjectileType<LingerFlame>(), Damage, 1, Main.myPlayer, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 160)
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
                case 5:
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
                                    
                            ShootSpeed.X *= 1.5f;
                            ShootSpeed.Y *= 1.5f;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), position.X, position.Y, ShootSpeed.X, 
                            ShootSpeed.Y, ModContent.ProjectileType<GiantFlameBall>(), Damage, 1, Main.myPlayer, 0, 0);
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
                                    
                            Recoil.X *= -20;
                            Recoil.Y *= -20;
                            NPC.velocity.X = Recoil.X;
                            NPC.velocity.Y = Recoil.Y;

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                                    
                            ShootSpeed.X *= 4.5f;
                            ShootSpeed.Y *= 4.5f;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), position.X, position.Y, ShootSpeed.X, 
                            ShootSpeed.Y, ModContent.ProjectileType<MassiveFlameBall>(), Damage, 1, Main.myPlayer, 0, 0);
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

                //special attacks

                //shoot seeds that turn into flowers that heal big bone
                case 6:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(300);

                    if (NPC.localAI[0] == 1)
                    {
                        SoundEngine.PlaySound(GrowlSound2, NPC.Center);
                    }

                    if (NPC.localAI[0] <= 80)
                    {
                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                            Main.dust[dustEffect].color = Phase2 ? Color.OrangeRed : Color.Red;
                            Main.dust[dustEffect].scale = 0.1f;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-18f, -5f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] == 80 || NPC.localAI[0] == 90 || NPC.localAI[0] == 100)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                        Vector2 Speed = new Vector2(Main.rand.NextFloat(5f, 8.5f), 0f).RotatedByRandom(2 * Math.PI);

                        for (int numProjectiles = 0; numProjectiles < 4; numProjectiles++)
                        {
                            Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                            int ProjType = Phase2 ? ModContent.ProjectileType<DefensiveFlowerSeed>() : ModContent.ProjectileType<HealingFlowerSeed>();

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, speed, ProjType, 0, 0f, Main.myPlayer, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 145)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                //stay still, then charge and crash into the wall
                case 7:
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
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                            Main.dust[dustEffect].color = Color.Yellow;
                            Main.dust[dustEffect].scale = 0.1f;
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

                        Vector2 Recoil = player.Center - NPC.Center;
                        Recoil.Normalize();
                                
                        Recoil.X *= -5;
                        Recoil.Y *= -5;  
                        NPC.velocity.X = Recoil.X;
                        NPC.velocity.Y = Recoil.Y;

                        SavePlayerPosition = player.Center;
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

                    if (NPC.localAI[0] == 105)
                    {
                        //charge
                        Vector2 ChargeDirection = SavePlayerPosition - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection.X *= 45;
                        ChargeDirection.Y *= 45;  
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;
                    }

                    if (NPC.localAI[0] > 105 && NPC.localAI[1] == 0)
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
                            if (Phase2)
                            {
                                Vector2 Speed = new Vector2(10f, 0f).RotatedByRandom(2 * Math.PI);

                                for (int numProjectiles = 0; numProjectiles < 8; numProjectiles++)
                                {
                                    Vector2 Position = new Vector2(NPC.Center.X, NPC.Center.Y);
                                    Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                                    Projectile.NewProjectile(NPC.GetSource_Death(), Position, speed, 
                                    ModContent.ProjectileType<MassiveFlameBallBolt>(), Damage, 0f, Main.myPlayer, 0, 0);
                                }
                            }

                            SoundEngine.PlaySound(SoundID.NPCDeath43, NPC.Center);
                            
                            SpookyPlayer.ScreenShakeAmount = 25;

                            NPC.velocity *= 0;

                            NPC.localAI[1] = 1;
                        }
                    }

                    if (NPC.localAI[0] >= 160 && NPC.localAI[1] == 1)
                    {
                        GoAboveFlowerPot(300);
                    }

                    if (NPC.localAI[0] >= 260)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;

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

            //trophy and mask always drop directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BigBoneTrophyItem>(), 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BigBoneMask>(), 7));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlowerPotHead>(), 20));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            if (!Flags.downedBigBone)
            {
                string text = "The curse of the catacombs has been lifted!";

                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(text, Color.Yellow);
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), Color.Yellow);
                }
            }

            NPC.SetEventFlagCleared(ref Flags.downedBigBone, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}