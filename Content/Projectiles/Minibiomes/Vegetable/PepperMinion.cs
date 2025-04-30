using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
    public class PepperMinion : ModProjectile
    {
        bool isAttacking = false;

        public override void SetStaticDefaults()
        {
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 22;
            Projectile.height = 24;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
			Projectile.minionSlots = 0.5f;
            Projectile.aiStyle = -1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 2;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<PepperMinionBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<PepperMinionBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f;

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 600f)
                {
                    AttackingAI(Target, player);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 600f)
                {
                    AttackingAI(NPC, player);

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

            //teleport to the player if they get too far
            if (Projectile.Distance(player.Center) > 1200f)
            {
                Projectile.position = player.Center;
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

        public void AttackingAI(NPC target, Player player)
		{
            bool HasLineOfSight = Collision.CanHitLine(target.position, target.width, target.height, player.position, player.width, player.height);
            if (HasLineOfSight)
            {
                isAttacking = true;

                int ProjDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, Color.Orange, 1f);
                Main.dust[ProjDust].noGravity = true;
                Main.dust[ProjDust].noLight = true;
                Main.dust[ProjDust].velocity /= 4f;
                Main.dust[ProjDust].velocity -= Projectile.velocity;

                if (Projectile.Distance(target.Center) > 300)
                {
                    Projectile.ai[1] = 0;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 8, 0.1f);

                    Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation() + MathHelper.PiOver2, 0.04f);
                }
                else
                {
                    Projectile.ai[1]--;
                    if (Projectile.ai[1] <= 0)
                    {
                        Projectile.ai[1] = 50;
                        Projectile.velocity = Projectile.DirectionTo(target.Center).RotatedByRandom(MathHelper.Pi / 16) * 12 * MathHelper.Clamp(Projectile.Distance(target.Center) / 100, 1f, 1.33f);
                        Projectile.netUpdate = true;
                    }

                    Projectile.velocity = Projectile.velocity.Length() * 0.985f * Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * Projectile.velocity.Length(), 0.2f));
                }
            }
            else
            {
                IdleAI(player);
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.ai[0]++;
            if (Projectile.ai[0] % 60 == 0)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * Projectile.Distance(player.Center);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }
        }
    }
}