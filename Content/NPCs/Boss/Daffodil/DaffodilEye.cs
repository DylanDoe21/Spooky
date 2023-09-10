using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Biomes;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    [AutoloadBossHead]
    public class DaffodilEye : ModNPC
    {
        public bool Phase2 = false;
        public bool SpawnedHands = false;
        public bool ActuallyDead = false;

        Vector2[] SavePoint = new Vector2[5];
        Vector2 SavePlayerPosition;

        public static readonly SoundStyle SeedSpawnSound = new("Spooky/Content/Sounds/Daffodil/SeedSpawn", SoundType.Sound);
        public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBone/BigBoneMagic", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle MagicCastSound2 = new("Spooky/Content/Sounds/BigBone/BigBoneMagic2", SoundType.Sound) { PitchVariance = 0.6f };
        public static readonly SoundStyle FlySound = new("Spooky/Content/Sounds/FlyBuzzing", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Daffodil/DaffodilBC",
                Position = new Vector2(1f, 30f),
                PortraitPositionXOverride = 2f,
                PortraitPositionYOverride = 30f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] 
                {
                    BuffID.Confused, 
                    BuffID.Poisoned,
                    BuffID.Venom
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffData);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(SavePlayerPosition.X);
            writer.Write(SavePlayerPosition.Y);

            //bools
            writer.Write(Phase2);
            writer.Write(SpawnedHands);
            writer.Write(ActuallyDead);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            SavePlayerPosition.X = reader.ReadInt32();
            SavePlayerPosition.Y = reader.ReadInt32();

            //bools
            Phase2 = reader.ReadBoolean();
            SpawnedHands = reader.ReadBoolean();
            ActuallyDead = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 22000;
            NPC.damage = 45;
            NPC.defense = 35;
            NPC.width = 58;
            NPC.height = 58;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
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
            NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
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

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/Daffodil/DaffodilEyePupil");

            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = new Vector2(NPC.Center.X, NPC.Center.Y + 2) - screenPos;

            float lookX = (player.Center.X - NPC.Center.X) * 0.015f;
            float lookY = (player.Center.Y - NPC.Center.Y) * 0.01f;

            drawPos.X += lookX;
            drawPos.Y += lookY;

            //do not draw the pupil when the eye is closed
            if (NPC.frame.Y != 0)
            {
                spriteBatch.Draw(texture, drawPos, null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }
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
            //close eye when despawning
            else if (NPC.ai[0] == -4)
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
            //slightly open eye 
            else if (NPC.ai[0] == -5)
            {
                NPC.frame.Y = frameHeight * 1;
            }
            else if (NPC.ai[0] == 0 && NPC.localAI[0] >= 60 && NPC.localAI[0] <= 155)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.ai[0] == 3 && NPC.localAI[0] >= 120 && NPC.localAI[0] < 400)
            {
                NPC.frame.Y = frameHeight * 0;
            }
            else if (NPC.ai[0] == 4 && NPC.localAI[0] >= 120 && NPC.localAI[0] <= 300)
            {
                NPC.frame.Y = frameHeight * 0;
            }
            else if (NPC.ai[0] == 5 && NPC.localAI[0] > 160)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.ai[0] == 6 && NPC.localAI[0] >= 160 && NPC.localAI[0] <= 240)
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
            
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 70 / 3 : Main.expertMode ? 50 / 2 : 35;

            Lighting.AddLight(NPC.Center, 0.5f, 0.45f, 0f);

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

            //despawn if the player is dead or they leave the catacombs
            if (player.dead)
            {
                NPC.ai[0] = -4;
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
                case -5:
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

                //despawning
                case -4:
                {
                    NPC.localAI[1]++;
                    
                    if (NPC.localAI[1] == 120)
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
                        for (int k = 0; k < Main.projectile.Length; k++)
                        {
                            if (Main.projectile[k].active && Main.projectile[k].hostile) 
                            {
                                Main.projectile[k].Kill();
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
                        for (int k = 0; k < Main.projectile.Length; k++)
                        {
                            if (Main.projectile[k].active && Main.projectile[k].hostile) 
                            {
                                Main.projectile[k].Kill();
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
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X - 680, NPC.Center.Y + 400, 0, 0, ModContent.ProjectileType<ThornPillarBarrierSide>(), 
                        Damage + 20, 0, Main.myPlayer, new Vector2(0, 32).ToRotation() + MathHelper.Pi, -16 * 60);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 685, NPC.Center.Y + 400, 0, 0, ModContent.ProjectileType<ThornPillarBarrierSide>(), 
                        Damage + 20, 0, Main.myPlayer, new Vector2(0, 32).ToRotation() + MathHelper.Pi, -16 * 60);

                        //spawn pillars on the floor
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 385, 0, 0, ModContent.ProjectileType<ThornPillarBarrierFloor>(), 
                        Damage + 20, 0, Main.myPlayer, new Vector2(32, 0).ToRotation(), -16 * 60);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 391, 0, 0, ModContent.ProjectileType<ThornPillarBarrierFloor>(), 
                        Damage + 20, 0, Main.myPlayer, new Vector2(-32, 0).ToRotation(), -16 * 60);
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

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-5, 5), NPC.Center.Y + 10 + Main.rand.Next(-5, 5), 
                            ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<SolarLaser>(), Damage, 0f, Main.myPlayer);
                        }
                    }

                    if (NPC.localAI[0] >= 200)
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

                    if (NPC.localAI[0] >= 320)
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
                        SoundEngine.PlaySound(MagicCastSound, NPC.Center);
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

                        int NumProjectiles = Main.rand.Next(3, 6);
                        for (int numProjs = 0; numProjs < NumProjectiles; numProjs++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y + 200, Main.rand.NextFloat(-15f, 15f), 
                            Main.rand.NextFloat(-5f, -2f), ModContent.ProjectileType<ThornBall>(), Damage, 2, NPC.target, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 480)
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
                        //shake the screen for funny rumbling effect
                        SpookyPlayer.ScreenShakeAmount = 5;

                        //spawn flies from the left
                        if (Main.rand.NextBool(17))
                        {
                            SoundEngine.PlaySound(FlySound, NPC.Center);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X - 800, NPC.Center.Y + Main.rand.Next(-65, 420),
                            Main.rand.Next(7, 10), 0, ModContent.ProjectileType<DaffodilFly>(), Damage, 0, NPC.target, 0, 0);
                        }

                        //shoot flies from the right
                        if (Main.rand.NextBool(17))
                        {
                            SoundEngine.PlaySound(FlySound, NPC.Center);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 800, NPC.Center.Y + Main.rand.Next(-65, 420),
                            Main.rand.Next(-10, -7), 0, ModContent.ProjectileType<DaffodilFly>(), Damage, 0, NPC.target, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 480)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //drop seeds from the ceiling that spawn thorn pillars 
                case 4:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 60)
                    {
                        SoundEngine.PlaySound(MagicCastSound2, NPC.Center);
                    }

                    if (NPC.localAI[0] >= 120 && NPC.localAI[0] < 300)
                    {
                        //shake the screen for funny rumbling effect
                        SpookyPlayer.ScreenShakeAmount = 5;

                        if (NPC.localAI[0] % 10 == 2)
                        {
                            SoundEngine.PlaySound(SeedSpawnSound, NPC.Center);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-750, 750), NPC.Center.Y - 50,
                            0, 8, ModContent.ProjectileType<ThornPillarSeed>(), Damage, 0, NPC.target, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 500)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = Phase2 ? 6 : 5;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //fire constant deathray at the player that follows them, unable to go through blocks
                case 5:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 20)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X - 450, NPC.Center.Y + 350, 
                        0, 0, ModContent.ProjectileType<TakeCoverTelegraph>(), 0, 0f, Main.myPlayer);

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + 465, NPC.Center.Y + 350, 
                        0, 0, ModContent.ProjectileType<TakeCoverTelegraph>(), 0, 0f, Main.myPlayer);
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
                        SpookyPlayer.ScreenShakeAmount = 12;
                                    
                        SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
                    }

                    if (NPC.localAI[0] >= 160 && NPC.localAI[0] <= 240)
                    {
                        Vector2 ShootSpeed = player.Center - NPC.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 45f;

                        int laserbeam = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-5, 5), NPC.Center.Y + 10 + Main.rand.Next(-5, 5),
                        ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<SolarDeathbeam>(), Damage + 30, 0f, Main.myPlayer);
                        Main.projectile[laserbeam].tileCollide = true;
                    }

                    if (NPC.localAI[0] >= 280)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                }
                
                //Save positions, then shoot solar deathrays at them
                case 6:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 120)
                    {
                        for (int savePoints = 0; savePoints < SavePoint.Length; savePoints++)
                        {
                            int positionX = (int)player.Center.X + Main.rand.Next(-300, 300);
                            int positionY = (int)player.Center.Y + Main.rand.Next(-300, 100);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), positionX, positionY + 5, 0, 0, 
                            ModContent.ProjectileType<SolarDeathbeamTelegraph>(), 0, 0f, Main.myPlayer);

                            SavePoint[savePoints] = new Vector2(positionX, positionY);
                        }
                    }

                    if (NPC.localAI[0] == 160)
                    {
                        SpookyPlayer.ScreenShakeAmount = 12;
                                    
                        SoundEngine.PlaySound(SoundID.Zombie104, NPC.Center);
                    }

                    if (NPC.localAI[0] >= 160 && NPC.localAI[0] <= 240)
                    {
                        for (int i = 0; i < SavePoint.Length; i++)
                        {
                            Vector2 ShootSpeed = SavePoint[i] - NPC.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed *= 75f;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-5, 5), NPC.Center.Y + 10 + Main.rand.Next(-5, 5),
                            ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<SolarDeathbeam>(), Damage + 30, 0f, Main.myPlayer);
                        }
                    }

                    if (NPC.localAI[0] >= 280)
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

            //drop boss mask
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<DaffodilMask>(), 7));

            //trophy always drops directly from the boss
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<DaffodilTrophyItem>(), 10));

            npcLoot.Add(notExpertRule);
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref Flags.downedDaffodil, -1);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}