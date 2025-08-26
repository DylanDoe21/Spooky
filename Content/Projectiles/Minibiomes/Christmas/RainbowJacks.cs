using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class RainbowJacks : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }

        public override void SetDefaults()
        {
			Projectile.width = 20;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 420;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            Projectile.velocity = Projectile.Center.X < target.Center.X ? new Vector2(-5, -2) : new Vector2(5, -2);
        }

        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[1];

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }

            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 5f)
            {
                Projectile.ai[0] = 5f;
                if (Projectile.velocity.Y == 0f && Projectile.velocity.X != 0f)
                {
                    Projectile.velocity.X *= 0.97f;
                    if ((double)Projectile.velocity.X > -0.01 && (double)Projectile.velocity.X < 0.01)
                    {
                        Projectile.velocity.X = 0f;
                        Projectile.netUpdate = true;
                    }
                }

                Projectile.velocity.Y += 0.2f;
            }

            Projectile.rotation += Projectile.velocity.X * 0.1f;
			
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}
        }
    }
}