using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;

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
			//end the event and reset everything if no players are in the biome
			if (!ModContent.GetInstance<MocoIdol1>().AnyPlayersInBiome())
			{
				AmbushActive = false;
			}
		}

		public override void NetSend(BinaryWriter writer)
        {
			writer.WriteFlags(AmbushActive);
		}

		public override void NetReceive(BinaryReader reader)
        {
			reader.ReadFlags(out AmbushActive);
		}
	}
}