using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Biomes;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public static bool GhostEvent = false;
        public static bool ShouldStartGhostEvent = false;
        public static bool DaySwitched;
        private static bool LastTime;

        public override void PostUpdateWorld()
        {
            Main.halloween = true;

            if (Main.dayTime != LastTime)
            {
                DaySwitched = true;
            }
            else
            {
                DaySwitched = false;
            }

            LastTime = Main.dayTime;

            /*
            if (DaySwitched)
            {
                if (!Main.dayTime && NPC.downedBoss2)
                {
                    if (Main.rand.Next(12) == 0 || ShouldStartGhostEvent)
                    {
                        Main.NewText("Spooky fog envelopes the world", 109, 97, 179);
                        GhostEvent = true;
                        ShouldStartGhostEvent = false;
                    }
                }

                if (Main.dayTime && GhostEvent)
                {
                    Main.NewText("The spooky fog dissipates for now", 109, 97, 179);

                    if (!Flags.downedGhostEvent)
                    {
                        Flags.downedGhostEvent = true;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.WorldData);
                        }
                    }

                    GhostEvent = false;
                    ShouldStartGhostEvent = false;
                }
            }
            */
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            //use purple sky if shadow biome is mushroom version
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