using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class EyeMineExplosion : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 150;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.Red), Projectile.rotation, drawOrigin, Projectile.ai[0] / 37, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, (Projectile.ai[0] / 37) * 0.75f, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    
        public override void AI()
        {
            if (Projectile.ai[0] < 74)
            {
                Projectile.ai[0] += 20;
            }
            else
            {
                Projectile.alpha += 25;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}