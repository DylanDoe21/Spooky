using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.SpookyHell;
using Spooky.Content.NPCs.Boss.Moco;
using Spooky.Content.NPCs.Boss.Orroboro;

namespace Spooky
{
	public partial class Spooky : Mod
	{
        private List<IAutoload> loadCache;

        public static Effect vignetteEffect;
        public static Vignette vignetteShader;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                Filters.Scene["Spooky:HalloweenSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(255f, 116f, 23f).UseOpacity(0.001f), EffectPriority.VeryHigh);

                Filters.Scene["Spooky:SpookyHellTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(100f, 12f, 150f).UseOpacity(0.001f), EffectPriority.VeryHigh);
                //Filters.Scene["Spooky:SpookyHellTint"] = new Filter(new SpookyScreenShader("FilterBloodMoon").UseColor(0.2f, -0.2f, 0.35f), EffectPriority.VeryHigh);
            }

            if (Main.netMode != NetmodeID.Server)
			{
				vignetteEffect = ModContent.Request<Effect>("Spooky/Effects/Vignette", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				vignetteShader = new Vignette(vignetteEffect, "MainPS");
				Filters.Scene["Spooky:Vignette"] = new Filter(vignetteShader, (EffectPriority)100);
            }

            //hell background loading
            HellBGManager.Load();

            //IAutoload stuff for hell background
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

        public override void AddRecipeGroups()
        {
            //add recipe group for any mech boss soul for orroboro summon item
            RecipeGroup group = new RecipeGroup(() => "Any Mechanical Boss Soul", new int[]
            {
                ItemID.SoulofSight,
                ItemID.SoulofMight,
                ItemID.SoulofFright
            });
            RecipeGroup.RegisterGroup("SpookyMod:AnyMechBossSoul", group);
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
                case SpookyMessageType.SpawnOrroboro:
                {
                    //for now, just use SpawnOnPlayer until an actual spawn intro fix is made
                    NPC.SpawnOnPlayer(whoAmI, ModContent.NPCType<OrroboroHead>());
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
        SpawnOrroboro,
    }
}