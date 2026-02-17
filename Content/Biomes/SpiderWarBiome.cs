using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class SpiderWarBiome : ModBiome
    {
		public override int Music => MusicID.UndergroundDesert;

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

		public override string BestiaryIcon => "Spooky/Content/Biomes/SpiderCaveBiomeIcon";

        public override bool IsBiomeActive(Player player)
        {
            return SpiderWarWorld.SpiderWarActive && player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>());
        }
	}
}