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
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.NoseCult;
using Spooky.Content.NPCs.PandoraBox;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public bool initializeHalloween;
        public bool storedHalloween;
        public bool storedHalloweenForToday;

        public static bool DaySwitched;
        public static bool LastTime;

		//check to make sure the player isnt in a subworld so that all of the npcs meant to be saved in specific locations are not spawned in subworlds
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
                //spawn daffodil
                if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilBody>()) && Flags.DaffodilPosition != Vector2.Zero)
                {
                    int Daffodil = NPC.NewNPC(null, (int)Flags.DaffodilPosition.X, (int)Flags.DaffodilPosition.Y, ModContent.NPCType<DaffodilBody>());
                    Main.npc[Daffodil].position.X -= 9;
                    Main.npc[Daffodil].position.Y += 10;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: Daffodil);
                    }
                }

                //spawn pandoras box
                if (!NPC.AnyNPCs(ModContent.NPCType<PandoraBox>()) && Flags.PandoraPosition != Vector2.Zero)
                {
                    int PandoraBox = NPC.NewNPC(null, (int)Flags.PandoraPosition.X, (int)Flags.PandoraPosition.Y, ModContent.NPCType<PandoraBox>());
                    Main.npc[PandoraBox].position.X -= 8;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: PandoraBox);
                    }
                }

                //spawn big bone pot
                if (!NPC.AnyNPCs(ModContent.NPCType<BigFlowerPot>()) && Flags.FlowerPotPosition != Vector2.Zero)
                {
                    int FlowerPot = NPC.NewNPC(null, (int)Flags.FlowerPotPosition.X, (int)Flags.FlowerPotPosition.Y, ModContent.NPCType<BigFlowerPot>());
                    Main.npc[FlowerPot].position.X -= 6;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: FlowerPot);
                    }
                }

                //spawn giant cobweb, only if you have not assembled the old hunter npc
                if (!NPC.AnyNPCs(ModContent.NPCType<GiantWeb>()) && !NPC.AnyNPCs(ModContent.NPCType<GiantWebAnimationBase>()) && Flags.SpiderWebPosition != Vector2.Zero && !Flags.OldHunterAssembled)
                {
                    int GiantWeb = NPC.NewNPC(null, (int)Flags.SpiderWebPosition.X, (int)Flags.SpiderWebPosition.Y, ModContent.NPCType<GiantWeb>());
                    Main.npc[GiantWeb].position.X += 18;
                    Main.npc[GiantWeb].position.Y += 1518;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: GiantWeb);
                    }
                }

                //spawn giant egg
                if (!NPC.AnyNPCs(ModContent.NPCType<OrroboroEgg>()) && Flags.EggPosition != Vector2.Zero)
                {
                    int Egg = NPC.NewNPC(null, (int)Flags.EggPosition.X, (int)Flags.EggPosition.Y, ModContent.NPCType<OrroboroEgg>());
                    Main.npc[Egg].position.X += 2;
                }

                //spawn moco idols in each ambush room
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol1>()) && Flags.MocoIdolPosition1 != Vector2.Zero && !Flags.downedMocoIdol1)
                {
                    int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition1.X, (int)Flags.MocoIdolPosition1.Y, ModContent.NPCType<MocoIdol1>());
                    Main.npc[Idol].position.X += 8;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol2>()) && Flags.MocoIdolPosition2 != Vector2.Zero && !Flags.downedMocoIdol2)
                {
                    int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition2.X, (int)Flags.MocoIdolPosition2.Y, ModContent.NPCType<MocoIdol2>());
                    Main.npc[Idol].position.X += 8;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol3>()) && Flags.MocoIdolPosition3 != Vector2.Zero && !Flags.downedMocoIdol3)
                {
                    int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition3.X, (int)Flags.MocoIdolPosition3.Y, ModContent.NPCType<MocoIdol3>());
                    Main.npc[Idol].position.X += 8;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol4>()) && Flags.MocoIdolPosition4 != Vector2.Zero && !Flags.downedMocoIdol4)
                {
                    int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition4.X, (int)Flags.MocoIdolPosition4.Y, ModContent.NPCType<MocoIdol4>());
                    Main.npc[Idol].position.X += 8;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol5>()) && Flags.MocoIdolPosition5 != Vector2.Zero && !Flags.downedMocoIdol5)
                {
                    int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition5.X, (int)Flags.MocoIdolPosition5.Y, ModContent.NPCType<MocoIdol5>());
                    Main.npc[Idol].position.X += 8;
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol6>()) && Flags.LeaderIdolPositon != Vector2.Zero && !Flags.downedMocoIdol6)
                {
                    int Idol = NPC.NewNPC(null, (int)Flags.LeaderIdolPositon.X, (int)Flags.LeaderIdolPositon.Y, ModContent.NPCType<MocoIdol6>());
                    Main.npc[Idol].position.X += 8;
                }

                //chance to activate raveyard each night
                if (DaySwitched && !Main.dayTime && Main.rand.NextBool(15))
                {
                    Flags.RaveyardHappening = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }

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
                if (Main.dayTime && Flags.RaveyardHappening)
                {
                    Flags.RaveyardHappening = false;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.WorldData);
                    }

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

            //store whatever vanilla halloween is set to before setting it based on the config
            if (!initializeHalloween)
            {
                storedHalloween = Main.halloween;
                storedHalloweenForToday = Main.forceHalloweenForToday;
                initializeHalloween = true;
            }

            //if the halloween config is on, force halloween to be active
            if (ModContent.GetInstance<SpookyServerConfig>().HalloweenEnabled)
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