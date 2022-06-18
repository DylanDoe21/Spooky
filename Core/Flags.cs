using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;
using Spooky.Content.Tiles;
using Spooky.Core;

namespace Spooky.Core
{
    public sealed partial class Flags : ModSystem
    {
        public static bool downedRotGourd = false;
        public static bool downedGhostEvent = false;
        public static bool downedOrroboro = false;

        public static bool SpookyBackgroundAlt = false;
        public static bool CorrodedEgg = false;

        public override void SaveWorldData(TagCompound tag)
        {
            if (downedRotGourd) tag["downedRotGourd"] = true;
            if (downedGhostEvent) tag["downedGhostEvent"] = true;
            if (downedOrroboro) tag["downedOrroboro"] = true;

            if (SpookyBackgroundAlt) tag["SpookyBackgroundAlt"] = true;
            if (CorrodedEgg) tag["CorrodedEgg"] = true;
        }

        public override void LoadWorldData(TagCompound tag) 
        {
			downedRotGourd = tag.ContainsKey("downedRotGourd");
            downedGhostEvent = tag.ContainsKey("downedGhostEvent");
            downedOrroboro = tag.ContainsKey("downedOrroboro");

            SpookyBackgroundAlt = tag.ContainsKey("SpookyBackgroundAlt");
            CorrodedEgg = tag.ContainsKey("CorrodedEgg");
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
            miscFlags[1] = CorrodedEgg;
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
            CorrodedEgg = miscFlags[1];
        }
    }
}
