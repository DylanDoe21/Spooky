using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
    public class CursedFlamePillar : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        int trailWidth = 75;

        private List<Vector2> cache;
        private Trail trail;

        public override void SetDefaults()
        {
			Projectile.width = 50;                   			 
            Projectile.height = 50;
            Projectile.friendly = true;                               			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;        
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 5;
            Projectile.timeLeft = 100;
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
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/ShadowTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return true;
        }

        const int TrailLength = 100;

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => trailWidth * (1 - factor), factor =>
            {
                return Color.Lerp(Color.Green, Color.Lime, factor.X);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 3;
        }

        public override void AI()
        {
            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.ai[1]++;

            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity.Y = -5f;

                SpookyPlayer.ScreenShakeAmount = 2;

                //spawn dusts
                for (int numDust = 0; numDust < 50; numDust++)
				{                                                                                  
					int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 1.5f);
					Main.dust[dustGore].color = Color.Lime;
					Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-1f, 1f);
					Main.dust[dustGore].velocity.Y -= Main.rand.NextFloat(1f, 12f);
					Main.dust[dustGore].scale = 0.1f; 
					Main.dust[dustGore].noGravity = true;
				}

                Projectile.ai[0] = 1;
            }

            if (Projectile.ai[1] > 60)
            {
                Projectile.velocity *= 0;

                trailWidth -= 3;
            }
        }
    }
}
     
          






