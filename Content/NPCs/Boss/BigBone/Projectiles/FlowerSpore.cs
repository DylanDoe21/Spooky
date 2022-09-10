using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{ 
    public class FlowerSpore : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Seed");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 20;                   			 
            Projectile.height = 24;         
			Projectile.hostile = true;                                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;                  					
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
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/MotionTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        const int TrailLength = 5;

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
                //use (* 1 - factor.X) at the end to make it fade at the beginning, or use (* factor.X) at the end to make it fade at the end
                return Color.Lerp(Color.DarkOrange, Color.Yellow, factor.X);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }
		
		public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 0f);

            if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 5;
			}

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.localAI[0]++;
            
            //make Projectile spin
            if (Projectile.localAI[0] > 30 && Projectile.localAI[0] < 75)
            {
                Projectile.spriteDirection = Projectile.velocity.X  > 0 ? 1 : -1;
                
                if (Projectile.velocity.X >= 0)
                {
                    Projectile.rotation += 0.35f;
                }
                if (Projectile.velocity.X < 0)
                {
                    Projectile.rotation += -0.35f;
                }

                Projectile.velocity *= 0.97f;
            }

            //make projectile charge at the player
            if (Projectile.localAI[0] == 75)
            {
                Projectile.tileCollide = true;

                double Velocity = Math.Atan2(Main.player[Main.myPlayer].position.Y - Projectile.position.Y, Main.player[Main.myPlayer].position.X - Projectile.position.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 8; //10 is the "charge" speed
            }

            if (Projectile.localAI[0] > 75 || Projectile.localAI[0] < 30)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;	
            }
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Grass, Projectile.Center);
        
        	for (int i = 0; i < 25; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Grass, 0f, -2f, 0, default(Color), 1.5f);
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