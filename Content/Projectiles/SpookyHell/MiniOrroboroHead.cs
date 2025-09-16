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
        bool segmentsSpawned = false;
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
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
				player.GetModPlayer<SpookyPlayer>().GoreArmorMouth = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorMouth)
			{
				Projectile.timeLeft = 2;
			}

            if (!segmentsSpawned)
            {
                int latestSegment = Projectile.whoAmI;

                for (int numSegment = 0; numSegment < 6; numSegment++)
                {
                    Vector2 Center = new Vector2((int)Projectile.Center.X + (Projectile.width / 2), (int)Projectile.Center.Y + (Projectile.height / 2));

                    if (numSegment < 5)
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Center, Vector2.Zero, 
                        ModContent.ProjectileType<MiniBoroBody>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.whoAmI, ai1: latestSegment);
                    }
                    else
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Center, Vector2.Zero, 
                        ModContent.ProjectileType<MiniBoroTail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.whoAmI, ai1: latestSegment);
                    }
                }

                segmentsSpawned = true;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;

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

                Projectile.netUpdate = true;
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
                Projectile.netUpdate = true;
            }
        }
    }

    public class MiniOrroHead : ModProjectile
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
            Projectile.width = 22;
            Projectile.height = 22;
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
				player.GetModPlayer<SpookyPlayer>().GoreArmorEye = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().GoreArmorEye)
			{
				Projectile.timeLeft = 2;
			}

            if (!segmentsSpawned)
            {
                int latestSegment = Projectile.whoAmI;

                for (int numSegment = 0; numSegment < 6; numSegment++)
                {
                    Vector2 Center = new Vector2((int)Projectile.Center.X + (Projectile.width / 2), (int)Projectile.Center.Y + (Projectile.height / 2));

                    if (numSegment < 5)
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Center, Vector2.Zero, 
                        ModContent.ProjectileType<MiniOrroBody>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.whoAmI, ai1: latestSegment);
                    }
                    else
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Center, Vector2.Zero, 
                        ModContent.ProjectileType<MiniOrroTail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.whoAmI, ai1: latestSegment);
                    }
                }

                segmentsSpawned = true;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;

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