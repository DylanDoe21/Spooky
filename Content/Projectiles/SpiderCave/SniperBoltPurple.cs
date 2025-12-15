using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SniperBoltPurple : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 360;
			Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Orchid * 0.5f);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 600);
        }

        public override void AI()       
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            float WaveIntensity = 5f;
            float Wave = 8f;

            Projectile.ai[0]++;
            if (Projectile.ai[0] <= Wave)
            {
                Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(WaveIntensity));
                Projectile.velocity = perturbedSpeed;
            }
            else
            {
                Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedBy(MathHelper.ToRadians(-WaveIntensity));
                Projectile.velocity = perturbedSpeed;
            }
            if (Projectile.ai[0] >= Wave * 2)
            {
                Projectile.ai[0] = 0;
            }

            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);
			Dust dust = Dust.NewDustPerfect(position, 205, Vector2.Zero);
			dust.noGravity = true;
            dust.noLight = true;
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item126, Projectile.Center);
		}
    }
}