using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientChikProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 500f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 275f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 12f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 1;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
        }
    }
}
