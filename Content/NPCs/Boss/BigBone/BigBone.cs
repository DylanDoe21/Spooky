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
            writer.Write(SpecialAttack[0]);
            writer.Write(SpecialAttack[1]);

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
            SpecialAttack[0] = reader.ReadInt32();
            SpecialAttack[1] = reader.ReadInt32();

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
            NPC.lifeMax = 65000;
            NPC.damage = 70;
            NPC.defense = 45;
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
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            //only draw the neck stem if the parent is active
            if (Parent.active)
			{
                NeckTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneStem");

                Vector2 rootPosition = Parent.Center;

                Vector2[] bezierPoints = { rootPosition, rootPosition + new Vector2(0, -60), NPC.Center + new Vector2(-60 * NPC.direction, 0).RotatedBy(NPC.rotation), NPC.Center + new Vector2(-14 * NPC.direction, 0).RotatedBy(NPC.rotation) };
                float bezierProgress = 0;
                float bezierIncrement = 27;

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
                    
                    spriteBatch.Draw(NeckTexture.Value, (oldPos + newPos) / 2 - Main.screenPosition, NeckTexture.Frame(), drawColor, rotation, textureCenter, NPC.scale, SpriteEffects.None, 0f);
                }
            }

            //draw aura when healed or protected by flowers
            AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/BigBoneAura");

            var effects = SpriteEffects.None;

            //while charging, use the saved direction and not the actual npc direction
            if (NPC.ai[0] == 7 && NPC.localAI[0] > 85 && NPC.localAI[0] <= 160)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            //while shooting roses in phase 1, use the saved direction and not the actual npc direction
            else if (NPC.ai[0] == 3 && NPC.localAI[0] > 60 && NPC.localAI[0] <= 240 && !Phase2)
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

            //draw healing aura if any healing flowers exist
            if (NPC.CountNPCS(ModContent.NPCType<HealingFlower>()) > 0)
			{
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(AuraTexture.Value, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time, NPC.frame, color, NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }
            }

            //draw invulnerability aura if defensive flowers exist
            if (NPC.CountNPCS(ModContent.NPCType<DefensiveFlower>()) > 0)
			{
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.OrangeRed);

                for (float i = 0f; i < 1f; i += 0.25f)
                {
                    float radians = (i + (fade / 2)) * MathHelper.TwoPi;
                    spriteBatch.Draw(AuraTexture.Value, drawPos + new Vector2(0f, 25f).RotatedBy(radians) * time, NPC.frame, color, NPC.rotation, frameOrigin, NPC.scale, effects, 0);
                }
            }

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
            //while shooting roses in phase 1, use the saved direction and not the actual npc direction
            else if (NPC.ai[0] == 3 && NPC.localAI[0] > 60 && NPC.localAI[0] <= 240 && !Phase2)
            {
                effects = SaveDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Transparent, Color.White, fade);

            //draw different glowmask colors based on the current attack being used
            if (!Phase2)
            {
                if (NPC.ai[0] == 0 || NPC.ai[0] == 7)
                {
                    Main.EntitySpriteDraw(GlowTexture1.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                }

                if (NPC.ai[0] == 2 || NPC.ai[0] == 3 || NPC.ai[0] == 6)
                {
                    Main.EntitySpriteDraw(GlowTexture2.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                }

                if (NPC.ai[0] == 4 || NPC.ai[0] == 5 || NPC.ai[0] == 8)
                {
                    Main.EntitySpriteDraw(GlowTexture3.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                }

                if (NPC.ai[0] == 1)
                {
                    Main.EntitySpriteDraw(GlowTexture4.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
                }
            }
            else
            {
                Main.EntitySpriteDraw(GlowTexture3.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
            }

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

            int Damage = Main.masterMode ? 90 / 3 : Main.expertMode ? 70 / 2 : 45;

            NPC.spriteDirection = NPC.direction;

            //EoC rotation
            Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //despawn if all players are dead
            if (player.dead || !player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome2>()))
            {
                NPC.ai[0] = -3;
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
                //if his healing/immunity flowers do exist, then add his charging attack to the list to prevent more healing/immunity flowers from being spammed
                else
                {
                    AttackPattern = AttackPattern.Append(SpecialAttack[1]).ToArray();
                }

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
                //despawning
                case -3:
                {
                    NPC Parent = Main.npc[(int)NPC.ai[3]];

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
                        if (Main.npc[k].type == ModContent.NPCType<HealingFlower>() || Main.npc[k].type == ModContent.NPCType<DefensiveFlower>() || Main.npc[k].type == ModContent.NPCType<BigFlower>()) 
                        {
                            Main.npc[k].active = false;
                        }
                    }

                    break;
                }

                //death animation
                case -2:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(350);

                    SpookyPlayer.ScreenShakeAmount = NPC.localAI[0] / 10;

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

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100), 
                        Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), ModContent.ProjectileType<FlowerSpore>(), Damage, 1, Main.myPlayer, 0, 0);
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

                //shoot flower spreads that bounce around off walls
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

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, 
                                10f * NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(13) * numProjectiles), 
                                ProjType, Damage, 0f, Main.myPlayer);
                            }
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

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-100, 100), NPC.Center.Y + Main.rand.Next(-100, 100), 
                                ShootSpeed.X, ShootSpeed.Y, ProjType, Damage, 1, Main.myPlayer, 0, NPC.whoAmI);
                            }
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
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                                    {
                                        float distance = Main.rand.NextFloat(8f, 12f);

                                        Vector2 Position = (Vector2.One * new Vector2((float)player.width / 3f, (float)player.height / 3f) * distance).RotatedBy((double)((float)(numProjectiles - (5 / 2 - 1)) * 6.28318548f / 5f), default(Vector2)) + player.Center;
                                    
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), Position, Vector2.Zero, ModContent.ProjectileType<BigBoneThornTelegraph>(), 0, 0f, Main.myPlayer);

                                        SavePoint[numProjectiles] = new Vector2(Position.X, Position.Y);
                                    }

                                    for (int numPoints = 0; numPoints < SavePoint.Length; numPoints++)
                                    {
                                        Vector2 Direction = NPC.Center - SavePoint[numPoints];
                                        Direction.Normalize();

                                        Vector2 lineDirection = new Vector2(Direction.X, Direction.Y);

                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<BigBoneThorn>(), Damage + 20, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                                    }
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

                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0, 0,
                                        ModContent.ProjectileType<SolarThorn>(), Damage + 20, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
                                    }
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

                //use razor rose attack
                case 3:
                {
                    NPC.localAI[0]++;

                    GoAboveFlowerPot(200);

                    //save the players location, then shoot a barrage of roses at that location
                    if (!Phase2)
                    {
                        if (NPC.localAI[0] == 60)
                        {
                            SavePlayerPosition = player.Center + player.velocity * 15f;
                            
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), SavePlayerPosition, Vector2.Zero, ModContent.ProjectileType<RazorRoseTelegraph>(), 0, 0f, Main.myPlayer);

                            //set big bone's rotation to the predicted spot so it is correct when shooting roses
                            Vector2 newVector = new Vector2(NPC.Center.X, NPC.Center.Y);
                            float newRotateX = player.Center.X + player.velocity.X * 15f - newVector.X;
                            float newRotateY = player.Center.Y + player.velocity.Y * 15f - newVector.Y;
                            NPC.rotation = (float)Math.Atan2((double)newRotateY, (double)newRotateX) + 4.71f;

                            //set big bone's direction based on the saved location so his sprite doesnt flip backwards
                            NPC.direction = SavePlayerPosition.X > NPC.Center.X ? 1 : -1;

                            SaveRotation = NPC.rotation;
                            SaveDirection = NPC.direction;

                            NPC.netUpdate = true;
                        }

                        if (NPC.localAI[0] > 60 && NPC.localAI[0] <= 240)
                        {
                            NPC.spriteDirection = SaveDirection;
                            NPC.rotation = SaveRotation;
                        }

                        if (NPC.localAI[0] >= 90 && NPC.localAI[0] <= 240)
                        {
                            NPC.velocity *= 0.98f;

                            if (Main.rand.NextBool(2))
                            {
                                //recoil
                                Vector2 Recoil = SavePlayerPosition - NPC.Center;
                                Recoil.Normalize();
                                Recoil *= -2;
                                NPC.velocity = Recoil;

                                Vector2 ShootSpeed = SavePlayerPosition - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= Main.rand.Next(25, 36);

                                SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                                    ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<RazorRose>(), Damage, 0f, Main.myPlayer, 0, 0);
                                }
                            }
                        }
                    }

                    //in phase 2 create a continuous stream of roses that is constantly shot at the player
                    if (Phase2)
                    {
                        if (NPC.localAI[0] == 60)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<RazorRoseTelegraphLockOn>(), 0, 0f, Main.myPlayer);
                        }

                        if (NPC.localAI[0] >= 100 && NPC.localAI[0] <= 240)
                        {
                            NPC.velocity *= 0.98f;

                            if (Main.rand.NextBool(2))
                            {
                                //recoil
                                Vector2 Recoil = player.Center - NPC.Center;
                                Recoil.Normalize();
                                Recoil *= -2;
                                NPC.velocity = Recoil;

                                Vector2 ShootSpeed = player.Center - NPC.Center;
                                ShootSpeed.Normalize();
                                ShootSpeed *= 16;

                                SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-50, 50), 
                                    ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<RazorRoseOrange>(), Damage, 0f, Main.myPlayer, 0, 0);
                                }
                            }
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

                            int[] Types = new int[] { ProjectileID.GreekFire1, ProjectileID.GreekFire2, ProjectileID.GreekFire3 };

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, 0 + Spread, 
                            Main.rand.Next(-5, -2), Main.rand.Next(Types), Damage, 1, Main.myPlayer, 0, 0);
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
                            ShootSpeed *= 1.5f;

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 70f;
                            Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

                            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                            {
                                position += muzzleOffset;
                            }

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), position.X, position.Y, ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<GiantFlameBall>(), Damage, 1, Main.myPlayer);
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
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                            Main.dust[dustEffect].color = Phase2 ? Color.OrangeRed : Color.Red;
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

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, speed, ProjType, 0, 0, Main.myPlayer, NPC.whoAmI);
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

                //stay still, then charge and crash into a wall
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
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}