using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class TarantulaHawkBowProj : ModProjectile
	{
        float ExtraUseSpeed = 20;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
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

			if (player.channel) 
            {
                Projectile.timeLeft = 2;

				player.velocity.X *= 0.98f;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] % ExtraUseSpeed == 0)
                {
                    Projectile.frame++;
                }

                if (Projectile.frame > 3)
                {
                    Projectile.frame = 3;
                }

                if (Projectile.frame > 2 && Projectile.localAI[1] < 20)
                {
                    Projectile.localAI[1]++;

                    if (Projectile.localAI[2] == 0)
                    {
                        //set ai[2] to 1 so it cannot shoot again
                        Projectile.localAI[2] = 1;
                        
                        SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 20;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 2, ShootSpeed.X, ShootSpeed.Y, 
                        ModContent.ProjectileType<TarantulaHawkArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                //reset the ai values so the bow shoots again
                if (Projectile.localAI[1] >= 20)
                {
                    if (ExtraUseSpeed > 2)
                    {
                        ExtraUseSpeed -= 2;
                    }

                    Projectile.frame = 0;
                    Projectile.localAI[0] = 0;
                    Projectile.localAI[1] = 0;
                    Projectile.localAI[2] = 0;
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}