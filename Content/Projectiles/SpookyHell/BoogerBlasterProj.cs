using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class BoogerBlasterProj : ModProjectile
	{
        int Charge = 0;
        float SaveRotation;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 64;
            Projectile.height = 120;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
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

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - player.Center;
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

			if (player.channel && Projectile.ai[2] == 0) 
            {
                Projectile.timeLeft = 30;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

				Projectile.position = player.position + new Vector2(-23, -42);
				player.velocity.X *= 0.98f;

                Projectile.localAI[0] += 0.25f;

                if (Projectile.localAI[0] == 10 || Projectile.localAI[0] == 20)
                {
                    SoundEngine.PlaySound(SoundID.Item95, Projectile.Center);

                    Projectile.frame++;
                }

                if (Projectile.frame >= 3)
                {   
                    Projectile.frame = 2;
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
                    Projectile.position = player.position + new Vector2(-23, -42);

                    if (Projectile.timeLeft >= 29)
                    {
                        SaveRotation = Projectile.rotation;

                        //set ai[2] to 1 so it cannot shoot again
                        Projectile.ai[2] = 1;

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();

                        int extraDamage = 0;

                        switch (Projectile.frame)
                        {
                            //shoot one small booger
                            case 0:
                            {
                                SoundEngine.PlaySound(SoundID.Item167, Projectile.Center);

                                ShootSpeed *= 12;

                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                ModContent.ProjectileType<BlasterBoogerSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                                break;
                            }

                            //shoot spread of 3 small boogers
                            case 1:
                            {
                                SoundEngine.PlaySound(SoundID.Item167, Projectile.Center);

                                for (int numProjectiles = -1; numProjectiles <= 1; numProjectiles++)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center,
                                    16f * Projectile.DirectionTo(Main.MouseWorld).RotatedBy(MathHelper.ToRadians(6) * numProjectiles), 
                                    ModContent.ProjectileType<BlasterBoogerSmall>(), Projectile.damage, 0f, Main.myPlayer);
                                }

                                break;
                            }

                            //shoot big booger that deals triple damage
                            case 2:
                            {
                                SoundEngine.PlaySound(SoundID.Item167, Projectile.Center);

                                ShootSpeed *= 22;

                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                                ModContent.ProjectileType<BlasterBoogerBig>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);

                                break;
                            }
                        }
                    }

                    Projectile.frame = 0;
                    Projectile.rotation = SaveRotation;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, SaveRotation);
                }
			}

			player.heldProj = Projectile.whoAmI;
			player.itemTime = 1;
			player.itemAnimation = 1;
		}
	}
}