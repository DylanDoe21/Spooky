using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Sentient
{
    public class GrugFireball : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
			Projectile.rotation += 0.5f * (float)Projectile.direction;

            for (int numDusts = 0; numDusts < 2; numDusts++)
			{
                Vector2 dustPosition = Projectile.Center;
                dustPosition -= Projectile.velocity * ((float)numDusts * 0.25f);
                int dust = Dust.NewDust(dustPosition, 1, 1, DustID.CursedTorch, 0f, 0f, 0, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = dustPosition;
                Main.dust[dust].velocity *= 0.2f;
            }
        }

        public override void Kill(int timeLeft)
		{
            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
			}
		}
    }
}