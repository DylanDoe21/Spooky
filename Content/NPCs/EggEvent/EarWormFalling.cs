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

namespace Spooky.Content.NPCs.EggEvent
{
    public class EarWormFalling : ModNPC
    {
        private bool segmentsSpawned;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 52;
            NPC.height = 48;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.NPCHit18;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Rectangle frame = new Rectangle(0, NPC.frame.Y, NPCTexture.Width(), NPCTexture.Height() / Main.npcFrameCount[NPC.type]);
            Vector2 origin = new Vector2(NPCTexture.Width() * 0.5f, NPCTexture.Height() / Main.npcFrameCount[NPC.type] * 0.5f);
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - Main.screenPosition, frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, effects, 0);
            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition, frame, NPC.GetAlpha(Color.White), NPC.rotation, origin, NPC.scale, effects, 0);

            return false;
        }
        
        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + 1.57f;

            NPC.spriteDirection = NPC.velocity.X < 0 ? -1 : 1;

            NPC.velocity.Y = NPC.velocity.Y + 0.15f;

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!segmentsSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int numSegments = 0; numSegments < 20; numSegments++)
                    {
                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), 
                        ModContent.NPCType<EarWormFallingBody>(), NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].lifeMax = NPC.lifeMax;
                        Main.npc[latestNPC].realLife = NPC.whoAmI;
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }

                    segmentsSpawned = true;
                    NPC.netUpdate = true;
                }
            }

            NPC.ai[0]++;
            if (NPC.ai[0] == 1)
            {
                NPC.velocity.X = Main.rand.Next(-5, 6);
            }
            if (NPC.ai[0] > 10 && NPCGlobalHelper.IsCollidingWithFloor(NPC))
            {
                NPC.velocity.X = 0;
                NPC.velocity.Y = 18;
                NPC.alpha += 25;

                if (NPC.ai[2] == 0)
                {
                    SoundEngine.PlaySound(SoundID.Dig, NPC.Center);
                    NPC.ai[2]++;
                }

                NPC.ai[1]++;
                if (NPC.ai[1] >= 20)
                {
                    Vector2 center = NPC.Center;

                    int numtries = 0;
                    int PositionX = (int)(NPC.Center.X / 16);
					int PositionY = (int)(NPC.Center.Y / 16);

                    while (PositionY < Main.maxTilesY - 10 && Main.tile[PositionX, PositionY] != null && !WorldGen.SolidTile2(PositionX, PositionY) && Main.tile[PositionX - 1, PositionY] != null && 
                    !WorldGen.SolidTile2(PositionX - 1, PositionY) && Main.tile[PositionX + 1, PositionY] != null && !WorldGen.SolidTile2(PositionX + 1, PositionY))
					{
						PositionY++;
						center.Y = PositionY * 16;
					}
					while ((WorldGen.SolidOrSlopedTile(PositionX, PositionY) || WorldGen.SolidTile2(PositionX, PositionY)) && numtries < 25)
					{
						numtries++;
						PositionY--;
						center.Y = PositionY * 16;
					}

                    SoundEngine.PlaySound(SoundID.Dig, center);
                    SoundEngine.PlaySound(SoundID.NPCDeath12, center);

                    for (int numDusts = 0; numDusts < 25; numDusts++)
                    {
                        Dust dust = Dust.NewDustPerfect(new Vector2((int)NPC.Center.X, (int)center.Y + 20), DustID.Blood, new Vector2(Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f)));
						dust.noGravity = true;
                        dust.scale = 3f;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int EarWorm = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)center.Y + 20, ModContent.NPCType<EarWormBase>());
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: EarWorm);
                        }
                    }

                    NPC.active = false;
                }
            }
        }
    }
}