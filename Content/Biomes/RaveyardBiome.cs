using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class RaveyardBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeRain");
        
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<LeanWaterStyle>();

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:Raveyard", isActive, player.Center);
        }

        public override string MapBackground => "Spooky/Content/Biomes/CemeteryBiome_Background";

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = player.InModBiome(ModContent.GetInstance<CemeteryBiome>()) && SpookyWorld.RaveyardHappening;

            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}