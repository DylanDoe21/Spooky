using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class ThrowingCard : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1200;
            Projectile.penetrate = 1;
        }

        /*
        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(Color.Orange, Color.Red, oldPos / (float)Projectile.oldPos.Length)) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
        */

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 9)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.ai[0]++;
            if (Projectile.ai[0] < 45)
            {
                Projectile.velocity *= 0.96f;
            }
            else
            {
                float update = Main.GlobalTimeWrappedHourly * 0.08f * 16;

                Projectile.velocity.X += (float)Math.Sin(update) * 0.01f;
                
                if (Projectile.velocity.Y < 4f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + Main.rand.NextFloat(0f, 0.01f);
                }
            }
        }
    }
}