using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.SpiderCave;
using Spooky.Content.Tiles.SpiderCave.Furniture;

namespace Spooky.Content.Biomes
{
    public class VegetableBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/VegetableBiome");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override int BiomeTorchItemType => ItemID.JungleTorch;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/VegetableBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().vegetableTiles >= 600;
            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return BiomeCondition && UndergroundCondition;
        }
    }
}