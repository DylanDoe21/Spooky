using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class BoogerBlasterProj : ModProjectile
	{
        int SaveDirection;
        float SaveRotation;

        int playerCenterOffset = 8;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 32;
            Projectile.height = 120;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 30;
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
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
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

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

			if (player.channel && Projectile.ai[2] == 0) 
            {
                Projectile.timeLeft = 30;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime && Projectile.frame < 2)
                {
                    SoundEngine.PlaySound(SoundID.Item95, Projectile.Center);

                    Projectile.frame++;

                    Projectile.localAI[0] = 0;
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
                Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

                if (Projectile.timeLeft >= 29)
                {
                    SoundEngine.PlaySound(SoundID.Item167, Projectile.Center);

                    SaveDirection = Projectile.spriteDirection;
                    SaveRotation = Projectile.rotation;

                    //set ai[2] to 1 so it cannot shoot again
                    Projectile.ai[2] = 1;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                        ShootSpeed.Normalize();

                        switch (Projectile.frame)
                        {
                            //shoot one small booger
                            case 0:
                            {
                                ShootSpeed *= 12;

                                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset) + muzzleOffset, ShootSpeed, 
                                ModContent.ProjectileType<BlasterBoogerSmall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                                break;
                            }

                            //shoot spread of 3 small boogers
                            case 1:
                            {
                                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

                                for (int numProjectiles = -1; numProjectiles <= 1; numProjectiles++)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset) + muzzleOffset,
                                    16f * Projectile.DirectionTo(Main.MouseWorld).RotatedBy(MathHelper.ToRadians(6) * numProjectiles), 
                                    ModContent.ProjectileType<BlasterBoogerSmall>(), Projectile.damage, 0f, Projectile.owner);
                                }

                                break;
                            }

                            //shoot big booger that deals triple damage
                            case 2:
                            {
                                ShootSpeed *= 22;

                                Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset) + muzzleOffset, ShootSpeed, 
                                ModContent.ProjectileType<BlasterBoogerBig>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);

                                break;
                            }
                        }
                    }

                    Projectile.frame = 0;
                    Projectile.spriteDirection = SaveDirection;
                    Projectile.rotation = SaveRotation;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, SaveRotation);
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}