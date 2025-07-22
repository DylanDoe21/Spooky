using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
    public class TarCannonSlime : ModProjectile
    {
        int playerStill = 0;
        bool playerFlying = false;
        bool isAttacking = false;

        NPC CurrentTarget = null;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 28;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Ranged;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
		{
            return isAttacking && Projectile.velocity.Y > 0;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC NPC = Main.npc[i];

                bool HasLineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, Projectile.position, Projectile.width, Projectile.height);
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && HasLineOfSight)
                {
                    AttackingAI(NPC);
                    CurrentTarget = NPC;

                    break;
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (!isAttacking)
            {
                Projectile.velocity.X *= 0.95f;
                Projectile.velocity.Y += 0.35f;

                CurrentTarget = null;
            }
		}

        public void JumpTo(NPC target)
        {
            Vector2 JumpTo = new Vector2(target.Center.X, target.Center.Y - 200);

            Vector2 velocity = JumpTo - Projectile.Center;

            float speed = MathHelper.Clamp(velocity.Length() / 36, 8, 20);
            velocity.Normalize();
            velocity.Y -= 0.18f;
            velocity.X *= 1.1f;
            Projectile.velocity = velocity * speed * 1.1f;
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            playerFlying = false;

            Projectile.tileCollide = true;

            Projectile.rotation = 0;

            Projectile.velocity.Y += 0.35f;

            if (Projectile.velocity.Y == 0.35f)
            {
                Projectile.velocity.X = 0;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 0)
            {
                if (Projectile.velocity.Y == 0.35f)
                {
                    JumpTo(target);
                }

                //slam down while above the current target
                if (Projectile.ai[1] == 0 && Projectile.position.X <= target.Center.X + 8 && Projectile.Center.X >= target.Center.X - 8)
                {
                    Projectile.velocity.X *= 0;
                    Projectile.velocity.Y = 16;

                    Projectile.ai[1] = 1;
                }

                //slam down on the ground
                if (Projectile.ai[1] == 1 && Projectile.velocity.Y <= 0.1f)
                {
                    Projectile.velocity.X = 0;

                    Projectile.ai[1] = 0;
                    Projectile.ai[0] = -40;
                }
            }

            if (Projectile.Center.X < target.Center.X)
            {
                Projectile.spriteDirection = 1;
            }
            else
            {
                Projectile.spriteDirection = -1;
            }
        }

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item126, Projectile.Center);

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Asphalt, 0f, -2f, 0, default, 0.85f);
				Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
			}
		}
    }
}