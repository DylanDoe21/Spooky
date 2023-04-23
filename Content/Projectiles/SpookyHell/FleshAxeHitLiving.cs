using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class FleshAxeHitLiving : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= target.lifeMax * 0.5)
            {
                target.takenDamageMultiplier = 1.65f;
            }

            if (hit.Crit)
            {
                target.AddBuff(ModContent.BuffType<LivingAxeBleed>(), 180);
            }
        }
    }
}