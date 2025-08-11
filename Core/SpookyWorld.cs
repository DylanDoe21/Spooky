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

using Spooky.Content.NPCs.Minibiomes.Ocean;

namespace Spooky.Core
{
    public class SpookyWorld : ModSystem
    {
        public static float BGTransitionFlash;

        public bool initializeHalloween;
        public bool storedHalloween;
        public bool storedHalloweenForToday;

        public bool KrampusDailyQuest = true;

        public static bool DaySwitched;
        public static bool LastTime;

		//check to make sure the player isnt in a subworld so that all of the npcs meant to be saved and spawned in specific world locations are not spawned in subworlds
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

		public bool AnyPlayersInZombieOceanBiome()
		{
			foreach (Player player in Main.ActivePlayers)
			{
				int playerInBiomeCount = 0;
				if (!player.dead && player.InModBiome(ModContent.GetInstance<ZombieOceanBiome>()))
				{
					playerInBiomeCount++;
				}

				if (playerInBiomeCount >= 1)
				{
					return true;
				}
			}

			return false;
		}

		public override void PostUpdateEverything()
        {
            BGTransitionFlash = MathHelper.Clamp(BGTransitionFlash - 0.05f, 0f, 1f);

            if (!IsInSubworld())
            {
				//spawn big dunk, only if a player is in the biome
                //even though big dunks pathfinding doesnt really affect preformance, still not a good idea to have it running in the background constantly
				if (!NPC.AnyNPCs(ModContent.NPCType<Dunkleosteus>()) && AnyPlayersInZombieOceanBiome() && !Flags.downedDunkleosteus)
				{
					int Count = Flags.ZombieBiomePositions.Count - 1;
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int BigDunk = NPC.NewNPC(null, (int)Flags.ZombieBiomePositions[Count].X * 16, (int)Flags.ZombieBiomePositions[Count].Y * 16, ModContent.NPCType<Dunkleosteus>());
						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: BigDunk);
						}
					}
				}

