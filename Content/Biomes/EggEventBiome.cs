using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Effects;
using Spooky.Content.Events;

namespace Spooky.Content.Biomes
{
    public class EggEventBiome : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Egg Incursion");
        }

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/EggEvent");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:EggEventTint", EggEventWorld.EggEventActive && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()), player.Center);
        }
        
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:EggEventTint", false, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/EggEventBiomeIcon";
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