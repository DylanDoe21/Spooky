using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class BigDunkSlingshotJuice : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/TrailCircle";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

		private static Asset<Texture2D> ProjTexture;
		
        public override void SetDefaults()
        {
			Projectile.width = 18;
            Projectile.height = 18;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 35;
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

				Color BasicColor = Projectile.ai[0] == 0 ? Color.Green : Color.DarkRed;
				
				Color color = BasicColor.MultiplyRGBA(lightColor);
                color *= (Projectile.timeLeft) / 90f;

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

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, scale * 0.35f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override void AI()
        {
            Projectile.velocity.Y += 0.25f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (Main.rand.NextBool(3))
            {
				int DustType = Projectile.ai[0] == 0 ? 46 : DustID.Blood;

                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType, 0f, -2f, 0, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity = -Projectile.velocity * 0.5f;
                Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
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
     
          






