using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Spooky.Core;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Backgrounds.SpiderCave;
using Spooky.Content.Backgrounds.SpookyHell;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.EggEvent;

namespace Spooky
{
	public class Spooky : Mod
	{
        internal static Spooky Instance;
        
        internal Mod subworldLibrary = null;

        public static int MocoSpawnX;
        public static int MocoSpawnY;

        public static int DaffodilSpawnX;
        public static int DaffodilSpawnY;
        public static int DaffodilParent;

        public static int OrroboroSpawnX;
        public static int OrroboroSpawnY;

        public static int GiantWebX;
        public static int GiantWebY;

        public static Effect vignetteEffect;
        public static Vignette vignetteShader;

        public static ModKeybind AccessoryHotkey { get; private set; }

        internal static Spooky mod;

        public Spooky()
		{
			mod = this;
            //MusicSkipsVolumeRemap = true; //disabled for now because it makes music TOO loud
		}

        public override void Load()
        {
            Instance = this;
            
            ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);

            AccessoryHotkey = KeybindLoader.RegisterKeybind(this, "AccessoryHotkey", "E");

            if (Main.netMode != NetmodeID.Server)
			{
                Filters.Scene["Spooky:CemeterySky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 135f, 35f).UseOpacity(0.001f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:CemeterySky"] = new CemeterySky();

                Filters.Scene["Spooky:RaveyardSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:RaveyardSky"] = new RaveyardSky();

                Filters.Scene["Spooky:SpookyForestTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(255f, 116f, 23f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:HallucinationEffect"] = new Filter(new SpookyScreenShader("FilterMoonLordShake").UseIntensity(0.5f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:SpookFishron"] = new Filter(new FishronScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:SpookFishron"] = new FishronSky();

				vignetteEffect = ModContent.Request<Effect>("Spooky/Effects/Vignette", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				vignetteShader = new Vignette(vignetteEffect, "MainPS");
				Filters.Scene["Spooky:Vignette"] = new Filter(vignetteShader, (EffectPriority)100);
            }

            SpiderCaveBG.Load();
            SpookyHellBG.Load();
        }

        public override void Unload()
        {
            subworldLibrary = null;

            AccessoryHotkey = null;

			mod = null;
		}

        public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SpookyMessageType messageType = (SpookyMessageType)reader.ReadByte();
			switch (messageType)
			{
                case SpookyMessageType.SpawnMoco:
                {
                    NPC.NewNPC(null, MocoSpawnX, MocoSpawnY, ModContent.NPCType<MocoSpawner>());
					break;
                }
                case SpookyMessageType.SpawnOrroboro:
                {
                    NPC.NewNPC(null, OrroboroSpawnX, OrroboroSpawnY, ModContent.NPCType<OrroHeadP1>(), ai0: -1);
                    break;
                }
                case SpookyMessageType.SpawnDaffodilEye:
                {
                    NPC.NewNPC(null, DaffodilSpawnX, DaffodilSpawnY, ModContent.NPCType<DaffodilEye>(), ai0: (Flags.downedDaffodil && Main.rand.NextBool(20)) ? -4 : -1, ai1: DaffodilParent);
                    break;
                }
                case SpookyMessageType.OldHunterHat:
                {
                    Flags.OldHunterHat = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterSkull:
                {
                    Flags.OldHunterSkull = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterTorso:
                {
                    Flags.OldHunterTorso = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterLegs:
                {
                    Flags.OldHunterLegs = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.OldHunterAssembled:
                {
                    Flags.OldHunterAssembled = true;
                    Flags.KillWeb = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.EggIncursionStart:
                {
                    EggEventWorld.EventTimeLeftUI = 21600;
                    EggEventWorld.EggEventActive = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey1:
                {
                    Flags.CatacombKey1 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey2:
                {
                    Flags.CatacombKey2 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.CatacombKey3:
                {
                    Flags.CatacombKey3 = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.BountyAccepted:
                {
                    Flags.BountyInProgress = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty1Complete:
                {
                    Flags.LittleEyeBounty1 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty2Complete:
                {
                    Flags.LittleEyeBounty2 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty3Complete:
                {
                    Flags.LittleEyeBounty3 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.Bounty4Complete:
                {
                    Flags.LittleEyeBounty4 = true;
                    Flags.BountyInProgress = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusQuestGiven:
                {
                    Flags.KrampusQuestGiven = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusQuestlineDone:
                {
                    Flags.KrampusQuestlineDone = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusDailyQuestDone:
                {
                    Flags.KrampusDailyQuestDone = true;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.KrampusDailyQuestReset:
                {
                    Flags.KrampusDailyQuest = false;
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
				//should never occur I think?
				default:
                {
					Logger.Warn("Spooky Mod: Unknown Message type: " + messageType);
					break;
                }
			}
		}
    }

    enum SpookyMessageType : byte
    {
        SpawnSpookySpirit,
        SpawnMoco,
        SpawnOrroboro,
        SpawnDaffodilEye,
        OldHunterHat,
        OldHunterSkull,
        OldHunterTorso,
        OldHunterLegs,
        OldHunterAssembled,
        EggIncursionStart,
        CatacombKey1,
        CatacombKey2,
        CatacombKey3,
        BountyAccepted,
        Bounty1Complete,
        Bounty2Complete,
        Bounty3Complete,
        Bounty4Complete,
        KrampusQuestGiven,
        KrampusQuestlineDone,
        KrampusDailyQuestDone,
        KrampusDailyQuestReset
	}
}