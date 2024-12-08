using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;
using System;
using System.Linq;

using Spooky.Content.Tiles.Minibiomes.Christmas;

namespace Spooky.Content.Biomes
{
    public class ChristmasDungeonBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/ChristmasDungeon");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int BiomeTorchItemType => ItemID.IceTorch;

        //bestiary stuff
        //public override string BestiaryIcon => "Spooky/Content/Biomes/VegetableBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            int PlayerX = (int)player.Center.X / 16;
            int PlayerY = (int)player.Center.Y / 16;

            int[] DungeonWalls = new int[] { ModContent.WallType<ChristmasBrickWall>(), ModContent.WallType<ChristmasBrickWallAlt>(), 
            ModContent.WallType<ChristmasWoodWall>(), ModContent.WallType<ChristmasWindow>() };

            bool BiomeCondition = DungeonWalls.Contains(Main.tile[PlayerX, PlayerY].WallType);

            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return BiomeCondition && UndergroundCondition;
        }
    }
}