using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.ZombieOcean;
using Spooky.Content.Tiles.Water;
using Spooky.Content.NPCs.Minibiomes.Ocean;

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
			if (!NPC.AnyNPCs(ModContent.NPCType<Dunkleosteus>()))
			{
				int Count = Flags.ZombieBiomePositions.Count - 1;
				NPC.NewNPC(null, (int)Flags.ZombieBiomePositions[Count].X * 16, (int)Flags.ZombieBiomePositions[Count].Y * 16, ModContent.NPCType<Dunkleosteus>());
			}
		}

		public override bool IsBiomeActive(Player player)
        {
            Point point = player.Center.ToTileCoordinates();
            
            bool InOceanZone = point.X < 380 || point.X > Main.maxTilesX - 380;
            bool BiomeCondition = ModContent.GetInstance<TileCount>().zombieOceanTiles > 500;
            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;

            return InOceanZone && BiomeCondition && UndergroundCondition;
        }
    }
}