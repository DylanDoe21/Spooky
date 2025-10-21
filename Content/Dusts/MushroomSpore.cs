using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Dusts
{
	public class MushroomSpore : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
			dust.velocity *= 0.05f;
			dust.velocity.Y *= 0.5f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.fadeIn = Main.rand.NextFloat(-0.04f, 0.04f);
			dust.frame = new Rectangle(0, 12 * Main.rand.Next(0, 3), 8, 12);
			dust.rotation = Main.rand.NextFloat(0.001f, 0.01f);
		}

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return dust.color * 0.5f;
        }

        public override bool Update(Dust dust)
        {
			dust.velocity.X += dust.fadeIn;
			dust.velocity.Y -= dust.rotation;
			dust.position.X += (float)Math.Sin(dust.velocity.X);
			dust.position.Y += dust.velocity.Y;

			dust.scale *= 0.999f;

			if (dust.scale < 0f || WorldGen.SolidTile(Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16]))
			{
				dust.active = false;
			}

			return false;
        }
    }
}