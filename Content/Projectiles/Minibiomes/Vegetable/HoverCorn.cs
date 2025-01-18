using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
    public class HoverCorn : ModProjectile
    {
		bool Shake = false;

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
			Projectile.alpha = 255;
        }

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active)
            {
				Projectile.Kill();
            }

			if (Projectile.timeLeft > 60 && Projectile.alpha > 0)
			{
				Projectile.alpha -= 5;
			}
			if (Projectile.timeLeft < 60)
			{
				Projectile.alpha += 5;

				if (Projectile.alpha >= 255)
				{
					Projectile.Kill();
				}
			}

			if (Shake)
			{
				Projectile.rotation += 0.025f;
				if (Projectile.rotation > 0.25f)
				{
					Shake = false;
				}
			}
			else
			{
				Projectile.rotation -= 0.025f;
				if (Projectile.rotation < -0.25f)
				{
					Shake = true;
				}
			}

			Projectile.Center = new Vector2(player.Center.X, player.Center.Y - 50) + Projectile.ai[0].ToRotationVector2() * 15f;
			Projectile.ai[0] -= MathHelper.ToRadians(5f);
			
			Projectile.ai[1]++;

			if (Projectile.ai[1] >= 180)
			{
				int foundTarget = FindTarget();
				if (foundTarget != -1)
				{
					SoundEngine.PlaySound(SoundID.Item54 with { Pitch = -1.2f }, Projectile.Center);

					NPC target = Main.npc[foundTarget];

					Vector2 ShootSpeed = target.Center - Projectile.Center;
					ShootSpeed.Normalize();
					ShootSpeed *= 25f;
							
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<HoverCornPopped>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);

					Projectile.Kill();
				}
			}
        }

		private int FindTarget()
		{
			const float homingMaximumRangeInPixels = 600;

			int selectedTarget = -1;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC target = Main.npc[i];
				if (target.CanBeChasedBy(Projectile))
				{
					float distance = Projectile.Distance(target.Center);
					if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
					{
						selectedTarget = i;
					}
				}
			}

			return selectedTarget;
		}
	}
}