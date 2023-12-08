using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.SpiderCave
{
	public class SpiderCaveUndergroundBG : ModUndergroundBackgroundStyle
	{
		//this just uses blank textures since the actual spider cave custom background draws behind vanilla underground backgrounds
		public override void FillTextureArray(int[] textureSlots)
        {
			for (int i = 0; i <= 3; i++)
			{
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpiderCave/SpiderCaveUG" + i.ToString());
			}
		}
	}
}