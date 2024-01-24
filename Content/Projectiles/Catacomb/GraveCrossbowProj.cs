using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class GraveCrossbowProj : ModProjectile
	{
        int SaveDirection;
        float SaveRotation;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 66;
            Projectile.height = 88;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
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

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

			if (player.channel && Projectile.ai[2] == 0) 
            {
                Projectile.timeLeft = 30;

                player.itemRotation = Projectile.rotation;

                switch (Projectile.frame)
                {
                    case 0:
                    {
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
                        break;
                    }
                    case 1:
                    {
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, player.itemRotation);
                        break;
                    }
                    case 2:
                    {
                        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, player.itemRotation);
                        break;
                    }
                }

				player.velocity.X *= 0.98f;

                Projectile.localAI[0] += 0.25f;

                if (Projectile.localAI[0] == 5 || Projectile.localAI[0] == 10 || Projectile.localAI[0] == 15)
                {
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
                    if (Projectile.timeLeft >= 29)
                    {
                        SaveDirection = Projectile.spriteDirection;
                        SaveRotation = Projectile.rotation;

                        //set ai[2] to 1 so it cannot shoot again
                        Projectile.ai[2] = 1;

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();

                        int extraDamage = 0;

                        switch (Projectile.frame)
                        {
                            case 0:
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot with { Pitch = SoundID.DD2_BallistaTowerShot.Pitch - 0.66f }, Projectile.Center);
                                ShootSpeed *= 10;
                                extraDamage = -15;
                                break;
                            }
                            case 1:
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot with { Pitch = SoundID.DD2_BallistaTowerShot.Pitch - 0.33f }, Projectile.Center);
                                ShootSpeed *= 15;
                                extraDamage = 0;
                                break;
                            }
                            case 2:
                            {
                                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
                                ShootSpeed *= 25;
                                extraDamage = 25;
                                break;
                            }
                        }

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 5, ShootSpeed.X, ShootSpeed.Y, 
                        ModContent.ProjectileType<GraveCrossbowArrow>(), Projectile.damage + extraDamage, Projectile.knockBack, Projectile.owner);
                    }

                    Projectile.frame = 3;
                    Projectile.spriteDirection = SaveDirection;
                    Projectile.rotation = SaveRotation;
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, SaveRotation);
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}