using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class SharkArmorFriend : ModProjectile
    {
        int saveDirection = 0;

        bool Charging = false;
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 8;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 44;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return Charging;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().SharkBoneSet = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().SharkBoneSet)
			{
				Projectile.timeLeft = 2;
			}

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }

            //scale based on players fishing rod power
			int scaling = Projectile.originalDamage + (ItemGlobal.ActiveItem(player).fishingPole / 2);
			Projectile.damage = scaling;

			//target an enemy
			for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 450f)
                {
                    AttackingAI(Target, player);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 450f)
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
		}

        public void AttackingAI(NPC target, Player player)
		{
            isAttacking = true;

            Projectile.rotation = 0;

            if (Projectile.ai[0] < 60)
            {
                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;
            }
            else
            {
                Projectile.spriteDirection = saveDirection;
            }

            Projectile.ai[0]++;

            //go to the side of the target to prepare for dashing
            if (Projectile.ai[0] < 20)
            {
                Vector2 GoTo = target.Center;
                GoTo.X += (player.Center.X < target.Center.X) ? -150 : 150;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 6, 20);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //dash at the target
            if (Projectile.ai[0] == 20)
            {
                Charging = true;

                saveDirection = Projectile.spriteDirection;
                
                Vector2 ChargeDirection = target.Center - Projectile.Center;
                ChargeDirection.Normalize();
                ChargeDirection *= 10;
                Projectile.velocity = ChargeDirection;
            }

            //slow down at the end of the charge
            if (Projectile.ai[0] >= 35)
            {
                Charging = false;
                Projectile.velocity *= 0.7f;
            }

            //loop ai
            if (Projectile.ai[0] >= 40)
            {
                Projectile.ai[0] = 0;
            }
        }

        public void IdleAI(Player player)
		{
            if (Projectile.velocity.X > 0)
            {
                Projectile.spriteDirection = -1;
            }
            else
            {
                Projectile.spriteDirection = 1;
            }

            //reset attacking ai stuff
            Charging = false;
            Projectile.ai[0] = 0;

            float Speed = 0.5f;
            float horiPos = player.Center.X - Projectile.Center.X;
            float vertiPos = player.Center.Y - Projectile.Center.Y;
            vertiPos += (float)Main.rand.Next(-10, 15);
            horiPos += (float)Main.rand.Next(-3, 4);
            horiPos += (float)(60 * -(float)player.direction);
            vertiPos -= 45f;

            float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));

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
                if (playerDistance < 150f)
                {
                    Speed = 0.1f;
                }
                if (playerDistance > 400f)
                {
                    Speed = 0.25f;
                }
                
                playerDistance = 18f / playerDistance;
                horiPos *= playerDistance / 2;
                vertiPos *= playerDistance / 2;
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
}