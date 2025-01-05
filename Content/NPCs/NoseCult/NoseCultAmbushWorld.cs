using Terraria;
using Terraria.ModLoader;
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
			var EventFlags = new BitsByte();
            EventFlags[0] = AmbushActive;
            writer.Write(EventFlags);
		}

		public override void NetReceive(BinaryReader reader)
        {
			BitsByte EventFlags = reader.ReadByte();
            AmbushActive = EventFlags[0];
		}
	}
}