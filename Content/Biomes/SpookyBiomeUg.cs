using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class SpookyBiomeUg : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Underground");
        }

        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => Flags.SpookyBackgroundAlt ? ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/SpookyForestBGAlt") : ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/SpookyForestBG");

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => Flags.SpookyBackgroundAlt ? ModContent.Find<ModUndergroundBackgroundStyle>("Spooky/SpookyUndergroundBG1") : ModContent.Find<ModUndergroundBackgroundStyle>("Spooky/SpookyUndergroundBG2");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeUnderground");
        
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Spooky/SpookyWaterStyle");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HalloweenSky", player.InModBiome(ModContent.GetInstance<SpookyBiome>()), player.Center);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:HalloweenSky", false, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyBiomeUgIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyTiles >= 500;
            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return BiomeCondition && UndergroundCondition;
        }
    }
}