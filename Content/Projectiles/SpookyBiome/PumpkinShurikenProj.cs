using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class PumpkinShurikenProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Putrid Shuriken");
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 3;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
			Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 20f)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.08f;
            }
        }

        int Bounces = 0;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Bounces++;
			if (Bounces >= 2)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] = 30;
                SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

				Projectile.velocity.X = -Projectile.velocity.X * 0.98f;
				Projectile.velocity.Y = -Projectile.velocity.Y * 0.98f;
			}

			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.Next(4) == 0)
            {
                for (int i = 0; i < Main.rand.Next(2, 3); i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, -1), 
                    ModContent.ProjectileType<PumpkinShurikenShrapnel>(), Projectile.damage / 2, 0f, Main.myPlayer, 0f, 0f);
                }

                Projectile.Kill();
            }
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

            for (int i = 0; i < 25; i++)
			{                                                                                  
				int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 288, 0f, -2f, 0, default, 1.5f);
				Main.dust[dustGore].noGravity = true;
				Main.dust[dustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

				if (Main.dust[dustGore].position != Projectile.Center)
                {
				    Main.dust[dustGore].velocity = Projectile.DirectionTo(Main.dust[dustGore].position) * 2f;
                }
			}
        }
    }
}