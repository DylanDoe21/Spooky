using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Blooms
{
	public class CemeteryMarigoldProj : ModProjectile
	{
		public override string Texture => "Spooky/Content/Items/Blooms/CemeteryMarigold";

		bool runOnce = true;
		Vector2[] trailLength = new Vector2[10];
		float[] rotations = new float[10];

		float SaveRotation = 0;

		private static Asset<Texture2D> ProjTexture;
		private static Asset<Texture2D> TrailTexture;

		public override void SetDefaults()
		{
			Projectile.width = 35;
			Projectile.height = 45;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 240;
			Projectile.penetrate = -1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/CemeteryMarigoldProjStem");

			Vector2 drawOriginTrail = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				if (trailLength[k] == Vector2.Zero)
				{
					return false;
				}

				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color lightColorAtPos = Lighting.GetColor((int)trailLength[k].X / 16, (int)(trailLength[k].Y / 16));
				Color color = Color.Lerp(lightColorAtPos * 0f, lightColorAtPos, scale);

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length() / 15;

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Color colorWithAlphaWhenDying = Projectile.timeLeft <= 60 ? Projectile.GetAlpha(color) : color;

					Main.EntitySpriteDraw(TrailTexture.Value, drawPos, null, colorWithAlphaWhenDying, rotations[k], drawOriginTrail, 1f, SpriteEffects.None, 0);
				}

				previousPosition = currentPos;
			}

			Vector2 drawOrigin = new Vector2(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

			Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
					rotations[i] = 0f;
				}

				runOnce = false;
			}

			if (Projectile.scale < 1f)
			{
				Projectile.scale += 0.12f;
			}

			if (Projectile.timeLeft <= 60)
			{
				Projectile.alpha += 5;
			}

			Projectile.localAI[0]++;
			if (Projectile.localAI[0] <= 10)
			{
				SaveRotation = Projectile.rotation;

				int ProjDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch);
				Main.dust[ProjDust].noGravity = true;
				Main.dust[ProjDust].scale = 1.2f;
				Main.dust[ProjDust].velocity /= 4f;
				Main.dust[ProjDust].velocity += Projectile.velocity / 2;

				//save previous positions, rotations, and direction
				if (Projectile.velocity != Vector2.Zero)
				{
					Vector2 current = Projectile.Center;
					float currentRot = Projectile.rotation;
					for (int i = 0; i < trailLength.Length; i++)
					{
						Vector2 previousPosition = trailLength[i];
						trailLength[i] = current;
						current = previousPosition;

						float previousRot = rotations[i];
						rotations[i] = currentRot;
						currentRot = previousRot;
					}
				}

				//move in a random wavy pattern
				float WaveIntensity = Main.rand.NextFloat(-10f, 11f);
				float Wave = Main.rand.NextFloat(-10f, 11f);

				Projectile.ai[0]++;
				if (Projectile.ai[1] == 0)
				{
					if (Projectile.ai[0] > Wave * 0.5f)
					{
						Projectile.ai[0] = 0;
						Projectile.ai[1] = 1;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
				}
				else
				{
					if (Projectile.ai[0] <= Wave)
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
					else
					{
						Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
						Projectile.velocity = perturbedSpeed;
					}
					if (Projectile.ai[0] >= Wave * 2)
					{
						Projectile.ai[0] = 0;
					}
				}
			}
			else
			{
				Projectile.velocity = Vector2.Zero;

				Projectile.rotation = SaveRotation;
			}
		}
	}
}