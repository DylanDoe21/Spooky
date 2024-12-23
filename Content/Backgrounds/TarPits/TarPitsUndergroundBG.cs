using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.TarPits
{
	public class TarPitsUndergroundBG : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
        {
			for (int i = 0; i <= 3; i++)
			{
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/TarPits/TarPitsUG" + i.ToString());
			}
		}
	}
}