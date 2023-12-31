using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class GourdSlingshotProj : ModProjectile
	{
        int SaveDirection;
        float SaveRotation;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 40;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 25;
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

			if (player.channel && Projectile.ai[2] == 0) 
            {
                Projectile.timeLeft = 25;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, player.itemRotation);

                Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

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
                    Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

                    if (Projectile.timeLeft >= 24)
                    {
                        SaveDirection = Projectile.spriteDirection;
                        SaveRotation = Projectile.rotation;

                        //set ai[2] to 1 so it cannot shoot again
                        Projectile.ai[2] = 1;

                        SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();

                        int extraDamage = 0;

                        switch (Projectile.frame)
                        {
                            case 0:
                            {
                                SoundEngine.PlaySound(SoundID.Item5 with { Pitch = SoundID.Item5.Pitch - 0.66f }, Projectile.Center);
                                ShootSpeed *= 3;
                                extraDamage = -5;

                                break;
                            }
                            case 1:
                            {
                                SoundEngine.PlaySound(SoundID.Item5 with { Pitch = SoundID.Item5.Pitch - 0.33f }, Projectile.Center);
                                ShootSpeed *= 7;
                                extraDamage = 0;

                                break;
                            }
                            case 2:
                            {
                                SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
                                ShootSpeed *= 12;
                                extraDamage = 10;

                                break;
                            }
                        }

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                        ModContent.ProjectileType<MoldyPelletProj>(), Projectile.damage + extraDamage, Projectile.knockBack, Projectile.owner);
                    }

                    Projectile.frame = 0;
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