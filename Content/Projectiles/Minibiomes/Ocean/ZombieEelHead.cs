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
        bool isAttacking = false;

        Dictionary<int, Projectile> segments = new Dictionary<int, Projectile>();

        private static Asset<Texture2D> HeadTexture;
        private static Asset<Texture2D> BodyTexture;
        private static Asset<Texture2D> TailTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
        }

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
            HeadTexture ??= ModContent.Request<Texture2D>(Texture);
            BodyTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Minibiomes/Ocean/ZombieEelBody");
            TailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Minibiomes/Ocean/ZombieEelTail");

            for (var i = segments.Count; i > 0; i--)
            {
                if (segments.ContainsKey(i))
                {
                    SpriteEffects effects = Math.Abs(segments[i].rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    if (i < segments.Count)
                    {
						Vector2 drawOrigin = new(BodyTexture.Width() * 0.5f, Projectile.height * 0.5f);
						Rectangle rectangle = new(0, BodyTexture.Height() / Main.projFrames[segments[i].type] * segments[i].frame, BodyTexture.Width(), BodyTexture.Height() / Main.projFrames[segments[i].type]);

						Main.EntitySpriteDraw(BodyTexture.Value, segments[i].Center - Main.screenPosition, rectangle, segments[i].GetAlpha(lightColor),
                        segments[i].rotation + MathHelper.Pi / 2f, drawOrigin, segments[i].scale, effects, 0);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
			target.AddBuff(ModContent.BuffType<EelTagDebuff>(), 240);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.channel && player.active && !player.dead && !player.noItems && !player.CCed)
            {
                Projectile.timeLeft = 2;
            }

            if (Projectile.owner == Main.myPlayer && Projectile.Distance(Main.MouseWorld) >= 50f)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(Main.MouseWorld) * 9;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            segments.Clear();
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<ZombieEelBody>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<ZombieEelBody>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<ZombieEelBody>().segmentIndex, projectile);
                }
                if (projectile.type == ModContent.ProjectileType<ZombieEelTail>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<ZombieEelTail>().segmentIndex))
                {
                    segments.Add(projectile.ModProjectile<ZombieEelTail>().segmentIndex, projectile);
                }
            }
            for (var i = 1; i <= segments.Count; i++)
            {
                if (i < segments.Count)
                {
                    if (segments.ContainsKey(i))
                    segments[i].ModProjectile<ZombieEelBody>().SegmentMove();
                }
                else
                {
                    if (segments.ContainsKey(i))
                    {
                        segments[i].ModProjectile<ZombieEelTail>().SegmentMove();
                    }
                }
            }
        }
    }
}