using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
    public class HazmatArmorMinion : ModProjectile
    {   
		bool isAttacking = false;
		bool LandOnGround = false;
		bool HasLanded = false;

		public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f };

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 32;
            Projectile.height = 34;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (LandOnGround && !HasLanded)
			{
				SoundEngine.PlaySound(SplatSound, Projectile.Center);

				Projectile.velocity.X *= 0;

				HasLanded = true;
			}

			return false;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			bool ValidEnemy = target.active && target.CanBeChasedBy(this) && !target.friendly && !target.dontTakeDamage && !NPCID.Sets.CountsAsCritter[target.type];

			if (ValidEnemy && !LandOnGround)
			{
				Projectile.velocity.X *= 0.5f;
				Projectile.velocity.Y -= 5;
				Projectile.tileCollide = true;
				LandOnGround = true;
			}
		}

		public override bool? CanDamage()
        {
            return isAttacking;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			Projectile.spriteDirection = -Projectile.direction;

			if (player.GetModPlayer<SpookyPlayer>().HazmatSet)
            {
				Projectile.timeLeft = 2;
			}

			if (!HasLanded)
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
			else
			{
				Projectile.frame = 4;
			}

			int MaxMinions = 0;

			foreach (string var in player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots)
			{
				if (var != string.Empty)
				{
					MaxMinions++;
				}
			}
			if (player.ownedProjectileCounts[Type] > MaxMinions)
			{
				Projectile.Kill();
			}

			if (LandOnGround)
			{
				Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

				Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
			}

			if (HasLanded)
			{
				Projectile.rotation = 0;

				Projectile.ai[1]++;

				if (Projectile.ai[1] >= 60)
				{
					LandOnGround = false;
					HasLanded = false;
					Projectile.tileCollide = false;

					Projectile.ai[0] = 0;
					Projectile.ai[1] = 0;
				}
			}

			if (!LandOnGround && !HasLanded)
			{
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
					if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 500f)
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

            //prevent Projectiles clumping together
			for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.1f;
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

			Projectile.ai[0]++;
			if (Projectile.ai[0] == 20)
			{
				Vector2 ChargeDirection = target.Center - Projectile.Center;
				ChargeDirection.Normalize();
				ChargeDirection *= 25;
				Projectile.velocity = ChargeDirection;
			}

			if (Projectile.ai[0] >= 35 && !LandOnGround)
			{
				Projectile.velocity *= 0.75f;
			}

			//try again if it didnt hit an enemy
			if (Projectile.ai[0] >= 45 && !LandOnGround)
			{
				Projectile.ai[0] = 0;
			}
		}

		public void IdleAI(Player player)
        {
			Projectile.rotation = 0;

			float num16 = 0.5f;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float horiPos = player.position.X + (float)(player.width / 2) - vector3.X;
            float vertiPos = player.position.Y + (float)(player.height / 2) - vector3.Y;
            vertiPos += (float)Main.rand.Next(-10, 15);
            horiPos += (float)Main.rand.Next(-10, 15);
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
                num16 = 0.02f;
            }
            else
            {
                if (playerDistance < 250f)
                {
                    num16 = 0.25f;
                }
                if (playerDistance > 500f)
                {
                    num16 = 0.35f;
                }
                
                playerDistance = num21 / playerDistance;
                horiPos *= playerDistance;
                vertiPos *= playerDistance;
            }

            if (Projectile.velocity.X <= horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X + num16;
                if (num16 > 0.05f && Projectile.velocity.X < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + num16;
                }
            }

            if (Projectile.velocity.X > horiPos)
            {
                Projectile.velocity.X = Projectile.velocity.X - num16;
                if (num16 > 0.05f && Projectile.velocity.X > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - num16;
                }
            }

            if (Projectile.velocity.Y <= vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + num16;
                if (num16 > 0.05f && Projectile.velocity.Y < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + num16 * 2f;
                }
            }

            if (Projectile.velocity.Y > vertiPos)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - num16;
                if (num16 > 0.05f && Projectile.velocity.Y > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - num16 * 2f;
                }
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

        }
    }
}