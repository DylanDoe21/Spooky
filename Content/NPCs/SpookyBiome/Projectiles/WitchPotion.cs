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
			Projectile.rotation += Projectile.velocity.X * 0.08f;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.65f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

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

            for (int numDust = 0; numDust < 20; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default, 1f);
				Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
			}
		}
    }
}