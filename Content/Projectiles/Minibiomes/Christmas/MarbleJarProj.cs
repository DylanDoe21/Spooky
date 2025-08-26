using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class MarbleJarProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Minibiomes/Christmas/MarbleJar";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 44;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
			Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();

			return false;
		}

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

            for (int numGores = 1; numGores <= 2; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/MarbleJarGore" + numGores).Type);
                }
            }

            for (int numProjs = 0; numProjs <= 5; numProjs++)
            {
                Vector2 velocity = new Vector2(0, Main.rand.Next(-12, -8)).RotatedByRandom(MathHelper.ToRadians(25));

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, 
                ModContent.ProjectileType<TinyJarMarble>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
            }

            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default, 1f);
				Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
			}
		}
    }
}