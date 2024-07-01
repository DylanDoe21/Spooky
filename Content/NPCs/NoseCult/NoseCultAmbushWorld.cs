using Terraria.ModLoader;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultAmbushWorld : ModSystem
	{
		public static bool AmbushActive;

		public override void OnWorldLoad()
		{
			AmbushActive = false;
		}

        public override void PostUpdateEverything()
		{
			//end the event and reset everything if you die
			if (!ModContent.GetInstance<MocoIdol1>().AnyPlayersInBiome())
			{
				AmbushActive = false;
			}
		}
	}
}