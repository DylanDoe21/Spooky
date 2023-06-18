using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
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
    public class DaffodilEye : ModNPC
    {
        public bool SpawnedHands = false;

        Vector2 SavePlayerPosition;

        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/Boss/Daffodil/DaffodilBC",
                Position = new Vector2(1f, 0f),
                PortraitPositionXOverride = 2f,
                PortraitPositionYOverride = 0f
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
            //writer.Write(INT);

            //bools
            writer.Write(SpawnedHands);

            //local ai
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            //INT = reader.ReadInt32();

            //bools
            SpawnedHands = reader.ReadBoolean();

            //local ai
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 18000;
            NPC.damage = 0;
            NPC.defense = 35;
            NPC.width = 58;
            NPC.height = 56;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
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

            spriteBatch.Draw(texture, drawPos, null, drawColor, NPC.rotation, drawOrigin, NPC.scale, SpriteEffects.None, 0f);
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
                
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[2]);
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[3]);
                }

                SpawnedHands = true;
            }

            if (player.dead)
            {
                NPC.localAI[2]++;

                if (NPC.localAI[2] >= 180)
                {
                    NPC.active = false;
                }
            }

            switch ((int)NPC.ai[0])
            {
                //spawn telegraph on the player, then shoot a solar laser barrage
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

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X + Main.rand.Next(-20, 20), NPC.Center.Y + Main.rand.Next(-20, 20), 
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

                //raise hands up, then shoot chlorophyll blasts from them (code this attack is handled in each hands ai)
                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] >= 320)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0] = 0;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                //thorn bulbs
                //shoot out thorn pods that each spawn a thorn when they hit the ground, thorns will linger for a bit, and when hit the thorn will die
                //thorns will inflict bleeding and poison on contact
                case 2:
                {
                    break;
                }

                //seed drop
                //drop seeds randomly around the arena that turn into short lived thorn pillars that form upward upon hitting the floor
                //based on that one kirby boss attack
                case 3:
                {
                    break;
                }

                //fly swarm
                //boss roars or makes a sound, then flies begin flying around the arena in a straight line in a random bullet hell fashion (like supreme calamitas)
                //omega flowey attack reference, but also works because the catacombs is filled with flies due to being a burial for the dead
                case 4:
                {
                    break;
                }

                //hand attack
                //either swipe at the player or grab the player and pull them up, can be avoided by going under the outposts in the arena
                case 5:
                {
                    break;
                }

                //solar beam
                //shoot sweeping solar beams across the arena
                //the only way to avoid this attack is by hiding under the outposts in the arena
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