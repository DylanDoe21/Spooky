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

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] > 80;
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

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                for (int numDust = 0; numDust < 10; numDust++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, -2f, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].scale = 1.5f;
                    Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

                    if (Main.dust[dust].position != Projectile.Center)
                    {
                        Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                    }
                }
            }

            if (Projectile.ai[0] < 80)
			{
				Projectile.velocity *= 0.98f;
			}
			
			if (Projectile.ai[0] > 80 && Projectile.ai[0] < 120)
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
            for (int numDust = 0; numDust < 15; numDust++)
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}
     
          






