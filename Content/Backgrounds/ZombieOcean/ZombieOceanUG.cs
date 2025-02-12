using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.ZombieOcean
{
	public class ZombieOceanUG : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
        {
			for (int i = 0; i <= 3; i++)
			{
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/ZombieOcean/ZombieOceanUG" + i.ToString());
			}
		}
	}
}