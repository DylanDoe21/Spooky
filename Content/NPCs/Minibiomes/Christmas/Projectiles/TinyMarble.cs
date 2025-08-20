using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{
    public class TinyMarble : ModProjectile
    {
		int Bounces = 0;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 2;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.45f, Pitch = 1.25f }, Projectile.Center);

			Bounces++;
			if (Bounces >= 5)
			{
				Projectile.Kill();
			}
			else
			{
				if (Projectile.velocity.X != oldVelocity.X)
				{
					Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
					Projectile.velocity.X = -oldVelocity.X * 0.85f;
				}
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
                }
			}

			return false;
		}
    }
}