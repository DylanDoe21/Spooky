using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class SpookyExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 94;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
			Projectile.alpha = 100;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                {
                    Projectile.Kill();
                }
            }

            Projectile.velocity.X *= 0;
            Projectile.velocity.Y *= 0;
			
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch);
            dust.noGravity = true;
            dust.scale = 1.6f;
        }
		
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
    }
}
