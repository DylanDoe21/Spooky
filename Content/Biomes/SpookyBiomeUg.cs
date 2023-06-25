using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.SpookyBiome;

namespace Spooky.Content.Biomes
{
    public class SpookyBiomeUg : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => Flags.SpookyBackgroundAlt ? ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/SpookyForestBGAlt") : ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/SpookyForestBG");

        //for whatever reason spooky mod underground backgrounds just break underground backgrounds
        //this will be disabled until it magically gets fixed or something
        //public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => Flags.SpookyBackgroundAlt ? ModContent.GetInstance<SpookyUndergroundBackgroundStyleAlt>() : ModContent.GetInstance<SpookyUndergroundBackgroundStyle>();

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeUnderground");
        
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Spooky/SpookyWaterStyle");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeMedium;

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