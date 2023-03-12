using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Biomes;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public static bool initializeHalloween;
        public bool storedHalloween;
        public bool storedHalloweenForToday;

        public override void PostUpdateTime()
        {
            //store whatever vanilla halloween is set to before setting it based on the config
            if (!initializeHalloween)
            {
                storedHalloween = Main.halloween;
                storedHalloweenForToday = Main.forceHalloweenForToday;
                initializeHalloween = true;
            }

            //if config is on, force halloween to be active
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

                return;
            }
        }
    }
}