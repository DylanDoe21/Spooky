using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using Spooky.Content.Tiles.NoseTemple;

namespace Spooky.Content.Biomes
{
    public class NoseTempleBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/NoseTemple");

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

            int[] DungeonWalls = new int[] { ModContent.WallType<NoseTempleWallPurple>(), ModContent.WallType<NoseTempleFancyWallPurple>(), ModContent.WallType<NoseTempleWallBGPurple>(),
            ModContent.WallType<NoseTempleWallGreen>(), ModContent.WallType<NoseTempleFancyWallGreen>(), ModContent.WallType<NoseTempleWallBGGreen>(),
            ModContent.WallType<NoseTempleWallGray>(), ModContent.WallType<NoseTempleFancyWallGray>(), ModContent.WallType<NoseTempleWallBGGray>(),
            ModContent.WallType<NoseTempleWallRed>(), ModContent.WallType<NoseTempleFancyWallRed>(), ModContent.WallType<NoseTempleWallBGRed>() };

            bool BiomeCondition = player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()) && DungeonWalls.Contains(Main.tile[PlayerX, PlayerY].WallType);

            return BiomeCondition;
        }
    }
}