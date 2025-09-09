using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class CrustyGunfishProj : ModProjectile
    {
		int numHits = 0;
		float ActualSpeed = -25;

		public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 90;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;
			Projectile.rotation += 0.35f * (float)Projectile.direction;

			if (Projectile.soundDelay == 0 && ActualSpeed > 2)
			{
				Projectile.soundDelay = 15;
				SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
			}

			Projectile.ai[0]++;

			if (Projectile.ai[0] % 8 == 0 && player.HasAmmo(ItemGlobal.ActiveItem(player)))
			{
				SoundEngine.PlaySound(SoundID.Item95, Projectile.Center);

				int ProjType = ProjectileID.Bullet;
				float Speed = 15f;
				float knockBack = ItemGlobal.ActiveItem(player).knockBack;
				player.PickAmmo(ItemGlobal.ActiveItem(player), out ProjType, out Speed, out Projectile.damage, out knockBack, out _);

				Vector2 position = Projectile.Center;
				Vector2 velocity = new Vector2(0, -15f).RotatedBy(Projectile.rotation);

				for (int numProjs = 0; numProjs < 5; numProjs++)
				{
					Vector2 muzzleOffset = Vector2.Normalize(velocity) * Main.rand.NextFloat(30f, 150f);
					if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
					{
						position += muzzleOffset;
					}

					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(16));

					Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, newVelocity, 
					ModContent.ProjectileType<MudSplatter>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

					position -= muzzleOffset;
				}
			}

			if (Projectile.ai[0] >= 5 && ActualSpeed < 30)
			{
				ActualSpeed += 0.5f;

				if (ActualSpeed < 0)
				{
					Projectile.velocity *= 0.95f;
				}
			}

			if (ActualSpeed > 0)
			{
				Projectile.tileCollide = false;

				Vector2 Velocity = player.Center - Projectile.Center;
				Velocity.Normalize();
				Velocity *= ActualSpeed;

				Projectile.velocity = Velocity;

				if (Projectile.Hitbox.Intersects(player.Hitbox))
				{
					Projectile.Kill();
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			Projectile.ai[0] = 5;
			ActualSpeed = -ActualSpeed;

			if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }
    }
}