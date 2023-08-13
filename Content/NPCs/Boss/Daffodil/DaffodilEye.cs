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
using Spooky.Content.NPCs.Boss.Daffodil.Projectiles;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
    [AutoloadBossHead]
    public class DaffodilEye : ModNPC
    {
        public bool Phase2 = false;
        public bool SpawnedHands = false;

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
            //save point for solar beams
            for (int i = 0; i <= SavePoint.Length; i++)
            {
                writer.Write(SavePoint[i].X);
                writer.Write(SavePoint[i].Y);
            }

            //ints
            writer.Write(SavePlayerPosition.X);
            writer.Write(SavePlayerPosition.Y);

            //bools
            writer.Write(Phase2);
            writer.Write(SpawnedHands);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //save point for solar beams
            for (int i = 0; i <= SavePoint.Length; i++)
            {
                SavePoint[i].X = reader.ReadInt32();
                SavePoint[i].Y = reader.ReadInt32();
            }

            //ints
            SavePlayerPosition.X = reader.ReadInt32();
            SavePlayerPosition.Y = reader.ReadInt32();

            //bools
            Phase2 = reader.ReadBoolean();
            SpawnedHands = reader.ReadBoolean();

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

            if (NPC.frame.Y != 0)
            {
                spriteBatch.Draw(texture, drawPos, null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            //frame number reminder
            //0 = fully closed 
            //1 = slightly open
            //2 = open
            //3 = wide open

            //open eye
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
            //use wide open eye when shooting solar lasers
            else if (NPC.ai[0] == 0 && NPC.localAI[0] >= 60 && NPC.localAI[0] <= 155)
            {
                NPC.frame.Y = frameHeight * 3;
            }
            else if (NPC.ai[0] == 3 && NPC.localAI[0] >= 120 && NPC.localAI[0] < 400)
            {
                NPC.frame.Y = frameHeight * 0;
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

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            int Damage = Main.masterMode ? 60 / 3 : Main.expertMode ? 40 / 2 : 30;

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

            if (player.dead)
            {
                NPC.localAI[2]++;

                if (NPC.localAI[2] >= 180)
                {
                    NPC.active = false;
                }
            }

            //set to phase 2 transition when she reaches half health
            if (NPC.life <= (NPC.lifeMax / 2) && !Phase2)
            {
                NPC.ai[0] = -2;
                Phase2 = true;
            }

            switch ((int)NPC.ai[0])
            {
                //phase transition
                case -2:
                {
                    NPC.localAI[0]++;
                
                    if (NPC.localAI[0] == 60)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PhaseTransition1"), true);
                    }

                    //spawn permanent thorn pillars that cover the whole arena
                    if (NPC.localAI[0] == 120)
                    {

                    }

                    if (NPC.localAI[0] == 240)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PhaseTransition2"), true);
                    }

                    if (NPC.localAI[0] >= 320)
                    {
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

                    if (!Flags.downedDaffodil)
                    {
                        if (NPC.localAI[0] == 120)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeat1"), true);
                        }

                        if (NPC.localAI[0] == 240)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeat2"), true);
                        }

                        if (NPC.localAI[0] == 360)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeat3"), true);
                        }

                        if (NPC.localAI[0] == 480)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PreDefeat4"), true);
                        }

                        if (NPC.localAI[0] >= 600)
                        {
                            NPC.localAI[0] = 0;
                            NPC.ai[0]++;
                            NPC.netUpdate = true;
                        }
                    }
                    else
                    {
                        if (NPC.localAI[0] == 120)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeat1"), true);
                        }

                        if (NPC.localAI[0] == 240)
                        {
                            CombatText.NewText(NPC.getRect(), Color.Gold, Language.GetTextValue("Mods.Spooky.Dialogue.Daffodil.PostDefeat2"), true);
                        }

                        if (NPC.localAI[0] >= 360)
                        {
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
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                            Main.dust[dustEffect].color = Color.Gold;
                            Main.dust[dustEffect].scale = 0.1f;
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
                            int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                            Main.dust[dustEffect].color = Main.rand.NextBool() ? Color.Lime : Color.Red;
                            Main.dust[dustEffect].scale = 0.1f;
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

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-700, 700), NPC.Center.Y - 50,
                            0, 8, ModContent.ProjectileType<ThornPillarSeed>(), Damage, 0, NPC.target, 0, 0);
                        }
                    }

                    if (NPC.localAI[0] >= 500)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }
                
                //Save positions, then shoot solar deathrays at them
                case 5:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] == 120)
                    {
                        for (int savePoints = 0; savePoints < SavePoint.Length; savePoints++)
                        {
                            int positionX = (int)player.Center.X + Main.rand.Next(-450, 450);
                            int positionY = (int)player.Center.Y + Main.rand.Next(-200, 100);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), positionX, positionY, 0, 0, 
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
                            Vector2 ShootSpeed = NPC.Center - SavePoint[i];
                            ShootSpeed.Normalize();
                            ShootSpeed *= -75f;

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

                //fire constant deathray at the player that follows them, unable to go through blocks
                case 6:
                {
                    break;
                }
            }
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