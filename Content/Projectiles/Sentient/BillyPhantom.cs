using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class BillyPhantom : ModProjectile
    {
        bool Charging = false;
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
			Projectile.width = 66;
            Projectile.height = 92;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 400;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
            return Charging;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lime);

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//aura
			for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            if (Charging && Projectile.frame == 6)
            {
                //after images
                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    Vector2 afterImageVector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 4 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                    Main.EntitySpriteDraw(ProjTexture.Value, afterImageVector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
                }
            }
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 2;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
            else
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 5;
                }
            }

            //flying animation
            if (!Charging)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 5)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 0;
                    }
                }

                Projectile.rotation = 0;
            }
            //charging frame
            else
            {
                Projectile.frame = 6;

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 850f)
                {
                    AttackingAI(Target);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 850f)
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

            Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;

            Projectile.ai[1]++;

            //slowly go toward the target
            if (Projectile.ai[1] <= 100)
            {
                Vector2 GoTo = target.Center;
                GoTo.X += (Projectile.Center.X < target.Center.X ? -400 : 400);

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 6, 13);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //screech
            if (Projectile.ai[1] == 100)
            {
                SoundEngine.PlaySound(SoundID.DD2_WyvernHurt, Projectile.Center);
            }

            //charge
            if (Projectile.ai[1] == 120)
            {
                SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, Projectile.Center);

                Charging = true;

                Vector2 ChargeDirection = target.Center - Projectile.Center;
                ChargeDirection.Normalize();     
                ChargeDirection *= 55;
                Projectile.velocity = ChargeDirection;
            }

            if (Projectile.ai[1] >= 120 && Projectile.ai[1] < 150)
            {
                if (Projectile.ai[1] % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_SonicBoomBladeSlash, Projectile.Center);
                            
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 
                    ModContent.ProjectileType<BillyFeather>(), Projectile.damage / 2, Projectile.knockBack, Main.myPlayer);
                }
            }

            if (Projectile.ai[1] >= 120)
            {
                Projectile.velocity *= 0.95f;
            }

            //loop ai
            if (Projectile.ai[1] >= 200)
            {
                Charging = false;

                Projectile.ai[1] = 0;
                Projectile.ai[2] = 0;
            }
        }

        public void IdleAI(Player player)
		{
            Charging = false;

            Projectile.ai[1] = 0;
            Projectile.ai[2] = 0;

            Projectile.spriteDirection = player.Center.X > Projectile.Center.X ? -1 : 1;

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
            
            direction.Y -= 70f;
            float distanceTo = direction.Length();
            if (distanceTo > 350f && speed < 9f)
            {
                speed = 9f;
            }
            if (distanceTo < 250f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 100f)
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