using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
    public class ThornBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 34;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
			Projectile.rotation += 0.3f * (float)Projectile.direction;
        }

		int Bounces = 0;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Bounces++;
			if (Bounces >= 3)
			{
				Projectile.Kill();
			}
			else
			{
				if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.8f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                }
			}

			return false;
		}

        public override void Kill(int timeLeft)
		{
            int NumProjectiles = Main.rand.Next(2, 4);
            for (int i = 0; i < NumProjectiles; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y + 20, Main.rand.Next(-2, 3),
                    Main.rand.Next(-2, -1), ModContent.ProjectileType<LingerThorn>(), Projectile.damage, 0, Main.myPlayer);
                }
            }
		}
    }
}