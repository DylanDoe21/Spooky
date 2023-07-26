using Terraria;
using Terraria.ModLoader;

using Spooky.Content.NPCs.Hallucinations;

namespace Spooky.Content.Biomes
{
    //this is not an actual in game biome, it only exists for the bestiary
    public class SpookyHellLake : ModBiome
    {
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyHellLakeIcon";

        public override bool IsBiomeActive(Player player)
        {
            return false;
        }
    }
}