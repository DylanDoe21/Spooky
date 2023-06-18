using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class MoldyPelletProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] < 75;
        }

        public override void AI()
        {
			Projectile.rotation += Projectile.velocity.X * 0.2f;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 25)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;

                if (Projectile.velocity.Y > 0 || Projectile.velocity.Y < 0)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
                }
            }

            if (Projectile.ai[0] >= 75)
            {
                if (Main.rand.NextBool(35))
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), 
                        Main.rand.Next(-3, -1), ModContent.ProjectileType<MoldPelletFly>(), Projectile.damage / 2, 0f, Main.myPlayer);
					}
				}
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.1f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.1f;
            }

			return false;
		}
    }
}