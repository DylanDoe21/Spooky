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
    public class PhantomSkullProj : ModProjectile
    {
        int target;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> EyeTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 24;
            Projectile.height = 28;     
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 120;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			EyeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookySpirit/Projectiles/PhantomBombGlow");

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Indigo);

			if (Flags.RaveyardHappening)
            {
                color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(new Color(18, 148, 0));
            }

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.oldRot[oldPos], drawOrigin, scale + (fade / 2), effects, 0);
				Main.EntitySpriteDraw(EyeTexture.Value, drawPos, rectangle, Color.White * 0.5f, Projectile.oldRot[oldPos], drawOrigin, scale + (fade / 2), effects, 0);
            }

            return true;
        }
		
		public override void AI()
        {
			Lighting.AddLight(Projectile.Center, 0.5f, 0.35f, 0.7f);

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            Projectile.velocity *= 0.985f;

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 0 && Projectile.ai[0] < 50)
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

				Projectile.velocity *= 1.045f;			
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[dust].noGravity = true;

				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}