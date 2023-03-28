using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class ZombiePart1 : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Zombie Piece");
		}
		
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 2;
            Projectile.timeLeft = 500;
        }

        public override void AI()       
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;  
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;   
            }
        }
		
		int Bounces = 0;

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
				SoundEngine.PlaySound(SoundID.NPCHit8, Projectile.position);

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

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			Bounces++;
			if (Bounces >= 3)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] = 30;
				SoundEngine.PlaySound(SoundID.NPCHit8, Projectile.position);

				Projectile.velocity.X = -Projectile.velocity.X * 1.1f;
				Projectile.velocity.Y = -Projectile.velocity.Y * 1.1f;
			}
		}

		public override void Kill(int timeLeft)
		{	
			SoundEngine.PlaySound(SoundID.NPCHit8, Projectile.position);

			for (int i = 0; i < 50; i++)
			{
				int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default(Color), 1.5f);
				Main.dust[dustGore].noGravity = true;
				Main.dust[dustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

				if (Main.dust[dustGore].position != Projectile.Center)
				{
					Main.dust[dustGore].velocity = Projectile.DirectionTo(Main.dust[dustGore].position) * 2f;
				}
			}
		}
    }

	public class ZombiePart2 : ZombiePart1
    {
	}

	public class ZombiePart3 : ZombiePart1
    {
	}
}