using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Biomes;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public override void PostUpdateWorld()
        {
            if (ModContent.GetInstance<SpookyConfig>().HalloweenEnabled)
            {
                Main.halloween = true;
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
        }
    }
}