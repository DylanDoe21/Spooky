using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientKatanaSlashSpawner : ModProjectile
    {	
        public override void SetDefaults()
        {
			Projectile.width = 18;                   			 
            Projectile.height = 18;
            Projectile.friendly = true;                               			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;                					
            Projectile.timeLeft = 2;
            Projectile.alpha = 255;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 1)
            {
                Vector2 ProjectilePosition = Projectile.Center + new Vector2(0, 65).RotatedByRandom(360);
                Vector2 Velocity = Projectile.Center - ProjectilePosition;
                Velocity.Normalize();
                Velocity *= 45f;
                Projectile.NewProjectile(Projectile.GetSource_Death(), ProjectilePosition.X, ProjectilePosition.Y,
                Velocity.X, Velocity.Y, ModContent.ProjectileType<SentientKatanaSlash>(), Projectile.damage, 0, 0);
            }
        }
    }
}
     
          






