using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Tiles.Cemetery.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class CemeteryBiome : ModBiome
    {
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<CemeteryBG>();

		//set the music to be consistent with vanilla's music priorities
		public override int Music
        {
            get
            {
                int music = Main.curMusic;

                if (!Main.bloodMoon && !Main.eclipse)
                {
                    if (!Main.IsItStorming)
                    {
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
							if (Main.dayTime)
							{
								music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Cemetery");
							}
							else
							{
								music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/CemeteryNight");
							}
						}
                    }
                    //play monsoon theme during a storm
                    else if (Main.IsItStorming)
                    {
                        music = MusicID.Monsoon;
                    }
                }
                //blood moon theme takes priority over everything
                else
                {
                    if (Main.bloodMoon)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyBloodmoon");
                    }

                    if (Main.eclipse)
                    {
                        music = MusicID.Eclipse;
                    }
                }

                return music;
            }
        }
       
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();

        public override int BiomeTorchItemType => ModContent.ItemType<CemeteryBiomeTorchItem>();

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/CemeteryBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:CemeterySky", isActive && !player.InModBiome(ModContent.GetInstance<RaveyardBiome>()), player.Center);
        }

        public override void OnInBiome(Player player)
        {
            //graveyard visuals
            player.ZoneGraveyard = true;

            if (Main.rand.NextBool(1200) && !player.InModBiome(ModContent.GetInstance<RaveyardBiome>()))
            {
                Main.NewLightning();
            }
        }

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().cemeteryTiles >= 650;
            bool SurfaceCondition = player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition && !player.ZoneBeach;
        }
    }
}