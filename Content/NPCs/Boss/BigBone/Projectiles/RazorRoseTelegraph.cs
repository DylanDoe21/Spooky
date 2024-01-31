using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class RazorRoseTelegraph : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 62;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 30;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(tex, vector, rectangle, Color.Red * 0.75f, Projectile.rotation, drawOrigin, Projectile.scale * 1.25f, SpriteEffects.None, 0);

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

    public class RazorRoseTelegraphLockOn : RazorRoseTelegraph
    {
        public override string Texture => "Spooky/Content/NPCs/Boss/BigBone/Projectiles/RazorRoseTelegraph";

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.position = new Vector2(player.Center.X - (Projectile.width / 2), player.Center.Y - (Projectile.height / 2));

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