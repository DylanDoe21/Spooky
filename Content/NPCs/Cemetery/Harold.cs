using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.Cemetery.Projectiles;

namespace Spooky.Content.NPCs.Cemetery
{
    public class Harold : ModNPC
    {
        float MoveSpeedX = 0;
		float MoveSpeedY = 0;
        
        float addedStretch = 0f;
		float stretchRecoil = 0f;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/CamelColonelBestiary",
				Position = new Vector2(8f, 20f),
              	PortraitPositionXOverride = 0f,
              	PortraitPositionYOverride = 0f
            };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //ints
            writer.Write(MoveSpeedX);
            writer.Write(MoveSpeedY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //ints
            MoveSpeedX = reader.ReadSingle();
            MoveSpeedY = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 3200;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 80;
			NPC.height = 168;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit54 with { Pitch = -1f };
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Harold"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;

			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//draw npc manually for stretching
			spriteBatch.Draw(NPCTexture.Value, new Vector2(NPC.Center.X, NPC.Center.Y + 80) - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, NPC.height), scaleStretch, effects, 0f);

			return false;
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
		{
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.015f;

            //stretch stuff
			if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.05f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            if (NPC.ai[1] == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int Hand1 = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<HaroldHandLeft>(), ai3: NPC.whoAmI);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: Hand1);
                    }

                    int Hand2 = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<HaroldHandRight>(), ai3: NPC.whoAmI);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: Hand2);
                    }
                }

                NPC.ai[1]++;
                NPC.netUpdate = true;
            }

            NPC.ai[0]++;
            if (NPC.ai[0] <= 360)
            {
                float lifeRatio = (float)(NPC.lifeMax / NPC.life);

                if (lifeRatio >= 14)
                {
                    lifeRatio = 14;
                }
                
                float MaxSpeedX = 2f + lifeRatio;
                float AccelerationX = 0.5f + lifeRatio;

                float MaxSpeedY = 3;

                //flies to players X position
                if (NPC.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeedX) 
                {
                    MoveSpeedX -= AccelerationX;
                }
                else if (NPC.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeedX)
                {
                    MoveSpeedX += AccelerationX;
                }

                NPC.velocity.X += MoveSpeedX * 0.01f;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeedX, MaxSpeedX);
                
                //flies to players Y position
                if (NPC.Center.Y >= player.Center.Y - 20 && MoveSpeedY >= -MaxSpeedY)
                {
                    MoveSpeedY -= 0.5f;
                }
                else if (NPC.Center.Y <= player.Center.Y - 20 && MoveSpeedY <= MaxSpeedY)
                {
                    MoveSpeedY += 0.5f;
                }

                NPC.velocity.Y += MoveSpeedY * 0.1f;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeedY, MaxSpeedY);
            }
            else
            {
                NPC.velocity *= 0.95f;

                if (NPC.ai[0] % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item103 with { Pitch = -0.5f, Volume = 0.5f }, NPC.Center);
                    SoundEngine.PlaySound((Main.rand.NextBool() ? SoundID.Zombie53 : SoundID.Zombie54), NPC.Center);

                    stretchRecoil = 0.5f;

                    Vector2 Velocity = new Vector2(0, 15).RotatedByRandom(MathHelper.ToRadians(25));
                    NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Bottom.Y), Velocity, ModContent.ProjectileType<HaroldBolt>(), NPC.damage, 4.5f);
                }

                if (NPC.ai[0] >= 480)
                {
                    NPC.ai[0] = 0;
                    NPC.netUpdate = true;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
            }
        }
    }
}