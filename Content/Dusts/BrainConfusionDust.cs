using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Dusts
{
	public class BrainConfusionDust : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.noLight = true;
            dust.alpha = 0;
            dust.fadeIn = 12f;
            dust.scale *= Main.rand.NextFloat(0.8f, 1f);
            dust.frame = new Rectangle(0, 0, 14, 22);
        }

        public override bool Update(Dust dust)
        {
            dust.alpha += 6;

            if (dust.alpha > 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}