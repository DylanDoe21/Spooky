using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class StockingMinion : ModProjectile
    {
        int playerStill = 0;
        bool playerFlying = false;
        bool isAttacking = false;

        NPC CurrentTarget = null;

		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
			Projectile.height = 36;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
			Projectile.minionSlots = 1;
            Projectile.aiStyle = -1;
        }

        public override bool? CanDamage()
		{
            return isAttacking;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            Player player = Main.player[Projectile.owner];

            fallThrough = CurrentTarget == null ? (Projectile.position.Y < player.Center.Y - (Projectile.height) && !isAttacking) : (Projectile.position.Y < CurrentTarget.Center.Y - (Projectile.height));

            return true;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new((ProjTexture.Width() / 7) * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			Rectangle rectangle = new((ProjTexture.Width() / 7) * (int)Projectile.ai[2], 
			ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, 
			ProjTexture.Width() / 7, 
			ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			return false;
		}

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<StockingMinionBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<StockingMinionBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                //first, if the player gets too far away while an enemy is being targetted then have the minion stop attacking and return to the player
                if (Vector2.Distance(player.Center, Projectile.Center) >= 550f)
                {
                    isAttacking = false;
                    IdleAI(player);

                    break;
                }

				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type])
                {
					AttackingAI(Target);
                    CurrentTarget = Target;

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 450f)
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
                IdleAI(player);
                CurrentTarget = null;
            }

            for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];

				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.5f;

					if (Projectile.position.X < other.position.X)
					{
                        if (Projectile.velocity.X > -2)
                        {
						    Projectile.velocity.X -= pushAway;
                        }
					}
					else
					{
                        if (Projectile.velocity.X < 2)
                        {
						    Projectile.velocity.X += pushAway;
                        }
					}
				}
			}
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.tileCollide = true;

            Projectile.rotation = 0;

            Vector2 vector48 = target.Center - Projectile.Center;
            float targetDistance = vector48.Length();

            if (Projectile.velocity.Y == 0 && (HoleBelow() || (targetDistance > 50f && Projectile.position.X == Projectile.oldPosition.X)))
            {
                Projectile.velocity.Y = -9f;
            }

            //jump if the target is too high
            if (Projectile.velocity.Y == 0 && target.Center.Y < Projectile.Center.Y - 50)
            {
                Projectile.velocity.Y = Main.rand.Next(-12, -6);
            }

            Projectile.velocity.Y += 0.35f;

            if (Projectile.velocity.Y > 15f)
            {
                Projectile.velocity.Y = 15f;
            }

            if (target.position.X - Projectile.position.X > 0f)
            {
                Projectile.velocity.X += 0.35f;
                if (Projectile.velocity.X > 6f)
                {
                    Projectile.velocity.X = 6f;
                }
            }
            else
            {
                Projectile.velocity.X -= 0.35f;
                if (Projectile.velocity.X < -6f)
                {
                    Projectile.velocity.X = -6f;
                }
            }

            //falling frame
            if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
            {
                Projectile.frame = 2;
            }
            //moving animation
            else if (Projectile.velocity.X != 0)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter > 2)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                }
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.velocity.X > 0.8f)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.velocity.X < -0.8f)
            {
                Projectile.spriteDirection = 1;
            }
        }

        public void IdleAI(Player player)
        {
            if (!playerFlying)
            {
                Projectile.rotation = 0;

                Vector2 vector48 = player.Center - Projectile.Center;
                float playerDistance = vector48.Length();

                if (Projectile.velocity.Y == 0 && ((HoleBelow() && playerDistance > 100f) || (playerDistance > 100f && Projectile.position.X == Projectile.oldPosition.X)))
                {
                    Projectile.velocity.Y = -6f;
                }

                Projectile.velocity.Y += 0.35f;

                if (Projectile.velocity.Y > 15f)
                {
                    Projectile.velocity.Y = 15f;
                }

                if (playerDistance > 500f)
                {
                    playerFlying = true;
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                }

                if (playerDistance > 80f)
                {
                    if (player.position.X - Projectile.position.X > 0f)
                    {
                        Projectile.velocity.X += 0.12f;
                        if (Projectile.velocity.X > 6f)
                        {
                            Projectile.velocity.X = 6f;
                        }
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.12f;
                        if (Projectile.velocity.X < -6f)
                        {
                            Projectile.velocity.X = -6f;
                        }
                    }
                }

                if (playerDistance <= 80f)
                {
                    if (Projectile.velocity.X != 0f)
                    {
                        if (Projectile.velocity.X > 0.8f)
                        {
                            Projectile.velocity.X -= 0.1f;
                        }
                        else if (Projectile.velocity.X < -0.8f)
                        {
                            Projectile.velocity.X += 0.1f;
                        }
                        else if (Projectile.velocity.X < 0.8f && Projectile.velocity.X > -0.8f)
                        {
                            Projectile.velocity.X = 0f;
                        }
                    }
                }

                //set frames when idle
                if (Projectile.position.X == Projectile.oldPosition.X && Projectile.position.Y == Projectile.oldPosition.Y && Projectile.velocity.X == 0)
                {
                    Projectile.frame = 0;
                }
                //falling frame
                else if (Projectile.velocity.Y > 0.3f && Projectile.position.Y != Projectile.oldPosition.Y)
                {
                    Projectile.frame = 2;
                }
                //moving animation
                else if (Projectile.velocity.X != 0)
                {
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter > 2)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    if (Projectile.frame >= 5)
                    {
                        Projectile.frame = 0;
                    }
                }

                if (Projectile.velocity.X > 0.8f)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.velocity.X < -0.8f)
                {
                    Projectile.spriteDirection = 1;
                }
            }
            else if (playerFlying)
            {
                float num16 = 0.5f;
                Projectile.tileCollide = false;
                Vector2 vector3 = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                float horiPos = player.position.X + (float)(player.width / 2) - vector3.X;
                float vertiPos = player.position.Y + (float)(player.height / 2) - vector3.Y;
                vertiPos += (float)Main.rand.Next(-10, 21);
                horiPos += (float)Main.rand.Next(-10, 21);
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

                if (playerDistance < 100f)
                {
                    num16 = 0.5f;
                    if (player.velocity.Y == 0f)
                    {
                        playerStill++;
                    }
                    else
                    {
                        playerStill = 0;
                    }
                    if (playerStill > 10 && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                    {
                        playerFlying = false;
                        Projectile.velocity *= 0.2f;
                        Projectile.tileCollide = true;
                    }
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

                Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

                if (Projectile.Center.X < player.Center.X)
                {
                    Projectile.spriteDirection = -1;
                }
                else if (Projectile.Center.X > player.Center.X)
                {
                    Projectile.spriteDirection = 1;
                }

                Projectile.frame = 0;
                Projectile.frameCounter = 0;
            }
        }

        private bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(Projectile.Center.X / 16f) - tileWidth;
            if (Projectile.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((Projectile.position.Y + Projectile.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}