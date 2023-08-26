using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.PandoraBox.Projectiles
{
    public class StitchBolt : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private List<Vector2> cache;
        private Trail trail;
		
        public override void SetDefaults()
        {
			Projectile.width = 18;                   			 
            Projectile.height = 18;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 180;
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 6 * factor, factor =>
            {
                return Color.Lerp(Color.Blue, Color.Cyan, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }
		
		public override void AI()
        {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 10)
            {
                if (!Main.dedServ && Projectile.velocity != Vector2.Zero)
                {
                    ManageCaches();
                    ManageTrail();
                }
            }

            Projectile.ai[1]++;
            if (Projectile.ai[1] <= 20)
            {   
                Projectile.velocity *= 0.97f;
            }
            else
            {
                Projectile.velocity *= 1.05f;
            }
		}
    }
}
     
          






