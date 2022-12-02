using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;

namespace Spooky.Core
{
    public class Flags : ModSystem
    {
        public static bool downedRotGourd = false;
        public static bool downedMoco = false;
        public static bool downedOrroboro = false;
        public static bool downedBigBone = false;

        public static bool SpookyBackgroundAlt = false;

        public static bool CatacombKey1 = false; 
        public static bool CatacombKey2 = false;
        public static bool CatacombKey3 = false;

        public static bool EyeQuest1 = false; 
        public static bool EyeQuest2 = false;
        public static bool EyeQuest3 = false;
        public static bool EyeQuest4 = false;

        public override void OnWorldLoad() 
        {
			downedRotGourd = false;
            downedMoco = false;
            downedOrroboro = false;
            downedBigBone = false;

            SpookyBackgroundAlt = false;

            CatacombKey1 = false; 
            CatacombKey2 = false;
            CatacombKey3 = false;

            EyeQuest1 = false; 
            EyeQuest2 = false;
            EyeQuest3 = false;
            EyeQuest4 = false;
		}

		public override void OnWorldUnload() 
        {
			downedRotGourd = false;
            downedMoco = false;
            downedOrroboro = false;
            downedBigBone = false;

            SpookyBackgroundAlt = false;

            CatacombKey1 = false; 
            CatacombKey2 = false;
            CatacombKey3 = false;

            EyeQuest1 = false; 
            EyeQuest2 = false;
            EyeQuest3 = false;
            EyeQuest4 = false;
		}

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedRotGourd) tag["downedRotGourd"] = true;
            if (downedMoco) tag["downedMoco"] = true;
            if (downedOrroboro) tag["downedOrroboro"] = true;
            if (downedBigBone) tag["downedBigBone"] = true;

            if (SpookyBackgroundAlt) tag["SpookyBackgroundAlt"] = true;
            
            if (CatacombKey1) tag["CatacombKey1"] = true;
            if (CatacombKey2) tag["CatacombKey2"] = true;
            if (CatacombKey3) tag["CatacombKey3"] = true;

            if (EyeQuest1) tag["EyeQuest1"] = true;
            if (EyeQuest2) tag["EyeQuest2"] = true;
            if (EyeQuest3) tag["EyeQuest3"] = true;
            if (EyeQuest4) tag["EyeQuest4"] = true;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
			downedRotGourd = tag.ContainsKey("downedRotGourd");
            downedMoco = tag.ContainsKey("downedMoco");
            downedOrroboro = tag.ContainsKey("downedOrroboro");
            downedBigBone = tag.ContainsKey("downedBigBone");

            SpookyBackgroundAlt = tag.ContainsKey("SpookyBackgroundAlt");

            CatacombKey1 = tag.ContainsKey("CatacombKey1");
            CatacombKey2 = tag.ContainsKey("CatacombKey2");
            CatacombKey3 = tag.ContainsKey("CatacombKey3");

            EyeQuest1 = tag.ContainsKey("EyeQuest1");
            EyeQuest2 = tag.ContainsKey("EyeQuest2");
            EyeQuest3 = tag.ContainsKey("EyeQuest3");
            EyeQuest4 = tag.ContainsKey("EyeQuest4");
		}

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedRotGourd;
            flags[1] = downedMoco;
            flags[2] = downedOrroboro;
            flags[3] = downedBigBone;
            writer.Write(flags);

            var miscFlags = new BitsByte();
            miscFlags[0] = SpookyBackgroundAlt;
            miscFlags[1] = CatacombKey1;
            miscFlags[2] = CatacombKey2;
            miscFlags[3] = CatacombKey3;
            miscFlags[4] = EyeQuest1;
            miscFlags[5] = EyeQuest2;
            miscFlags[6] = EyeQuest3;
            miscFlags[7] = EyeQuest4;
            writer.Write(miscFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedRotGourd = flags[0];
            downedMoco = flags[1];
            downedOrroboro = flags[2];
            downedBigBone = flags[3];

            BitsByte miscFlags = reader.ReadByte();
            SpookyBackgroundAlt = miscFlags[0];
            CatacombKey1 = miscFlags[1];
            CatacombKey2 = miscFlags[2];
            CatacombKey3 = miscFlags[3];
            EyeQuest1 = miscFlags[4];
            EyeQuest2 = miscFlags[5];
            EyeQuest3 = miscFlags[6];
            EyeQuest4 = miscFlags[7];
        }
    }
}
