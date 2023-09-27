using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.PandoraBox;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public bool initializeHalloween;
        public bool storedHalloween;
        public bool storedHalloweenForToday;

        public static bool RaveyardHappening;

        public static bool DaySwitched;
        private static bool LastTime;

        public override void PostUpdateTime()
        {
            //spawn daffodil if she despawns for any reason
            if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilBody>()))
            {
                int Daffodil = NPC.NewNPC(null, (int)Spooky.DaffodilPosition.X, (int)Spooky.DaffodilPosition.Y, ModContent.NPCType<DaffodilBody>());
                Main.npc[Daffodil].position.Y += Main.npc[Daffodil].height / 2;

                NetMessage.SendData(MessageID.SyncNPC, number: Daffodil);
            }

            //spawn pandoras box if it despawns for any reason
            if (!NPC.AnyNPCs(ModContent.NPCType<PandoraBox>()))
            {
                int PandoraBox = NPC.NewNPC(null, (int)Spooky.PandoraPosition.X, (int)Spooky.PandoraPosition.Y, ModContent.NPCType<PandoraBox>());
                Main.npc[PandoraBox].position.Y += Main.npc[PandoraBox].height / 2;

                NetMessage.SendData(MessageID.SyncNPC, number: PandoraBox);
            }

            //spawn big bone pot if it despawns for any reason
            if (!NPC.AnyNPCs(ModContent.NPCType<BigFlowerPot>()))
            {
                int FlowerPot = NPC.NewNPC(null, (int)Spooky.FlowerPotPosition.X, (int)Spooky.FlowerPotPosition.Y, ModContent.NPCType<BigFlowerPot>());
                Main.npc[FlowerPot].position.Y += Main.npc[FlowerPot].height / 2;

                NetMessage.SendData(MessageID.SyncNPC, number: FlowerPot);
            }

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

            //chance to activate raveyard each night
            if (DaySwitched && !Main.dayTime && Main.rand.NextBool(20))
            {
                RaveyardHappening = true;

                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.RaveyardStart");

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(text, 171, 64, 255);
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                }
            }

            //if a raveyard is happening, end it during the day
            if (Main.dayTime && RaveyardHappening)
            {
                RaveyardHappening = false;
            }
        }

        public override void ModifySunLightColor(ref Color tileColor, ref Color backgroundColor)
        {
            //spooky forest ambient lighting
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