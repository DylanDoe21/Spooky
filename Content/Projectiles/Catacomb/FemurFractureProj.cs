using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class FemurFractureProj : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 10000;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/ShadowTrail").Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["repeats"].SetValue(1);

            trail?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lime) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale * 1.2f, effects, 0);
            }

            return true;
        }

        const int TrailLength = 12;

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TrailLength; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > TrailLength)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 12 * factor, factor =>
            {
                return Color.Lerp(Color.Gray, Color.Lime, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpookyPlayer.ScreenShakeAmount = 3;

            target.immune[Projectile.owner] = 8;

            SoundEngine.PlaySound(SoundID.Item62, target.Center);

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{
                int dustGore = Dust.NewDust(target.Center, target.width / 2, target.height / 2, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].color = Color.Lime;
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
            Projectile.rotation += 0.85f * (float)Projectile.direction;

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 13)
            {
                //remove knockback here so the projectile doesnt fling enemies directly towards you when returning
                Projectile.knockBack = 0;

                Vector2 ReturnSpeed = owner.Center - Projectile.Center;
                ReturnSpeed.Normalize();
                ReturnSpeed *= 35;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }

            //fire off skulls if super charged
            if (Projectile.ai[1] == 1)
            {
                if (Projectile.ai[0] == 2 || Projectile.ai[0] == 4 || Projectile.ai[0] == 6 || Projectile.ai[0] == 8 || Projectile.ai[0] == 10 || Projectile.ai[0] == 12)
                {
                    Vector2 Speed = new Vector2(1f, 0f).RotatedByRandom(2 * Math.PI);
                    Vector2 newVelocity = Projectile.velocity / Main.rand.Next(3, 6);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, newVelocity, ModContent.ProjectileType<FemurFractureSkull>(), Projectile.damage, 0f, Main.myPlayer);
                }
            }
        }
	}
}