using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class OpalSlingshotBone : ModProjectile
    {
		int Bounces = 0;

		public override void SetStaticDefaults()
        {
			Main.projFrames[Projectile.type] = 3;
			ProjectileGlobal.IsSlingshotAmmoProj[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 500;
        }

        public override void AI()       
        {
			Projectile.frame = (int)Projectile.ai[1];

			Projectile.rotation += 0.2f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.45f;
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;
            }
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
				
				SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = 0.5f }, Projectile.Center);

				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
					Projectile.velocity.X = -oldVelocity.X * 0.85f;
				}
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
                }
			}

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.ai[0] = 30;

			SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = 0.5f }, Projectile.Center);

			Projectile.velocity.Y = -Projectile.velocity.Y * 1.1f;
		}

		public override void OnKill(int timeLeft)
		{	
			SoundEngine.PlaySound(SoundID.NPCHit2 with { Volume = 0.5f }, Projectile.Center);
		}
    }
}