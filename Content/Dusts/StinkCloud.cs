using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Dusts
{
	public class StinkCloud1 : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity *= 0.05f;
			dust.velocity.Y *= 0.5f;
			dust.noGravity = true;
			dust.noLight = true;
			dust.alpha = 125;
			dust.fadeIn = 12f;
			dust.scale *= Main.rand.NextFloat(0.8f, 1f);
			dust.frame = new Rectangle(0, 0, 34, 48);
		}

		public override bool Update(Dust dust)
		{
			dust.alpha += 1;

			if (dust.alpha % 10 == 0)
			{
				dust.frame.Y += 48;
			}

			if (dust.frame.Y > 48 * 11)
			{
				dust.active = false;
			}

			if (dust.frame.Y > 48 * 5)
			{
				dust.velocity.X += (float)Math.Sin(Main.GameUpdateCount / 20) * 0.01f;
				dust.velocity.Y -= 0.005f;

				dust.position += dust.velocity;
			}

			return false;
		}
	}

	public class StinkCloud2 : StinkCloud1
	{
	}
}