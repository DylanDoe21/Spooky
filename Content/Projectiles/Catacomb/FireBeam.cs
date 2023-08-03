using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class FireBeam : ModProjectile
	{
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private List<Vector2> cache;
        private Trail trail;

		public override void SetDefaults()
		{
			Projectile.width = 2;
			Projectile.height = 2;
            Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 500;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 15;
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
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/GlowTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        const int TrailLength = 200;

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
            //using (factor => 12 * factor) makes the trail get smaller the further from the projectile, the number (12 in this case) affects how thick it is
            //just using (factor => 12) makes the trail the same size, where again the number (12 in this case) is the constant thickness
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 10 * factor, factor =>
            {
                //use (* 1 - factor.X) at the end to make it fade at the beginning, or use (* factor.X) at the end to make it fade at the end
                return Color.Lerp(Color.Red, Color.OrangeRed, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

		public override void AI()
		{
            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
			}

            Projectile.localAI[0]++;
			if (Projectile.localAI[0] == 1)
            {
                Projectile.damage *= 5;
            }

			bool flag25 = false;
			int target = 1;

			for (int index1 = 0; index1 < 200; index1++)
			{
				if (Main.npc[index1].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
				{
					float num23 = Main.npc[index1].position.X + (float)(Main.npc[index1].width / 2);
					float num24 = Main.npc[index1].position.Y + (float)(Main.npc[index1].height / 2);
					float num25 = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - num23) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - num24);
					if (num25 < 500f)
					{
						flag25 = true;
						target = index1;
					}
				}
			}

			if (flag25)
			{
				float num1 = 10f;
				Vector2 vector2 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
				float num2 = Main.npc[target].Center.X - vector2.X;
				float num3 = Main.npc[target].Center.Y - vector2.Y;
				float num4 = (float)Math.Sqrt((double)num2 * (double)num2 + (double)num3 * (double)num3);
				float num5 = num1 / num4;
				float num6 = num2 * num5;
				float num7 = num3 * num5;
				int num8 = 10;
				Projectile.velocity.X = (Projectile.velocity.X * (float)(num8 - 1) + num6) / (float)num8;
				Projectile.velocity.Y = (Projectile.velocity.Y * (float)(num8 - 1) + num7) / (float)num8;
			}
		}

		public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);

            for (int numDust = 0; numDust < 25; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
                Main.dust[dust].velocity.X *= Main.rand.NextFloat(-12f, 12f);
                Main.dust[dust].velocity.Y *= Main.rand.NextFloat(-12f, 12f);
                Main.dust[dust].scale = Main.rand.NextFloat(1f, 2f);
                Main.dust[dust].noGravity = true;
			}
		}
	}
}