using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class BoogerBlasterProj : ModProjectile
	{
        int Charge = 0;

		Vector2 holdOffset = new Vector2(-3, -2);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Booger Blaster");
		}

		public override void SetDefaults()
		{
            Projectile.width = 32;
            Projectile.height = 45;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool? CanHitNPC(NPC target)
        {
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override bool PreAI()
		{
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - Projectile.position;
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

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

				Projectile.position = player.position + holdOffset;
				player.velocity.X *= 0.95f;

                Projectile.localAI[0] += 0.25f;

                //charges
                //charge = 1, shoot one small booger
                //charge = 2, shoot a few small boogers
                //charge = 3, shoot one big booger
                //charge = 4, shoot one big booger that splits into a spread mid flight
                if (Projectile.localAI[0] == 2 || Projectile.localAI[0] == 15 || Projectile.localAI[0] == 28)
                {
                    SoundEngine.PlaySound(SoundID.Item95, Projectile.Center);

                    Charge++;
                }

                if (direction.X > 0) 
                {
					player.direction = 1;
				}
				else 
                {
					player.direction = -1;
				}
			}
			else 
            {
				if (Projectile.owner == Main.myPlayer)
				{
                    SoundEngine.PlaySound(SoundID.Item167, Projectile.Center);

                    Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                    ShootSpeed.Normalize();

                    switch (Charge)
				    {
                        //no charge, do nothing
                        case 0:
                        {
                            break;
                        }   

                        //shoot one small booger
                        case 1:
                        {
                            ShootSpeed.X *= 12;
                            ShootSpeed.Y *= 12;	

                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                            ModContent.ProjectileType<BlasterBoogerSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                            break;
                        }

                        //shoot spread of 3 small boogers
                        case 2:
                        {
                            for (int numProjectiles = -1; numProjectiles <= 1; numProjectiles++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center,
                                16f * Projectile.DirectionTo(Main.MouseWorld).RotatedBy(MathHelper.ToRadians(6) * numProjectiles), 
                                ModContent.ProjectileType<BlasterBoogerSmall>(), Projectile.damage, 0f, Main.myPlayer);
                            }

                            break;
                        }

                        //shoot big booger that deals double damage
                        case 3:
                        {
                            ShootSpeed.X *= 22;
                            ShootSpeed.Y *= 22;	

                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                            ModContent.ProjectileType<BlasterBoogerBig>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);

                            break;
                        }
                    }
				}

				Projectile.active = false;
			}

			player.heldProj = Projectile.whoAmI;
			player.itemTime = 1;
			player.itemAnimation = 1;

			return true;
		}
	}
}