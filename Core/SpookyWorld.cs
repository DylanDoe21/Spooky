using Terraria;
using Terraria.ModLoader;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public static bool GhostEvent = false;
        public static bool ShouldStartGhostEvent = false;
        public static bool DaySwitched;
        private static bool LastTime;

        public static bool NoGoods = false;

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

            if (DaySwitched)
            {
                NoGoods = false;

                /*
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
                */
            }
        }
    }
}