using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class BanditBruiserMinion : ModProjectile
    {
        public override string Texture => "Spooky/Content/NPCs/Quest/BanditBruiser";

        int saveDirection = 0;

        float addedStretch = 0f;
		float stretchRecoil = 0f;

        bool Charging = false;
        bool isAttacking = false;
        bool Shake = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 90;
            Projectile.height = 102;
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

            float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;
			
			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lerp(Color.Red, Color.OrangeRed, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, scaleStretch * 1.1f, effects, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, scaleStretch, effects, 0);

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().GhostBookRed = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().GhostBookRed)
			{
				Projectile.timeLeft = 2;
			}

            //stretch stuff
            if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.1f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 450f)
                {
                    AttackingAI(Target);

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

            Projectile.rotation = 0;

            if (Projectile.ai[1] < 60)
            {
                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;
            }
            else
            {
                Projectile.spriteDirection = saveDirection;
            }

            //idle animation
            if (Projectile.ai[1] < 45)
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
            //punching animation
            else
            {
                if (Projectile.frame < 5)
                {
                    Projectile.frame = 5;
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 8)
                    {
                        Projectile.frame = 8;
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

                stretchRecoil = 0.5f;
                
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
            if (Projectile.velocity.X > 0)
            {
                Projectile.spriteDirection = -1;
            }
            else
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

            if (Shake)
            {
                Projectile.rotation += 0.01f;
                if (Projectile.rotation > 0.12f)
                {
                    Shake = false;
                }
            }
            else
            {
                Projectile.rotation -= 0.01f;
                if (Projectile.rotation < -0.12f)
                {
                    Shake = true;
                }
            }

            //reset attacking ai stuff
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
            
            direction.Y -= 75f;
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