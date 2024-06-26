using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles
{
    public class PhantomBomb : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 2000;
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
            effect.Parameters["sampleTexture"].SetValue(ShaderLoader.ShadowTrail.Value);
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.01f);
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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 8 * factor, factor =>
            {
                return (Flags.RaveyardHappening ? Color.Lerp(Color.Green, Color.Purple, factor.X) : Color.Lerp(Color.Gray, new Color(60, 42, 255), factor.X)) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.35f, 0.7f);

            Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
            Projectile.velocity.X = Projectile.velocity.X * 0.99f;

            if (!Main.dedServ && Projectile.velocity != Vector2.Zero)
            {
                ManageCaches();
                ManageTrail();
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y - 25, 0, 0, 
                ModContent.ProjectileType<PhantomExplosion>(), Projectile.damage, 0, Main.myPlayer, 0, 0);
            }
        }
    }
}