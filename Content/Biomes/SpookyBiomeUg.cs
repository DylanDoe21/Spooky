using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class SpookyBiomeUg : ModBiome
    {
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<SpookyForestBG>();

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<TileCount>().glowshroomTiles >= 250 ? ModContent.GetInstance<GlowshroomUndergroundBG>() : ModContent.GetInstance<SpookyUndergroundBackgroundStyle>();

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBiomeUnderground");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();

        public override int BiomeTorchItemType => ModContent.ItemType<SpookyBiomeTorchItem>();

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyBiomeUgIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyTiles >= 800 && ModContent.GetInstance<TileCount>().spiderCaveTiles < 1200;
            bool UndergroundCondition = (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight) && !player.ZoneDungeon;

            return BiomeCondition && UndergroundCondition;
        }
    }
}