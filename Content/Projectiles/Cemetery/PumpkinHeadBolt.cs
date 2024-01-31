using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class PumpkinHeadBolt : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private List<Vector2> cache;
        private Trail trail;
		
        public override void SetDefaults()
        {
			Projectile.width = 18;                   			 
            Projectile.height = 26;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/GlowTrail").Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["repeats"].SetValue(1);

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new RoundedTip(4), factor => 6 * factor, factor =>
            {
                return Color.Lerp(Color.Gray, Color.OrangeRed, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }
		
		public override void AI()
        {
            //add lighting so you can see it in the dark
            Lighting.AddLight((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f), 0.5f, 0.25f, 0f);

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            if (!Main.dedServ && Projectile.velocity != Vector2.Zero)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] <= 60)
            {   
                Projectile.velocity *= 0.97f;
            }
            else
            {
                Projectile.velocity *= 1.05f;
            }
		}

		public override void OnKill(int timeLeft)
		{
			for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
                
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}