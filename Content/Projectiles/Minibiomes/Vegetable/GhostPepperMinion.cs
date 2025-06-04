using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
    public class GhostPepperMinionTier1 : ModProjectile
    {
		bool isAttacking = false;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 20;
            Projectile.height = 56;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
			Projectile.minionSlots = 1;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

		public override bool? CanDamage()
        {
            return isAttacking;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 20;
			}
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			Projectile.spriteDirection = -Projectile.direction;

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<GhostPepperMinionBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<GhostPepperMinionBuff>()))
            {
				Projectile.timeLeft = 2;
			}

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

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type])
				{
					AttackingAI(Target);

					break;
				}
				else
				{
					isAttacking = false;
				}

				NPC NPC = Main.npc[i];
				if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 700f)
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
		}

        public void AttackingAI(NPC target)
        {
			isAttacking = true;

			if (Projectile.Distance(target.Center) >= 20f)
            {
				int Speed = 4;
				if (Projectile.type == ModContent.ProjectileType<GhostPepperMinionTier2>()) Speed = 8;
				if (Projectile.type == ModContent.ProjectileType<GhostPepperMinionTier3>()) Speed = 12;
				if (Projectile.type == ModContent.ProjectileType<GhostPepperMinionTier4>()) Speed = 16;

				Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * Speed;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
			}

            if (Projectile.Distance(target.Center) >= 120f)
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 20;
                }
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 20;
                }
            }
		}

		public void IdleAI(Player player)
        {
			Projectile.rotation = 0;

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 20;
			}

			float Speed = 0.25f;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float horiPos = player.position.X + (float)(player.width / 2) - vector3.X;
            float vertiPos = player.position.Y + (float)(player.height / 2) - vector3.Y;
            vertiPos += (float)Main.rand.Next(-30, -15);
            horiPos += (float)Main.rand.Next(-15, 16);
            vertiPos -= 60f;
            float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

            float num21 = 18f;
            float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

            if (playerDistance > 1200f)
            {
                Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
                Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
                Projectile.netUpdate = true;
            }

            if (playerDistance < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.90f;
                }
                Speed = 0.02f;
            }
            else
            {
                if (playerDistance < 250f)
                {
                    Speed = 0.1f;
                }
                if (playerDistance > 500f)
                {
                    Speed = 0.2f;
                }
                
                playerDistance = num21 / playerDistance;
                horiPos *= playerDistance;
                vertiPos *= playerDistance;
            }

            if (Projectile.velocity.X <= horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X + Speed;
                if (Speed > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + Speed;
                }
            }

            if (Projectile.velocity.X > horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X - Speed;
                if (Speed > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - Speed;
                }
            }

            if (Projectile.velocity.Y <= vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + Speed;
                if (Speed > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + Speed * 2f;
                }
            }

            if (Projectile.velocity.Y > vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - Speed;
                if (Speed > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - Speed * 2f;
                }
            }
        }
    }

	public class GhostPepperMinionTier2 : GhostPepperMinionTier1
    {
		public override void SetDefaults()
        {
			Projectile.width = 28;
            Projectile.height = 62;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
			Projectile.minionSlots = 2;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }
	}

	public class GhostPepperMinionTier3 : GhostPepperMinionTier1
    {
		public override void SetDefaults()
        {
			Projectile.width = 38;
            Projectile.height = 92;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
			Projectile.minionSlots = 3;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }
	}

	public class GhostPepperMinionTier4 : GhostPepperMinionTier1
    {
		public override void SetDefaults()
        {
			Projectile.width = 62;
            Projectile.height = 120;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
			Projectile.minionSlots = 4;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }
	}
}