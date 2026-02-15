using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class MoldyPelletProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Slingshots/Ammo/MoldyPellet";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
			Projectile.rotation += Projectile.velocity.X * 0.2f;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 25)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            for (int numProjectile = 0; numProjectile < 3; numProjectile++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), 
                    Main.rand.Next(-3, -1), ModContent.ProjectileType<MoldPelletFly>(), Projectile.damage / 2, 0f, Main.myPlayer);
                }
            }
		}
    }
}