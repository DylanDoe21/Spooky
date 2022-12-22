using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Capture;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Gores;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class CemeteryBiome : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Swampy Cemetery");
        }
        
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("Spooky/CemeteryBG");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Cemetery");
        
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("Spooky/CemeteryWaterStyle");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CemeteryBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            //graveyard visuals
            player.ZoneGraveyard = true;
            Main.GraveyardVisualIntensity = 0.42f;

            if (Main.rand.Next(800) == 0)
            {
                Main.NewLightning();
            }
        }

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().cemeteryTiles >= 500;
            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}