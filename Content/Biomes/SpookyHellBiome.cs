using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Effects;
using Spooky.Content.Events;

namespace Spooky.Content.Biomes
{
    public class SpookyHellBiome : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Valley of Eyes");
        }

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyHell");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
            {
                //VignettePlayer vignettePlayer = player.GetModPlayer<VignettePlayer>();
                //vignettePlayer.SetVignette(1, 1200, 1.2f, Color.Black, player.Center);
            }

            player.ManageSpecialBiomeVisuals("Spooky:SpookyHellTint", player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()), player.Center);
        }
        
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:SpookyHellTint", false, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyHellBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyHellTiles >= 500 && player.ZoneUnderworldHeight;

            return BiomeCondition;
        }
    }

    //exists for the events special effects
    public class EggEventBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/EggEvent");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (EggEventWorld.EggEventActive && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
            {
                //VignettePlayer vignettePlayer = player.GetModPlayer<VignettePlayer>();
                //vignettePlayer.SetVignette(1, 1200, 1.2f, Color.Black, player.Center);
            }

            player.ManageSpecialBiomeVisuals("Spooky:EggEventTint", EggEventWorld.EggEventActive && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()), player.Center);
            player.ManageSpecialBiomeVisuals("Spooky:EggEventEffect", EggEventWorld.EggEventActive && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()), player.Center);
        }
        
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:EggEventTint", false, player.Center);
            player.ManageSpecialBiomeVisuals("Spooky:EggEventEffect", false, player.Center);
        }

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = EggEventWorld.EggEventActive && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>());

            return BiomeCondition;
        }
    }
}