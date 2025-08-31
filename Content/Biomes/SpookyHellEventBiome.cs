using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class SpookyHellEventBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/EggEvent");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyHellEventBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = EggEventWorld.EggEventActive && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>());

            return BiomeCondition;
        }
    }
}