using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.SpiderCave
{
	public class SpiderCaveUndergroundBG : ModUndergroundBackgroundStyle
	{
		//uses blank textures so the actual spider cave custom background can show up 
		public override void FillTextureArray(int[] textureSlots)
        {
			for (int i = 0; i <= 3; i++)
			{
                textureSlots[i] = BackgroundTextureLoader.GetBackgroundSlot("Spooky/Content/Backgrounds/SpiderCave/SpiderCaveUG" + i.ToString());
			}
		}
	}
}