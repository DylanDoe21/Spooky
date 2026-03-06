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
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Food;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class TripletsEyeGreen : ModNPC
    {
        float DistFromParent = 75f;
        float SaveRotation;

        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1500;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 28;
            NPC.height = 34;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Y = (int)NPC.ai[2] * frameHeight;
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawBody(false, NPC.ai[2] == 0);

			return true;
		}

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public void DrawBody(bool SpawnGore, bool Purple)
		{
			NPC Parent = Main.npc[(int)NPC.ai[3]];

			if (Parent.active && Parent.type == ModContent.NPCType<TripletsBody>() && !SpawnGore)
			{
                Texture2D ChainTexture;

                if (Purple)
                {
				    ChainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TripletsChainPurple").Value;
                }
                else
                {
                    ChainTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyHell/TripletsChainRed").Value;
                }

				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, 7);
				Vector2 myCenter = NPC.Center - new Vector2(2, 10).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(0, 45).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 8;

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

					Main.spriteBatch.Draw(ChainTexture, drawPos2 - Main.screenPosition, new Rectangle(0, 0, 14, 14), NPC.GetAlpha(color), rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)ChainTexture.Width, 1), SpriteEffects.None, 0f);
				}
			}

			if (SpawnGore)
			{
				bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 myCenter = NPC.Center - new Vector2(2, 10).RotatedBy(NPC.rotation);
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2(0, 45).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 8;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					
					if (Main.netMode != NetmodeID.Server)
					{
                        if (Purple)
                        {
                            Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/TripletsChainPurpleGore").Type);
                        }
                        else
                        {
                            Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/TripletsChainRedGore").Type);
                        }
					}
				}
			}
		}
        
        public override void AI()
		{
            NPC Parent = Main.npc[(int)NPC.ai[3]];
            Player player = Main.player[Parent.target];

            NPC.spriteDirection = NPC.direction;

            Vector2 RotateTowards = player.Center - NPC.Center;

            float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X) + 4.71f;
            float RotateSpeed = 0.05f;

            NPC.rotation = NPC.rotation.AngleTowards(RotateDirection - MathHelper.TwoPi, RotateSpeed);
            
            Vector2 destination = Parent.Center + (MathHelper.TwoPi * (NPC.ai[1] / 3) - MathHelper.PiOver2 + NPC.localAI[0]).ToRotationVector2() * DistFromParent;
            NPC.position = destination - (NPC.Size / 2);

            if (Parent.localAI[0] < 180)
            {
                NPC.localAI[0] -= MathHelper.ToRadians(0.5f);
            }

            if (NPC.Distance(destination) >= 50)
            {
                Vector2 GoTo = destination;

                float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 14, 22);
                NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
            }
            else
            {
                NPC.velocity *= 0.95f;
            }

            if (Parent.localAI[0] == 225)
            {   
                SaveRotation = NPC.rotation;
                SavePlayerPosition = player.Center;
            }

            if (Parent.localAI[0] > 225)
            {
                NPC.rotation = SaveRotation;
            }

            if (Parent.localAI[0] % 10 == 0 && Parent.localAI[0] >= 180 && Parent.localAI[0] <= 240)
            {
                int DustType = DustID.GreenTorch;

                if (NPC.type == ModContent.NPCType<TripletsEyePurple>())
                {
                    DustType = DustID.PurpleTorch;
                }
                if (NPC.type == ModContent.NPCType<TripletsEyeRed>())
                {
                    DustType = DustID.RedTorch;
                }

                int Distance = Main.rand.Next(50, 76);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 offset = new Vector2();
                    double angle = Main.rand.NextDouble() * 2d * Math.PI;
                    offset.X += (float)(Math.Sin(angle) * Distance);
                    offset.Y += (float)(Math.Cos(angle) * Distance);
                    Vector2 DustPos = NPC.Center + offset - new Vector2(4, 4);
                    Dust dust = Main.dust[Dust.NewDust(DustPos, 0, 0, DustType, 0, 0, 100, Color.White, 1f)];
                    dust.velocity = -((DustPos - NPC.Center) * Main.rand.NextFloat(0.01f, 0.12f));
                    dust.noGravity = true;
                    dust.scale = 2f;
                }
            }

            if (Parent.localAI[0] == 239)
            {
                int BeamColor = 0;

                if (NPC.type == ModContent.NPCType<TripletsEyePurple>())
                {
                    BeamColor = 1;
                }
                if (NPC.type == ModContent.NPCType<TripletsEyeRed>())
                {
                    BeamColor = 2;
                }

                LaunchLaser(NPC.Center, SavePlayerPosition, BeamColor);
            }
        }

        private void LaunchLaser(Vector2 fromArea, Vector2 toArea, int AIValue)
		{
			Vector2 direction = fromArea - toArea;
			direction.Normalize();
			direction *= 1500;
            NPCGlobalHelper.ShootHostileProjectile(NPC, fromArea, Vector2.Zero, ModContent.ProjectileType<TripletsLaser>(), NPC.damage, 2f, ai0: toArea.X - direction.X, ai1: toArea.Y - direction.Y, ai2: AIValue);
		}

        public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
                NPC Parent = Main.npc[(int)NPC.ai[3]];

                Parent.ai[1]++;

				DrawBody(true, NPC.ai[2] == 0);

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TripletsEyeGreenGore" + (NPC.ai[2] + 1)).Type);
                }
			}
		}
    }

    public class TripletsEyePurple : TripletsEyeGreen
    {
        private static Asset<Texture2D> GlowTexture;

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
                NPC Parent = Main.npc[(int)NPC.ai[3]];

                Parent.ai[1]++;

				DrawBody(true, NPC.ai[2] == 0);

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TripletsEyePurpleGore" + (NPC.ai[2] + 1)).Type);
                }
			}
		}
    }

    public class TripletsEyeRed : TripletsEyeGreen
    {
        private static Asset<Texture2D> GlowTexture;

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(GlowTexture.Value, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), 
            NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life <= 0)
			{
                NPC Parent = Main.npc[(int)NPC.ai[3]];

                Parent.ai[1]++;

				DrawBody(true, NPC.ai[2] == 0);

                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TripletsEyeRedGore" + (NPC.ai[2] + 1)).Type);
                }
			}
		}
    }
}