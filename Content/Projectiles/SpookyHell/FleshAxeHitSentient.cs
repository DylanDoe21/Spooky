using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class FleshAxeHitSentient : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sentient Flesh Mincer");
        }

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

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life <= target.lifeMax * 0.5)
            {
                target.takenDamageMultiplier = 1.65f;
            }

            if (crit)
            {
                target.AddBuff(ModContent.BuffType<SentientAxeBleed>(), 180);
            }
        }
    }
}