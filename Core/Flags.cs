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
        public static bool downedEggEvent = false;
        public static bool downedOrroboro = false;
        public static bool downedBigBone = false;

        public static bool SpookyBackgroundAlt = false;

        public static bool EyeQuest1 = false; 
        public static bool EyeQuest2 = false;
        public static bool EyeQuest3 = false;
        public static bool EyeQuest4 = false;
        public static bool EyeQuest5 = false;

        public override void OnWorldLoad() 
        {
			downedRotGourd = false;
            downedMoco = false;
            downedEggEvent = false;
            downedOrroboro = false;
            downedBigBone = false;

            SpookyBackgroundAlt = false;

            EyeQuest1 = false; 
            EyeQuest2 = false;
            EyeQuest3 = false;
            EyeQuest4 = false;
            EyeQuest5 = false;
		}

		public override void OnWorldUnload() 
        {
			downedRotGourd = false;
            downedMoco = false;
            downedEggEvent = false;
            downedOrroboro = false;
            downedBigBone = false;

            SpookyBackgroundAlt = false;

            EyeQuest1 = false; 
            EyeQuest2 = false;
            EyeQuest3 = false;
            EyeQuest4 = false;
            EyeQuest5 = false;
		}

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedRotGourd) tag["downedRotGourd"] = true;
            if (downedMoco) tag["downedMoco"] = true;
            if (downedEggEvent) tag["downedEggEvent"] = true;
            if (downedOrroboro) tag["downedOrroboro"] = true;
            if (downedBigBone) tag["downedBigBone"] = true;

            if (SpookyBackgroundAlt) tag["SpookyBackgroundAlt"] = true;

            if (EyeQuest1) tag["EyeQuest1"] = true;
            if (EyeQuest2) tag["EyeQuest2"] = true;
            if (EyeQuest3) tag["EyeQuest3"] = true;
            if (EyeQuest4) tag["EyeQuest4"] = true;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
			downedRotGourd = tag.ContainsKey("downedRotGourd");
            downedMoco = tag.ContainsKey("downedMoco");
            downedEggEvent = tag.ContainsKey("downedEggEvent");
            downedOrroboro = tag.ContainsKey("downedOrroboro");
            downedBigBone = tag.ContainsKey("downedBigBone");

            SpookyBackgroundAlt = tag.ContainsKey("SpookyBackgroundAlt");
            
            EyeQuest1 = tag.ContainsKey("EyeQuest1");
            EyeQuest2 = tag.ContainsKey("EyeQuest2");
            EyeQuest3 = tag.ContainsKey("EyeQuest3");
            EyeQuest4 = tag.ContainsKey("EyeQuest4");
            EyeQuest5 = tag.ContainsKey("EyeQuest5");
		}

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedRotGourd;
            flags[1] = downedMoco;
            flags[2] = downedEggEvent;
            flags[3] = downedOrroboro;
            flags[4] = downedBigBone;
            writer.Write(flags);

            var miscFlags = new BitsByte();
            miscFlags[0] = SpookyBackgroundAlt;
            writer.Write(miscFlags);

            var questFlags = new BitsByte();
            questFlags[0] = EyeQuest1;
            questFlags[1] = EyeQuest2;
            questFlags[2] = EyeQuest3;
            questFlags[3] = EyeQuest4;
            questFlags[4] = EyeQuest5;
            writer.Write(questFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedRotGourd = flags[0];
            downedMoco = flags[1];
            downedEggEvent = flags[2];
            downedOrroboro = flags[3];
            downedBigBone = flags[4];

            BitsByte miscFlags = reader.ReadByte();
            SpookyBackgroundAlt = miscFlags[0];

            BitsByte questFlags = reader.ReadByte();
            EyeQuest1 = miscFlags[0];
            EyeQuest2 = miscFlags[1];
            EyeQuest3 = miscFlags[2];
            EyeQuest4 = miscFlags[3];
            EyeQuest4 = miscFlags[4];
        }
    }
}
