using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class SpookFishronMinion : ModProjectile
    {
        int saveDirection = 0;

        bool Charging = false;
        bool isAttacking = false;

        Vector2 SaveTargetPos;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 52;
            Projectile.height = 72;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.minionSlots = 2;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return Charging;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            if (Charging)
            {
                Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

                var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
                {
                    float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length;
                    Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color RealColor = Projectile.GetAlpha(Color.Orange) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                    Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, RealColor, Projectile.rotation, drawOrigin, scale, effects, 0);
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

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<SpookFishronMinionBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<SpookFishronMinionBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 750f)
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

            if (!isAttacking)
            {
                IdleAI(player);
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

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            if (Charging)
            {
                Projectile.frame = 1;
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 0;
                    }
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.Center.X < target.Center.X ? -1 : 1;

            //EoC rotation
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            float RotateX = target.Center.X - vector.X;
            float RotateY = target.Center.Y - vector.Y;
            Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 2)
            {
                Projectile.ai[1] = target.Center.X > Projectile.Center.X ? Main.rand.Next(-150, -125) : Main.rand.Next(125, 150);
                Projectile.ai[2] = Main.rand.Next(-100, 0);
            }

            if (Projectile.ai[0] > 2 && Projectile.ai[0] < 150)
            {
                Vector2 GoTo = target.Center;
                GoTo.X += Projectile.ai[1];
                GoTo.Y += Projectile.ai[2];

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 5, 10);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            if (Projectile.ai[0] > 60 && Projectile.ai[0] < 140)
            {
                if (Projectile.ai[0] % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item87, Projectile.Center);

                    Vector2 ShootSpeed = target.Center - Projectile.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 15;

                    float SpreadX = Main.rand.NextFloat(-1.5f, 1.5f);
                    float SpreadY = Main.rand.NextFloat(-1.5f, 1.5f);
                    
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(ShootSpeed.X + SpreadX, ShootSpeed.Y + SpreadY), 
					ModContent.ProjectileType<SpookFishronMinionBubble>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                }
            }

            if (Projectile.ai[0] == 150)
            {
                SoundEngine.PlaySound(SoundID.Zombie9 with { Pitch = 1.1f }, Projectile.Center);

                Charging = true;

                saveDirection = Projectile.spriteDirection;

                Vector2 ChargeDirection = target.Center - Projectile.Center;
                ChargeDirection.Normalize();
                ChargeDirection *= 25f;
                Projectile.velocity = ChargeDirection;
            }

            //attempt to spin around the target
            if (Projectile.ai[0] > 150)
            {
                Projectile.direction = saveDirection;
                Projectile.spriteDirection = saveDirection;

                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                Projectile.rotation += 0f * (float)Projectile.direction;
            }
            
            if (Projectile.ai[0] > 155)
            {
                double angle = Projectile.DirectionTo(target.Center).ToRotation() - Projectile.velocity.ToRotation();
                while (angle > Math.PI)
                {
                    angle -= 2.0 * Math.PI;
                }
                while (angle < -Math.PI)
                {
                    angle += 2.0 * Math.PI;
                }

                float Angle = Math.Sign(angle);
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 22;

                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(8f) * Angle);
            }

            if (Projectile.ai[0] >= 210)
            {
                Charging = false;

                Projectile.ai[0] = 0;
                Projectile.netUpdate = true;
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;

            Projectile.rotation = Projectile.spriteDirection == -1 ? -MathHelper.PiOver2 : MathHelper.PiOver2;

            Projectile.ai[0] = 0;
            Charging = false;

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
        }
    }
}