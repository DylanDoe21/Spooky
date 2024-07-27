using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class SkullAmuletSoul : ModProjectile
    {
		public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[6];

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 75;
            Projectile.penetrate = -1;
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

                Color color = Color.Lerp(Color.Green, Color.Lime, scale);

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
					float x = Main.rand.Next(-1, 2) * scale;
					float y = Main.rand.Next(-1, 2) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, color, Projectile.rotation, drawOrigin, scale * 0.6f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (!player.active || player.dead)
            {
                Projectile.Kill();
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

            Vector2 ReturnSpeed = player.Center - Projectile.Center;
            ReturnSpeed.Normalize();

            ReturnSpeed *= 20;

            Projectile.velocity = ReturnSpeed;

            if (Projectile.Hitbox.Intersects(player.Hitbox))
            {
                player.GetModPlayer<SpookyPlayer>().SkullFrenzyCharge++;

                Projectile.Kill();
            }
        }
    }
}