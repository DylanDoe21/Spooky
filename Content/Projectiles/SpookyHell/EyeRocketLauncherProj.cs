using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class EyeRocketLauncherProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("I.C.U");
            Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults()
		{
            Projectile.width = 78;
            Projectile.height = 80;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
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

		public override bool PreAI()
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

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

                Projectile.position = player.position + new Vector2(-28, -23);
				player.velocity.X *= 0.99f;

                Projectile.localAI[0]++;

                //charge up the rocket
                if (Projectile.localAI[0] == 10 || Projectile.localAI[0] == 20 || Projectile.localAI[0] == 30 || Projectile.localAI[0] == 40)
                {
                    Projectile.frame++;
                }

                //shoot rocket
                if (Projectile.localAI[0] >= 80)
                {
                    SpookyPlayer.ScreenShakeAmount = 3;

                    SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

                    Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed.X *= 55;
                    ShootSpeed.Y *= 55;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 8, 
                    ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<EyeRocket>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    Projectile.frame = 0;
                    Projectile.localAI[0] = 0;
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
			player.itemTime = 1;
			player.itemAnimation = 1;

			return true;
		}
	}
}