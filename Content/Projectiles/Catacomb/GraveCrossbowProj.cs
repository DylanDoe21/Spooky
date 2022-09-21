using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class GraveCrossbowProj : ModProjectile
	{
		float counter = 0;
        float shootSpeed = 5;
		Vector2 holdOffset = new Vector2(-3, -10);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grave Crossbow");
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 32;
            Projectile.height = 68;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool? CanHitNPC(NPC target)
        {
			return false;
        }

		public override bool PreAI()
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

            if (direction.X >= 0)
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

				Projectile.position = player.position + holdOffset;
				player.velocity.X *= 0.95f;

                counter += 0.25f;

                if (counter == 5 || counter == 10 || counter == 15)
                {
                    Projectile.frame++;
                    shootSpeed += 10f;
                }

                if (Projectile.frame >= 3)
                {   
                    Projectile.frame = 2;
                }

				if (direction.X >= 0) 
                {
					holdOffset.X = 2;
					player.direction = 1;
				}
				else 
                {
					holdOffset.X = -15;
					player.direction = 0;
				}
			}
			else 
            {
				if (Projectile.owner == Main.myPlayer)
				{
					Vector2 ProjDirection = Main.MouseWorld - Projectile.Center;
					ProjDirection.Normalize();
					ProjDirection.X *= shootSpeed;
					ProjDirection.Y *= shootSpeed;	

					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, ProjDirection.X, ProjDirection.Y, 
                    ProjectileID.WoodenArrowFriendly, Projectile.damage, Projectile.knockBack, Projectile.owner);
				}

				Projectile.active = false;
			}

			player.heldProj = Projectile.whoAmI;
			player.itemTime = 15;
			player.itemAnimation = 15;

			return true;
		}
	}
}