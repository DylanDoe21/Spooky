using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.SpookyBiome
{
	public class SpookyUndergroundBG1 : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
        {
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG0");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG1");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG3");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG1");
		}
	}

    public class SpookyUndergroundBG2 : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
        {
			textureSlots[0] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG0");
			textureSlots[1] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG2");
            textureSlots[2] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG3");
            textureSlots[3] = BackgroundTextureLoader.GetBackgroundSlot(Mod, "Content/Backgrounds/SpookyBiome/SpookyUndergroundBG2");
		}
	}
}