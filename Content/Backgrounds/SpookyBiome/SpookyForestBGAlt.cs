using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.SpookyBiome
{
	public class SpookyForestBGAlt : ModSurfaceBackgroundStyle
	{
		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBGAlt1");
		}

		public override int ChooseMiddleTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBGAlt2");
		}

		public override int ChooseFarTexture()
		{
			return BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyForestBGAlt3");
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