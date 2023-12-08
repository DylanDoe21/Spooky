using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Backgrounds.Cemetery;
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

                if (!Main.bloodMoon)
                {
                    //play normal theme if its not storming and the player isnt in a town
                    if (!Main.IsItStorming && Main.LocalPlayer.townNPCs < 3f)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/Cemetery");
                    }
                    //play town theme if the player is in a town
                    else if (!Main.IsItStorming && Main.LocalPlayer.townNPCs >= 3f)
                    {
                        music = MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/TownThemeTest");
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
                    music = MusicID.Eerie;
                }

                return music;
            }
        }
       
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int BiomeTorchItemType => ItemID.GreenTorch;
        
        public override ModWaterStyle WaterStyle => ModContent.GetInstance<SpookyWaterStyle>();

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

            Main.GraveyardVisualIntensity = 0.25f;

            if (Main.rand.NextBool(800) && !player.InModBiome(ModContent.GetInstance<RaveyardBiome>()))
            {
                Main.NewLightning();
            }
        }

        //conditions to be in the biome
        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().cemeteryTiles >= 500;
            bool SurfaceCondition = player.ZoneSkyHeight || player.ZoneOverworldHeight;

            return BiomeCondition && SurfaceCondition;
        }
    }
}