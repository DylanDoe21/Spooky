using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles
{
    public class Blank : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1;
            Projectile.alpha = 255;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }
}