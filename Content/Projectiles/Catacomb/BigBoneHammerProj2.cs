using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class BigBoneHammerProj2 : ModProjectile
    {
        private List<Vector2> cache;
        private Trail trail;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skull Smasher");
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
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/LightningTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 15 * factor, factor =>
            {
                //use (* 1 - factor.X) at the end to make it fade at the beginning, or use (* factor.X) at the end to make it fade at the end
                return Color.Lerp(Color.Black, Color.Yellow, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            SpookyPlayer.ScreenShakeAmount = 8;

            target.immune[Projectile.owner] = 1;

            SoundEngine.PlaySound(SoundID.Item62, target.Center);

            for (int numDusts = 0; numDusts < 45; numDusts++)
            {
                int NewDust = Dust.NewDust(new Vector2(Projectile.Center.X, Projectile.Center.Y), Projectile.width / 2, Projectile.height / 2, 
                DustID.YellowTorch, Main.rand.Next(-10, 10), Main.rand.Next(-20, 20), 0, Color.Transparent, Main.rand.NextFloat(1.5f, 3.5f));
                Main.dust[NewDust].velocity.X *= Main.rand.Next(-5, 5);
                Main.dust[NewDust].velocity.Y *= Main.rand.Next(-20, 20);
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += 0.5f * (float)Projectile.direction;

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
            }

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] >= 30)
            {
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