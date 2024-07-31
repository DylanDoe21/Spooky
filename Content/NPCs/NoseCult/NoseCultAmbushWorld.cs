using Terraria.ModLoader;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultAmbushWorld : ModSystem
	{
		public static bool AmbushActive;
		public static bool CanDisableAmbush;

		public override void OnWorldLoad()
		{
			AmbushActive = false;
			CanDisableAmbush = false;
		}

        public override void PostUpdateEverything()
		{
			//end the event and reset everything if no players are in the biome
			if (!ModContent.GetInstance<MocoIdol1>().AnyPlayersInBiome())
			{
				AmbushActive = false;
				CanDisableAmbush = false;
			}
		}
	}
}