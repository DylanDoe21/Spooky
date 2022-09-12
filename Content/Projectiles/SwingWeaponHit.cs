using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles
{
    public class SwingWeaponHit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
        }
    }
}