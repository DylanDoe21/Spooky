using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class OrbWeaverSentryBig : ModProjectile
    {
        public int numAttacks = 0;
        public bool isAttacking = false;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
        }

        public override bool? CanDamage()
		{
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

        public override void AI()
        {
            if (!isAttacking || (isAttacking && Projectile.ai[0] < 50))
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            if (isAttacking && numAttacks < 10 && Projectile.ai[0] >= 50)
            {
                Projectile.frame = 4;
            }

            //fall down constantly
            if (numAttacks < 10)
            {
                Projectile.velocity.X *= 0.95f;

                Projectile.velocity.Y++;
                if (Projectile.velocity.Y > 20f)
                {
                    Projectile.velocity.Y = 20f;
                }
            }

            //target an enemy
            for (int i = 0; i < 200; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(Projectile.Center, Target.Center) <= 500f)
                {
					AttackingAI(Target);

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 500f)
                {
					AttackingAI(NPC);

					break;
				}
                else
                {
                    isAttacking = false;
                }
            }
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.ai[0]++;

            if (numAttacks < 10)
            {
                if (Projectile.ai[0] == 50)
                {
                    SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);

                    float Velocity = Main.rand.NextFloat(15f, 26f);

                    for (int numProjectiles = -3; numProjectiles <= -1; numProjectiles++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X - 10, Projectile.Center.Y), 
                            Velocity * Projectile.DirectionTo(new Vector2(Projectile.Center.X, Projectile.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(10) * numProjectiles),
                            ModContent.ProjectileType<OrbWeaverSentryBigSpike>(), Projectile.damage, 3f, Main.myPlayer);
                        }
                    }

                    for (int numProjectiles = 1; numProjectiles <= 3; numProjectiles++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X + 10, Projectile.Center.Y), 
                            Velocity * Projectile.DirectionTo(new Vector2(Projectile.Center.X, Projectile.Center.Y - 100)).RotatedBy(MathHelper.ToRadians(10) * numProjectiles),
                            ModContent.ProjectileType<OrbWeaverSentryBigSpike>(), Projectile.damage, 3f, Main.myPlayer);
                        }
                    }
                }
            }
            else 
            {
                if (Projectile.ai[0] >= 50)
                {
                    //set where the it should be jumping towards
                    Vector2 JumpTo = new Vector2(target.Center.X, target.Center.Y - 100);

                    //set velocity and speed
                    Vector2 velocity = JumpTo - Projectile.Center;
                    velocity.Normalize();

                    float speed = MathHelper.Clamp(velocity.Length() / 36, 22, 35);

                    Projectile.ai[1]++;

                    if (Projectile.ai[1] > 10)
                    {
                        velocity.X *= 1.2f;
                        velocity.Y *= 0.5f;
                        Projectile.velocity = velocity * speed;
                    }
                }
            }

            if (Projectile.ai[0] >= 80)
            {
                if (numAttacks < 10)
                {
                    numAttacks++;
                }
                else
                {
                    numAttacks = 0;
                }

                Projectile.ai[1] = 0;
                Projectile.ai[0] = 0;
            }
		}
    }
}