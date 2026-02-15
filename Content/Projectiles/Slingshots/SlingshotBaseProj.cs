using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
	public class SlingshotBaseProj : ModProjectile
	{
        public static readonly SoundStyle ShootSound = new("Spooky/Content/Sounds/SlingshotShoot", SoundType.Sound);

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 16;
            Projectile.height = 60;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
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

			SlinghotHoldoutBasicBehavior(player);

			if (Projectile.frame < 3)
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
				Projectile.frame = 3;

				if (Projectile.timeLeft >= 19)
				{
					SoundEngine.PlaySound(ShootSound, Projectile.Center);

					int Type = GetProjectileToShoot(player);

					if (Projectile.owner == Main.myPlayer)
					{
						Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= ItemGlobal.ActiveItem(player).shootSpeed;

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
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
						player.PickAmmo(ItemGlobal.ActiveItem(player), out _, out _, out _, out _, out _);
						Projectile.frame = 0;
						Projectile.ai[2] = 0;
					}
				}
			}

			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2);
		}

		public void SlinghotHoldoutBasicBehavior(Player player)
		{
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

			Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - Projectile.height / 2);

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
					player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, player.itemRotation);
					break;
				}
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

		public int GetProjectileToShoot(Player player, int SpecialAmmo = -1)
		{
			int type = -1;
			float speed = ItemGlobal.ActiveItem(player).shootSpeed;
			float knockBack = ItemGlobal.ActiveItem(player).knockBack;

			player.PickAmmo(ItemGlobal.ActiveItem(player), out type, out speed, out Projectile.damage, out knockBack, out _);

			//if a special ammo is defined, then use that projectile type to replace regular mossy pebbles
			if (SpecialAmmo != -1 && type == ModContent.ProjectileType<MossyPebbleProj>())
			{
				type = SpecialAmmo;
			}

			return type;
		}
	}
}