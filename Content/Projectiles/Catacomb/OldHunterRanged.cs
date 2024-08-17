using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class OldHunterRanged : ModProjectile
    {
        int saveDirection = 0;

        bool Shooting = false;
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 64;
            Projectile.height = 50;
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
            //the old hunter ranger doesnt need to deal contact damage
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Shooting)
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

            if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().OldHunter = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().OldHunter)
			{
				Projectile.timeLeft = 2;
			}

            //target an enemy
            for (int i = 0; i < 200; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this, false) && !NPCID.Sets.CountsAsCritter[Target.type])
                {
                    AttackingAI(Target);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 750f)
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

            for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.08f;
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
            if (!Shooting)
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
            //shooting animation
            else
            {
                if (Projectile.frame < 5)
                {
                    Projectile.frame = 4;
                }

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 7)
                    {
                        Projectile.frame = 0;
                        Shooting = false;
                    }
                }
            }

            Projectile.ai[1]++;

            //go to the upper side of the target to prepare for shooting
            if (Projectile.ai[1] < 60)
            {
                Vector2 GoTo = target.Center;
                GoTo.X += (Projectile.Center.X < target.Center.X) ? -200 : 200;
                GoTo.Y -= 20;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 6, 20);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //dash at the target
            if (Projectile.ai[1] == 60)
            {
                Shooting = true;

                Projectile.velocity *= 0;

                saveDirection = Projectile.spriteDirection;

                for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
                {
                    SoundEngine.PlaySound(SoundID.Item36, Projectile.Center);

                    Vector2 Recoil = target.Center - Projectile.Center;
                    Recoil.Normalize(); 
                    Recoil *= -8;
                    Projectile.velocity = Recoil;

                    Vector2 ShootSpeed = target.Center - Projectile.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 18f;

                    int offset = saveDirection == 1 ? -50 : 50;
                            
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + offset, Projectile.Center.Y - 10, ShootSpeed.X + Main.rand.Next(-3, 4), 
                    ShootSpeed.Y + Main.rand.Next(-5, 6), ModContent.ProjectileType<OldHunterRangedBullet>(), Projectile.damage / 2, 2f, Main.myPlayer, 0f, 0f);
                }
            }

            if (Projectile.ai[1] > 60)
            {
                Projectile.velocity *= 0.9f;
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
            Shooting = false;
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
            
            direction.X -= 50f;
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