				//spawn daffodil
				if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilBody>()) && Flags.DaffodilPosition != Vector2.Zero)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
                        int Daffodil = NPC.NewNPC(null, (int)Flags.DaffodilPosition.X, (int)Flags.DaffodilPosition.Y, ModContent.NPCType<DaffodilBody>());
                        Main.npc[Daffodil].position.X -= 9;
                        Main.npc[Daffodil].position.Y += 10;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Daffodil);
                        }
                    }
                }
                //spawn daffodil background handler
                if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilArenaBG>()) && Flags.DaffodilPosition != Vector2.Zero)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
                        int DaffodilBG = NPC.NewNPC(null, (int)Flags.DaffodilPosition.X, (int)Flags.DaffodilPosition.Y, ModContent.NPCType<DaffodilArenaBG>());
                        Main.npc[DaffodilBG].position.X -= 8;
                        Main.npc[DaffodilBG].position.Y += 25;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: DaffodilBG);
                        }
                    }
                }

                //spawn pandoras box
                if (!NPC.AnyNPCs(ModContent.NPCType<PandoraBox>()) && Flags.PandoraPosition != Vector2.Zero)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
                        int PandoraBox = NPC.NewNPC(null, (int)Flags.PandoraPosition.X, (int)Flags.PandoraPosition.Y, ModContent.NPCType<PandoraBox>());
                        Main.npc[PandoraBox].position.X -= 8;
                        Main.npc[PandoraBox].position.Y += 235;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: PandoraBox);
                        }
                    }
                }

                //spawn big bone pot
                if (!NPC.AnyNPCs(ModContent.NPCType<BigFlowerPot>()) && Flags.FlowerPotPosition != Vector2.Zero)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
                        int FlowerPot = NPC.NewNPC(null, (int)Flags.FlowerPotPosition.X, (int)Flags.FlowerPotPosition.Y, ModContent.NPCType<BigFlowerPot>());
                        Main.npc[FlowerPot].position.X -= 6;
                        Main.npc[FlowerPot].position.Y += 523;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: FlowerPot);
                        }
                    }
                }
                //spawn big bone background handler
                if (!NPC.AnyNPCs(ModContent.NPCType<BigBoneArenaBG>()) && Flags.FlowerPotPosition != Vector2.Zero)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
                        int BigBoneBG = NPC.NewNPC(null, (int)Flags.FlowerPotPosition.X, (int)Flags.FlowerPotPosition.Y, ModContent.NPCType<BigBoneArenaBG>());
                        Main.npc[BigBoneBG].position.X -= 15;
                        Main.npc[BigBoneBG].position.Y += 22;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: BigBoneBG);
                        }
                    }
                }

                //spawn giant cobweb, only if you have not assembled the old hunter npc
                if (!NPC.AnyNPCs(ModContent.NPCType<GiantWeb>()) && !NPC.AnyNPCs(ModContent.NPCType<GiantWebAnimationBase>()) && Flags.SpiderWebPosition != Vector2.Zero && !Flags.OldHunterAssembled)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
					{
                        int GiantWeb = NPC.NewNPC(null, (int)Flags.SpiderWebPosition.X, (int)Flags.SpiderWebPosition.Y, ModContent.NPCType<GiantWeb>());
                        Main.npc[GiantWeb].position.X += 18;
                        Main.npc[GiantWeb].position.Y += 670;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: GiantWeb);
                        }
                    }
                }

                //spawn giant egg
                if (!NPC.AnyNPCs(ModContent.NPCType<OrroboroEgg>()) && Flags.EggPosition != Vector2.Zero)
                {
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int Egg = NPC.NewNPC(null, (int)Flags.EggPosition.X, (int)Flags.EggPosition.Y, ModContent.NPCType<OrroboroEgg>());
						Main.npc[Egg].position.X += 1;
						Main.npc[Egg].position.Y -= 32;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Egg);
                        }
					}
                }

                //spawn krampus
                if (!NPC.AnyNPCs(ModContent.NPCType<Krampus>()) && Flags.KrampusPosition != Vector2.Zero)
                {
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int Krampus = NPC.NewNPC(null, (int)Flags.KrampusPosition.X, (int)Flags.KrampusPosition.Y, ModContent.NPCType<Krampus>());

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Krampus);
                        }
					}
                }
                
                //spawn moco idols in each ambush room
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol1>()) && Flags.MocoIdolPosition1 != Vector2.Zero && !Flags.downedMocoIdol1)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition1.X, (int)Flags.MocoIdolPosition1.Y, ModContent.NPCType<MocoIdol1>());
                        Main.npc[Idol].position.X += 8;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Idol);
                        }
                    }
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol2>()) && Flags.MocoIdolPosition2 != Vector2.Zero && !Flags.downedMocoIdol2)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition2.X, (int)Flags.MocoIdolPosition2.Y, ModContent.NPCType<MocoIdol2>());
                        Main.npc[Idol].position.X += 8;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Idol);
                        }
                    }
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol3>()) && Flags.MocoIdolPosition3 != Vector2.Zero && !Flags.downedMocoIdol3)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition3.X, (int)Flags.MocoIdolPosition3.Y, ModContent.NPCType<MocoIdol3>());
                        Main.npc[Idol].position.X += 8;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Idol);
                        }
                    }
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol4>()) && Flags.MocoIdolPosition4 != Vector2.Zero && !Flags.downedMocoIdol4)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition4.X, (int)Flags.MocoIdolPosition4.Y, ModContent.NPCType<MocoIdol4>());
                        Main.npc[Idol].position.X += 8;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Idol);
                        }
                    }
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol5>()) && Flags.MocoIdolPosition5 != Vector2.Zero && !Flags.downedMocoIdol5)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int Idol = NPC.NewNPC(null, (int)Flags.MocoIdolPosition5.X, (int)Flags.MocoIdolPosition5.Y, ModContent.NPCType<MocoIdol5>());
                        Main.npc[Idol].position.X += 8;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Idol);
                        }
                    }
                }
                if (!NPC.AnyNPCs(ModContent.NPCType<MocoIdol6>()) && Flags.LeaderIdolPositon != Vector2.Zero && !Flags.downedMocoIdol6)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int Idol = NPC.NewNPC(null, (int)Flags.LeaderIdolPositon.X, (int)Flags.LeaderIdolPositon.Y, ModContent.NPCType<MocoIdol6>());
                        Main.npc[Idol].position.X += 8;

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendData(MessageID.SyncNPC, number: Idol);
                        }
                    }
                }

                //chance to activate raveyard each night
                if (DaySwitched && !Main.dayTime && (Main.rand.NextBool(15) || Flags.GuaranteedRaveyard))
                {
                    Flags.RaveyardHappening = true;

                    if (Flags.GuaranteedRaveyard)
                    {
                        Flags.GuaranteedRaveyard = false;
                    }

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

            if (DaySwitched && !Flags.KrampusDailyQuest)
            {
                Flags.KrampusDailyQuest = true;

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.WorldData);
                }
            }

            //kill the giant web manually when all pieces are inserted for multiplayer purposes
            //because setting the npcs health to zero in the web itself doesnt work or update in mp correctly, doing it here and spawning the animation allows it to spawn correctly
            if (Flags.KillWeb)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i] != null && Main.npc[i].type == ModContent.NPCType<GiantWeb>())
                    {
                        int Animation = NPC.NewNPC(Main.npc[i].GetSource_FromAI(), (int)Main.npc[i].Center.X, (int)Main.npc[i].Center.Y + 25, ModContent.NPCType<GiantWebAnimationBase>());

                        if (Main.netMode == NetmodeID.Server)
                        {   
                            NetMessage.SendData(MessageID.SyncNPC, number: Animation);
                        }

                        Main.npc[i].life = 0;

                        if (Main.netMode == NetmodeID.Server) 
                        {
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i, 0f, 0f, 0f, 0);
                        }
                    }
                }

                Flags.KillWeb = false;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
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

				Color LightColor = new Color(sunR, sunG, sunB);
                backgroundColor = LightColor;

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

				Color LightColor = new Color(sunR, sunG, sunB);
				backgroundColor = LightColor;

				return;
            }
        }
    }
}