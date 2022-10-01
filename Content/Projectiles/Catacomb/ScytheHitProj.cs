using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class ScytheHitProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Harvester's Scythe");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Player player = Main.player[Projectile.owner];

            if (target.life <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<SoulBolt>()] < 10)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                ModContent.ProjectileType<SoulBolt>(), Projectile.damage, 0f, Main.myPlayer, 0, 0);
            }
        }
    }
}