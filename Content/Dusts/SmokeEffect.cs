using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
    public class SmokeEffect : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, Main.rand.Next(3) * 32, 32, 32);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity *= 0.98f;
            dust.color *= 0.99f;

            if (dust.alpha > 100)
            {
                dust.scale *= 1.002f;
                dust.alpha += 1;
            }
            else
            {
                dust.scale *= 1.002f;
                dust.alpha += 2;
            }

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}