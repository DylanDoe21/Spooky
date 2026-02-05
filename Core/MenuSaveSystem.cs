using Terraria;
using Terraria.ModLoader;
using System.IO;

namespace Spooky.Core
{
    internal class MenuSaveSystem : ModSystem
    {
        //these bools are to check if the file actually exists
        private static bool? hasDefeatedRotGourdField;
        private static bool? hasDefeatedSpookySpiritField;
        private static bool? hasDefeatedMocoField;
        private static bool? hasDefeatedDaffodilField;
        private static bool? hasDefeatedOldHunterField;
        private static bool? hasDefeatedOrroboroField;
        private static bool? hasDefeatedBigBoneField;
        private static bool? hasDefeatedSpookFishronField;

        //default file path for the README file
        public static string ReadMeFilePath => $"{Main.SavePath}/SpookyModMenuScreen/README.txt";

        //individual file paths for each boss
        public static string RotGourdSavePath => $"{Main.SavePath}/SpookyModMenuScreen/RotGourd";
        public static string SpookySpiritSavePath => $"{Main.SavePath}/SpookyModMenuScreen/SpookySpirit";
        public static string MocoSavePath => $"{Main.SavePath}/SpookyModMenuScreen/Moco";
        public static string DaffodilSavePath => $"{Main.SavePath}/SpookyModMenuScreen/Daffodil";
        public static string OldHunterSavePath => $"{Main.SavePath}/SpookyModMenuScreen/OldHunter";
        public static string OrroboroSavePath => $"{Main.SavePath}/SpookyModMenuScreen/Orroboro";
        public static string BigBoneSavePath => $"{Main.SavePath}/SpookyModMenuScreen/BigBone";
        public static string SpookFishronSavePath => $"{Main.SavePath}/SpookyModMenuScreen/SpookFishron";

        public override void Load()
        {
            //create the directory for the menu saves if it doesn't exist
            if (!File.Exists(ReadMeFilePath))
            {
                string ActualDirectory = Path.GetDirectoryName(ReadMeFilePath);

                if (!Directory.Exists(ActualDirectory))
                {
                    Directory.CreateDirectory(ActualDirectory);
                }

                //TODO: turn this into localized text
                var pathWriter = File.CreateText(ReadMeFilePath);
                pathWriter.WriteLine("Spooky mod's custom menu screen has blackened versions of each spooky mod boss on it" +
                "\nEach time a spooky mod boss is defeated in game, a blank file with that bosses name will be created in this folder (If it doesn't exist already)" +
                "\nSpooky mod itself will check if these files exist, and if they do, then that respective boss will become un-blackened on the menu screen" +
                "\nDeleting any of those files in this folder will re-blacken that respective boss on the menu, but you need reload the mod in game for this to take effect");
                pathWriter.Close();
            }
        }

        public static bool hasDefeatedRotGourd
        {
            get
            {
                hasDefeatedRotGourdField ??= hasDefeatedRotGourdField = File.Exists(RotGourdSavePath);
                return hasDefeatedRotGourdField.Value;
            }
            set
            {
                hasDefeatedRotGourdField = value;

                if (!value)
                {
                    File.Delete(RotGourdSavePath);
                }
                else
                {
                    File.Create(RotGourdSavePath);
                }
            }
        }

        public static bool hasDefeatedSpookySpirit
        {
            get
            {
                hasDefeatedSpookySpiritField ??= hasDefeatedSpookySpiritField = File.Exists(SpookySpiritSavePath);
                return hasDefeatedSpookySpiritField.Value;
            }
            set
            {
                hasDefeatedSpookySpiritField = value;

                if (!value)
                {
                    File.Delete(SpookySpiritSavePath);
                }
                else
                {
                    File.Create(SpookySpiritSavePath);
                }
            }
        }

        public static bool hasDefeatedMoco
        {
            get
            {
                hasDefeatedMocoField ??= hasDefeatedMocoField = File.Exists(MocoSavePath);
                return hasDefeatedMocoField.Value;
            }
            set
            {
                hasDefeatedMocoField = value;

                if (!value)
                {
                    File.Delete(MocoSavePath);
                }
                else
                {
                    File.Create(MocoSavePath);
                }
            }
        }

        public static bool hasDefeatedDaffodil
        {
            get
            {
                hasDefeatedDaffodilField ??= hasDefeatedDaffodilField = File.Exists(DaffodilSavePath);
                return hasDefeatedDaffodilField.Value;
            }
            set
            {
                hasDefeatedDaffodilField = value;

                if (!value)
                {
                    File.Delete(DaffodilSavePath);
                }
                else
                {
                    File.Create(DaffodilSavePath);
                }
            }
        }

        public static bool hasDefeatedOldHunter
        {
            get
            {
                hasDefeatedOldHunterField ??= hasDefeatedOldHunterField = File.Exists(OldHunterSavePath);
                return hasDefeatedOldHunterField.Value;
            }
            set
            {
                hasDefeatedOldHunterField = value;

                if (!value)
                {
                    File.Delete(OldHunterSavePath);
                }
                else
                {
                    File.Create(OldHunterSavePath);
                }
            }
        }

        public static bool hasDefeatedOrroboro
        {
            get
            {
                hasDefeatedOrroboroField ??= hasDefeatedOrroboroField = File.Exists(OrroboroSavePath);
                return hasDefeatedOrroboroField.Value;
            }
            set
            {
                hasDefeatedOrroboroField = value;

                if (!value)
                {
                    File.Delete(OrroboroSavePath);
                }
                else
                {
                    File.Create(OrroboroSavePath);
                }
            }
        }

        public static bool hasDefeatedBigBone
        {
            get
            {
                hasDefeatedBigBoneField ??= hasDefeatedBigBoneField = File.Exists(BigBoneSavePath);
                return hasDefeatedBigBoneField.Value;
            }
            set
            {
                hasDefeatedBigBoneField = value;

                if (!value)
                {
                    File.Delete(BigBoneSavePath);
                }
                else
                {
                    File.Create(BigBoneSavePath);
                }
            }
        }

        public static bool hasDefeatedSpookFishron
        {
            get
            {
                hasDefeatedSpookFishronField ??= hasDefeatedSpookFishronField = File.Exists(SpookFishronSavePath);
                return hasDefeatedSpookFishronField.Value;
            }
            set
            {
                hasDefeatedSpookFishronField = value;

                if (!value)
                {
                    File.Delete(SpookFishronSavePath);
                }
                else
                {
                    File.Create(SpookFishronSavePath);
                }
            }
        }
    }
}