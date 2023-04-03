using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class ScyclerProj : ModProjectile
    {
        Vector2 saveMousePositon;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scycler");
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 44;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += 0.5f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] <= 30)
            {
                Vector2 ReturnSpeed = Main.MouseWorld - Projectile.Center;
                ReturnSpeed.Normalize();

                ReturnSpeed *= 25;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 5, 5)))
                {
                    Projectile.ai[0] = 30;
                }
            }
            if (Projectile.ai[0] > 30)
            {
                Vector2 ReturnSpeed = player.Center - Projectile.Center;
                ReturnSpeed.Normalize();

                ReturnSpeed *= 25;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
	}
}