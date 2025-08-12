using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class KrampusBricksProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			target.velocity = Vector2.Zero;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity.X *= 0;

                Projectile.ai[0]++;
            }

            return false;
        }

        public override void AI()
        {
            Projectile.frame = (int)Projectile.ai[1];

            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation += 0.5f * (float)Projectile.direction;

                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;

                Projectile.timeLeft = 300;
            }
            else
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;

                Projectile.rotation = 0;
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
        }
    }
}