using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class CannonEggSmall : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 500;
        }

        public override void AI()       
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f; 
            }
        }

		public override void OnKill(int timeLeft)
		{	
			SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

			for (int numDusts = 0; numDusts < 5; numDusts++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Web, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
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