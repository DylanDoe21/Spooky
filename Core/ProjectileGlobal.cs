using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Core
{
    public class ProjectileGlobal : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().MagicCandle && projectile.DamageType == DamageClass.Magic)
            {
                if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 120);
                }
            }
        }
    }
}
