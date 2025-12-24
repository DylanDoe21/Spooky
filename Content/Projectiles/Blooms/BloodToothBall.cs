using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Blooms
{
    public class BloodToothBall : ModProjectile
    {
        int foundTarget = -1;
        
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            SoundEngine.PlaySound(SoundID.Item177 with { Volume = 0.25f }, Projectile.Center);

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.95f;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
            }

			return false;
		}

        public override bool? CanDamage()
		{
			return Projectile.velocity.Y >= 0;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //explode into teeth
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            foundTarget = FindTarget();

            if (foundTarget != -1)
            {
                NPC target = Main.npc[foundTarget];

                Projectile.velocity.X += target.Center.X < Projectile.Center.X ? -0.12f : 0.12f;
                Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -5, 5);
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] > 30)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
            }
        }

        private int FindTarget()
        {
            const float homingMaximumRangeInPixels = 1000;

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

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = 0.5f }, Projectile.Center);

            float maxAmount = 10;
            int currentAmount = 0;
            while (currentAmount <= maxAmount)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f));
                float intensity = Main.rand.NextFloat(1f, 3f);

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                int newDust = Dust.NewDust(Projectile.Center, 0, 0, DustID.Blood, 0f, 0f, 100, default, 3f);
                Main.dust[newDust].noGravity = true;
                Main.dust[newDust].position = Projectile.Center + vector12;
                Main.dust[newDust].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                currentAmount++;
            }
		}
    }
}