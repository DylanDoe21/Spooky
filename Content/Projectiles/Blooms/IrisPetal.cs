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
    public class IrisPetal : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/IrisEyePoke", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Purple) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile Parent = Main.projectile[(int)Projectile.ai[0]];
            
            Projectile.timeLeft = 5;

            if (!Parent.active || player.dead || !player.GetModPlayer<BloomBuffsPlayer>().SpringIris)
            {
                Projectile.Kill();
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
            
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            Projectile.ai[1] += 0.01f;

            Vector2 Speed = Parent.Center - Projectile.Center;
            Speed.Normalize();
            Speed *= Projectile.ai[1];

            Projectile.velocity = Speed;

            if (Projectile.Hitbox.Intersects(Parent.Hitbox))
            {
                Parent.Kill();
                Projectile.Kill();

                SoundEngine.PlaySound(DeathSound, Parent.Center);

                Dust.NewDustPerfect(new Vector2(Parent.Center.X - 34, Parent.Center.Y - 19), ModContent.DustType<IrisPetalLockOnDeath>(), Projectile.velocity / 2, 0, default, 1f);
            }
        }
	}
}