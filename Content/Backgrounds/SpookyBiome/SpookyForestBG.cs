using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Backgrounds.SpookyBiome
{
	public class SpookyForestBG : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG1");
		}

		public override int ChooseMiddleTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG2");
		}

		public override int ChooseFarTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBG3");
		}
		
		public override void ModifyFarFades(float[] fades, float transitionSpeed)
		{
			for (int i = 0; i < fades.Length; i++)
			{
				if (i == Slot)
				{
					fades[i] += transitionSpeed;
					if (fades[i] > 1f)
					{
						fades[i] = 1f;
					}
				}
				else
				{
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f)
					{
						fades[i] = 0f;
					}
				}
			}
		}
	}
}