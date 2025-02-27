using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class MiniBoroHead : ModProjectile
    {
        bool isAttacking = false;

        Dictionary<int, Projectile> segments = new Dictionary<int, Projectile>();

        private static Asset<Texture2D> HeadTexture;
        private static Asset<Texture2D> BodyTexture;
        private static Asset<Texture2D> TailTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.localNPCHitCooldown = 30;
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
            HeadTexture ??= ModContent.Request<Texture2D>(Texture);
            BodyTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyHell/MiniOrroboroBody");
            TailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyHell/MiniOrroboroTail");

            for (var i = segments.Count; i > 0; i--)
            {
                if (segments.ContainsKey(i))
                {
                    SpriteEffects effects = Math.Abs(segments[i].rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    if (i < segments.Count)
                    {
                        Main.EntitySpriteDraw(BodyTexture.Value, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor),
                        segments[i].rotation + MathHelper.Pi / 2f, BodyTexture.Size() / 2f, segments[i].scale, effects, 0);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(TailTexture.Value, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor),
                        segments[i].rotation + MathHelper.Pi / 2f, TailTexture.Size() / 2f, segments[i].scale, effects, 0);
                    }
                }
            }

            Main.EntitySpriteDraw(HeadTexture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi / 2f,
            HeadTexture.Size() / 2f, Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

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
				player.GetModPlayer<SpookyPlayer>().GoreArmorMouth = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorMouth)
			{
				Projectile.timeLeft = 2;
			}

            Projectile.ai[0]++;

            Projectile.rotation = Projectile.velocity.ToRotation();

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
                if (projectile.type == ModContent.ProjectileType<MiniBoroBody>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<MiniBoroBody>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<MiniBoroBody>().segmentIndex, projectile);
                }
                if (projectile.type == ModContent.ProjectileType<MiniBoroTail>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<MiniBoroTail>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<MiniBoroTail>().segmentIndex, projectile);
                }
            }
            for (var i = 1; i <= segments.Count; i++)
            {
                if (i < segments.Count)
                {
                    if (segments.ContainsKey(i))
                    segments[i].ModProjectile<MiniBoroBody>().SegmentMove();
                }
                else
                {
                    if (segments.ContainsKey(i))
                    {
                        segments[i].ModProjectile<MiniBoroTail>().SegmentMove();
                    }
                }
            }
        }

        public void IdleMovement(Player player)
        {
            Projectile.ai[2] = 0;

            Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 25);

            float distanceFromOwner = Projectile.Distance(GoTo);

            float hoverAcceleration = 0.08f;

            if (distanceFromOwner > 3000f)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
            }

            if (distanceFromOwner >= 200f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 7;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
            if (distanceFromOwner < 200f)
            {
                hoverAcceleration = 0.12f;
            }
            if (distanceFromOwner < 140f)
            {
                hoverAcceleration = 0.08f;
            }

            if (distanceFromOwner > 100f)
            {
                if (Math.Abs(player.Center.X - Projectile.Center.X) > 20f)
                {
                    Projectile.velocity.X += hoverAcceleration * Math.Sign(GoTo.X - Projectile.Center.X);
                }

                if (Math.Abs(player.Center.Y - Projectile.Center.Y) > 10f)
                {
                    Projectile.velocity.Y += hoverAcceleration * Math.Sign(GoTo.Y - Projectile.Center.Y);
                }
            }
            else if (Projectile.velocity.Length() > 1f)
            {
                Projectile.velocity *= 0.85f;
            }

            if (Math.Abs(Projectile.velocity.Y) < 1f)
            {
                Projectile.velocity.Y -= 0.1f;
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
                Projectile.velocity = Projectile.DirectionTo(destination) * 20;
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
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20;
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

    public class MiniOrroHead : ModProjectile
    {
        bool isAttacking = false;

        Dictionary<int, Projectile> segments = new Dictionary<int, Projectile>();

        private static Asset<Texture2D> HeadTexture;
        private static Asset<Texture2D> BodyTexture;
        private static Asset<Texture2D> TailTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.localNPCHitCooldown = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            HeadTexture ??= ModContent.Request<Texture2D>(Texture);
            BodyTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyHell/MiniOrroboroBody");
            TailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyHell/MiniOrroboroTail");

            for (var i = segments.Count; i > 0; i--)
            {
                if (segments.ContainsKey(i))
                {
                    SpriteEffects effects = Math.Abs(segments[i].rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    if (i < segments.Count)
                    {
                        Main.EntitySpriteDraw(BodyTexture.Value, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor),
                        segments[i].rotation + MathHelper.Pi / 2f, BodyTexture.Size() / 2f, segments[i].scale, effects, 0);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(TailTexture.Value, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor),
                        segments[i].rotation + MathHelper.Pi / 2f, TailTexture.Size() / 2f, segments[i].scale, effects, 0);
                    }
                }
            }

            Main.EntitySpriteDraw(HeadTexture.Value, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi / 2f,
            HeadTexture.Size() / 2f, Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

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
				player.GetModPlayer<SpookyPlayer>().GoreArmorEye = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorEye)
			{
				Projectile.timeLeft = 2;
			}

            Projectile.ai[0]++;

            Projectile.rotation = Projectile.velocity.ToRotation();

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
                if (projectile.type == ModContent.ProjectileType<MiniOrroBody>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<MiniOrroBody>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<MiniOrroBody>().segmentIndex, projectile);
                }
                if (projectile.type == ModContent.ProjectileType<MiniOrroTail>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<MiniOrroTail>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<MiniOrroTail>().segmentIndex, projectile);
                }
            }
            for (var i = 1; i <= segments.Count; i++)
            {
                if (i < segments.Count)
                {
                    if (segments.ContainsKey(i))
                    segments[i].ModProjectile<MiniOrroBody>().SegmentMove();
                }
                else
                {
                    if (segments.ContainsKey(i))
                    {
                        segments[i].ModProjectile<MiniOrroTail>().SegmentMove();
                    }
                }
            }
        }

        public void IdleMovement(Player player)
        {
            Projectile.ai[2] = 0;

            Vector2 GoTo = new Vector2(player.Center.X, player.Center.Y - 25);

            float distanceFromOwner = Projectile.Distance(GoTo);

            float hoverAcceleration = 0.08f;

            if (distanceFromOwner > 3000f)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
            }

            if (distanceFromOwner >= 200f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 7;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
            if (distanceFromOwner < 200f)
            {
                hoverAcceleration = 0.12f;
            }
            if (distanceFromOwner < 140f)
            {
                hoverAcceleration = 0.08f;
            }

            if (distanceFromOwner > 100f)
            {
                if (Math.Abs(player.Center.X - Projectile.Center.X) > 20f)
                {
                    Projectile.velocity.X += hoverAcceleration * Math.Sign(GoTo.X - Projectile.Center.X);
                }

                if (Math.Abs(player.Center.Y - Projectile.Center.Y) > 10f)
                {
                    Projectile.velocity.Y += hoverAcceleration * Math.Sign(GoTo.Y - Projectile.Center.Y);
                }
            }
            else if (Projectile.velocity.Length() > 1f)
            {
                Projectile.velocity *= 0.85f;
            }

            if (Math.Abs(Projectile.velocity.Y) < 1f)
            {
                Projectile.velocity.Y -= 0.1f;
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
                Projectile.velocity = Projectile.DirectionTo(destination) * 20;
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
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 20;
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