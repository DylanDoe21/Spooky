using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class PhantomSeed : ModProjectile
    {
		private List<Vector2> cache;
        private Trail trail;

		int target;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phantom Seed");
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;                   			 
            Projectile.height = 26;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;                  					
            Projectile.timeLeft = 240;
			Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			//draw prim trail
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/GlowTrailThin").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        const int TrailLength = 15;

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TrailLength; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > TrailLength)
            {
                cache.RemoveAt(0);
            }
        }

		private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 5 * factor, factor =>
            {
                return Color.Lerp(Color.DarkViolet, Color.DarkSlateBlue, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

		public override void AI()
		{		
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 5;
			}

			if (!Main.dedServ && Projectile.velocity != Vector2.Zero)
            {
                ManageCaches();
                ManageTrail();
            }

			Projectile.localAI[0]++;
            if (Projectile.localAI[0] == 1)
            {
                for (int i = 0; i < 10; i++)
				{
					int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 65, 0f, -2f, 0, default(Color), 1.5f);
					Main.dust[num].noGravity = true;
                    Main.dust[num].scale = 1.5f;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
					if (Main.dust[num].position != Projectile.Center)
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 2f;
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
				Projectile.velocity *= 1.045f;	 //1.058				
			}
		}

        public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 25; i++)
			{                                                                                  
				int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 65, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[num].noGravity = true;
				Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[num].position != Projectile.Center)
				{
					Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 2f;
				}
			}
		}
    }
}
     
          






