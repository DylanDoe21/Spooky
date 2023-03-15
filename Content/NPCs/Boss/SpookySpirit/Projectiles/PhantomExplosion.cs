using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class PhantomExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phantom Explosion");
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 94;
            Projectile.hostile = true;
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
			
            Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch);
            dust.noGravity = true;
            dust.scale = 1.6f;
        }
		
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

        public override void Kill(int timeLeft)
        {
            Projectile.timeLeft = 0;
        }
    }
}
