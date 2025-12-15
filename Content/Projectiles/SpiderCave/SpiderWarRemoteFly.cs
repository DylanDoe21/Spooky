using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SpiderWarRemoteFly : ModProjectile
    {
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.localNPCHitCooldown = 45;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
			Projectile.minionSlots = 1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Gray) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<SpiderWarRemoteFlyBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<SpiderWarRemoteFlyBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            //fix rotation
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 1500f)
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

			if (!isAttacking && Projectile.Distance(new Vector2(player.Center.X, player.Center.Y - 100)) >= 50)
            {
                IdleAI(player);
            }

            //teleport to the player if they get too far
            if (Projectile.Distance(player.Center) > 1200f)
            {
                Projectile.position = player.Center;
            }

            //prevent Projectiles clumping together
            for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.2f;
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

            if (Projectile.Distance(target.Center) >= 65f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 40;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
        }

        public void IdleAI(Player player)
		{
            Vector2 PlayerGoTo = new Vector2(player.Center.X, player.Center.Y - 100);

            if (Projectile.Distance(PlayerGoTo) <= 50f)
            {
                float goToX = player.Center.X - Projectile.Center.X;
                float goToY = (player.Center.Y - 100) - Projectile.Center.Y;
                float speed = 0.3f;
                
                if (Projectile.velocity.X < goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speed;
                    if (Projectile.velocity.X < 0f && goToX > 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + speed;
                    }
                }
                else if (Projectile.velocity.X > goToX)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speed;
                    if (Projectile.velocity.X > 0f && goToX < 0f)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - speed;
                    }
                }
                if (Projectile.velocity.Y < goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speed;
                    if (Projectile.velocity.Y < 0f && goToY > 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + speed;
                        return;
                    }
                }
                else if (Projectile.velocity.Y > goToY)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speed;
                    if (Projectile.velocity.Y > 0f && goToY < 0f)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - speed;
                        return;
                    }
                }
            }
            else
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(PlayerGoTo) * 40;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
        }
    }
}