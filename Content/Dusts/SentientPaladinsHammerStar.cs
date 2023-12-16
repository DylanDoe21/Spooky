using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Dusts
{
    public class SentientPaladinsHammerStar : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 18, 18);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return lightColor;
        }

        public override bool Update(Dust dust)
        {
            dust.scale *= 0.95f;
            dust.position += dust.velocity;

            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}