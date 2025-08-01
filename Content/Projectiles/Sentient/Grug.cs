using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Sentient
{
    public class Grug : ModProjectile
    {
        int saveDirection = 0;

        bool StoneForm = true;
        bool UsingMagic = false;
        bool Charging = false;
        bool isAttacking = false;

        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 84;
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
            return StoneForm || Charging;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Charging || UsingMagic)
            {
                ProjTexture ??= ModContent.Request<Texture2D>(Texture);

                Color color = new Color(125, 125, 125, 0).MultiplyRGBA(UsingMagic ? Color.Lime : Color.Red);

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

        public override void PostDraw(Color lightColor)
        {
            if (UsingMagic)
            {
                GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/GrugMagicGlow");

                var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                int height = GlowTexture.Height() / Main.projFrames[Projectile.type];
                int frameHeight = height * Projectile.frame;
                Rectangle rectangle = new Rectangle(0, frameHeight, GlowTexture.Width(), height);

                Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                rectangle, Color.White, Projectile.rotation, new Vector2(GlowTexture.Width() / 2f, height / 2f), Projectile.scale, spriteEffects, 0);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = Charging ? 3 : 10;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            StoneForm = false;

            Projectile.velocity *= 0;
            Projectile.frame = 1;
            
            SoundEngine.PlaySound(SoundID.NPCDeath43 with { Volume = SoundID.NPCDeath43.Volume * 0.3f }, Projectile.Center);

            for (int numGores = 1; numGores <= 6; numGores++)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-6, -3)), ModContent.Find<ModGore>("Spooky/GrugStatueGore" + numGores).Type);
                }
            }

			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<GrugBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<GrugBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            if (StoneForm)
            {
                Projectile.frame = 0;

                Projectile.rotation += 0.2f * (float)Projectile.direction;

                Projectile.ai[2]++;
                if (Projectile.ai[2] >= 30)
                {
                    Projectile.tileCollide = true;

                    Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
                }
                else
                {
                    Projectile.tileCollide = false;
                }
            }
            else
            {
                Projectile.rotation = 0;

                Projectile.tileCollide = false;

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
            }

            if (!isAttacking && !StoneForm)
            {
                IdleAI(player);
            }
		}

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            if (Projectile.ai[1] < 200)
            {
                Projectile.spriteDirection = target.Center.X > Projectile.Center.X ? -1 : 1;
            }
            else
            {
                Projectile.spriteDirection = saveDirection;
            }

            //flying animation
            if (!Charging)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 5)
                    {
                        Projectile.frame = 1;
                    }
                }
            }
            //charging animation
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 9)
                    {
                        Projectile.frame = 5;
                    }
                }
            }

            Projectile.ai[1]++;

            //go above the target to prepare for magic attack
            if (Projectile.ai[1] <= 70)
            {
                Vector2 GoTo = target.Center;
                GoTo.Y -= 200;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 12, 25);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //shoot cursed fireballs
            if (Projectile.ai[1] == 80 || Projectile.ai[1] == 90 || Projectile.ai[1] == 100 || Projectile.ai[1] == 110 || Projectile.ai[1] == 120)
            {
                UsingMagic = true;

                Projectile.velocity *= 0.2f;

                Vector2 ShootSpeed = target.Center - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 25f;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 
                ShootSpeed.X, ShootSpeed.Y, ModContent.ProjectileType<GrugFireball>(), Projectile.damage / 2, 2f, Projectile.owner);
            }

            //go to the side of the target to prepare for dashing
            if (Projectile.ai[1] >= 140 && Projectile.ai[1] < 200)
            {
                UsingMagic = false;

                Vector2 GoTo = target.Center;
                GoTo.X += (Projectile.Center.X < target.Center.X) ? -200 : 200;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 12, 25);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
            }

            //charge at the target
            if (Projectile.ai[1] == 200)
            {
                Charging = true;

                saveDirection = Projectile.spriteDirection;

                //set frame to charging immediately 
                Projectile.frame = 5;
                
                Vector2 ChargeSpeed = target.Center - Projectile.Center;
                ChargeSpeed.Normalize();

                ChargeSpeed.X *= 25;
                ChargeSpeed.Y *= 1;
                Projectile.velocity.X = ChargeSpeed.X;
                Projectile.velocity.Y = ChargeSpeed.Y;
            }

            //slow down at the end of the charge
            if (Projectile.ai[1] >= 230)
            {
                Projectile.velocity *= 0.7f;
            }

            //loop ai
            if (Projectile.ai[1] >= 250)
            {
                Charging = false;
                Projectile.ai[1] = 0;
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.spriteDirection = player.Center.X > Projectile.Center.X ? -1 : 1;

            //idle animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 5)
                {
                    Projectile.frame = 1;
                }
            }

            ///reset attacking ai stuff
            UsingMagic = false;
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
            
            direction.Y -= 70f;
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