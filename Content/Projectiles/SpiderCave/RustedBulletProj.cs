using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class RustedBulletProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpiderCave/OldHunter/RustedBullet";

        private List<Vector2> cache;
        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1800;
            Projectile.extraUpdates = 2;
			Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/GlowTrail").Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["repeats"].SetValue(1);

            trail?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            return true;
        }

        const int TrailLength = 15;

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new RoundedTip(4), factor => 1 * factor, factor =>
            {
                return Color.Lerp(Color.Red, Color.Gold, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void AI()       
        {
            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            Vector2 Speed = new Vector2(2f, -12f).RotatedByRandom(2 * Math.PI);

            for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
            {
                Vector2 speed = -(Projectile.velocity / 8) + new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, 6));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, speed, ModContent.ProjectileType<RustedBulletShrapnel>(), Projectile.damage / 2, 0f, Main.myPlayer);
                }
            }
        }
    }
}