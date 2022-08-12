using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class GiantFlameBall : ModProjectile
    {
        int target;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flame Blast");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 96;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1000;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
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

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.Lerp(Color.Yellow, Color.Red, oldPos / (float)Projectile.oldPos.Length) * 0.65f * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.6f, 0.5f, 0f);

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[1]++;

			if (Projectile.ai[1] < 80)
			{
				Projectile.velocity *= 0.95f;
			}
			
			if (Projectile.ai[1] > 80 && Projectile.ai[1] < 180)
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
                
                if (Projectile.ai[1] < 180)
                {
                    Projectile.velocity *= 1.068f;
                }

                if (Projectile.ai[1] >= 240)
                {
                    Projectile.velocity *= 0.5f;	

                    Projectile.ai[2]++;
                    if (Projectile.ai[2] < 2)
                    {
                        Projectile.scale -= 3;
                    }
                    if (Projectile.ai[2] >= 2)
                    {
                        Projectile.scale += 3;
                    }

                    if (Projectile.ai[2] > 4)
                    {
                        Projectile.ai[2] = 0;
                        Projectile.scale = 1f;
                    }
                }

                if (Projectile.ai[1] >= 300)
                {
                    Projectile.Kill();
                }			
			}
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

            for (int numDust = 0; numDust < 50; numDust++)
			{                                                                                  
				int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-8f, 8f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-8f, 8f);
                Main.dust[dustGore].scale = Main.rand.NextFloat(2f, 3f);
                Main.dust[dustGore].noGravity = true;
			}

            Vector2 Speed = new Vector2(8f, 0f).RotatedByRandom(2 * Math.PI);

            for (int numProjectiles = 0; numProjectiles < 6; numProjectiles++)
            {
                Vector2 Position = new Vector2(Projectile.Center.X, Projectile.Center.Y);
                Vector2 speed = Speed.RotatedBy(2 * Math.PI / 2 * (numProjectiles + Main.rand.NextDouble() - 0.5));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int InfernoBolt = Projectile.NewProjectile(Projectile.GetSource_Death(), Position, speed, 
                    ProjectileID.InfernoHostileBolt, Projectile.damage, 0f, Main.myPlayer, 0, 0);
                    Main.projectile[InfernoBolt].tileCollide = true;
                    Main.projectile[InfernoBolt].timeLeft = 250;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, 0, 0, 
                ProjectileID.InfernoHostileBlast, Projectile.damage, 0f, Main.myPlayer, 0, 0);
            }

            for (int numExplosion = 0; numExplosion < 15; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.8f, 1.2f));

                Main.dust[DustGore].velocity *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}