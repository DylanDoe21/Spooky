using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Core;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class BouncingFlower : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flower");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/ShadowTrail").Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["repeats"].SetValue(3);

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new Vector2(tex.Width * 0.5f, Projectile.height * 0.5f);

            /*for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.Lerp(Color.White, Color.Yellow, oldPos / (float)Projectile.oldPos.Length) * 0.65f * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new Rectangle(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }*/

            return true;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.3f, 0f);

			Projectile.rotation += 0.15f * (float)Projectile.direction;

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < 25; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > 25)
            {
                cache.RemoveAt(0);
            }

        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, 25, new TriangularTip(4), factor => 12 * factor, factor =>
            {
                return Color.Lerp(Color.HotPink, Color.Cyan, factor.Y) * (1 - factor.X);
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }


        int bounces = 0;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            Projectile.velocity.X = -Projectile.velocity.X * 1.05f;
            Projectile.velocity.Y = -Projectile.velocity.Y * 1.05f;

            bounces++;

            if (bounces > 2)
            {
                Projectile.Kill();
            }

			return false;
		}

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        
        	for (int i = 0; i < 25; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, -2f, 0, default(Color), 1.5f);
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