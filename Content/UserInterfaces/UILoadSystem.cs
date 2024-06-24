using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections.Generic;

namespace Spooky.Content.UserInterfaces
{
    [Autoload(Side = ModSide.Client)]
	public class UILoadSystem : ModSystem
	{
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //snotty schnoz UI
            int resourceBarIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (resourceBarIndex != -1)
            {
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer("Snotty Schnoz UI", () =>
                {
                    MocoNoseBar.Draw(Main.spriteBatch, Main.LocalPlayer);
                    return true;
                },
                InterfaceScaleType.None));
            }

            //little eye bounty UI
            int mouseTextIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Little Eye Bounty UI", () =>
                {
                    LittleEyeQuestUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));
            }

            //bloom buff UI
            int inGameOptionsIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Ingame Options");
            if (inGameOptionsIndex != -1)
            {
                layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("Bloom Buffs UI", () =>
                {
                    BloomBuffUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));
            }
        }
	}
}