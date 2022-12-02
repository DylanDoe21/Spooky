using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class BoogerFlailFlung : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Snot Ball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
			Projectile.rotation += 0.12f * (float)Projectile.direction;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 20f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.1f;
            }
        }

        int Bounces = 0;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Bounces++;
			if (Bounces >= 3)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] = 0;
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);

				Projectile.velocity.X = -Projectile.velocity.X;
				Projectile.velocity.Y = -Projectile.velocity.Y * 1.05f;
			}

			return false;
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.Kill();
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, Projectile.Center);

            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.KryptonMoss, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[DustGore].noGravity = true;
				Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[DustGore].position != Projectile.Center)
                {
				    Main.dust[DustGore].velocity = Projectile.DirectionTo(Main.dust[DustGore].position) * 2f;
                }
			}
		}
    }
}