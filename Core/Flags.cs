using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.IO;

namespace Spooky.Core
{
    public sealed partial class Flags : ModSystem
    {
        public static bool downedRotGourd = false;
        public static bool downedGhostEvent = false;
        public static bool downedOrroboro = false;

        public static bool SpookyBackgroundAlt = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedRotGourd) tag["downedRotGourd"] = true;
            if (downedGhostEvent) tag["downedGhostEvent"] = true;
            if (downedOrroboro) tag["downedOrroboro"] = true;

            if (SpookyBackgroundAlt) tag["SpookyBackgroundAlt"] = true;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
			downedRotGourd = tag.ContainsKey("downedRotGourd");
            downedGhostEvent = tag.ContainsKey("downedGhostEvent");
            downedOrroboro = tag.ContainsKey("downedOrroboro");

            SpookyBackgroundAlt = tag.ContainsKey("SpookyBackgroundAlt");
		}

        public override void NetSend(BinaryWriter writer)
        {
            var flags = new BitsByte();
            flags[0] = downedRotGourd;
            flags[1] = downedGhostEvent;
            flags[2] = downedOrroboro;
            writer.Write(flags);

            var miscFlags = new BitsByte();
            miscFlags[0] = SpookyBackgroundAlt;
            writer.Write(miscFlags);
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            downedRotGourd = flags[0];
            downedGhostEvent = flags[1];
            downedOrroboro = flags[2];

            BitsByte miscFlags = reader.ReadByte();
            SpookyBackgroundAlt = miscFlags[0];
        }
    }
}
