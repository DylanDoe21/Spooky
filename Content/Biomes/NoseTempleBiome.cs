using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Tiles.SpookyHell.NoseTemple;

namespace Spooky.Content.Biomes
{
    public class NoseTempleBiome : ModBiome
    {
        public override int Music => MusicID.Eerie;

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/NoseTempleBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            int PlayerX = (int)player.Center.X / 16;
            int PlayerY = (int)player.Center.Y / 16;

            bool WallCondition = Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleWallPurple>() ||
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleFancyWallPurple>() || 
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleWallBGPurple>() ||
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleWallGreen>() ||
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleFancyWallGreen>() || 
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleWallBGGreen>() ||
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleWallRed>() ||
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleFancyWallRed>() || 
            Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<NoseTempleWallBGRed>();

            bool DeepEnoughCondition = PlayerY >= Main.maxTilesY - 103;

            bool BiomeCondition = DeepEnoughCondition && WallCondition && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>());

            return BiomeCondition;
        }
    }
}