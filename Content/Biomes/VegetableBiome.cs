using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class VegetableBiome : ModBiome
    {
		//set the music to be consistent with vanilla's music priorities
		public override int Music
		{
			get
			{
				int music = Main.curMusic;

				//play town music if enough town npcs exist
				if (Main.LocalPlayer.townNPCs > 2f)
				{
					if (Main.dayTime)
					{
						music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownDay");
					}
					else
					{
						music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyTownNight");
					}
				}
				//play normal music
				else
				{
					music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/VegetableBiome");
				}

				return music;
			}
		}

		public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int BiomeTorchItemType => ItemID.JungleTorch;

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/VegetableBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            //part of the biome condition makes it so that there must be more garden tiles than jungle tiles so the biome zone doesnt overreach in game
            bool BiomeCondition = ModContent.GetInstance<TileCount>().vegetableTiles >= 200 && (ModContent.GetInstance<TileCount>().vegetableTiles > Main.SceneMetrics.JungleTileCount / 5);
            bool UndergroundCondition = player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight;
			bool NotInDungeons = !player.InModBiome<CatacombBiome>() && !player.InModBiome<CatacombBiome2>() && !player.ZoneLihzhardTemple;

            return BiomeCondition && UndergroundCondition && NotInDungeons;
        }
    }
}