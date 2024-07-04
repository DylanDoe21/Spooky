using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class BigBoneHammerProj2 : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Catacomb/BigBoneHammer";

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 82;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Gold);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color realColor = Projectile.GetAlpha(color) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, realColor, Projectile.oldRot[oldPos], drawOrigin, scale * 1.2f, effects, 0);
            }
            
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpookyPlayer.ScreenShakeAmount = 5;

            target.immune[Projectile.owner] = 2;

            SoundEngine.PlaySound(SoundID.Item62, target.Center);

            for (int numDusts = 0; numDusts < 30; numDusts++)
			{
                int dustGore = Dust.NewDust(target.position, target.width, target.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, Color.Gold, 1.5f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[dustGore].scale = 0.25f;
                Main.dust[dustGore].noGravity = true;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += 1f * (float)Projectile.direction;

            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 30)
            {
                //remove knockback here so the projectile doesnt fling enemies directly towards you when returning
                Projectile.knockBack = 0;

                Vector2 ReturnSpeed = owner.Center - Projectile.Center;
                ReturnSpeed.Normalize();
                ReturnSpeed *= 55;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
	}
}