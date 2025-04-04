using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs;

namespace Spooky.Content.Projectiles.Blooms
{
    public class Cauliflower : ModProjectile
    {
        Vector2 SavePosition;

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
		}

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[1] > 25;
        }

        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.frame = (int)Projectile.ai[0];

            Projectile.ai[1]++;
            if (Projectile.ai[1] > 25)
            {
                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 12;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
            }
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 250;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }
    }
}