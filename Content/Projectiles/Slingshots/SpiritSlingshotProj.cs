using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
	public class SpiritSlingshotProj : SlingshotBaseProj
	{
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SlingshotDraw", SoundType.Sound);
        public static readonly SoundStyle ShootSound = new("Spooky/Content/Sounds/SlingshotShoot", SoundType.Sound);

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

					int Type = GetProjectileToShoot(player, ModContent.ProjectileType<SpiritSlingshotGhost>());
					
					int NumShotsForEffect = 3;
					if (Projectile.ai[2] < NumShotsForEffect)
					{
						Projectile.ai[2]++;
					}
					else
					{
						Projectile.ai[2] = 0;
					}

					if (Projectile.owner == Main.myPlayer)
					{
						Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= ItemGlobal.ActiveItem(player).shootSpeed;

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, Type, Projectile.damage, 
						Projectile.knockBack, Projectile.owner, ai1: Projectile.ai[2] >= NumShotsForEffect ? 1 : 0);
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
						SoundEngine.PlaySound(UseSound, player.Center);

						player.PickAmmo(ItemGlobal.ActiveItem(player), out _, out _, out _, out _, out _);
						Projectile.frame = 0;
					}
				}
			}

			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2);
		}
	}
}