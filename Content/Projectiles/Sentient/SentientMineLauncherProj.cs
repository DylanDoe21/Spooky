using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientMineLauncherProj : ModProjectile
	{
        int playerCenterOffset = 4;

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults()
		{
            Projectile.width = 36;
            Projectile.height = 132;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 70;
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

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

            player.itemRotation = Projectile.rotation;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 7 - Projectile.height / 2);

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime - 20 && Projectile.localAI[0] < ItemGlobal.ActiveItem(player).useTime)
            {
                Projectile.position += Main.rand.NextVector2Square(-5, 5);
            }

            if (Projectile.localAI[0] == ItemGlobal.ActiveItem(player).useTime)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath13 with { Pitch = 1.2f }, Projectile.Center);

                for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                        ShootSpeed.Normalize();
                        ShootSpeed.X *= Main.rand.Next(15, 25);
                        ShootSpeed.Y *= Main.rand.Next(15, 25);

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y - playerCenterOffset, 
                        ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<EyeMine>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                Projectile.timeLeft = 20;
            }

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}