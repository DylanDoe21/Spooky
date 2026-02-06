using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class DaffodilHairpinPetal : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

		public override bool? CanDamage()
		{
			return Projectile.ai[1] > 0 && Projectile.localAI[0] >= 20;
		}

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active || !player.GetModPlayer<SpookyPlayer>().DaffodilHairpin)
            {
				Projectile.Kill();
            }

			if (Projectile.ai[1] == 0)
			{
				Projectile.timeLeft = 120;

				Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
				float RotateX = player.Center.X - vector.X;
				float RotateY = player.Center.Y - vector.Y;
				Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

				Projectile.Center = player.Center + Projectile.ai[0].ToRotationVector2() * 50f;
				Projectile.ai[0] -= MathHelper.ToRadians(3.5f);
			}
			else
			{
				Projectile.localAI[0]++;

				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

				if (Projectile.ai[2] == 0)
				{
					Vector2 ShootSpeed = player.Center - Projectile.Center;
					ShootSpeed.Normalize();
					ShootSpeed.X *= Main.rand.Next(10, 25);
					ShootSpeed.Y *= Main.rand.Next(10, 25);

					Projectile.velocity = ShootSpeed;

					Projectile.ai[2]++;
				}
				else
				{
					int foundTarget = HomeOnTarget();
					if (foundTarget != -1)
					{
						NPC target = Main.npc[foundTarget];
						Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 12;
						Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						Projectile.Kill();
					}
				}
			}
        }

		private int HomeOnTarget()
		{
			const float homingMaximumRangeInPixels = 500;

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