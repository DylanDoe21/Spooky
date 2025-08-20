using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{
    public class TeddyBearSnowHead : ModProjectile
    {
        float SaveRotation = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 30;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 2;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[2] = 6f;

                SaveRotation = Projectile.rotation;
            }
            else
            {
                Projectile.rotation = SaveRotation;

                Projectile.ai[2] -= 0.05f;

                Projectile.ai[1]++;
                if (Projectile.ai[1] % 5 == 0 && Projectile.timeLeft > 60)
                {
                    Vector2 PosToShootTo = new Vector2(Projectile.Center.X, Projectile.Center.Y - 30);

                    Vector2 ShootSpeed = PosToShootTo - Projectile.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= Projectile.ai[2];

                    Vector2 velocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(12));

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(0, 15), velocity,
                    ModContent.ProjectileType<SnowCloud>(), Projectile.damage, 0f, Projectile.owner, Main.myPlayer);
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = Vector2.Zero;

            Projectile.ai[0] = 1;

			return false;
		}
    }
}