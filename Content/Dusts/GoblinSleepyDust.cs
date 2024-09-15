using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Dusts
{
	public class GoblinSleepyDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.05f;
            dust.velocity.Y *= 0.5f;
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 0;
            dust.fadeIn = 12f;
            dust.scale *= Main.rand.NextFloat(0.8f, 1f);
            dust.frame = new Rectangle(0, 0, 18, 16);
        }

        public override bool Update(Dust dust)
        {
            dust.alpha += 2;

            dust.velocity.X += (float)Math.Sin(Main.GameUpdateCount / 20) * 0.02f;
            dust.velocity.Y -= 0.01f;

            dust.position += dust.velocity;

            if (dust.alpha > 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}