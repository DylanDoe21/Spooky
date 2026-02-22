using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

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
                case 3:
                {
                    player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, player.itemRotation);
                    break;
                }
            }

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 2 - Projectile.height / 2);

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (Projectile.frame <= 1)
            {
                Projectile.timeLeft = 20;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime / 3)
                {
                    Projectile.frame++;

                    Projectile.localAI[0] = 0;
                }
			}
			else 
            {
                Projectile.frame = 2;

                if (Projectile.timeLeft >= 19)
                {
                    SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        int TypeToShoot = -1;
			            player.PickAmmo(ItemGlobal.ActiveItem(player), out TypeToShoot, out _, out _, out _, out _);

                        if (TypeToShoot == ProjectileID.WoodenArrowFriendly)
                        {
                            TypeToShoot = ModContent.ProjectileType<DaffodilBowFly>();
                        }

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= ItemGlobal.ActiveItem(player).shootSpeed;

                        for (int numProjs = 0; numProjs < 5; numProjs++)
                        {
                            Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(22));

                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, newVelocity, TypeToShoot, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                    }
                }

                if (Projectile.timeLeft < 5)
                {
                    if (!player.channel || !player.HasAmmo(ItemGlobal.ActiveItem(player)))
                    {
                        Projectile.Kill();
                    }
                    else
                    {
                        Projectile.frame = 0;
                    }
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
    }
}