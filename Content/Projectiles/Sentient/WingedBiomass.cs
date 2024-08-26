using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Sentient
{
    public class WingedBiomass : ModProjectile
    {
        int PreviousTarget = 0;

        float EyeScale = 1f;

        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle EyePopSound = new("Spooky/Content/Sounds/Moco/MocoEyePop", SoundType.Sound) { PitchVariance = 0.75f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 94;
            Projectile.height = 60;
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
            return false;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/WingedBiomassGlow");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(GlowTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, EyeScale, SpriteEffects.None, 0);

			return false;
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 16)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().WingedBiomass = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().WingedBiomass)
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

            if (Projectile.Distance(new Vector2(target.Center.X, target.Center.Y - 300)) > 100f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(new Vector2(target.Center.X, target.Center.Y - 300)) * 25;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
            else
            {
                Projectile.velocity *= 0.95f;
            }

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
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

            //shoot its eye out
            Projectile.ai[1]++;
            
            if (Projectile.ai[1] >= 30)
            {
                if (EyeScale < 1f)
                {
                    EyeScale += 0.2f;
                }
            }

            if (Projectile.ai[1] >= 60)
            {
                SoundEngine.PlaySound(EyePopSound, Projectile.Center);

                Vector2 ShootSpeed = target.Center - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 35f;

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y), ShootSpeed, ModContent.ProjectileType<WingedBiomassEye>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, target.whoAmI);

				EyeScale = 0f;

				Projectile.ai[1] = 0;
            }

            //reset the wing detach timer if this minion changes its target
            if (PreviousTarget != target.whoAmI)
            {
                Projectile.ai[2] = 0;
            }

            PreviousTarget = target.whoAmI;

            //while the same target is being chased, start the death countdown
            Projectile.ai[2]++;

            if (Projectile.ai[2] == 600)
            {
                Vector2 ChargeSpeed = target.Center - Projectile.Center;
                ChargeSpeed.Normalize();
                ChargeSpeed *= -15f;
                Projectile.velocity = ChargeSpeed;
            }

            if (Projectile.ai[2] >= 615)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<WingedBiomassFalling>(), Projectile.damage * 5, Projectile.knockBack, Main.myPlayer, target.whoAmI);
                
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/BiomassWingGore" + Main.rand.Next(1, 3)).Type);
                    }
                }
                
                Projectile.Kill();
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.ai[1] = 0;
            Projectile.ai[2] = 0;

            if (EyeScale < 1f)
			{
				EyeScale += 0.1f;
			}

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
            if (distanceTo > 400f && speed < 9f)
            {
                speed = 9f;
            }
            if (distanceTo < 300f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 150f)
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