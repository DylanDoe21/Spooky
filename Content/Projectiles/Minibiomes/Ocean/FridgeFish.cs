using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class FridgeFish : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 44;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            if (Projectile.ai[1] == 2)
            {
                target.AddBuff(BuffID.Stinky, 600);
                target.AddBuff(BuffID.Poisoned, 600);
            }
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.frame = (int)Projectile.ai[1];

            if (Projectile.ai[1] == 2)
            {
                if (Main.rand.NextBool(10))
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 188, 0f, -2f, 0, default, 1.5f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Projectile.velocity;
                    Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                    Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                }
            }
            
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 15)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
            }
        }
    }
}