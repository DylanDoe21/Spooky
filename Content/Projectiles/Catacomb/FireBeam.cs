using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class FireBeam : ModProjectile
	{
		public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[32];

		private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
		{
			Projectile.width = 14;
            Projectile.height = 14;
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
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.Lerp(Color.Red, Color.Yellow, scale);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / (4 * scale);

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					//gives the projectile after images a shaking effect
					float x = Main.rand.Next(-2, 3) * scale;
					float y = Main.rand.Next(-2, 3) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void AI()
		{
            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

            Projectile.localAI[0]++;
			if (Projectile.localAI[0] == 1)
            {
                Projectile.damage *= 3;
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

		public override void OnKill(int timeLeft)
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