using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.ZombieOcean;
using Spooky.Content.NPCs.Minibiomes.Ocean;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class ZombieOceanBiome : ModBiome
    {
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<ZombieOceanUG>();

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/ZombieOcean");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<ZombieWaterStyle>();

        //public override int BiomeTorchItemType => ItemID.IceTorch;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/ZombieOceanBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            VignettePlayer vignettePlayer = Main.LocalPlayer.GetModPlayer<VignettePlayer>();
            if (Main.netMode != NetmodeID.Server) 
            {
                vignettePlayer.SetVignette(1f, 750f, 0.95f, new Color(7, 17, 3), player.Center);
            }
        }

		public override bool IsBiomeActive(Player player)
        {
            Point point = player.Center.ToTileCoordinates();
            
            bool InOceanZone = point.X < 380 || point.X > Main.maxTilesX - 380;
            bool BiomeCondition = ModContent.GetInstance<TileCount>().zombieOceanTiles > 600;
            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return InOceanZone && BiomeCondition && UndergroundCondition && !player.ZoneDungeon;
        }
    }
}