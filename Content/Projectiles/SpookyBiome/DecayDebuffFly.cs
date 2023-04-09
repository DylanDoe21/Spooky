using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class DecayDebuffFly : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.scale = 0.9f;
            Projectile.aiStyle = -1;
        }

        public override bool CanHitPlayer(Player target)
        {
            if (Projectile.ai[0] == 1 || Projectile.ai[0] == 3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void AI()
		{
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            Projectile.alpha += 5;

            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
		}
	}
}