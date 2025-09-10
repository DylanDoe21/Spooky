using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class Phantom : ModProjectile
    {
		int target;

		bool runOnce = true;

		Vector2[] trailLength = new Vector2[10];

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			Projectile.width = 14;
            Projectile.height = 22;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
			Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
		{   
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawTrailOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1.75f;

				Color color = Projectile.GetAlpha(Color.Lerp(Color.Purple, Color.White, scale));

				if (Flags.RaveyardHappening)
				{
					color = Projectile.GetAlpha(Color.Lerp(Color.Indigo, Color.HotPink, scale));
				}

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawTrailOrigin, scale, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return false;
		}

		public override void PostDraw(Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 30)
            {
                Vector2 circular = new Vector2(2f, 2f).RotatedBy(MathHelper.ToRadians(i));

				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Orchid);

				if (Flags.RaveyardHappening)
				{
					color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lime);
				}

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, 1.05f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);
        }

		public override void AI()
		{
			Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}

                Projectile.alpha = 0;

				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

			if (Projectile.alpha > 0 && Projectile.timeLeft > 60)
			{
				Projectile.alpha -= 5;
			}
			if (Projectile.timeLeft <= 60)
			{
				Projectile.alpha += 5;
			}

			Projectile.localAI[0]++;
            if (Projectile.localAI[0] == 1)
            {
                for (int numDusts = 0; numDusts < 10; numDusts++)
				{
					int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, -2f, 0, default, 1.5f);
					Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
					Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
					Main.dust[newDust].noGravity = true;

					if (Main.dust[newDust].position != Projectile.Center)
					{
						Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
					}
				}
            }

            Projectile.ai[1]++;

			if (Projectile.ai[1] < 80)
			{
				Projectile.velocity *= 0.98f;
			}
			
			if (Projectile.ai[1] > 80 && Projectile.ai[1] < 120)
			{
				if (Projectile.ai[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient) 
                {
					target = -1;
					float distance = 2000f;
					for (int k = 0; k < 255; k++) 
                    {
						if (Main.player[k].active && !Main.player[k].dead)
                        {
							Vector2 center = Main.player[k].Center;
							float currentDistance = Vector2.Distance(center, Projectile.Center);
							if (currentDistance < distance || target == -1) 
                            {
								distance = currentDistance;
								target = k;
							}
						}
					}
					if (target != -1) 
                    {
						Projectile.ai[0] = 1;
						Projectile.netUpdate = true;
					}
				}
				else if (target >= 0 && target < Main.maxPlayers) 
                {
					Player targetPlayer = Main.player[target];
					if (!targetPlayer.active || targetPlayer.dead) 
                    {
						target = -1;
						Projectile.ai[0] = 0;
						Projectile.netUpdate = true;
					}
					else 
                    {
						float currentRot = Projectile.velocity.ToRotation();
						Vector2 direction = targetPlayer.Center - Projectile.Center;
						float targetAngle = direction.ToRotation();
						if (direction == Vector2.Zero)
                        {
							targetAngle = currentRot;
                        }

						float desiredRot = currentRot.AngleLerp(targetAngle, 0.1f);
						Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0f).RotatedBy(desiredRot);
					}
                }
				
				Projectile.velocity *= 1.042f;				
			}
		}
    }
}