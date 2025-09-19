using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Blooms.Accessory;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    [AutoloadBossHead]
    public class DaffodilEye : ModNPC
    {
        bool Phase2 = false;
        bool SpawnedHands = false;
        bool ActuallyDead = false;

        Vector2[] SavePoint = new Vector2[5];

        private static Asset<Texture2D> EyeTexture;

        public static readonly SoundStyle SeedSpawnSound = new("Spooky/Content/Sounds/Daffodil/SeedSpawn", SoundType.Sound);
        public static readonly SoundStyle FlySound = new("Spooky/Content/Sounds/FlyBuzzing", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/DaffodilBestiary",
                Position = new Vector2(1f, 30f),
                PortraitPositionXOverride = 2f,
                PortraitPositionYOverride = 30f
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			//vector2
			for (int i = 0; i < SavePoint.Length; i++)
            {
                writer.WriteVector2(SavePoint[i]);
            }

			//bools
			writer.Write(Phase2);
            writer.Write(SpawnedHands);
            writer.Write(ActuallyDead);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
			for (int i = 0; i < SavePoint.Length; i++)
            {
                SavePoint[i] = reader.ReadVector2();
            }

			//bools
			Phase2 = reader.ReadBoolean();
            SpawnedHands = reader.ReadBoolean();
            ActuallyDead = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 22000;
            NPC.damage = 50;
            NPC.defense = 25;
            NPC.width = 58;
            NPC.height = 58;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 6, 0, 0);
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.netAlways = true;
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit7;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Daffodil");
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Daffodil"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[NPC.target];

            EyeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilEyePupil");

            Vector2 drawOrigin = new Vector2(EyeTexture.Width() / 2, EyeTexture.Height() / 2);
            Vector2 drawPos = new Vector2(NPC.Center.X, NPC.Center.Y + 2) - screenPos;

            float lookX = (player.Center.X - NPC.Center.X) * 0.015f;
            float lookY = (player.Center.Y - NPC.Center.Y) * 0.01f;

            drawPos.X += lookX;
            drawPos.Y += lookY;

            //do not draw the pupil when the eye is closed
            if (NPC.frame.Y != 0)
            {
                spriteBatch.Draw(EyeTexture.Value, drawPos, null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }

            if ((NPC.ai[0] == 6 && NPC.localAI[0] >= 160 && NPC.localAI[0] <= 260) || (NPC.ai[0] == 7 && NPC.localAI[0] >= 90 && NPC.localAI[0] <= 190))
            {
                //draw sparkle
                Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y + 10) - Main.screenPosition;
                float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;
                DrawPrettyStarSparkle(NPC.Opacity, SpriteEffects.None, vector, Color.Gold, Color.Gold, 0.5f, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(5f * time, 4f * time), new Vector2(5, 5));
            }
        }

        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) 
        {
			Texture2D Texture = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = Texture.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float Intensity = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * Intensity;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * Intensity;
			color *= Intensity;
			color2 *= Intensity;
			Main.EntitySpriteDraw(Texture, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color, 0f + rotation, origin, vector2, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
		}

        public override void FindFrame(int frameHeight)
        {
            //frame numbers:
            //0 = fully closed 
            //1 = slightly open
            //2 = open
            //3 = wide open

            //open eye when awoken
            if (NPC.ai[0] == -1)
            {
                if (NPC.localAI[0] <= 30)
                {
                    NPC.frame.Y = frameHeight * 0;
                }
                if (NPC.localAI[0] <= 60 && NPC.localAI[0] > 30)
                {
                    NPC.frame.Y = frameHeight * 1;
                }
                if (NPC.localAI[0] <= 90 && NPC.localAI[0] > 60)
                {
                    NPC.frame.Y = frameHeight * 2;
                }
            }
            else if (NPC.ai[0] == -2)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            //close eye at the end of the ending dialogue
            else if (NPC.ai[0] == -3)
            {
                if (NPC.localAI[2] >= 450)
                {
                    NPC.frame.Y = frameHeight * 1;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 2;
                }
            }
            //slightly open eye during the joke awakening
            else if (NPC.ai[0] == -4)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            //close eye when despawning
            else if (NPC.ai[0] == -5)
            {
                if (NPC.localAI[1] >= 90)
                {
                    NPC.frame.Y = frameHeight * 1;
                }
                else
                {
                    NPC.frame.Y = frameHeight * 2;
                }
            }
            else if (NPC.ai[0] == 0 && NPC.localAI[0] >= 60 && NPC.localAI[0] <= 155)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.ai[0] == 3 && NPC.localAI[0] >= 120 && NPC.localAI[0] < 400)
            {
                NPC.frame.Y = frameHeight * 0;
            }
            else if (NPC.ai[0] == 4 && NPC.localAI[0] >= 70 && NPC.localAI[0] <= 135)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.ai[0] == 5 && NPC.localAI[0] >= 120 && NPC.localAI[0] <= 300)
            {
                NPC.frame.Y = frameHeight * 0;
            }
            else if (NPC.ai[0] == 6 && NPC.localAI[0] > 160 && NPC.localAI[0] < 280)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.ai[0] == 7 && NPC.localAI[0] >= 160 && NPC.localAI[0] <= 240)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            //if none of the above is true, use the default open eye frame
            else
            {
                NPC.frame.Y = frameHeight * 2;
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

        public override bool CheckDead()
        {
            //death animation
            if (!ActuallyDead)
            {
                NPC.ai[0] = -3;
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
            NPC Parent = Main.npc[(int)NPC.ai[1]];
            
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (!Flags.downedDaffodil)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/DaffodilWithIntro1");
            }
            else
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/DaffodilWithIntro2");
            }

            int Damage = Main.masterMode ? 60 / 3 : Main.expertMode ? 40 / 2 : 30;

            //despawn if the player dies or leaves the biome
            if (player.dead || !player.active || !player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome>()))
            {
                NPC.localAI[1]++;
                    
                if (NPC.localAI[1] >= 120)
                {
                    NPC.active = false;
                }

                return;
            }
            else
            {
                NPC.localAI[1] = 0;
            }

            if (!SpawnedHands)
            {
                NPC.ai[2] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DaffodilHandLeft>(), ai2: NPC.whoAmI);
                NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DaffodilHandRight>(), ai3: NPC.whoAmI);
                
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[2]);
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[3]);
                }

                SpawnedHands = true;
                NPC.netUpdate = true;
            }

            //kill the eye if its not "attached" to the main body
            if (Parent.type != ModContent.NPCType<DaffodilBody>())
            {
                NPC.active = false;
            }

            //set to phase 2 transition when she reaches half health
            if (NPC.life <= (NPC.lifeMax / 2) && !Phase2)
            {
                NPC.ai[0] = -2;
                NPC.localAI[0] = 0;
                Phase2 = true;
            }

            switch ((int)NPC.ai[0])
            {
                //go back to sleep
                case -4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1)
                    {
                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;
                    }

                    if (NPC.localAI[0] == 150)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.BackToSleep"), true);
                    }

                    if (NPC.localAI[0] >= 260)
                    {
                        NPC.active = false;
                    }

                    break;
                }

                //ending dialogue and death animation, dialogue only happens on first defeat (not implemented yet)
                case -3: 
                {
                    NPC.localAI[2]++;

                    //kill every single hostile projectile to prevent unfair hits or deaths during the death animation
                    if (NPC.localAI[2] <= 5)
                    {
                        foreach (var Proj in Main.ActiveProjectiles)
                        {
                            if (Proj != null && Proj.hostile && Proj.type != ModContent.ProjectileType<ThornPillarBarrierFloor>() && Proj.type != ModContent.ProjectileType<ThornPillarBarrierSide>()) 
                            {
                                Proj.timeLeft = 2;
                            }
                        }
                    }

                    //ending dialogue
                    if (!Flags.downedDaffodil)
                    {
                        if (NPC.localAI[2] == 120)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatOutro1"), true);
                        }

                        if (NPC.localAI[2] == 260)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatOutro2"), true);
                        }
                    }
                    else
                    {
                        if (NPC.localAI[2] == 120)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeatOutro1"), true);
                        }

                        if (NPC.localAI[2] == 260)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeatOutro2"), true);
                        }
                    }

                    //kill daffodil
                    if (NPC.localAI[2] >= 400)
                    {
                        ActuallyDead = true;
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;
                        NPC.netUpdate = true;
                        player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                    }

                    break;
                }

                //phase two transition, arena spikes
                case -2:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1)
                    {
                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;

                        NPC.netUpdate = true;
                    }

                    //kill every single hostile projectile to prevent unfair hits or deaths during the transition
                    if (NPC.localAI[0] <= 5)
                    {
                        foreach (var Proj in Main.ActiveProjectiles)
                        {
                            if (Proj != null && Proj.hostile)
                            {
                                Proj.timeLeft = 2;
                            }
                        }
                    }
                
                    if (NPC.localAI[0] == 90)
                    {
                        if (!Flags.downedDaffodil)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatPhaseTransition1"), true);
                        }
                        else
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeatPhaseTransition1"), true);
                        }
                    }

                    //spawn permanent thorn pillars that cover the whole arena
                    if (NPC.localAI[0] == 200)
                    {
                        //spawn pillars on the walls
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X - 678, NPC.Center.Y + 400), Vector2.Zero, 
                        ModContent.ProjectileType<ThornPillarBarrierSide>(), NPC.damage, 0f, ai0: new Vector2(0, 32).ToRotation() + MathHelper.Pi, ai1: -16 * 60);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + 672, NPC.Center.Y + 400), Vector2.Zero, 
                        ModContent.ProjectileType<ThornPillarBarrierSide>(), NPC.damage, 0f, ai0: new Vector2(0, 32).ToRotation() + MathHelper.Pi, ai1: -16 * 60);

                        //spawn pillars on the floor
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 385), Vector2.Zero, 
                        ModContent.ProjectileType<ThornPillarBarrierFloor>(), NPC.damage, 0f, ai0: new Vector2(32, 0).ToRotation(), ai1: -16 * 60);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 391), Vector2.Zero, 
                        ModContent.ProjectileType<ThornPillarBarrierFloor>(), NPC.damage, 0f, ai0: new Vector2(-32, 0).ToRotation(), ai1: -16 * 60);
                    }

                    if (NPC.localAI[0] == 360)
                    {
                        if (!Flags.downedDaffodil)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatPhaseTransition2"), true);
                        }
                        else
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeatPhaseTransition2"), true);
                        }
                    }

                    if (NPC.localAI[0] >= 480)
                    {
                        NPC.immortal = false;
                        NPC.dontTakeDamage = false;

                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //spawn intro dialogue
                case -1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 1)
                    {
                        NPC.immortal = true;
                        NPC.dontTakeDamage = true;
                    }

                    if (!Flags.downedDaffodil)
                    {
                        if (NPC.localAI[0] == 120)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatIntro1"), true);
                        }

                        if (NPC.localAI[0] == 240)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatIntro2"), true);
                        }

                        if (NPC.localAI[0] == 360)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatIntro3"), true);
                        }

                        if (NPC.localAI[0] == 480)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatIntro4"), true);
                        }

                        if (NPC.localAI[0] >= 600)
                        {
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;

                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (NPC.localAI[0] == 120)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeatIntro1"), true);
                        }

                        if (NPC.localAI[0] == 240)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeatIntro3"), true);
                        }

                        if (NPC.localAI[0] == 360)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeatIntro2"), true);
                        }

                        if (NPC.localAI[0] >= 420)
                        {
                            NPC.immortal = false;
                            NPC.dontTakeDamage = false;

                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                    }

                    break;
                }

                //fire solar laser barrage at the player
                case 0:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 85)
                    {
                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                            Main.dust[dustEffect].color = Color.Gold;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] >= 85 && NPC.localAI[0] <= 145)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            SoundEngine.PlaySound(SoundID.Item12, NPC.Center);

                            Vector2 ShootSpeed = player.Center - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 25f;

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-5, 5), NPC.Center.Y + 10 + Main.rand.Next(-5, 5)), 
                            ShootSpeed, ModContent.ProjectileType<SolarLaser>(), NPC.damage, 4.5f);
                        }
                    }

                    if (NPC.localAI[0] >= 240)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //raise hands up, then shoot chlorophyll blasts from them (code for this attack is handled in each hand's ai)
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 420)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //create bouncing thorn balls that petrude out thorns
                case 2:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 120)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, NPC.Center);
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 145)
                    {
                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 NPCCenter = new Vector2(NPC.Center.X, NPC.Center.Y + 200);
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPCCenter;
                            Vector2 velocity = dustPos - NPCCenter;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                            Main.dust[dustEffect].color = Main.rand.NextBool() ? Color.Lime : Color.Red;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] == 160 || NPC.localAI[0] == 190 || NPC.localAI[0] == 220)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, NPC.Center);

                        int MaxProjectiles = Main.rand.Next(3, 6);
                        for (int numProj = 0; numProj < MaxProjectiles; numProj++)
                        {
                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 200), 
                            new Vector2(Main.rand.NextFloat(-15f, 15f), Main.rand.NextFloat(-7f, -3f)), ModContent.ProjectileType<ThornBall>(), NPC.damage, 4.5f);
                        }
                    }

                    if (NPC.localAI[0] >= 580)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //hold hands out, and create a bullet hell of flies all over the place
                case 3:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_SkeletonSummoned with { Volume = SoundID.DD2_SkeletonSummoned.Volume * 80f }, NPC.Center);
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 300)
                    {
                        //spawn flies from the left
                        if (Main.rand.NextBool(17))
                        {
                            //shake the screen for funny rumbling effect
                            Screenshake.ShakeScreenWithIntensity(NPC.Center, 3f, 350f);

                            SoundEngine.PlaySound(FlySound, NPC.Center);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X - 800, NPC.Center.Y + Main.rand.Next(-100, 450)), 
                            new Vector2(Main.rand.Next(7, 11), 0), ModContent.ProjectileType<DaffodilFly>(), NPC.damage, 4.5f);
                        }

                        //shoot flies from the right
                        if (Main.rand.NextBool(17))
                        {
                            //shake the screen for funny rumbling effect
                            Screenshake.ShakeScreenWithIntensity(NPC.Center, 3f, 350f);

                            SoundEngine.PlaySound(FlySound, NPC.Center);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + 800, NPC.Center.Y + Main.rand.Next(-100, 450)), 
                            new Vector2(Main.rand.Next(-10, -6), 0), ModContent.ProjectileType<DaffodilFly>(), NPC.damage, 4.5f);
                        }
                    }

                    if (NPC.localAI[0] >= 480)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = Phase2 ? 4 : 5;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //raise hands up, then punch the player (code for this attack is handled in each hand's ai)
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 260)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //drop seeds from the ceiling that spawn thorn pillars 
                case 5:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, NPC.Center);
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 300)
                    {
                        //shake the screen for rumbling effect
                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 3f, 350f);

                        if (NPC.localAI[0] % 10 == 2)
                        {
                            SoundEngine.PlaySound(SeedSpawnSound, NPC.Center);

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-750, 750), NPC.Center.Y - 50), 
                            new Vector2(0, 8), ModContent.ProjectileType<ThornPillarSeed>(), NPC.damage, 4.5f);
                        }
                    }

                    if (NPC.localAI[0] >= 570)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = Phase2 ? 7 : 6;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //fire constant deathray at the player that follows them, unable to go through blocks
                case 6:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 20)
                    {
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X - 470, NPC.Center.Y + 350), Vector2.Zero, ModContent.ProjectileType<TakeCoverTelegraph>(), 0, 0f);
                        NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + 485, NPC.Center.Y + 350), Vector2.Zero, ModContent.ProjectileType<TakeCoverTelegraph>(), 0, 0f);
                    }

                    if (NPC.localAI[0] >= 60 && NPC.localAI[0] < 120)
                    {
                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                            Main.dust[dustEffect].color = Color.Gold;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] == 160)
                    {
                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 12f, 350f);
                                    
                        SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);

                        float rotation = player.Center.X < NPC.Center.X ? -0.035f : 0.035f;

                        float theta = (player.Center - NPC.Center).ToRotation() - MathHelper.ToRadians(rotation == -0.035f ? -35 : 35);

                        NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)), ModContent.ProjectileType<DaffodilBeam>(), NPC.damage + 30, 0f, ai0: NPC.whoAmI, ai1: rotation);
                    }

                    if (NPC.localAI[0] >= 360)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                }
                
                //Save positions, then shoot solar deathrays at them
                case 7:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 30)
                    {
                        for (int savePoints = 0; savePoints < SavePoint.Length; savePoints++)
                        {
                            int positionX = (int)(player.Center.X + Main.rand.Next(-300, 300) + player.velocity.X * 30);
                            int positionY = (int)(player.Center.Y + Main.rand.Next(-200, 200));

                            if (savePoints == 0)
                            {
                                positionX = (int)player.Center.X;
                                positionY = (int)player.Center.Y;
                            }

                            NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(positionX, positionY + 5), Vector2.Zero, ModContent.ProjectileType<SolarDeathbeamTelegraph>(), 0, 0f);

                            SavePoint[savePoints] = new Vector2(positionX, positionY);
                        }

						NPC.netUpdate = true;
                    }

                    if (NPC.localAI[0] >= 30 && NPC.localAI[0] < 70)
                    {
                        int MaxDusts = Main.rand.Next(5, 15);
                        for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                        {
                            Vector2 dustPos = (Vector2.One * new Vector2((float)NPC.width / 3f, (float)NPC.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + NPC.Center;
                            Vector2 velocity = dustPos - NPC.Center;
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 0.1f);
                            Main.dust[dustEffect].color = Color.Gold;
                            Main.dust[dustEffect].noGravity = true;
                            Main.dust[dustEffect].noLight = false;
                            Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                            Main.dust[dustEffect].fadeIn = 1.3f;
                        }
                    }

                    if (NPC.localAI[0] == 90)
                    {
                        Screenshake.ShakeScreenWithIntensity(NPC.Center, 12f, 350f);
                                    
                        SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);

                        for (int i = 0; i < SavePoint.Length; i++)
                        {
                            float theta = (SavePoint[i] - NPC.Center).ToRotation();

                            NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)), ModContent.ProjectileType<DaffodilBeam>(), NPC.damage + 30, 0f, ai0: NPC.whoAmI);
                        }
                    }
                    
                    if (NPC.localAI[0] >= 290)
                    {
                        NPC.localAI[0] = 0;
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
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BossBagDaffodil>()));
            
			//master relic and pet
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<DaffodilRelicItem>()));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<SmallDaffodil>(), 4));

            //weapon drops
            int[] MainItem = new int[] 
			{ 
				ModContent.ItemType<DaffodilBlade>(),
                ModContent.ItemType<DaffodilBow>(), 
				ModContent.ItemType<DaffodilRod>(),
                ModContent.ItemType<DaffodilStaff>()
			};

            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1, MainItem));

            //farmer glove
			notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<FarmerGlove>(), 2));

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DaffodilMask>(), 7));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DaffodilTrophyItem>(), 10));

            //pollen bloom seed, drop directly from the boss
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpringSeed>()));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            for (int numPlayer = 0; numPlayer <= Main.maxPlayers; numPlayer++)
            {
                if (Main.player[numPlayer].active && !Main.player[numPlayer].GetModPlayer<BloomBuffsPlayer>().UnlockedSlot3)
                {
                    int newItem = Item.NewItem(NPC.GetSource_DropAsItem(), NPC.Hitbox, ModContent.ItemType<Slot3Unlocker>());

                    if (Main.netMode == NetmodeID.MultiplayerClient && newItem >= 0)
                    {
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, newItem, 1f);
                    }
                }
            }

            if (!Flags.downedDaffodil)
            {
                Flags.GuaranteedRaveyard = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }

            NPC.SetEventFlagCleared(ref Flags.downedDaffodil, -1);

            if (!MenuSaveSystem.hasDefeatedDaffodil)
            {
                MenuSaveSystem.hasDefeatedDaffodil = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ModContent.ItemType<CranberryJuice>();
		}
    }
}