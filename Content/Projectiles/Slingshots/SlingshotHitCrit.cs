using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class SlingshotHitCrit : ModProjectile
    {
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
            Projectile.alpha += 8;
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }
}