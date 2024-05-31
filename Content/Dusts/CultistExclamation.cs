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

        public override bool Update(Dust dust)
        {
            dust.alpha += 10;

            if (dust.alpha >= 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}