using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class GlowBulbThorn : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;                  			 
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;          
			Projectile.friendly = true;              			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;             					
            Projectile.timeLeft = 2000;
		}

        public override void AI()
        {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 20f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);

			int newDust = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.Dirt, 0f, 0f, 0, default, 1f);
			Main.dust[newDust].position = position;
            Main.dust[newDust].scale = 0.95f;
			Main.dust[newDust].fadeIn = 0.5f;
			Main.dust[newDust].noGravity = true;
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            for (int numDust = 0; numDust < 10; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-12, 12) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-12, 12) * 0.05f - 1.5f;
                
				if (Main.dust[dust].position != Projectile.Center)
                {
				    Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                }
			}
		}
    }
}
     
          






