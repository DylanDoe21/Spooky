using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
	public class TarCannonProj : ModProjectile
	{
		public override void SetDefaults()
		{
            Projectile.width = 46;
            Projectile.height = 100;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
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

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

                Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

                //shoot
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime)
                {
                    SoundEngine.PlaySound(SoundID.Item54 with { Pitch = -1f }, Projectile.Center);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                        ShootSpeed.Normalize();
                        ShootSpeed *= 10;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;

                        for (int numDusts = 0; numDusts < 4; numDusts++)
						{
							Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y), DustID.Asphalt,
							new Vector2(ShootSpeed.X + Main.rand.Next(-7, 8), ShootSpeed.Y + Main.rand.Next(-7, 8)) * 0.5f, default, default, 1f);
							dust.noGravity = true;
							dust.velocity += player.velocity;
						}

                        bool CanSpawnSlime = Main.rand.NextBool(5) && player.ownedProjectileCounts[ModContent.ProjectileType<TarCannonSlime>()] < 3;

                        int Type = CanSpawnSlime ? ModContent.ProjectileType<TarCannonSlime>() : ModContent.ProjectileType<TarCannonBlob>();

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y, 
                        ShootSpeed.X, ShootSpeed.Y, Type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Type == ModContent.ProjectileType<TarCannonSlime>() ? 0 : Main.rand.Next(0, 3));
                    }
    
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
            player.SetDummyItemTime(2);
        }
	}
}