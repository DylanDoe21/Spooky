using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class CemeteryBiome : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/CemeteryBG");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Cemetery");
        
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Spooky/CemeteryWaterStyle");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:Cemetery", isActive && !player.InModBiome(ModContent.GetInstance<RaveyardBiome>()), player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CemeteryBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            //graveyard visuals
            player.ZoneGraveyard = true;

            if (!player.InModBiome(ModContent.GetInstance<RaveyardBiome>()))
            {
                Main.GraveyardVisualIntensity = 0.42f;
            }

            if (Main.rand.NextBool(800))
            {
                Main.NewLightning();
            }
        }

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().cemeteryTiles >= 500000;
            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }

    public class RaveyardBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeRain");
        
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Spooky/LeanWaterStyle");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:Raveyard", isActive, player.Center);
        }

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = player.InModBiome(ModContent.GetInstance<CemeteryBiome>()) && ModContent.GetInstance<TileCount>().raveyardTiles >= 8;

            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}