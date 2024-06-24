using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
    public class MocoSpawnerPower : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        private List<Vector2> cache;
        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //draw prim trail
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ShaderLoader.MagicTrail.Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            effect.Parameters["repeats"].SetValue(1);

            trail?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new RoundedTip(4), factor => 6 * factor, factor =>
            {
                return Color.Lerp(Color.Green, Color.Lime, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void AI()
        {
            Projectile.timeLeft = 5;

            NPC Parent = Main.npc[(int)Projectile.ai[0]];

            Projectile.ai[1]++;

            if (!Main.dedServ && Projectile.ai[1] >= 15)
            {
                ManageCaches();
                ManageTrail();
            }

            if (Projectile.ai[1] >= 60)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(Parent.Center) * 20;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                Projectile.tileCollide = false;

                if (Projectile.Hitbox.Intersects(Parent.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
    }
}