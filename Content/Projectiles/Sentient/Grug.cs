using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Sentient
{
    public class Grug : ModProjectile
    {
        bool StoneForm = true;
        bool UsingMagic = false;
        bool Charging = false;
        bool isAttacking = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 84;
            Projectile.height = 72;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 600;
            Projectile.minionSlots = 2;
            Projectile.penetrate = -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            StoneForm = false;

            Projectile.velocity *= 0;
            Projectile.frame = 1;
            
            SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.3f }, Projectile.position);

			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().Grug = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().Grug)
			{
				Projectile.timeLeft = 2;
			}

            if (StoneForm)
            {
                Projectile.frame = 0;

                Projectile.ai[2]++;
                if (Projectile.ai[2] >= 30)
                {
                    Projectile.tileCollide = true;

                    Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
                }
                else
                {
                    Projectile.tileCollide = false;
                }
            }
            else
            {
                Projectile.tileCollide = false;

                //target an enemy
                for (int i = 0; i < 200; i++)
                {
                    NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                    if (Target != null && Target.CanBeChasedBy(this, false) && Vector2.Distance(Projectile.Center, Target.Center) <= 750f)
                    {
                        AttackingAI(Target);

                        break;
                    }
                    else
                    {
                        isAttacking = false;
                    }

                    NPC NPC = Main.npc[i];
                    if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && Vector2.Distance(Projectile.Center, NPC.Center) <= 750f)
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

            if (!isAttacking && !StoneForm)
            {
                IdleAI(player);
            }
		}

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            //flying animation
            if (!Charging)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 8)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 5)
                    {
                        Projectile.frame = 1;
                    }
                }
            }
            //charging animation
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 8)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 9)
                    {
                        Projectile.frame = 5;
                    }
                }
            }

            Projectile.ai[1]++;

            //go above the target to prepare for magic attack
            if (Projectile.ai[1] <= 70)
            {
                Vector2 GoTo = target.Center;
                GoTo.Y -= 200;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 12, 25);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //shoot cursed flames
            if (Projectile.ai[1] == 80 || Projectile.ai[1] == 100 || Projectile.ai[1] == 120)
            {
                UsingMagic = true;

                Projectile.velocity *= 0.75f;
            }

            //go to the side of the target to prepare for dashing
            if (Projectile.ai[1] >= 140 && Projectile.ai[1] < 200)
            {
                UsingMagic = false;

                Vector2 GoTo = target.Center;
                GoTo.X += (Projectile.Center.X < target.Center.X) ? -200 : 200;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 12, 25);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //charge at the target
            if (Projectile.ai[1] == 200)
            {
                Charging = true;

                Projectile.frame = 5;
                
                Vector2 ChargeDirection = target.Center - Projectile.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= 35;
                ChargeDirection.Y *= 0;
                Projectile.velocity.X = ChargeDirection.X;
                Projectile.velocity.Y = ChargeDirection.Y;
            }

            if (Projectile.ai[1] >= 220)
            {
                Charging = false;
                Projectile.velocity *= 0.85f;
                Projectile.ai[1] = 0;
            }
        }

        public void IdleAI(Player player)
		{
            //idle animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 1;
                }
            }

            ///reset attacking ai stuff
            UsingMagic = false;
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
            
            direction.Y -= 70f;
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

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
                return;
            }
        }
    }
}