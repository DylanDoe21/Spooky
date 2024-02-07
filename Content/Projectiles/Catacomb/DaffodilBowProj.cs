using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class DaffodilBowProj : ModProjectile
	{
        float SaveRotation;

        public static readonly SoundStyle FlySound = new("Spooky/Content/Sounds/FlyBuzzing", SoundType.Sound);

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
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

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - Projectile.Center;
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

            Projectile.position = new Vector2(player.MountedCenter.X - 1 - Projectile.width / 2, player.MountedCenter.Y - Projectile.height / 2);

			if (player.channel && Projectile.ai[2] == 0) 
            {
                Projectile.timeLeft = 20;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
                
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

                Projectile.localAI[0] += 0.25f;

                if (Projectile.localAI[0] == 5 || Projectile.localAI[0] == 10)
                {
                    Projectile.frame++;
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
                    if (Projectile.timeLeft >= 19)
                    {
                        SaveRotation = Projectile.rotation;

                        //set ai[2] to 1 so it cannot shoot again
                        Projectile.ai[2] = 1;

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();

                        int MaxProjectiles = 1;

                        switch (Projectile.frame)
                        {
                            case 0:
                            {
                                SoundEngine.PlaySound(SoundID.Item5 with { Pitch = SoundID.Item5.Pitch - 0.66f }, Projectile.Center);
                                SoundEngine.PlaySound(FlySound, Projectile.Center);
                                ShootSpeed *= 10;

                                break;
                            }
                            case 1:
                            {
                                SoundEngine.PlaySound(SoundID.Item5 with { Pitch = SoundID.Item5.Pitch - 0.33f }, Projectile.Center);
                                SoundEngine.PlaySound(FlySound, Projectile.Center);
                                MaxProjectiles = 3;
                                ShootSpeed *= 13;
                                break;
                            }
                            case 2:
                            {
                                SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
                                SoundEngine.PlaySound(FlySound, Projectile.Center);
                                MaxProjectiles = 6;
                                ShootSpeed *= 16;
                                break;
                            }
                        }

                        for (int numProjectiles = 0; numProjectiles < MaxProjectiles; numProjectiles++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X + Main.rand.Next(-5, 6), 
                            ShootSpeed.Y + Main.rand.Next(-5, 6), ModContent.ProjectileType<DaffodilBowFly>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }

                    Projectile.frame = 0;
                    Projectile.rotation = SaveRotation;
                    player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, SaveRotation);
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, SaveRotation);
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}