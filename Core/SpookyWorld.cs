using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.PandoraBox;
using Spooky.Content.UI;

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

        //check to make sure the player isnt in a subworld so that daffodil, pandora's box, and big bone's flower pot are not spawned in subworlds
        public bool IsInSubworld()
        {
            if (Spooky.Instance.subworldLibrary == null)
            {
                return false;
            }

            foreach (Mod mod in ModLoader.Mods)
            {
                if (mod.Name.Equals(Spooky.Instance.subworldLibrary.Name))
                {
                    continue;
                }

                bool anySubworldForMod = (Spooky.Instance.subworldLibrary.Call("AnyActive", mod) as bool?) ?? false;

                if (anySubworldForMod)
                {
                    return true;
                }
            }

            return false;
        }

        public override void PostUpdateTime()
        {
            if (!IsInSubworld())
            {
                //spawn daffodil if she despawns for any reason
                if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilBody>()))
                {
                    int Daffodil = NPC.NewNPC(null, (int)Flags.DaffodilPosition.X, (int)Flags.DaffodilPosition.Y, ModContent.NPCType<DaffodilBody>());
                    Main.npc[Daffodil].position.X -= 3;

                    NetMessage.SendData(MessageID.SyncNPC, number: Daffodil);
                }

                //spawn pandoras box if it despawns for any reason
                if (!NPC.AnyNPCs(ModContent.NPCType<PandoraBox>()))
                {
                    int PandoraBox = NPC.NewNPC(null, (int)Flags.PandoraPosition.X, (int)Flags.PandoraPosition.Y, ModContent.NPCType<PandoraBox>());
                    Main.npc[PandoraBox].position.X += 7;

                    NetMessage.SendData(MessageID.SyncNPC, number: PandoraBox);
                }

                //spawn big bone pot if it despawns for any reason
                if (!NPC.AnyNPCs(ModContent.NPCType<BigFlowerPot>()))
                {
                    int FlowerPot = NPC.NewNPC(null, (int)Flags.FlowerPotPosition.X, (int)Flags.FlowerPotPosition.Y, ModContent.NPCType<BigFlowerPot>());
                    Main.npc[FlowerPot].position.X -= 6;

                    NetMessage.SendData(MessageID.SyncNPC, number: FlowerPot);
                }

                //spawn giant cobweb in the spider cave, only if you have not assembled the old hunter npc
                if (!NPC.AnyNPCs(ModContent.NPCType<GiantWeb>()) && !NPC.AnyNPCs(ModContent.NPCType<GiantWebAnimationBase>()) && !Flags.OldHunterAssembled)
                {
                    int GiantWeb = NPC.NewNPC(null, (int)Flags.SpiderWebPosition.X, (int)Flags.SpiderWebPosition.Y, ModContent.NPCType<GiantWeb>());
                    Main.npc[GiantWeb].position.X += 18;
                    Main.npc[GiantWeb].position.Y += 1518;

                    NetMessage.SendData(MessageID.SyncNPC, number: GiantWeb);
                }
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

            //reset little eye quest
            if (DaySwitched)
            {
                LittleEye.ChosenQuestForToday = Main.rand.Next(5);
                Flags.DailyQuest = false;
            }

            //chance to activate raveyard each night
            if (DaySwitched && !Main.dayTime && Main.rand.NextBool(15))
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

                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.RaveyardEnd");

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(text, 171, 64, 255);
                }
                if (Main.netMode == NetmodeID.Server)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                }
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Moco Nose UI", () =>
                {
                    MocoNoseBar.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                }, InterfaceScaleType.None));
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

            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<CemeteryBiome>()))
            {
                float Intensity = ModContent.GetInstance<TileCount>().cemeteryTiles / 200f;
                Intensity = Math.Min(Intensity, 1f);
                int sunR = backgroundColor.R;
                int sunG = backgroundColor.G;
                int sunB = backgroundColor.B;
                sunR -= (int)(60f * Intensity * (backgroundColor.R / 255f));
                sunG -= (int)(20f * Intensity * (backgroundColor.G / 255f));
                sunB -= (int)(60f * Intensity * (backgroundColor.B / 255f));
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