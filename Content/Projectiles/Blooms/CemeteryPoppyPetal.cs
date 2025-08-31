using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spooky.Core;

namespace Spooky.Content.Projectiles.Blooms
{
    public class CemeteryPoppyPetal : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
			{
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length;
				Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(Color.Red) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
				Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
				Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
			}

			return true;
		}

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			float WaveIntensity = 8f;
			float Wave = 8f;

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
    }
}