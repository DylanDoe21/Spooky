using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Sentient
{
    public class ToxicBubbleRed : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 45;
            Projectile.alpha = 125;
        }

        public override void AI()
        {
			Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

        public override void Kill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

            for (int numBlood = 0; numBlood < 5; numBlood++)
            {
                Vector2 speed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-10f, -5f));
                
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, speed, 
                ModContent.ProjectileType<ToxicBloodSplatter>(), Projectile.damage, 0f, Main.myPlayer, 0, 0);
            }

            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
            }
		}
    }
}