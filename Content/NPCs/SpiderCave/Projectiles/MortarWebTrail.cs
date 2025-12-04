using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class MortarWebTrail : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 8;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
			Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()       
        {
            Projectile.rotation = Projectile.ai[0];

            if (Projectile.timeLeft <= 10)
            {
                Projectile.alpha += 26;
            }

            if (Projectile.timeLeft >= 50)
            {
                Projectile.alpha -= 26;
            }
        }
    }
}