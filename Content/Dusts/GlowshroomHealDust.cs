using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
	public class GlowshroomHealDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.05f;
            dust.velocity.Y *= 0.5f;
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 0;
            dust.fadeIn = 12f;
            dust.scale *= Main.rand.NextFloat(0.75f, 1f);
            dust.frame = new Rectangle(0, 0, 16, 16);
        }

        public override bool Update(Dust dust)
        {
            dust.alpha += Main.rand.Next(5, 10);

            if (Main.rand.NextBool(12))
            {
                dust.velocity.X += Main.rand.NextFloat(-0.01f, 0.01f);
                dust.velocity.Y += Main.rand.NextFloat(0.2f, 0.5f);
            }

            dust.position -= dust.velocity;

            if (dust.alpha > 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}