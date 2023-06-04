using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles.Sentient
{
    public class IchorCloud : ModProjectile
    {
        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 255;
            Projectile.penetrate = 3;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.alpha += 5;
            
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }

			Projectile.rotation += 0.35f * (float)Projectile.direction;

            Projectile.velocity *= 0.95f;
        }
    }
}