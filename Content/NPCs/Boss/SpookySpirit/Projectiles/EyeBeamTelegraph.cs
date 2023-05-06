using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class EyeBeamTelegraph : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 46;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 45;
        }
       
        public override Color? GetAlpha(Color lightColor)
		{
			Color[] ColorList = new Color[]
            {
                Color.White,
                Color.OrangeRed
            };

            float fade = Main.GameUpdateCount % 20 / 20f;
			int index = (int)(Main.GameUpdateCount / 20 % 2);
			return Color.Lerp(ColorList[index], ColorList[(index + 1) % 2], fade);
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(tex, vector, rectangle, Color.OrangeRed * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 30)
            {
                Projectile.alpha -= 10;

                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                }
            }
            
            Projectile.ai[1] += 2;
            if (Projectile.ai[1] < 10)
            {
                Projectile.scale += 0.15f;
            }
            if (Projectile.ai[1] >= 10)
            {
                Projectile.scale -= 0.15f;
            }

            if (Projectile.ai[1] > 20)
            {
                Projectile.ai[1] = 0;
                Projectile.scale = 1f;
            }
        }
    }
}