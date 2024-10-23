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

            if (player.channel)
            {
                Projectile.timeLeft = 2;

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 GoTo = Main.MouseWorld;

                    float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 10, 20);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
                }

                Projectile.ai[0]++;

                if (Projectile.ai[0] >= ItemGlobal.ActiveItem(player).useTime && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                {
                    SoundEngine.PlaySound(SoundID.Item171, Projectile.Center);

                    player.statMana -= ItemGlobal.ActiveItem(player).mana;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y + 20, Main.rand.Next(-1, 2), 
                    Main.rand.Next(15, 22), ModContent.ProjectileType<ControllableNoseBooger>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    Projectile.ai[0] = 0;
                }

                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
            }
        }
	}
}