using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.OldHunter.Projectiles
{
    public class SlingshotRotBall : ModProjectile
    {
        bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
			Projectile.width = 28;
            Projectile.height = 26;
			Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
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

        public override void AI()
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.025f * (float)Projectile.direction;
			
			Projectile.velocity.Y += 0.15f;
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
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

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath11, Projectile.Center);

			//split into a spread of chunks
			for (int numProjs = 0; numProjs <= 4; numProjs++)
			{
				Vector2 UpVelocity = new Vector2(0, Main.rand.Next(-8, -4));
				Vector2 ActualVelocity = UpVelocity.RotatedByRandom(MathHelper.ToRadians(55));
				Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, ActualVelocity, ModContent.ProjectileType<SlingshotRotChunk>(), Projectile.damage, Projectile.knockBack);
			}
		}
    }
}