using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Dusts
{
    public class CartoonHeart : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 18, 16);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
			return Color.White * dust.scale;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.X += dust.alpha == 0 ? -0.08f : 0.08f;
			dust.velocity.Y -= 0.01f;
            dust.position.X += (float)Math.Sin(dust.velocity.X);
			dust.position.Y += dust.velocity.Y;
			dust.scale *= 0.975f;

            if (dust.scale <= 0.1f)
                dust.active = false;

            return false;
        }
    }
}