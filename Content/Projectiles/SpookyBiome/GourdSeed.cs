using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class GourdSeed : ModProjectile
    {		
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gourd Seed");
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
			Projectile.timeLeft = 60;
            Projectile.aiStyle = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 0.80f;

            if (Projectile.timeLeft <= 20) 
            {
				Projectile.alpha += 15;
			}
			else 
            {
				Projectile.alpha -= 15;
			}

            if (Projectile.alpha >= 255)
            {
                Projectile.alpha = 255;
            }

            if (Projectile.alpha <= 0)
            {
                Projectile.alpha = 0;
            }
        }
    }
}