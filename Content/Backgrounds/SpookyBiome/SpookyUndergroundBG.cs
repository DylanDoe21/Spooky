using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.SpookyBiome
{
	public class SpookyUndergroundBackgroundStyle : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
        {
			for (int i = 0; i <= 3; i++)
			{
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyUG" + i.ToString());
			}
		}
	}

    public class SpookyUndergroundBackgroundStyleAlt : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
        {
			for (int i = 0; i <= 3; i++)
			{
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpookyBiome/SpookyUGAlt" + i.ToString());
			}
		}
	}
}