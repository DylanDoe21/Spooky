using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{
    public class ThornPillarSeed : ModProjectile
    {
        public static readonly SoundStyle ThornSpawnSound = new("Spooky/Content/Sounds/Daffodil/SeedThorn", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.Lime);

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 3; numEffect++)
            {
                Color newColor = color;
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;
                Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 3 * 6 + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            //add light for visibility
            Lighting.AddLight(Projectile.Center, 0.2f, 0.35f, 0f);

            Projectile.ai[0]++;
			if (Projectile.ai[0] >= 60)
			{
				Projectile.velocity *= 0;
            }

            if (Projectile.ai[0] == 90)
			{
                Vector2 lineDirection = new Vector2(0, 16);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 20, Vector2.Zero.X, Vector2.Zero.Y,
                ModContent.ProjectileType<ThornPillar>(), Projectile.damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
            }

            if (Projectile.ai[0] >= 90)
			{
                int MaxDusts = Main.rand.Next(5, 8);
                for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                {
                    Vector2 dustPos = (Vector2.One * new Vector2((float)Projectile.width / 3f, (float)Projectile.height / 3f) * Main.rand.NextFloat(1.25f, 1.75f)).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + Projectile.Center;
                    Vector2 velocity = dustPos - Projectile.Center;
                    int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                    Main.dust[dustEffect].color = Color.Lime;
                    Main.dust[dustEffect].scale = 0.05f;
                    Main.dust[dustEffect].noGravity = true;
                    Main.dust[dustEffect].noLight = false;
                    Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-5f, -2f);
                    Main.dust[dustEffect].fadeIn = 1.3f;
                }
            }

            if (Projectile.ai[0] >= 140)
			{
                SoundEngine.PlaySound(ThornSpawnSound, Projectile.Center);

                Projectile.Kill();
            }
        }
    }
}