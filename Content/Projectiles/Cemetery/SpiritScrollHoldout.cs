using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Cemetery
{
	public class SpiritScrollHoldout : ModProjectile
	{
		public override void SetDefaults()
		{
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Summon;
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

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - player.position;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            Projectile.position = player.Center - Projectile.Size / 2 + new Vector2((Projectile.direction == -1 ? -15 : 15), 0);

			if (player.channel && player.statMana > 12)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;

                player.bodyFrame.Y = player.bodyFrame.Height * 3;

                if (player.ownedProjectileCounts[ModContent.ProjectileType<ScrollPumpkin>()] < 5)
                {
                    Projectile.ai[2]++;
                }

                if (Projectile.ai[2] >= 30)
                {
                    player.statMana -= 12;

                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = SoundID.DD2_EtherianPortalSpawnEnemy.Volume * 2.5f }, Projectile.Center);

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 10, 0, Main.rand.Next(-5, -3), 
                    ModContent.ProjectileType<ScrollPumpkin>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    Projectile.ai[2] = 0;
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
            player.SetDummyItemTime(2);
        }
	}
}