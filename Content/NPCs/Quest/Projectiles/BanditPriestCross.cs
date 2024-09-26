using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class BanditPriestCross : ModProjectile
    {
		int target;

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[12];

		private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 26;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = -1;
        }

		public override bool PreDraw(ref Color lightColor)
		{   
			if (runOnce)
			{
				return false;
			}

            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawTrailOrigin = new Vector2(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				Color color = Projectile.GetAlpha(Color.Lerp(Color.Blue, Color.Cyan, scale));

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

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color, Projectile.rotation, drawTrailOrigin, scale * 1.1f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return false;
		}

        public override void PostDraw(Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 30)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, Projectile.GetAlpha(Color.Cyan), Projectile.rotation, drawOrigin, 1.05f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);
        }

		public override void AI()
        {
            Player Target = Main.player[Projectile.owner];

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

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            Vector2 desiredVelocity = Projectile.DirectionTo(Target.Center) * 7;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
        }
    }
}