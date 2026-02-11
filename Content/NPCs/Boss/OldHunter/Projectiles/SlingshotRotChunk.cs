using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.OldHunter.Projectiles
{
	public class SlingshotRotChunk : ModProjectile
	{
		bool runOnce = true;
		Vector2[] trailLength = new Vector2[5];

		private static Asset<Texture2D> TrailTexture;

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 300;
			Projectile.penetrate = 1;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Color.OrangeRed;

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.65f, Projectile.rotation, drawOrigin, scale * 1.3f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.ai[0] == 0)
			{
				Projectile.velocity = Vector2.Zero;

				Projectile.ai[0]++;
			}

			return false;
		}

		public override void AI()
		{
			if (Projectile.timeLeft <= 60)
			{
				Projectile.alpha += 5;
			}

			if (Projectile.ai[0] == 0)
			{
				Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.025f * (float)Projectile.direction;

				if (Projectile.velocity.Y < 11f)
				{
					Projectile.velocity.Y += 0.2f;
				}

				Projectile.timeLeft = 600;
			}
			else
			{
				Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;

				Projectile.ai[2]++;
				if (Projectile.ai[2] % 120 == 0)
				{
					Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SlingshotRotFly>(), Projectile.damage, Projectile.knockBack);
				}
			}

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
		}
	}
}