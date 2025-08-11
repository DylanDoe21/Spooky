using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using System;
using System.Collections.Generic;

namespace Spooky.Content.UserInterfaces
{
	[Autoload(Side = ModSide.Client)]
	public class UILoadSystem : ModSystem
	{
		public UserInterface TextLayer;
		public KrampusDialogueUI TextBox;

		public override void Load()
		{
			TextLayer = new UserInterface();
			TextBox = new KrampusDialogueUI();
			TextLayer.SetState(TextBox);
		}

		public override void ClearWorld()
		{
			if (!Main.dedServ && KrampusDialogueUI.Visible)
				KrampusDialogueUI.Clear();
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            //snotty schnoz UI
            int resourceBarIndex1 = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (resourceBarIndex1 != -1)
            {
                layers.Insert(resourceBarIndex1, new LegacyGameInterfaceLayer("Snotty Schnoz UI", () =>
                {
                    MocoNoseBar.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));
            }

			//stoned kidney UI
			int resourceBarIndex2 = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
			if (resourceBarIndex2 != -1)
			{
				layers.Insert(resourceBarIndex2, new LegacyGameInterfaceLayer("Stoned Kidney UI", () =>
				{
					StonedKidneyBar.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));
			}

			//rotten depths lab UI
			//little eye bounty UI
			int mouseTextIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseTextIndex != -1)
            {
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Rotten Depths Lab Email", () =>
				{
					RottenDepthsEmailUI.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));

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

				layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("Krampus UI",
				delegate
				{
					if (KrampusDialogueUI.Visible)
					{
						TextLayer.Update(Main._drawInterfaceGameTime);
						TextBox.Draw(Main.spriteBatch);
					}
					return true;
				}, InterfaceScaleType.None));
			}
        }
	}
}