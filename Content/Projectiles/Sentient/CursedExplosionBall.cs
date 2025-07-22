using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class CursedExplosionBall : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override void AI()
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 1f;

            for (int numDust = 0; numDust < 2; numDust++)
			{
                Vector2 dustPosition = Projectile.Center;
                dustPosition -= Projectile.velocity * ((float)numDust * 0.25f);
                int dust = Dust.NewDust(dustPosition, 1, 1, DustID.CursedTorch, 0f, 0f, 0, default, 2f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = dustPosition;
                Main.dust[dust].velocity *= 0.2f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            for (int numDust = 0; numDust < 35; numDust++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, -2f, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
			}
		}
    }
}