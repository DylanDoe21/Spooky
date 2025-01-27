using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientKatanaSlashSpawner : ModProjectile
    {	
        public override string Texture => "Spooky/Content/Projectiles/Blank";

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

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft <= 1)
            {
                Vector2 ProjectilePosition = Projectile.Center + new Vector2(65, 0).RotatedByRandom(360);
                Vector2 Velocity = Projectile.Center - ProjectilePosition;
                Velocity.Normalize();
                Velocity *= 45f;
                Projectile.NewProjectile(Projectile.GetSource_Death(), ProjectilePosition, Velocity, ModContent.ProjectileType<SentientKatanaSlash>(), Projectile.damage, 0, 0);
            }
        }
    }
}
     
          






