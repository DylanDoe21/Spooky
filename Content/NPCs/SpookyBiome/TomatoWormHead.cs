using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class TomatoWormHead : ModNPC
    {
        private bool segmentsSpawned;

        Vector2 SaveGroundPosition;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/TomatoWormBestiary",
                Position = new Vector2(-12f, 35f),
            };

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(segmentsSpawned);

            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            segmentsSpawned = reader.ReadBoolean();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 350;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 42;
            NPC.height = 54;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.Item178 with { Pitch = -0.5f };
			NPC.DeathSound = SoundID.Item178 with { Pitch = -1.5f, Volume = 2f };
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.9f * bossAdjustment);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TomatoWorm"),
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.BloodMoon,
				new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiomeBloodMoon_Background", Color.White)
			});
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            Rectangle frame = new Rectangle(0, NPC.frame.Y, NPCTexture.Width(), NPCTexture.Height() / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.spriteBatch.Draw(NPCTexture.Value, NPC.Center - Main.screenPosition, frame, drawColor, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }
        
        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            //despawn if all players are dead or not in the biome
            if (player.dead)
            {
                NPC.localAI[1]++;
                if (NPC.localAI[1] >= 75)
                {
                    NPC.velocity.Y = 35;
                }

                if (NPC.localAI[1] >= 240)
                {
                    NPC.active = false;
                }
            }

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<TomatoWormBody1>(), NPC.whoAmI, 0, latestNPC);                   
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    for (int numSegments = 0; numSegments < 6; numSegments++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<TomatoWormBody2>(), NPC.whoAmI, 0, latestNPC);                   
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }
                    
                    latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                    ModContent.NPCType<TomatoWormTail>(), NPC.whoAmI, 0, latestNPC);
                    Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                    Main.npc[latestNPC].realLife = NPC.whoAmI;
                    Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                    NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            switch ((int)NPC.ai[0])
            {
                //burrow in the ground
                case 0:
                {
                    NPC.localAI[0]++;

                    Vector2 center = player.Center;
                    int numtries = 0;
                    int x = (int)(center.X / 16);
                    int y = (int)(center.Y / 16);
                    while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                    Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                    {
                        y++;
                        center.Y = y * 16;
                    }
                    while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                    {
                        numtries++;
                        y--;
                        center.Y = y * 16;
                    }

                    if (numtries >= 10)
                    {
                        break;
                    }

                    Vector2 GoTo = new Vector2(center.X, center.Y + 250);

                    if (NPC.Distance(GoTo) >= 50)
                    {
                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 3f, 8f);
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
                    }

                    if (NPC.localAI[0] >= 300)
                    {
                        NPC.localAI[0] = 0;
                        NPC.ai[0]++;

                        NPC.netUpdate = true;
                    }

                    break;
                }

                case 1:
                {
                    NPC.localAI[0]++;

                    if (NPC.localAI[0] <= 70)
                    {
                        NPC.velocity = Vector2.Zero;
                        
                        Vector2 RotateTowards = Vector2.Zero;

                        float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X);
                        float RotateSpeed = 0.05f;

                        NPC.rotation = NPC.rotation.AngleTowards(RotateDirection, RotateSpeed);

                        if (NPC.localAI[0] % 10 == 0)
                        {
                            Vector2 center = new Vector2(NPC.Center.X, player.Center.Y);
                            int numtries = 0;
                            int x = (int)(center.X / 16);
                            int y = (int)(center.Y / 16);
                            while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && 
                            Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y)) 
                            {
                                y++;
                                center.Y = y * 16;
                            }
                            while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10) 
                            {
                                numtries++;
                                y--;
                                center.Y = y * 16;
                            }

                            if (numtries >= 10)
                            {
                                break;
                            }

                            SaveGroundPosition = center;

                            for (int numDusts = 0; numDusts <= 3; numDusts++)
                            {
                                Dust dust = Dust.NewDustDirect(SaveGroundPosition + new Vector2(0, 5), 1, 1, DustID.AmberBolt, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, Color.White, 2.5f);
                                dust.noGravity = true;
                            }
                        }
                    }

                    if (NPC.localAI[0] == 70)
                    {
                        NPC.velocity.Y = -16;
                    }

                    if (NPC.localAI[0] >= 70 && NPC.localAI[1] == 0)
                    {
                        if (NPC.Center.Y <= SaveGroundPosition.Y)
                        {
                            SoundEngine.PlaySound(SoundID.Dig, NPC.Center);

                            Screenshake.ShakeScreenWithIntensity(NPC.Center, 7f, 350f);
						    SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);

                            for (int numDusts = 0; numDusts <= 25; numDusts++)
                            {
                                Dust dust = Dust.NewDustDirect(NPC.BottomLeft, NPC.width, 1, DustID.AmberBolt, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-15f, -10f), 50, Color.White, 2.5f);
                                dust.noGravity = true;
                            }

                            NPC.localAI[1]++;
                        }
                    }

                    if (NPC.localAI[0] >= 110)
                    {
                        NPC.velocity.Y += 0.35f;

                        Vector2 GoTo = new Vector2(player.Center.X, NPC.Center.Y + 250);
                        float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5f, 15f);
                        NPC.velocity.X = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f).X;
                    }

                    if (NPC.localAI[0] >= 220)
                    {
                        NPC.localAI[0] = 0;
                        NPC.localAI[1] = 0;
                        NPC.ai[0] = 0;

                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public override bool CheckDead()
        {
            if (Main.netMode != NetmodeID.Server) 
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity / 2, ModContent.Find<ModGore>("Spooky/TomatoWormGore").Type);
            }

            return true;
        }
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.2f;
            return null;
        }
    }
}