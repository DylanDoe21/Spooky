using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Spooky.Core;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Backgrounds.SpookyHell;
using Spooky.Content.NPCs.Boss.BigBone;
using Spooky.Content.NPCs.Boss.Daffodil;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.Cemetery;
using Spooky.Content.NPCs.Friendly;
using Spooky.Content.NPCs.PandoraBox;
using Spooky.Content.UserInterfaces;

namespace Spooky
{
	public class Spooky : Mod
	{
        internal static Spooky Instance;
        
        internal Mod subworldLibrary = null;

        public static int MistGhostSpawnX;
        public static int MistGhostSpawnY;

        public static int SpookySpiritSpawnX;
        public static int SpookySpiritSpawnY;

        public static int DaffodilSpawnX;
        public static int DaffodilSpawnY;
        public static int DaffodilParent;

        public static int PandoraBoxX;
        public static int PandoraBoxY;

        public static int OrroboroSpawnX;
        public static int OrroboroSpawnY;

        public static int GiantWebX;
        public static int GiantWebY;

        public static Effect vignetteEffect;
        public static Vignette vignetteShader;

        public static ModKeybind AccessoryHotkey { get; private set; }
        public static ModKeybind ArmorBonusHotkey { get; private set; }

        public override void Load()
        {
            Instance = this;
            
            ModLoader.TryGetMod("SubworldLibrary", out subworldLibrary);

            AccessoryHotkey = KeybindLoader.RegisterKeybind(this, "AccessoryHotkey", "E");
            ArmorBonusHotkey = KeybindLoader.RegisterKeybind(this, "ArmorBonusHotkey", "F");

            if (!Main.dedServ)
            {
                Filters.Scene["Spooky:CemeterySky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 135f, 35f).UseOpacity(0.001f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:CemeterySky"] = new CemeterySky();

                Filters.Scene["Spooky:RaveyardSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:RaveyardSky"] = new RaveyardSky();

                Filters.Scene["Spooky:SpookyForestTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(255f, 116f, 23f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:EyeValleyTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(112f, 11f, 176f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:HallucinationEffect"] = new Filter(new SpookyScreenShader("FilterMoonLordShake").UseIntensity(0.5f), EffectPriority.VeryHigh);
            }

            if (Main.netMode != NetmodeID.Server)
			{
				vignetteEffect = ModContent.Request<Effect>("Spooky/Effects/Vignette", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				vignetteShader = new Vignette(vignetteEffect, "MainPS");
				Filters.Scene["Spooky:Vignette"] = new Filter(vignetteShader, (EffectPriority)100);
            }

            SpiderCaveBG.Load();
            SpookyHellBG.Load();

            ShaderLoader.Load();
        }

        public override void Unload()
        {
            subworldLibrary = null;

            AccessoryHotkey = null;
            ArmorBonusHotkey = null;

            ShaderLoader.Unload();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SpookyMessageType messageType = (SpookyMessageType)reader.ReadByte();
			switch (messageType)
			{
                case SpookyMessageType.SpawnMistGhost:
                {
                    NPC.NewNPC(null, MistGhostSpawnX, MistGhostSpawnY, ModContent.NPCType<MistGhost>());
                    break;
                }
                case SpookyMessageType.SpawnSpookySpirit:
                {
                    NPC.NewNPC(null, SpookySpiritSpawnX, SpookySpiritSpawnY, ModContent.NPCType<SpookySpirit>());
                    break;
                }
                case SpookyMessageType.SpawnMoco:
                {
                    int Type = ModContent.NPCType<Moco>();
                    NPC.SpawnOnPlayer(whoAmI, Type);
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: whoAmI, number2: Type);
					break;
                }
                case SpookyMessageType.SpawnOrroboro:
                {
                    NPC.NewNPC(null, OrroboroSpawnX, OrroboroSpawnY, ModContent.NPCType<OrroHeadP1>());
                    break;
                }
                case SpookyMessageType.SpawnDaffodilEye:
                {
                    NPC.NewNPC(null, DaffodilSpawnX, DaffodilSpawnY, ModContent.NPCType<DaffodilEye>(), ai0: (Flags.downedDaffodil && Main.rand.NextBool(20)) ? -4 : -1, ai1: DaffodilParent);
                    break;
                }
                case SpookyMessageType.SpawnBobbert:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Bobbert>());
                    Main.npc[NewNPC].velocity.X = Main.rand.Next(-10, 11);
                    Main.npc[NewNPC].velocity.Y = Main.rand.Next(-10, -5);
                    break;
                }
                case SpookyMessageType.SpawnStitch:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Stitch>());
                    Main.npc[NewNPC].velocity.X = Main.rand.Next(-10, 11);
                    Main.npc[NewNPC].velocity.Y = Main.rand.Next(-10, -5);
                    break;
                }
                case SpookyMessageType.SpawnSheldon:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Sheldon>());
                    Main.npc[NewNPC].velocity.X = Main.rand.Next(-10, 11);
                    Main.npc[NewNPC].velocity.Y = Main.rand.Next(-10, -5);
                    break;
                }
                case SpookyMessageType.SpawnChester:
                {
                    int NewNPC = NPC.NewNPC(null, PandoraBoxX, PandoraBoxY, ModContent.NPCType<Chester>());
                    Main.npc[NewNPC].velocity.Y = -8;
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
                    NetMessage.SendData(MessageID.WorldData);
                    break;
                }
                case SpookyMessageType.SpawnHunterAnimation:
                {
                    NPC.NewNPC(null, GiantWebX, GiantWebY, ModContent.NPCType<GiantWebAnimationBase>());
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
        SpawnMistGhost,
        SpawnSpookySpirit,
        SpawnMoco,
        SpawnOrroboro,
        SpawnDaffodilEye,
        SpawnBobbert,
        SpawnStitch,
        SpawnSheldon,
        SpawnChester,
        OldHunterHat,
        OldHunterSkull,
        OldHunterTorso,
        OldHunterLegs,
        OldHunterAssembled,
        SpawnHunterAnimation,
    }
}