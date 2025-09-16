using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Blooms
{
    public class WormyHead : ModProjectile
    {
        bool segmentsSpawned = false;
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead)
			{
				player.GetModPlayer<BloomBuffsPlayer>().Wormy = false;
			}

			if (player.GetModPlayer<BloomBuffsPlayer>().Wormy)
			{
				Projectile.timeLeft = 2;
			}

            if (!segmentsSpawned)
            {
                int latestSegment = Projectile.whoAmI;

                for (int numSegment = 0; numSegment < 6; numSegment++)
                {
                    if (numSegment < 5)
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, 
                        ModContent.ProjectileType<WormySegment>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.identity, ai1: latestSegment);
                    }
                    else
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, 
                        ModContent.ProjectileType<WormyTail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.identity, ai1: latestSegment);
                    }
                }

                segmentsSpawned = true;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            int AdditionalDamage = 0;
            bool ShouldScaleDamage = false;

            //scale based on how many bloom buffs you have
			foreach (string var in player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots)
            {
                if (var != string.Empty)
                {
                    AdditionalDamage += 10;
                    ShouldScaleDamage = true;
                }
            }

            if (ShouldScaleDamage)
            {
                Projectile.damage = Projectile.originalDamage + AdditionalDamage;
            }
            else
            {
                Projectile.damage = Projectile.originalDamage;
            }

			Projectile.ai[0]++;

			if (Projectile.Distance(player.Center) >= 2000f)
			{
				Projectile.Center = player.Center;
                Projectile.netUpdate = true;
			}

			for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 600f)
                {
                    AttackingAI(Target);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 600f)
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
                IdleMovement(player);
            }
        }

        public void IdleMovement(Player player)
        {
            Projectile.ai[2] = 0;

            Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 15);

            float distanceFromOwner = Projectile.Distance(GoTo);

            float hoverAcceleration = 0.08f;

            if (distanceFromOwner > 3000f)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
            }

            if (distanceFromOwner >= 100f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 8;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
            if (distanceFromOwner < 100f)
            {
                hoverAcceleration = 0.12f;
            }
            if (distanceFromOwner < 50f)
            {
                hoverAcceleration = 0.08f;
            }

            if (distanceFromOwner > 10f)
            {
                if (Math.Abs(player.Center.X - Projectile.Center.X) > 10f)
                {
                    Projectile.velocity.X += hoverAcceleration * Math.Sign(GoTo.X - Projectile.Center.X);
                }

                if (Math.Abs(player.Center.Y - Projectile.Center.Y) > 10f)
                {
                    Projectile.velocity.Y += hoverAcceleration * Math.Sign(GoTo.Y - Projectile.Center.Y);
                }
            }
			else
			{
				Projectile.velocity *= 0.98f;
			}

			if (Projectile.velocity.Length() > 25)
			{
				Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 15;
			}
        }

        public void AttackingAI(NPC target)
        {
            isAttacking = true;

            Vector2 destination = target.Center;

            Projectile.ai[2]++;

            //charge at the target
            if (Projectile.ai[2] == 1)
            {
                Projectile.velocity = Projectile.DirectionTo(destination) * 12;
            }

            //once the target is passed, loop back around
            if (Projectile.ai[2] >= 20)
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

                if (Math.Abs(angle) > Math.PI / 2)
                {
                    Projectile.ai[1] = Math.Sign(angle);
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 12;
                }

                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(6f) * Projectile.ai[1]);
            }

            //loop attack
            if (Projectile.ai[2] >= 60)
            {
                Projectile.ai[2] = 0;
            }
        }
    }
}