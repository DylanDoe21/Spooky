using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class VesicatorDeath : ModProjectile
    {
        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/SpookyHell/VesicatorExplosion", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vesicator");
        }

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 118;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            Color glowColor = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/VesicatorExplosionAura").Value;

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Color newColor = glowColor;
            newColor = Projectile.GetAlpha(newColor);
            newColor *= 1f;
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(tex, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.localAI[0] / 35 + (Projectile.localAI[0] < 450 ? fade : fade2), SpriteEffects.None, 0);

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    
        public override void AI()
        {
            Projectile.rotation += 0.12f * (float)Projectile.direction;
            
            if (Projectile.localAI[0] < 450)
            {
                Projectile.localAI[0] += 10;
            }

            if (Projectile.localAI[0] >= 225)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
            }

            if (Projectile.localAI[0] >= 450 && Projectile.velocity.Y >= 15)
            {
                Projectile.tileCollide = true;
            }
            else
            {
                Projectile.tileCollide = false;
            }

            for (int i = 0; i < 20; i++)
            {
                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

                float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

                Vector2 offset = new();
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                offset.X += (float)(Math.Sin(angle) * Projectile.localAI[0] + (Projectile.localAI[0] < 100 ? fade : fade2));
                offset.Y += (float)(Math.Cos(angle) * Projectile.localAI[0] + (Projectile.localAI[0] < 100 ? fade : fade2));
                Dust dust = Main.dust[Dust.NewDust(Projectile.Center + offset - new Vector2(4, 4), 0, 0, DustID.GemRuby, 0, 0, 100, Color.White, 1f)];
                dust.velocity *= 0;
                dust.noGravity = true;
                dust.scale = 0.5f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.localAI[0] >= 450 && Projectile.velocity.Y >= 15)
            {
                Projectile.Kill();
            }

            return true;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            SpookyPlayer.ScreenShakeAmount = 15;

            float fade2 = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;

            if (Main.LocalPlayer.Distance(Projectile.Center) <= Projectile.localAI[0] + fade2)
            {
                Main.LocalPlayer.Hurt(PlayerDeathReason.ByCustomReason(Main.LocalPlayer.name + " exploded."), Projectile.damage * 2, 0);
            }

            for (int numDust = 0; numDust < 55; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Blood, 0f, 0f, 100, default(Color), 2f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-30, 30);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-30, 30);
                Main.dust[newDust].scale *= 2.5f;
                Main.dust[newDust].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
        }
    }
}