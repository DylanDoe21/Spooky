using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Blooms
{
    public class Romanesco : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f;

			Projectile.velocity *= 0.975f;

			Projectile.ai[0]++;
			if (Projectile.ai[0] > 70)
			{
				foreach (var Proj in Main.ActiveProjectiles)
				{
					//check for valid magic projectile
					if (Proj.owner == Projectile.owner && Projectile.DamageType == DamageClass.Magic && Proj.type != Type && Proj.type != ModContent.ProjectileType<RomanescoLeaf>())
					{
						//if valid, check for hitbox intersection and then spawn the homing leaves
						if (Proj.Hitbox.Intersects(Projectile.Hitbox))
						{
							SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryShot, Projectile.Center);

							for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
							{
								Vector2 ProjectilePosition = Projectile.Center + new Vector2(0, 15).RotatedByRandom(360);

								Vector2 ShootSpeed = Projectile.Center - ProjectilePosition;
								ShootSpeed.Normalize();
								ShootSpeed *= -5f;

								Projectile.NewProjectile(Projectile.GetSource_Death(), ProjectilePosition, ShootSpeed, ModContent.ProjectileType<RomanescoLeaf>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
							}

							for (int numDust = 0; numDust < 10; numDust++)
							{
								int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworkFountain_Green, 0f, -2f, 0, default, 1.5f);
								Main.dust[dust].noGravity = true;
								Main.dust[dust].position.X += Main.rand.Next(-65, 66) * 0.05f - 1.5f;
								Main.dust[dust].position.Y += Main.rand.Next(-65, 66) * 0.05f - 1.5f;

								if (Main.dust[dust].position != Projectile.Center)
								{
									Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
								}
							}

							Projectile.Kill();
						}
					}
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int numDust = 0; numDust < 10; numDust++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JungleGrass, 0f, -2f, 0, default, 1f);
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
}