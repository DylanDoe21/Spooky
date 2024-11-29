using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Minibiomes.TarPits
{
    public class TarBlobSmallMouth : ModNPC  
    {
        int MoveSpeedX;
        int MoveSpeedY;

        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/TarBlobSmallMouthBestiary",
                Position = new Vector2(0f, 10f),
                PortraitPositionYOverride = 10f
            };
        }
        
        public override void SetDefaults()
        {
            NPC.lifeMax = 100;
            NPC.damage = 25;
            NPC.defense = 5;
            NPC.width = 50;
			NPC.height = 50;
            NPC.noTileCollide = false;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.TarPitsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TarBlobSmallMouth"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.TarPitsBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC Parent = Main.npc[(int)NPC.ai[0]];

			if (Parent.active && Parent.type == ModContent.NPCType<TarBlobSmall>())
            {
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/TarPits/TarBlobSmallChain");

				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NPC.Center - new Vector2(0 * (flip ? -1 : 1), 12).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 12;

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

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, NPC.GetAlpha(color), rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

            //draw the npc itself
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			return false;
		}

        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[0]];

            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (Parent.active && Parent.type == ModContent.NPCType<TarBlobSmall>())
            {
                ChasePlayer(player, Parent, 3, 0.01f);
            }
            else
            {
                NPC.active = false;
            }
        }

        public void ChasePlayer(Player player, NPC Parent, int MaxSpeed, float Acceleration)
        {
            //rotation
            Vector2 vector = new(NPC.Center.X, NPC.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //fly towards the player
            if (player.Distance(Parent.Center) <= 230f && !player.dead)
            {
                Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 5;
                NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
            }
            //if too far away, move back to the parent stem
            else
            {
                //flies to parent X position
                if (NPC.Center.X >= Parent.Center.X && MoveSpeedX >= -MaxSpeed) 
                {
                    MoveSpeedX--;
                }
                else if (NPC.Center.X <= Parent.Center.X && MoveSpeedX <= MaxSpeed)
                {
                    MoveSpeedX++;
                }

                NPC.velocity.X += MoveSpeedX * Acceleration;
                NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -MaxSpeed, MaxSpeed);
                
                //flies to parent Y position
                if (NPC.Center.Y >= Parent.Center.Y - 75 && MoveSpeedY >= -MaxSpeed)
                {
                    MoveSpeedY--;
                }
                else if (NPC.Center.Y <= Parent.Center.Y - 75 && MoveSpeedY <= MaxSpeed)
                {
                    MoveSpeedY++;
                }

                NPC.velocity.Y += MoveSpeedY * Acceleration;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, -MaxSpeed, MaxSpeed);

                NPC.velocity *= 0.97f;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/OrchidPinkBigGore" + numGores).Type);
                    }
                }
            }
        }
    }
}