using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Backgrounds.Christmas
{
	public class ChristmasIceUG : ModUndergroundBackgroundStyle
	{
		public override void FillTextureArray(int[] textureSlots)
        {
			//ice background code, taken from terrarias own background drawing for each individual ice underground background variant
			//this is a work-around so that krampus' workshop always has the same exact bg as the underground ice, since manually setting the snow biome doesnt work
			//this is because krampus' workshop is large and areas with very little ice blocks nearby wouldnt be considered an ice biome, and so this works with texture packs
			if (Main.iceBackStyle == 0)
			{
				textureSlots[1] = 33;
				textureSlots[3] = 32;
				textureSlots[0] = 40;
				textureSlots[2] = 34;
			}
			else if (Main.iceBackStyle == 1)
			{
				textureSlots[1] = 118;
				textureSlots[3] = 117;
				textureSlots[0] = 160;
				textureSlots[2] = 161;
			}
			else if (Main.iceBackStyle == 2)
			{
				textureSlots[1] = 165;
				textureSlots[3] = 167;
				textureSlots[0] = 164;
				textureSlots[2] = 166;
			}
			else
			{
				textureSlots[1] = 120;
				textureSlots[3] = 119;
				textureSlots[0] = 162;
				textureSlots[2] = 163;
			}
		}
	}
}