using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
	public class DunkleosteusCannonProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			Projectile.width = 66;
			Projectile.height = 122;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.timeLeft = 20;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            if (Projectile.direction >= 0)
            {
                Projectile.rotation = direction.ToRotation() - 1.57f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation = direction.ToRotation() + 1.57f * (float)Projectile.direction;
            }

            player.itemRotation = Projectile.rotation;

			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
			player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.Center = new Vector2(player.MountedCenter.X, player.MountedCenter.Y);

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (Projectile.frame <= 2 && Projectile.ai[2] == 0)
			{
				Projectile.timeLeft = 15;

				Projectile.localAI[0]++;

				if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime / 3)
				{
					if (Projectile.frame >= 2)
					{
						Projectile.ai[2] = 1;
					}
					else
					{
						Projectile.frame++;
					}

					Projectile.localAI[0] = 0;
				}
			}
			else
			{
				Projectile.frame = 2;

				if (Projectile.timeLeft >= 14)
				{
					if (Projectile.owner == Main.myPlayer)
					{
						//if the player has bones, use them as "ammo" and shoot a bone spread
						if (player.ConsumeItem(ItemID.Cannonball))
						{
							Screenshake.ShakeScreenWithIntensity(Projectile.Center, 2.5f, 350f);

							SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.5f }, Projectile.Center);

							Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
							ShootSpeed.Normalize();
							ShootSpeed *= 20;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 2f;

							SpawnMuzzleDust(new Vector2(Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y), ShootSpeed * 0.5f);

							Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y, 
							ShootSpeed.X, ShootSpeed.Y, ProjectileID.CannonballFriendly, Projectile.damage, Projectile.knockBack, Projectile.owner);
						}
						//if the player doesnt have bones fire out weak bubbles
						else
						{
							SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

							Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
							ShootSpeed.Normalize();
							ShootSpeed *= 10;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 42f;

							SpawnMuzzleDust(new Vector2(Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y), ShootSpeed * 0.5f);

							for (int numProjs = 0; numProjs < 3; numProjs++)
							{
								Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(45));

								Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y, 
								newVelocity.X, newVelocity.Y, ModContent.ProjectileType<SharkboneCannonBubble>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, ai0: Main.rand.Next(0, 10));
							}
						}
					}
				}

				if (Projectile.timeLeft < 5)
				{
					if (!player.channel)
					{
						Projectile.Kill();
					}
					else
					{
						Projectile.frame = 0;
						Projectile.ai[2] = 0;
					}
				}
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }

		public void SpawnMuzzleDust(Vector2 Pos, Vector2 velocity)
		{
			for (int j = 0; j < 10; j++)
			{
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(60));

				Dust dust = Dust.NewDustPerfect(Pos, 31, newVelocity, default, default, 1f);
				dust.velocity *= Main.rand.NextFloat(0.5f, 0.8f);
				dust.velocity += velocity * 0.1f;
			}
		}
	}
}