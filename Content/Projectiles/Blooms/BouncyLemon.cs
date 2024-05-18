using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Blooms
{
    public class BouncyLemon : ModProjectile
    {
		int Bounces = 0;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 500;
        }

        public override void AI()       
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;  
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;   
            }
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Bounces++;
			if (Bounces >= 3)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] = 30;
				
				SoundEngine.PlaySound(SoundID.Item177, Projectile.Center);

				if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.8f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                }
			}

			return false;
		}

		public override void OnKill(int timeLeft)
		{
		}
    }
}