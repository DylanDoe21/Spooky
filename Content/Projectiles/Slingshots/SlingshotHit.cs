using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class SlingshotHit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 90;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[0];

            Projectile.alpha += 15;
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
}