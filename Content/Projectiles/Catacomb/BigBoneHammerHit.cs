using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class BigBoneHammerHit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull Smasher");
        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 3;
        }

        public override void AI()
        {
            for (int numDusts = 0; numDusts < 30; numDusts++)
			{
                int NewDust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width / 2, Projectile.height / 2, 
                DustID.YellowTorch, Main.rand.Next(-10, 10), Main.rand.Next(-20, 20), 0, Color.Transparent, Main.rand.NextFloat(1.5f, 3.5f));
                Main.dust[NewDust].velocity.X *= Main.rand.Next(-5, 5);
                Main.dust[NewDust].velocity.Y *= Main.rand.Next(-20, 20);
            }
        }
    }
}