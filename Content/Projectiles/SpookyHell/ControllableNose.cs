using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class ControllableNose : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
            Projectile.width = 84;
            Projectile.height = 54;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
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
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

			if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - Projectile.Center;
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

                Projectile.Center = new Vector2(player.Center.X, player.Center.Y - 60);

                Projectile.ai[2]++;
                if (Projectile.ai[2] >= ItemGlobal.ActiveItem(player).useTime && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                {
                    SoundEngine.PlaySound(SoundID.Item171, Projectile.Center);

                    player.statMana -= ItemGlobal.ActiveItem(player).mana;

                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
                        ShootSpeed.Normalize();
                        ShootSpeed *= 25;

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, 
                        ModContent.ProjectileType<ControllableNoseBooger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                    Projectile.ai[2] = 0;
                }

                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
            }
        }
	}
}