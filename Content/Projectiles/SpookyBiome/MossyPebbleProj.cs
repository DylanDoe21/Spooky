using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class MossyPebbleProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 15)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
            }
        }
		
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            for (int numDust = 0; numDust < 10; numDust++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, 0f, -2f, 0, default, 0.75f);
				Main.dust[dust].position.X += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
					
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
        }
    }
}