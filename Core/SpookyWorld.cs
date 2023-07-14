using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Friendly;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public bool initializeHalloween;
        public bool storedHalloween;
        public bool storedHalloweenForToday;

        public static bool DaySwitched;
        private static bool LastTime;

        public override void PostUpdateTime()
        {
            //store whatever vanilla halloween is set to before setting it based on the config
            if (!initializeHalloween)
            {
                storedHalloween = Main.halloween;
                storedHalloweenForToday = Main.forceHalloweenForToday;
                initializeHalloween = true;
            }

            //if the halloween config is on, force halloween to be active
            if (ModContent.GetInstance<SpookyConfig>().HalloweenEnabled)
            {
                Main.halloween = true;
            }
            //otherwise set halloween to what it was before
            else
            {
                //set initializeHalloween to false so vanillas stuff is constantly saved
                initializeHalloween = false;
                Main.halloween = storedHalloween;
                Main.forceHalloweenForToday = storedHalloweenForToday;
            }

            //for when day and night switch
            if (Main.dayTime != LastTime)
            {
                DaySwitched = true;
            }
			else
            {
				DaySwitched = false;
            }

			LastTime = Main.dayTime;

            //reset little eye quest
            if (DaySwitched)
            {
                LittleEye.ChosenQuestForToday = Main.rand.Next(5);
                Flags.DailyQuest = false;
            }
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyBiome>()))
            {
                float Intensity = ModContent.GetInstance<TileCount>().spookyTiles / 200f;
                Intensity = Math.Min(Intensity, 1f);
                int sunR = backgroundColor.R;
                int sunG = backgroundColor.G;
                int sunB = backgroundColor.B;
                sunR -= (int)(20f * Intensity * (backgroundColor.R / 255f));
                sunG -= (int)(60f * Intensity * (backgroundColor.G / 255f));
                sunB -= (int)(100f * Intensity * (backgroundColor.B / 255f));
                sunR = Utils.Clamp(sunR, 15, 255);
                sunG = Utils.Clamp(sunG, 15, 255);
                sunB = Utils.Clamp(sunB, 15, 255);
                backgroundColor.R = (byte)sunR;
                backgroundColor.G = (byte)sunG;
                backgroundColor.B = (byte)sunB;

                return;
            }

            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<CemeteryBiome>()) && !Main.LocalPlayer.InModBiome(ModContent.GetInstance<RaveyardBiome>()))
            {
                float Intensity = ModContent.GetInstance<TileCount>().cemeteryTiles;
                Intensity = Math.Min(Intensity, 1f);
                int sunR = backgroundColor.R;
                int sunG = backgroundColor.G;
                int sunB = backgroundColor.B;
                sunR -= (int)(100f * Intensity * (backgroundColor.R / 255f));
                sunG -= (int)(50f * Intensity * (backgroundColor.G / 255f));
                sunB -= (int)(100f * Intensity * (backgroundColor.B / 255f));
                sunR = Utils.Clamp(sunR, 15, 255);
                sunG = Utils.Clamp(sunG, 15, 255);
                sunB = Utils.Clamp(sunB, 15, 255);
                backgroundColor.R = (byte)sunR;
                backgroundColor.G = (byte)sunG;
                backgroundColor.B = (byte)sunB;

                int tileR = tileColor.R;
                int tileG = tileColor.G;
                int tileB = tileColor.B;
                tileR -= (int)(100f * Intensity * (tileColor.R / 255f));
                tileG -= (int)(50f * Intensity * (tileColor.G / 255f));
                tileB -= (int)(100f * Intensity * (tileColor.B / 255f));
                tileR = Utils.Clamp(tileR, 15, 255);
                tileG = Utils.Clamp(tileG, 15, 255);
                tileB = Utils.Clamp(tileB, 15, 255);
                tileColor.R = (byte)tileR;
                tileColor.G = (byte)tileG;
                tileColor.B = (byte)tileB;

                return;
            }

            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<RaveyardBiome>()))
            {
                float Intensity = ModContent.GetInstance<TileCount>().raveyardTiles;
                Intensity = Math.Min(Intensity, 1f);
                int sunR = backgroundColor.R;
                int sunG = backgroundColor.G;
                int sunB = backgroundColor.B;
                sunR -= (int)(240f * Intensity * (backgroundColor.R / 255f));
                sunG -= (int)(240f * Intensity * (backgroundColor.G / 255f));
                sunB -= (int)(240f * Intensity * (backgroundColor.B / 255f));
                sunR = Utils.Clamp(sunR, 15, 255);
                sunG = Utils.Clamp(sunG, 15, 255);
                sunB = Utils.Clamp(sunB, 15, 255);
                backgroundColor.R = (byte)sunR;
                backgroundColor.G = (byte)sunG;
                backgroundColor.B = (byte)sunB;

                int tileR = tileColor.R;
                int tileG = tileColor.G;
                int tileB = tileColor.B;
                tileR -= (int)(100f * Intensity * (tileColor.R / 255f));
                tileG -= (int)(100f * Intensity * (tileColor.G / 255f));
                tileB -= (int)(100f * Intensity * (tileColor.B / 255f));
                tileR = Utils.Clamp(tileR, 15, 255);
                tileG = Utils.Clamp(tileG, 15, 255);
                tileB = Utils.Clamp(tileB, 15, 255);
                tileColor.R = (byte)tileR;
                tileColor.G = (byte)tileG;
                tileColor.B = (byte)tileB;

                return;
            }
        }
    }
}