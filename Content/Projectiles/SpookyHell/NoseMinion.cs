using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class NoseMinion : ModProjectile
    {   
		bool isAttacking = false;

        Vector2 SaveProjPosition;

		public static readonly SoundStyle SneezeSound = new("Spooky/Content/Sounds/Moco/MocoSneeze1", SoundType.Sound) { Volume = 0.75f, Pitch = 0.8f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 44;
            Projectile.height = 42;
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

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			Projectile.spriteDirection = -Projectile.direction;

			if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().NoseMinion = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().NoseMinion)
			{
				Projectile.timeLeft = 2;
			}

			Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

			for (int i = 0; i < 200; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				NPC NPC = Main.npc[i];
                if (Target != null && Target.CanBeChasedBy(this, false) && !NPCID.Sets.CountsAsCritter[Target.type])
                {
                    Shoot(Target);

                    break;
                }
				else
				{
					isAttacking = false;
				}

                if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 450f)
                {
                    Shoot(NPC);

                    break;
                }
				else
				{
					isAttacking = false;
				}
            }

			if (!isAttacking)
			{
				IdleMovement(player);
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

		public void IdleMovement(Player player)
        {
			Projectile.rotation = 0;

			Projectile.localAI[0] = 0;

			float num16 = 0.5f;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float horiPos = player.position.X + (float)(player.width / 2) - vector3.X;
            float vertiPos = player.position.Y + (float)(player.height / 2) - vector3.Y;
            vertiPos += (float)Main.rand.Next(-10, 15);
            horiPos += (float)Main.rand.Next(-10, 15);
            horiPos += (float)(60 * -(float)player.direction);
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
                if (playerDistance < 100f)
                {
                    num16 = 0.35f;
                }
                if (playerDistance > 300f)
                {
                    num16 = 1f;
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

            if (Projectile.Center.X < player.Center.X)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.Center.X > player.Center.X)
            {
                Projectile.spriteDirection = 1;
            }
        }

		public void IdleMovementButForNPCs(NPC npc)
        {
			float num16 = 0.5f;
            Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float horiPos = npc.Center.X - vector3.X;
            float vertiPos = (npc.Center.Y - 150) - vector3.Y;
            vertiPos += (float)Main.rand.Next(-15, 15);
            horiPos += (float)Main.rand.Next(-15, 15);
            float npcDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
            float num21 = 18f;
            float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

            if (npcDistance < 50f)
            {
                if (Math.Abs(Projectile.velocity.X) > 2f || Math.Abs(Projectile.velocity.Y) > 2f)
                {
                    Projectile.velocity *= 0.90f;
                }
                num16 = 0.02f;
            }
            else
            {
                if (npcDistance < 100f)
                {
                    num16 = 0.35f;
                }
                if (npcDistance > 300f)
                {
                    num16 = 1f;
                }
                
                npcDistance = num21 / npcDistance;
                horiPos *= npcDistance;
                vertiPos *= npcDistance;
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

            if (Projectile.Center.X < npc.Center.X)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.Center.X > npc.Center.X)
            {
                Projectile.spriteDirection = 1;
            }
        }

		public void Shoot(NPC target)
        {
			isAttacking = true;

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			float RotateX = target.Center.X - vector.X;
			float RotateY = target.Center.Y - vector.Y;
			Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			Projectile.localAI[0]++;

			if (Projectile.localAI[0] < 60)
			{
				IdleMovementButForNPCs(target);
			}

			if (Projectile.localAI[0] == 60)
			{
				SaveProjPosition = Projectile.Center;
			}

			if (Projectile.localAI[0] > 60 && Projectile.localAI[0] < 90)
			{
				Projectile.Center = new Vector2(SaveProjPosition.X, SaveProjPosition.Y);
                Projectile.Center += Main.rand.NextVector2Square(-3, 3);
			}

			if (Projectile.localAI[0] == 90)
			{
				SoundEngine.PlaySound(SneezeSound, Projectile.Center);

				Vector2 Recoil = target.Center - Projectile.Center;
				Recoil.Normalize(); 
				Recoil *= -8;
				Projectile.velocity = Recoil;

				Vector2 ShootSpeed = target.Center - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 20f;
                        
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 
                ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<NoseMinionBooger>(), Projectile.damage, 2f, Main.myPlayer, 0f, 0f);
			}

			if (Projectile.localAI[0] >= 90)
			{
				Projectile.velocity *= 0.95f;
			}

			if (Projectile.localAI[0] >= 110)
			{
				Projectile.localAI[0] = 0;
			}
		}
    }
}