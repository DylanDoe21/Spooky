using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Backgrounds;
using Spooky.Content.Backgrounds.SpookyBiome;

namespace Spooky
{
	public partial class Spooky : Mod
	{
        private List<IAutoload> loadCache;

        public override void Load()
        { 
            if (!Main.dedServ)
            {
                Filters.Scene["Spooky:HalloweenSky"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(Color.DarkOrange).UseOpacity(0f), EffectPriority.High);
				SkyManager.Instance["Spooky:HalloweenSky"] = new HalloweenSky();

                Filters.Scene["Spooky:SpookyHellTint"] = new Filter(new SpookyScreenShader("FilterMiniTower").UseColor(Color.BlueViolet).UseOpacity(0.3f), EffectPriority.VeryHigh);
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

        public enum SpookyMessageType : byte
        {
            SpawnOrroboro,
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            SpookyMessageType messageType = (SpookyMessageType)reader.ReadByte();
            switch (messageType)
            {
                case SpookyMessageType.SpawnOrroboro:
                {
                    NPC.SpawnOnPlayer(whoAmI, ModContent.NPCType<Content.NPCs.Boss.Orroboro.OrroboroHead>());
                    break;
                }
                default:
                {
                    Logger.Warn("SpookyMod: Unknown Message type: " + messageType);
                    break;
                }
            }
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
    }
}