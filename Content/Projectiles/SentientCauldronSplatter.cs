using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles
{
    public class SentientCauldronSplatter : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acid Splatter");
            Main.projFrames[Projectile.type] = 10;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
		
        public override void SetDefaults()
        {
            Projectile.width = 30;                  			 
            Projectile.height = 56;          
			Projectile.friendly = false;
            Projectile.hostile = true;                 			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 400;
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 10)
                {
                    Projectile.Kill();
                }
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity *= Main.rand.NextFloat(2f, 2.5f);
                Projectile.scale = Main.rand.NextFloat(0.95f, 1.25f);
                Projectile.ai[0] = 1;
            }

            Projectile.velocity.Y += 0.08f;

			//fix Projectile direction
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.direction == 1)
            {
			    Projectile.rotation += 0.1f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation += -0.1f * (float)Projectile.direction;
            }
		}
    }
}
     
          






