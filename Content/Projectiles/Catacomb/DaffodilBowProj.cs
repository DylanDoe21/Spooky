using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class DaffodilBowProj : ModProjectile
	{
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
                Projectile.timeLeft = 20;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

				Projectile.position = player.position + new Vector2(-20, -12);

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
                    Projectile.alpha = 255;

                    Projectile.position = player.position + new Vector2(-20, -12);

                    if (Projectile.timeLeft >= 19)
                    {
                        //set ai[2] to 1 so it cannot shoot again
                        Projectile.ai[2] = 1;

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();

                        int extraDamage = 0;
                        int MaxProjectiles = 1;

                        switch (Projectile.frame)
                        {
                            case 0:
                            {
                                SoundEngine.PlaySound(SoundID.Item5 with { Pitch = SoundID.Item5.Pitch - 0.66f }, Projectile.Center);
                                SoundEngine.PlaySound(FlySound, Projectile.Center);
                                ShootSpeed *= 10;
                                extraDamage = -15;

                                break;
                            }
                            case 1:
                            {
                                SoundEngine.PlaySound(SoundID.Item5 with { Pitch = SoundID.Item5.Pitch - 0.33f }, Projectile.Center);
                                SoundEngine.PlaySound(FlySound, Projectile.Center);
                                MaxProjectiles = 3;
                                ShootSpeed *= 13;
                                extraDamage = -8;
                                break;
                            }
                            case 2:
                            {
                                SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);
                                SoundEngine.PlaySound(FlySound, Projectile.Center);
                                MaxProjectiles = 6;
                                ShootSpeed *= 16;
                                extraDamage = 0;
                                break;
                            }
                        }

                        for (int numProjectiles = 0; numProjectiles < MaxProjectiles; numProjectiles++)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X + Main.rand.Next(-5, 6), 
                            ShootSpeed.Y + Main.rand.Next(-5, 6), ModContent.ProjectileType<DaffodilBowFly>(), Projectile.damage + extraDamage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }
			}

			player.heldProj = Projectile.whoAmI;
			player.itemTime = 1;
			player.itemAnimation = 1;
		}
	}
}