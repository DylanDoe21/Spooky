using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Effects;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.PandoraBox;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{ 
    public class CatacombBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/CatacombUpper");
       
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();

        public override int BiomeTorchItemType => ModContent.ItemType<CatacombTorch1Item>();

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CatacombBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void OnInBiome(Player player)
        {
            //vignette effect
            if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()) && !NPC.AnyNPCs(ModContent.NPCType<BigBone>()) && !PandoraBoxWorld.PandoraEventActive && !Flags.downedBigBone)
            {
                VignettePlayer vignettePlayer = player.GetModPlayer<VignettePlayer>();
                vignettePlayer.SetVignette(0f, 650f, 1f, Color.Black, player.Center);
            }

            if (!Flags.downedBigBone)
            {
                player.AddBuff(ModContent.BuffType<CatacombDebuff>(), 2);
            }

            //graveyard visuals
            player.ZoneGraveyard = true;
            Main.GraveyardVisualIntensity = 0.12f;
        }

        public override bool IsBiomeActive(Player player)
        {
            int PlayerX = (int)player.Center.X / 16;
            int PlayerY = (int)player.Center.Y / 16;

            bool BiomeCondition = Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<CatacombBrickWall1>() || Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<CatacombGrassWall1>();
            
            bool UndergroundCondition = PlayerY > Main.worldSurface - 25;

            return BiomeCondition && UndergroundCondition;
        }
    }

    public class CatacombBiome2 : CatacombBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/CatacombLower");

        public override int BiomeTorchItemType => ModContent.ItemType<CatacombTorch2Item>();

        public override string BestiaryIcon => "Spooky/Content/Biomes/CatacombBiome2Icon";

        public override bool IsBiomeActive(Player player)
        {
            int PlayerX = (int)player.Center.X / 16;
            int PlayerY = (int)player.Center.Y / 16;

            bool BiomeCondition = Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<CatacombBrickWall2>() || Main.tile[PlayerX, PlayerY].WallType == ModContent.WallType<CatacombGrassWall2>();
            
            bool UndergroundCondition = PlayerY > Main.worldSurface - 25;

            return BiomeCondition && UndergroundCondition;
        }
    }
}