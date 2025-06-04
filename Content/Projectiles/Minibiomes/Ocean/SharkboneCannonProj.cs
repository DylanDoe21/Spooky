using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
	public class SharkboneCannonProj : ModProjectile
	{
		int playerCenterOffset = 3;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			Projectile.width = 38;
			Projectile.height = 90;
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
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
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

			player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
			player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - playerCenterOffset - Projectile.height / 2);

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (Projectile.frame <= 4 && Projectile.ai[2] == 0)
			{
				Projectile.timeLeft = 15;

				Projectile.localAI[0]++;

				if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime / 5)
				{
					if (Projectile.frame >= 4)
					{
						Projectile.ai[2] = 1;
					}
					else
					{
						Projectile.frame++;
					}

					Projectile.localAI[0] = 0;
				}
			}
			else
			{
				Projectile.frame = 0;

				if (Projectile.timeLeft >= 14)
				{
					if (Projectile.owner == Main.myPlayer)
					{
						//if the player has bones, use them as "ammo" and shoot a bone spread
						if (player.ConsumeItem(ItemID.Bone))
						{
							Screenshake.ShakeScreenWithIntensity(Projectile.Center, 2.5f, 350f);

							SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.5f }, Projectile.Center);

							Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
							ShootSpeed.Normalize();
							ShootSpeed *= 20;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 38f;

							for (int numProjs = 0; numProjs < 5; numProjs++)
							{
								int randomVelocityX = Main.rand.Next(-5, 6);
								int randomVelocityY = Main.rand.Next(-5, 6);

								Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y, 
								ShootSpeed.X + randomVelocityX, ShootSpeed.Y + randomVelocityY, ModContent.ProjectileType<SharkboneShard>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
							}
						}
						//if the player doesnt have bones fire out weak bubbles
						else
						{
							SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

							Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
							ShootSpeed.Normalize();
							ShootSpeed *= 5;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 42f;

							for (int numProjs = 0; numProjs < 3; numProjs++)
							{
								int randomVelocityX = Main.rand.Next(-2, 3);
								int randomVelocityY = Main.rand.Next(-2, 3);

								Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y - playerCenterOffset, 
								ShootSpeed.X + randomVelocityX, ShootSpeed.Y + randomVelocityY, ModContent.ProjectileType<SharkboneCannonBubble>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
							}
						}
					}
				}

				if (Projectile.timeLeft < 5)
				{
					if (!player.channel)
					{
						Projectile.Kill();
					}
					else
					{
						Projectile.ai[2] = 0;
					}
				}
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}