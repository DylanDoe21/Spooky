using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
    public class GhostPepperMinion : ModProjectile
    {
        int saveDirection = 0;

        bool Charging = false;
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 62;
            Projectile.height = 66;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return Charging;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Charging)
            {
                ProjTexture ??= ModContent.Request<Texture2D>(Texture);

                Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Cyan);

                Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

                var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 4 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                    Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, effects, 0);
                }
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<OldHunterBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<OldHunterBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 750f)
                {
                    AttackingAI(Target);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 750f)
                {
                    AttackingAI(NPC);

                    break;
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (!isAttacking)
            {
                IdleAI(player);
            }

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.45f;
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.X += pushAway;
                    }
                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.Y += pushAway;
                    }
                }
            }
		}

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            if (Projectile.ai[1] < 60)
            {
                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;
            }
            else
            {
                Projectile.spriteDirection = saveDirection;
            }

            //flying animation with sword held up
            if (!Charging)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 8)
                    {
                        Projectile.frame = 5;
                    }
                }
            }
            //slashing animation
            else
            {
                if (Projectile.frame < 7)
                {
                    Projectile.frame = 7;
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 11)
                    {
                        Projectile.frame = 10;
                    }
                }
            }

            Projectile.ai[1]++;

            //go to the side of the target to prepare for dashing
            if (Projectile.ai[1] < 60)
            {
                Vector2 GoTo = target.Center;
                GoTo.X += (Projectile.Center.X < target.Center.X) ? -150 : 150;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 6, 20);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //dash at the target
            if (Projectile.ai[1] == 60)
            {
                Charging = true;

                saveDirection = Projectile.spriteDirection;

                //set frame to charging immediately 
                Projectile.frame = 5;
                
                Vector2 ChargeDirection = target.Center - Projectile.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= 20;
                ChargeDirection.Y *= 5;
                Projectile.velocity.X = ChargeDirection.X;
                Projectile.velocity.Y = ChargeDirection.Y;
            }

            //slow down at the end of the charge
            if (Projectile.ai[1] >= 80)
            {
                Charging = false;
                Projectile.velocity *= 0.7f;
            }

            //loop ai
            if (Projectile.ai[1] >= 85)
            {
                Projectile.ai[1] = 0;
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.spriteDirection = player.Center.X > Projectile.Center.X ? -1 : 1;

            if (Projectile.velocity.X > 0)
            {
                Projectile.spriteDirection = -1;
            }
            if (Projectile.velocity.X < 0)
            {
                Projectile.spriteDirection = 1;
            }

            //idle animation
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

            ///reset attacking ai stuff
            Charging = false;
            Projectile.ai[1] = 0;

            //movement stuff
            if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 8f;
            if (Projectile.ai[0] == 1f)
            {
                speed = 15f;
            }

            Vector2 center = Projectile.Center;
            Vector2 direction = player.Center - center;
            Projectile.netUpdate = true;
            int num = 1;
            for (int k = 0; k < Projectile.whoAmI; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
                {
                    num++;
                }
            }
            
            direction.X += 50f;
            direction.Y -= 20f;
            float distanceTo = direction.Length();
            if (distanceTo > 200f && speed < 9f)
            {
                speed = 9f;
            }
            if (distanceTo < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 48f)
            {
                direction.Normalize();
                direction *= speed;
                float temp = 40 / 2f;
                Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
            }
            else
            {
                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
            }

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                return;
            }
        }
    }
}