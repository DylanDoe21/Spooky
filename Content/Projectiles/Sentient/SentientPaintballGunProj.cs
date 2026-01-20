using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientPaintballGunProj : ModProjectile
	{
        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 42;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 5;
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

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.position = player.MountedCenter - Projectile.Size / 2;

			if (player.channel) 
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

                Projectile.ai[2]++;
                if (Projectile.ai[2] >= ItemGlobal.ActiveItem(player).useTime)
                {
                    SoundEngine.PlaySound(SoundID.Item111 with { Pitch = 0.5f}, Projectile.Center);

                    for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                            ShootSpeed.Normalize();
                            ShootSpeed.X *= Main.rand.Next(3, 7);
                            ShootSpeed.Y *= Main.rand.Next(3, 7);

                            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 32f;

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + muzzleOffset, ShootSpeed, 
                            ModContent.ProjectileType<MilkSplatter>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }

                    Projectile.ai[2] = 0;
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
				Projectile.active = false;
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}