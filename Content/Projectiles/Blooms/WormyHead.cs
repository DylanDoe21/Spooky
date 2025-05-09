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
        bool isAttacking = false;

        Dictionary<int, Projectile> segments = new Dictionary<int, Projectile>();

        private static Asset<Texture2D> HeadTexture;
        private static Asset<Texture2D> BodyTexture;
        private static Asset<Texture2D> TailTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
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
			Player player = Main.player[Projectile.owner];

			HeadTexture ??= ModContent.Request<Texture2D>(Texture);
            BodyTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/WormySegment");
            TailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/WormyTail");

            for (var i = segments.Count; i > 0; i--)
            {
                if (segments.ContainsKey(i))
                {
                    if (i < segments.Count)
                    {
						Main.EntitySpriteDraw(BodyTexture.Value, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor),
                        segments[i].rotation + MathHelper.Pi / 2f, BodyTexture.Size() / 2f, segments[i].scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(TailTexture.Value, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor),
                        segments[i].rotation + MathHelper.Pi / 2f, TailTexture.Size() / 2f, segments[i].scale, SpriteEffects.None, 0);
                    }
                }
            }

            Main.EntitySpriteDraw(HeadTexture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi / 2f,
            HeadTexture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

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

            int AdditionalDamage = 0;
            bool ShouldScaleDamage = false;

            //scale based on how many bloom buffs you have
			foreach (string var in player.GetModPlayer<BloomBuffsPlayer>().BloomBuffSlots)
            {
                if (var != string.Empty)
                {
                    AdditionalDamage += 15;
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

            Projectile.rotation = Projectile.velocity.ToRotation();

			if (Projectile.Distance(player.Center) >= 2000f)
			{
				Projectile.Center = player.Center;
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

            segments.Clear();
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<WormySegment>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<WormySegment>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<WormySegment>().segmentIndex, projectile);
                }
                if (projectile.type == ModContent.ProjectileType<WormyTail>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<WormyTail>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<WormyTail>().segmentIndex, projectile);
                }
            }
            for (var i = 1; i <= segments.Count; i++)
            {
                if (i < segments.Count)
                {
                    if (segments.ContainsKey(i))
                    segments[i].ModProjectile<WormySegment>().SegmentMove();
                }
                else
                {
                    if (segments.ContainsKey(i))
                    {
                        segments[i].ModProjectile<WormyTail>().SegmentMove();
                    }
                }
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