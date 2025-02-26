using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class ZombiePart1 : ModProjectile
    {
		int Bounces = 0;
		int foundTarget = 0;

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
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;  
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;   
            }

			foundTarget = FindTarget();
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Bounces++;
			if (Bounces >= 5)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.ai[0] = 30;
				
				SoundEngine.PlaySound(SoundID.Item177, Projectile.Center);

				if (foundTarget != -1)
				{
					NPC target = Main.npc[foundTarget];
					Projectile.position.X = Projectile.position.X + Projectile.velocity.X;

					Vector2 BounceToward = target.Center - Projectile.Center;
					BounceToward.Normalize();
					BounceToward *= 10;

					Projectile.velocity.X = BounceToward.X;
				}
				else
				{
					if (Projectile.velocity.X != oldVelocity.X)
					{
						Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
						Projectile.velocity.X = -oldVelocity.X * 0.85f;
					}
				}

                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
                }
			}

			return false;
		}

		private int FindTarget()
        {
            const float homingMaximumRangeInPixels = 300;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.ai[0] = 30;

			SoundEngine.PlaySound(SoundID.Item177, Projectile.Center);

			Projectile.velocity.Y = -Projectile.velocity.Y * 1.1f;
		}

		public override void OnKill(int timeLeft)
		{	
			SoundEngine.PlaySound(SoundID.NPCDeath9, Projectile.Center);

			for (int numDust = 0; numDust < 25; numDust++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
					
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
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