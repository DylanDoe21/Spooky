using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.WhipDebuff;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class ZombieEelHead : ModProjectile
    {
        bool segmentsSpawned = false;
        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.DamageType = DamageClass.Summon;
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

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
			target.AddBuff(ModContent.BuffType<EelTagDebuff>(), 240);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.channel && player.active && !player.dead && !player.noItems && !player.CCed)
            {
                Projectile.timeLeft = 2;
            }

            if (!segmentsSpawned)
            {
                int latestSegment = Projectile.whoAmI;

                for (int numSegment = 0; numSegment < 8; numSegment++)
                {
                    if (numSegment < 7)
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, 
                        ModContent.ProjectileType<ZombieEelBody>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.identity, ai1: latestSegment, ai2: numSegment);
                    }
                    else
                    {
                        latestSegment = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, 
                        ModContent.ProjectileType<ZombieEelTail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 
                        ai0: Projectile.identity, ai1: latestSegment);
                    }
                }

                segmentsSpawned = true;
                Projectile.netUpdate = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;

            if (Projectile.owner == Main.myPlayer && Projectile.Distance(Main.MouseWorld) >= 50f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(Main.MouseWorld) * 10;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }
        }
    }
}