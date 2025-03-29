using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class HomingFlower : ModProjectile
    {
		int homingTarget;
		
		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
            Projectile.timeLeft = 480;
            Projectile.aiStyle = -1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/BouncingFlowerTrail");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
			{
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1.1f;
				Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Color.Lerp(Color.Orange, Color.Brown, oldPos / (float)Projectile.oldPos.Length) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(TrailTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale, SpriteEffects.None, 0);
			}

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override bool? CanDamage()
		{
			return Projectile.ai[0] > 35 && Projectile.ai[0] >= Projectile.localAI[0];
		}

		public override void AI()
		{
			Projectile Parent = Main.projectile[(int)Projectile.ai[1]];
			Player target = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] <= 35)
			{
				Projectile.Center = Parent.Center;

				if (Projectile.scale < 1f && Projectile.ai[0] > 10)
				{
					Projectile.scale += 0.1f;
				}
			}

			if (Projectile.ai[0] == 35)
			{
				Projectile.localAI[0] = Main.rand.Next(80, 200);
			}

			if (Projectile.ai[0] == Projectile.localAI[0] && Parent.type == ModContent.ProjectileType<VineBase>())
			{
				double Velocity = Math.Atan2(target.position.Y - Projectile.position.Y, target.position.X - Projectile.position.X);
				Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12;

				Parent.Kill();
			}

			if (Projectile.ai[0] > Projectile.localAI[0])
			{
				Projectile.localAI[1]++;
            	if (Projectile.localAI[1] >= 30)
				{
					Projectile.localAI[2]++;

					if (Projectile.localAI[2] < 80)
					{
						Projectile.velocity *= 0.98f;
					}

					if (Projectile.localAI[2] > 80 && Projectile.localAI[2] < 120)
					{
						if (Projectile.localAI[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
						{
							homingTarget = -1;
							float distance = 2000f;
							for (int k = 0; k < 255; k++)
							{
								if (Main.player[k].active && !Main.player[k].dead)
								{
									Vector2 center = Main.player[k].Center;
									float currentDistance = Vector2.Distance(center, Projectile.Center);
									if (currentDistance < distance || homingTarget == -1)
									{
										distance = currentDistance;
										homingTarget = k;
									}
								}
							}
							if (homingTarget != -1)
							{
								Projectile.localAI[1] = 1;
								Projectile.netUpdate = true;
							}
						}
						else if (homingTarget >= 0 && homingTarget < Main.maxPlayers)
						{
							Player targetPlayer = Main.player[homingTarget];
							if (!targetPlayer.active || targetPlayer.dead)
							{
								homingTarget = -1;
								Projectile.localAI[1] = 0;
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

						Projectile.velocity *= 1.055f;
					}
				}	
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X * 0.8f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
			}

			return false;
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        
        	for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
    }
}