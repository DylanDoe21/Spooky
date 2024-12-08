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
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Raveyard1");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<LeanWaterStyle>();

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:RaveyardSky", isActive, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/RaveyardBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = player.InModBiome(ModContent.GetInstance<CemeteryBiome>()) && Flags.RaveyardHappening;

            bool SurfaceCondition = player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}