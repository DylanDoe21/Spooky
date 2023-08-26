using Terraria.ModLoader;
using Terraria.Localization;

namespace Spooky.Core
{
    internal class MusicDisplayCompatibility : ModSystem
    {
        public override void PostSetupContent()
        {
            if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
            {
                return;
            }

            void AddMusic(string path, string name, string author) => display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(Mod, path), name, "by " + author, "Spooky Mod");

            //spooky forest
            AddMusic("Content/Sounds/Music/SpookyBiomeDay", Language.GetTextValue("Mods.Spooky.MusicDisplay.SpookyForestDay"), "BananaLizard");
            AddMusic("Content/Sounds/Music/SpookyBiomeNight", Language.GetTextValue("Mods.Spooky.MusicDisplay.SpookyForestNight"), "BananaLizard");
            AddMusic("Content/Sounds/Music/SpookyBiomeRain", Language.GetTextValue("Mods.Spooky.MusicDisplay.SpookyForestRain"), "BananaLizard");
            AddMusic("Content/Sounds/Music/SpookyBiomeUnderground", Language.GetTextValue("Mods.Spooky.MusicDisplay.SpookyForestUG"), "BananaLizard");
            AddMusic("Content/Sounds/Music/RotGourd", Language.GetTextValue("Mods.Spooky.MusicDisplay.RotGourd"), "BananaLizard");

            //cemetery
            AddMusic("Content/Sounds/Music/Cemetery", Language.GetTextValue("Mods.Spooky.MusicDisplay.SwampyCemetery"), "BananaLizard");
            AddMusic("Content/Sounds/Music/SpookySpirit", Language.GetTextValue("Mods.Spooky.MusicDisplay.SpookySpirit"), "BananaLizard");

            //catacombs
            AddMusic("Content/Sounds/Music/CatacombUpper", Language.GetTextValue("Mods.Spooky.MusicDisplay.UpperCatacombs"), "BananaLizard");
            AddMusic("Content/Sounds/Music/Daffodil", Language.GetTextValue("Mods.Spooky.MusicDisplay.Daffodil"), "BananaLizard");
            AddMusic("Content/Sounds/Music/CatacombLower", Language.GetTextValue("Mods.Spooky.MusicDisplay.LowerCatacombs"), "BananaLizard");
            AddMusic("Content/Sounds/Music/PandoraBox", Language.GetTextValue("Mods.Spooky.MusicDisplay.PandoraBox"), "Ennway");
            AddMusic("Content/Sounds/Music/BigBone", Language.GetTextValue("Mods.Spooky.MusicDisplay.BigBone"), "BananaLizard");

            //eye valley
            AddMusic("Content/Sounds/Music/SpookyHell", Language.GetTextValue("Mods.Spooky.MusicDisplay.EyeValley"), "BananaLizard");
            AddMusic("Content/Sounds/Music/Moco", Language.GetTextValue("Mods.Spooky.MusicDisplay.Moco"), "BananaLizard");
            AddMusic("Content/Sounds/Music/EggEvent", Language.GetTextValue("Mods.Spooky.MusicDisplay.EggEvent"), "Rockwizard5");
            AddMusic("Content/Sounds/Music/Orroboro", Language.GetTextValue("Mods.Spooky.MusicDisplay.Orroboro"), "BananaLizard");
        }
    }
}