using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
    public class CultistExclamation : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noLight = false;
            dust.frame = new Rectangle(0, Main.rand.Next(2) * 24, 12, 24);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
			return Color.White * dust.scale;
        }

        public override bool Update(Dust dust)
        {
            dust.scale *= 0.98f;

            if (dust.scale <= 0)
                dust.active = false;

            return false;
        }
    }
}