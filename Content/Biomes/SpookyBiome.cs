using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class SpookyBiome : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Forest");
        }
        
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => Flags.SpookyBackgroundAlt ? ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/SpookyForestBGAlt") : ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/SpookyForestBG");
        
        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiome") : MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeNight");
        
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Spooky/SpookyWaterStyle");
        
        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HalloweenSky", player.InModBiome(ModContent.GetInstance<SpookyBiome>()), player.Center);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HalloweenSky", false, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Content/Biomes/SpookyBiomeIcon";
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => Color.Orange;

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyTiles >= 200;
            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}