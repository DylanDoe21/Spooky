using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Blooms
{
    public class SeaSpongeProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Blooms/SeaSponge";

        float Scale = 0f;

        private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);
         
            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool? CanDamage()
        {
            return false;
        }

		public override void AI()
        {
            if (Projectile.timeLeft <= 30)
            {
                if (Scale > 0f)
                {
                    Scale -= 0.1f;
                }

                if (Scale <= 0.1f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                if (Scale < 1f)
                {
                    Scale += 0.1f;
                }
            }
        }
    }
}