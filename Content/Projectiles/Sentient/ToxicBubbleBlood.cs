using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class ToxicBubbleBlood : ModProjectile
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
                float divide = 1.75f;
                Vector2 speed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-10f, -5f));
                
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, speed, 
                ModContent.ProjectileType<ToxicBloodSplatter>(), Projectile.damage / (int)divide, 0f, Main.myPlayer, 0, 0);
            }

            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[DustGore].noGravity = true;
				Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
			}
		}
    }
}