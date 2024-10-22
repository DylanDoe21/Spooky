using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class VenomHarpoonProj : ModProjectile
	{
        int playerCenterOffset = 9;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 80;
            Projectile.height = 112;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
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

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.ai[1] == 0)
            {
                Projectile.timeLeft = 5;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<VenomHarpoonSpike>()] < 1 && Projectile.ai[0] == 0)
                {
                    Projectile.alpha = 255;

                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                    Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                    ShootSpeed.Normalize();
                    ShootSpeed *= 35;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - playerCenterOffset, ShootSpeed.X, ShootSpeed.Y,
                    ModContent.ProjectileType<VenomHarpoonSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, Projectile.whoAmI);

                    Projectile.ai[0] = 1;
                }
                else
                {
                    Projectile.alpha = 0;
                }
            }

            Projectile.direction = Projectile.spriteDirection = player.direction == 1 ? 1 : -1;

            player.itemRotation = Projectile.rotation;

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 2 - Projectile.height / 2);

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}