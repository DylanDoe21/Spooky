using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class BloodTooth : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 2;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] >= 50;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 40)
            {
                Projectile.velocity.X *= 0.98f;
                Projectile.velocity.Y = -22;
            }

            if (Projectile.ai[0] > 40 && Projectile.ai[0] < 50)
            {
                Projectile.velocity *= 0.98f;
            }

            if (Projectile.ai[0] == 50)
            {
                Vector2 ChargeDirection = Main.MouseWorld - Projectile.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection *= 50;
                Projectile.velocity = ChargeDirection;

                Projectile.tileCollide = true;
            }
        }

        public override void Kill(int timeLeft)
		{
            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[DustGore].noGravity = true;
				Main.dust[DustGore].position.X += Main.rand.Next(-25, 25) * .05f - 1.5f;
				Main.dust[DustGore].position.Y += Main.rand.Next(-25, 25) * .05f - 1.5f;
			}
		}
    }
}