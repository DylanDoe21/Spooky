using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class LivingFleshEye : ModProjectile
    {
        public int SaveDirection;
        public float SaveRotation;

        public override void SetDefaults()
        {
            Projectile.width = 14;
			Projectile.height = 14;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 45;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {   
            NPC target = Main.npc[(int)Projectile.ai[1]];

            Projectile.rotation += Projectile.velocity.X * 0.2f;

            Projectile.ai[0]++;
    
            if (Projectile.ai[0] == 1)
            {
                double Velocity = Math.Atan2(target.Center.Y - Projectile.Center.Y, target.Center.X - Projectile.Center.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12;
            }

            if (Projectile.ai[0] > 20)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }
        }
    }
}