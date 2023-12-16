using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class GraveDiggerShovelProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Cemetery/GraveDiggerShovel";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpookyPlayer.ScreenShakeAmount = 3;

            //return to the player upon hitting an enemy
            Projectile.ai[0] = 8;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += 0.35f * (float)Projectile.direction;

            Projectile.ai[0]++;
            
            if (Projectile.ai[0] >= 8)
            {
                Vector2 ReturnSpeed = owner.Center - Projectile.Center;
                ReturnSpeed.Normalize();
                ReturnSpeed *= 12;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
    }
}