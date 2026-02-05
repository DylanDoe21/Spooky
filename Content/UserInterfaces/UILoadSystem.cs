using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using System.Collections.Generic;

namespace Spooky.Content.UserInterfaces
{
	[Autoload(Side = ModSide.Client)]
	public class UILoadSystem : ModSystem
	{
		public UserInterface TextLayer;
		public DialogueUI TextBox;

		public override void Load()
		{
			TextLayer = new UserInterface();
			TextBox = new DialogueUI();
			TextLayer.SetState(TextBox);
		}

		public override void ClearWorld()
		{
			if (!Main.dedServ || !DialogueUI.Visible)
			{
				DialogueUI.Clear();
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int resourceBarIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Resource Bars");
            if (resourceBarIndex != -1)
            {
				//snotty schnoz UI
                layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer("Snotty Schnoz UI", () =>
                {
                    MocoNoseBar.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));
            
				//stoned kidney UI
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer("Stoned Kidney UI", () =>
				{
					StonedKidneyBar.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));

				//krampus chimney UI
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer("Backpack Chimney UI", () =>
				{
					KrampusChimneyBar.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));
			}

			int mouseTextIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (mouseTextIndex != -1)
            {
				//rotten depths lab UI
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Rotten Depths Lab Email UI", () =>
				{
					RottenDepthsEmailUI.Draw(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.None));

				//little eye bounty UI
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Little Eye Bounty UI", () =>
                {
                    LittleEyeQuestUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));

				//little eye dialogue UI
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Little Eye Dialogue Choice UI", () =>
                {
                    LittleEyeDialogueChoiceUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));

				//old hunter dialogue UI
				layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer("Old Hunter Dialogue Choice UI", () =>
                {
                    OldHunterDialogueChoiceUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));
			}

            int inGameOptionsIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Ingame Options");
            if (inGameOptionsIndex != -1)
            {
				//bloom buff UI
                layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("Bloom Buffs UI", () =>
                {
					BloomBuffUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));

				//new years resolution UI
				layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("New Years Resolution UI", () =>
                {
					KrampusResolutionUI.Draw(Main.spriteBatch);
                    return true;
                },
                InterfaceScaleType.None));

				//dialogue UI
				layers.Insert(inGameOptionsIndex, new LegacyGameInterfaceLayer("Dialogue UI",
				delegate
				{
					if (DialogueUI.Visible)
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