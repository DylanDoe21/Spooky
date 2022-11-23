using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
	public class SpookyHellParticle : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.05f;
            dust.velocity.Y *= 0.5f;
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 0;
            dust.fadeIn = 12f;
            dust.scale *= Main.rand.NextFloat(1f, 1.5f);
        }

        public override bool Update(Dust dust)
        {
            if (Main.rand.Next(20) == 0)
            {
                dust.alpha += 5;
            }

            if (Main.rand.Next(12) == 0)
            {
                dust.velocity.X += Main.rand.NextFloat(-0.1f, 0.1f);
                dust.velocity.Y += Main.rand.NextFloat(0.02f, 0.08f);
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