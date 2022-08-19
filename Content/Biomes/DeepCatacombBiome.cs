using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Tiles.Catacomb;

namespace Spooky.Content.Biomes
{
    public class DeepCatacombBiome : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Catacombs");
        }

        public override int Music => MusicID.Temple;
        //public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SchemingWeasel");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CatacombBiomeIcon";
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => Color.Orange;

        public override void OnInBiome(Player player)
        {
            player.AddBuff(ModContent.BuffType<CatacombDebuff>(), 2);
        }

        public override bool IsBiomeActive(Player player)
        {
            int PlayerX = (int)player.Center.X / 16;
            int PlayerY = (int)player.Center.Y / 16;

            bool BiomeCondition = Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<CatacombBrickWall2>();

            return BiomeCondition;
        }
    }
}