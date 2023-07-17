using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.Cemetery;
using Spooky.Content.Backgrounds.SpookyHell;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.SpookySpirit;

namespace Spooky
{
	public class Spooky : Mod
	{
        public static int SpookySpiritSpawnX;
        public static int SpookySpiritSpawnY;

        private List<IAutoload> loadCache;

        public static Effect vignetteEffect;
        public static Vignette vignetteShader;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Filters.Scene["Spooky:Cemetery"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:Cemetery"] = new CemeterySky();

                Filters.Scene["Spooky:Raveyard"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(75f, 0f, 130f).UseOpacity(0.005f), EffectPriority.VeryHigh);
                SkyManager.Instance["Spooky:Raveyard"] = new RaveyardSky();

                Filters.Scene["Spooky:HalloweenSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(255f, 116f, 23f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:EyeValleyTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(112f, 11f, 176f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:CatacombLayer1Tint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(92f, 124f, 53f).UseOpacity(0.0025f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:CatacombLayer2Tint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(139f, 109f, 61f).UseOpacity(0.0025f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:EntityEffect"] = new Filter(new SpookyScreenShader("FilterMoonLordShake").UseIntensity(0.5f), EffectPriority.VeryHigh);
            }

            if (Main.netMode != NetmodeID.Server)
			{
				vignetteEffect = ModContent.Request<Effect>("Spooky/Effects/Vignette", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				vignetteShader = new Vignette(vignetteEffect, "MainPS");
				Filters.Scene["Spooky:Vignette"] = new Filter(vignetteShader, (EffectPriority)100);
            }

            //hell background loading
            HellBGManager.Load();

            //IAutoload stuff
            loadCache = new List<IAutoload>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IAutoload)))
                {
                    var instance = Activator.CreateInstance(type);
                    loadCache.Add(instance as IAutoload);
                }
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load();
            }
        }

        public override void Unload()
        {
            HellBGManager.Unload();

            if (loadCache != null)
            {
                foreach (var loadable in loadCache)
                {
                    loadable.Unload();
                }
            }

            loadCache = null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SpookyMessageType messageType = (SpookyMessageType)reader.ReadByte();
			switch (messageType)
			{
                case SpookyMessageType.SpawnMoco:
                {
                    NPC.SpawnOnPlayer(whoAmI, ModContent.NPCType<Moco>());
					break;
                }
                case SpookyMessageType.SpawnSpookySpirit:
                {
                    NPC.NewNPC(null, SpookySpiritSpawnX, SpookySpiritSpawnY, ModContent.NPCType<SpookySpirit>());
                    break;
                }
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
        SpawnMoco,
        SpawnSpookySpirit,
    }
}