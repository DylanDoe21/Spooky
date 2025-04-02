using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SpookFishronBowProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 58;
            Projectile.height = 56;
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

            Vector2 pos = new Vector2(25, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2);
			Projectile.Center = pos + new Vector2(player.MountedCenter.X, player.MountedCenter.Y);

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

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (Projectile.frame <= 2 && Projectile.ai[2] == 0)
            {
                Projectile.timeLeft = 30;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime / 3)
                {
                    Projectile.frame++;

                    Projectile.localAI[0] = 0;
                }
			}
			else 
            {
                if (Projectile.timeLeft >= 2)
                {
                    //set ai[2] to 1 so it cannot shoot again
                    Projectile.ai[2] = 1;

                    if (Projectile.owner == Main.myPlayer && Projectile.timeLeft % 5 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item42 with { Volume = 0.35f }, Projectile.Center);

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 18;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, ShootSpeed.X, ShootSpeed.Y, 
                        ModContent.ProjectileType<SpookFishronBowBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                Projectile.frame = 3;
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}