using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Dusts
{
	public class GrottoTreeLeaf : ModDust
	{
        public override void OnSpawn(Dust dust)
        {
			dust.velocity *= 0.05f;
			dust.velocity.Y *= 0.5f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.fadeIn = Main.rand.NextFloat(-0.04f, 0.04f);
			dust.frame = new Rectangle(0, 18 * Main.rand.Next(0, 3), 18, 16);
			dust.rotation = Main.rand.NextFloat(0.001f, 0.01f);
		}

        public override bool Update(Dust dust)
        {
			if (WorldGen.SolidTile(Main.tile[(int)dust.position.X / 16, (int)dust.position.Y / 16]))
			{
				dust.velocity = Vector2.Zero;
				dust.alpha++;
			}
			else
			{
				dust.velocity.X += dust.fadeIn;
				dust.velocity.Y += dust.rotation;
				dust.position.X += (float)Math.Sin(dust.velocity.X);
				dust.position.Y += dust.velocity.Y;

				dust.scale *= 0.999f;
			}

			return false;
        }
    }
}