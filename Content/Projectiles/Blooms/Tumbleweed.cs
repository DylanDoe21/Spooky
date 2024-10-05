using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Blooms
{
    public class Tumbleweed : ModProjectile
    {
        private static Asset<Texture2D> TrailTexture;

        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/IrisEyePoke", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/TumbleweedTrail");

            float TrailRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			TrailRotation += 0f * (float)Projectile.direction;

            Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (TrailTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, TrailTexture.Width(), TrailTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(TrailTexture.Value, drawPos, rectangle, color, TrailRotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Projectile.rotation += 0.35f * (float)Projectile.direction;
        }
	}
}