using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs;

namespace Spooky.Content.Projectiles.Blooms
{
    public class StrawberryBoost : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 540;
            Projectile.penetrate = 1;
        }

        public override bool? CanDamage()
		{
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.velocity *= 0.97f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 70)
            {
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    SoundEngine.PlaySound(SoundID.Item2, Projectile.Center);

                    player.AddBuff(ModContent.BuffType<StrawberryBoostBuff>(), 600);

                    Projectile.Kill();
                }

                if (Projectile.Distance(player.Center) <= 150f)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 20;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
        }
    }
}