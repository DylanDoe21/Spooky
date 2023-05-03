using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class AmuletSeed : ModProjectile
    {
		private List<Vector2> cache;
        private Trail trail;

		public override void SetDefaults()
		{
			Projectile.width = 14;                   			 
            Projectile.height = 20;         
			Projectile.friendly = true;                                 			  		
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

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

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
                return Color.Gold * factor.X;
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
					int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 65, 0f, -2f, 0, default(Color), 1.5f);
					Main.dust[newDust].noGravity = true;
                    Main.dust[newDust].scale = 1.5f;
					Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

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
				int foundTarget = HomeOnTarget();
				if (foundTarget != -1)
				{
					NPC target = Main.npc[foundTarget];
					Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 100;
					Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
					Projectile.tileCollide = false;
				}
				else
				{
					Projectile.tileCollide = true;
				}			
			}
		}

		private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 350;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public override void Kill(int timeLeft)
		{
			for (int i = 0; i < 25; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 65, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
    }
}
     
          






