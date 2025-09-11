using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class BrainyExplosion : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);

            Vector2 vector = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 90)
            {
                Color color1 = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.HotPink);
                Color color2 = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);
                
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 12f), Main.rand.NextFloat(1f, 12f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color1, Projectile.rotation, drawOrigin, Projectile.ai[0] / 37, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color2, Projectile.rotation, drawOrigin, (Projectile.ai[0] / 37) * 0.75f, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    
        public override void AI()
        {
            if (Projectile.ai[0] < 135)
            {
                Projectile.ai[0] += 35;
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