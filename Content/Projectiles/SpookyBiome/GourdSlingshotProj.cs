using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class GourdSlingshotProj : ModProjectile
	{
        int SaveDirection;
        float SaveRotation;

        int playerCenterOffset = 10;

        public static readonly SoundStyle ShootSound = new("Spooky/Content/Sounds/SlingshotShoot", SoundType.Sound);

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 40;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 25;
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

            Projectile.position = new Vector2(player.MountedCenter.X - 1 - Projectile.width / 2, player.MountedCenter.Y - 5 - Projectile.height / 2);

			if (player.channel && Projectile.ai[2] == 0) 
            {
                Projectile.timeLeft = 25;

                player.itemRotation = Projectile.rotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, player.itemRotation);

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime && Projectile.frame < 2)
                {
                    Projectile.frame++;

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
                if (Projectile.timeLeft >= 24)
                {
                    SaveDirection = Projectile.spriteDirection;
                    SaveRotation = Projectile.rotation;

                    //set ai[2] to 1 so it cannot shoot again
                    Projectile.ai[2] = 1;

                    float VolumePitch = Projectile.frame == 2 ? 0f : (Projectile.frame == 1 ? 0.33f : 0.66f);
                    SoundEngine.PlaySound(ShootSound with { Pitch = ShootSound.Pitch - VolumePitch }, Projectile.Center);

                    int extraDamage = Projectile.frame == 2 ? 12 : (Projectile.frame == 1 ? 6 : 0);

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                        ShootSpeed.Normalize();

                        ShootSpeed *= Projectile.frame == 2 ? 12 : (Projectile.frame == 1 ? 8 : 4);

                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - playerCenterOffset, ShootSpeed.X, ShootSpeed.Y, 
                        ModContent.ProjectileType<MoldyPelletProj>(), Projectile.damage + extraDamage, Projectile.knockBack, Projectile.owner);
                    }
                }

                Projectile.frame = 0;
                Projectile.spriteDirection = SaveDirection;
                Projectile.rotation = SaveRotation;
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, SaveRotation);
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}