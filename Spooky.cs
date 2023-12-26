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
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.Cemetery;
using Spooky.Content.UI;

namespace Spooky
{
	public class Spooky : Mod
	{
        internal static Spooky Instance;
        
        internal Mod subworldLibrary = null;

        public static Vector2 DaffodilPosition;
        public static Vector2 PandoraPosition;
        public static Vector2 FlowerPotPosition;

        public static int MistGhostSpawnX;
        public static int MistGhostSpawnY;

        public static int SpookySpiritSpawnX;
        public static int SpookySpiritSpawnY;

        public static int OrroboroSpawnX;
        public static int OrroboroSpawnY;

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

            MocoNoseBar.Load();
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
                    int type = ModContent.NPCType<Moco>();
                    NPC.SpawnOnPlayer(whoAmI, type);
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: whoAmI, number2: type);
					break;
                }
                case SpookyMessageType.SpawnOrroboro:
                {
                    NPC.NewNPC(null, OrroboroSpawnX, OrroboroSpawnY, ModContent.NPCType<OrroHeadP1>());
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
    }
}