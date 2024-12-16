using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{
	public class ChungusSpore : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 22;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 500;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Blue);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 2 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(-3, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
            Player player = Main.player[(int)Projectile.ai[1]];

            if (Projectile.alpha > 0 && Projectile.timeLeft > 60)
            {
                Projectile.alpha -= 5;
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

			Projectile.rotation = Projectile.velocity.X * 0.1f;

			Projectile.ai[0]++;
            if (Projectile.ai[0] < 60)
            {
                float goToX = (player.Center.X + Main.rand.Next(-15, 15)) - Projectile.Center.X;
                float goToY = (player.Center.Y + Main.rand.Next(-15, 15)) - Projectile.Center.Y;
                float speed = 0.025f;

                if (Projectile.velocity.X < goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speed;
                    if (Projectile.velocity.X < 0f && goToX > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + speed;
                    }
                }
                else if (Projectile.velocity.X > goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speed;
                    if (Projectile.velocity.X > 0f && goToX < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - speed;
                    }
                }
                if (Projectile.velocity.Y < goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speed;
                    if (Projectile.velocity.Y < 0f && goToY > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + speed;
                        return;
                    }
                }
                else if (Projectile.velocity.Y > goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speed;
                    if (Projectile.velocity.Y > 0f && goToY < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - speed;
                        return;
                    }
                }
            }
            else
            {
                Projectile.velocity *= 0.97f;
            }
		}
	}
}