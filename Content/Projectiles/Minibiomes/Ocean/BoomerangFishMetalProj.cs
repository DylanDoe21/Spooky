using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class BoomerangFishMetalProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Minibiomes/Ocean/BoomerangFishMetal";

		int numHits = 0;
		float ActualSpeed = -25;

		public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            numHits++;
            if (numHits >= 4)
            {
				Projectile.ai[0] = 5;
				ActualSpeed = 25;
			}
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.direction = Projectile.Center.X > player.Center.X ? 1 : -1;
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.05f * (float)Projectile.direction;

			if (Projectile.soundDelay == 0 && ActualSpeed > 2)
			{
				Projectile.soundDelay = 15;
				SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
			}

			Projectile.ai[0]++;

			if (Projectile.ai[0] >= 5 && ActualSpeed < 25)
			{
				ActualSpeed += 0.5f;

				if (ActualSpeed < 0)
				{
					Projectile.velocity *= 0.95f;
				}
			}

			if (ActualSpeed > 0)
			{
				Projectile.tileCollide = false;

				Vector2 Velocity = player.Center - Projectile.Center;
				Velocity.Normalize();
				Velocity *= ActualSpeed;

				Projectile.velocity = Velocity;

				if (Projectile.Hitbox.Intersects(player.Hitbox))
				{
					Projectile.Kill();
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

			Projectile.ai[0] = 5;
			ActualSpeed = -ActualSpeed;

			if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }
    }
}