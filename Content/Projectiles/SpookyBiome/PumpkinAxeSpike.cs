using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.SpookyBiome
{ 
    public class PumpkinAxeSpike : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Root Thorn");
        }
		
        public override void SetDefaults()
        {
			Projectile.width = 16;                   			 
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Melee;         
			Projectile.friendly = true;                                 			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;      
            Projectile.penetrate = -1;          					
            Projectile.timeLeft = 60;
		}
		
		public override void AI()
        {
			//fix Projectile direction
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.08f;

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);

			int newDust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.Dirt, 0f, 0f, 0, default(Color), 1f);
			Main.dust[newDust].position = position;
            Main.dust[newDust].scale = 1f;
			Main.dust[newDust].fadeIn = 0.5f;
			Main.dust[newDust].noGravity = true;
		}

		public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

			for (int numDust = 0; numDust < 25; numDust++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
    }
}
     
          






