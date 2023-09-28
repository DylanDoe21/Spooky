using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class BrainJarProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
			Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.Kill();

			return false;
		}

        public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);

            player.AddBuff(ModContent.BuffType<BrainyBuff>(), 2);

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 
			0, 0, ModContent.ProjectileType<Brainy>(), Projectile.damage, 0f, Main.myPlayer, 0f, 0f);

            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass, 0f, -2f, 0, default, 1f);
				Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
			}
		}
    }
}