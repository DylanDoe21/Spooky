using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{
    public class WitchPotion : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
            Projectile.timeLeft = 1800;
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

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            for (int numProjectile = 0; numProjectile < 3; numProjectile++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int FlaskCloud = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 3), 
                    Main.rand.Next(-3, 4), ProjectileID.ToxicCloud2, Projectile.damage, 0f, Main.myPlayer);
                    Main.projectile[FlaskCloud].friendly = false;
                    Main.projectile[FlaskCloud].hostile = true;
                }
            }

            Projectile.Kill();

			return false;
		}
    }
}