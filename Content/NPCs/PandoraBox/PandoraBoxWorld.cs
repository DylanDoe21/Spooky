using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;

namespace Spooky.Content.NPCs.PandoraBox
{
	public class PandoraBoxWorld : ModSystem
	{
		public static int Wave = 0;
		public static bool PandoraEventActive;
		public static bool SpawnedEnemySpawners;

		public override void OnWorldLoad()
		{
			Wave = 0;
			PandoraEventActive = false;
			SpawnedEnemySpawners = false;
		}

        public override void PostUpdateEverything()
		{
			//end the event and reset everything if you die
			if (PandoraEventActive && Main.player[Main.myPlayer].dead)
			{
				Wave = 0;
				SpawnedEnemySpawners = false;
				PandoraEventActive = false;
			}

			if (!Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<CatacombBiome2>()))
			{
				PandoraEventActive = false;
			}

			//end the event, reset everything, and set it to downed when completed
			if (Wave > 4)
			{
				if (!Flags.downedPandoraBox)
				{
					//event end message
					NPC.SetEventFlagCleared(ref Flags.downedPandoraBox, -1);
				}
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (PandoraEventActive)
			{
				int EventIndex = layers.FindIndex(layer => layer is not null && layer.Name.Equals("Vanilla: Inventory"));
				LegacyGameInterfaceLayer NewLayer = new LegacyGameInterfaceLayer("Spooky: Pandora's Box UI",
				delegate
				{
					DrawEventUI(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.UI);

				layers.Insert(EventIndex, NewLayer);
			}
		}

		public void DrawEventUI(SpriteBatch spriteBatch)
		{
			if (PandoraEventActive)
			{
				const float Scale = 0.875f;
				const float Alpha = 0.5f;
				const int InternalOffset = 6;
				const int OffsetX = 20;
				const int OffsetY = 20;

				Texture2D EventIcon = ModContent.Request<Texture2D>("Spooky/Content/NPCs/PandoraBox/PandoraBoxIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Color NameBoxColor = new Color(39, 45, 90);
				Color ProgressBarColor1 = new Color(87, 60, 42);
				Color ProgressBarColor2 = new Color(35, 165, 161);

				int width = (int)(200f * Scale);
				int height = (int)(46f * Scale);

				Rectangle ProgressBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
				Utils.DrawInvBG(spriteBatch, ProgressBackground, new Color(39, 45, 90, 255) * 0.785f);

				float divide = 0.05f;

				string ProgressText = Language.GetTextValue("Mods.Spooky.EventsAndBosses.PandoraBoxBarProgress") + (Wave + 1);
				Utils.DrawBorderString(spriteBatch, ProgressText, new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + 5), Color.White, Scale, 0.5f, -0.1f);
				Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + ProgressBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

				var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp((Wave + 1) / divide, 0f, 100f)), TextureAssets.ColorBar.Height());
				var offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * Scale)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * Scale)) * 0.5f);
				spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, null, ProgressBarColor1 * Alpha, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, ProgressBarColor2, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);

				Vector2 descSize = new Vector2(175, 40) * Scale;
				Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 19f), new Vector2(width, height));
				Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), descSize * 0.9f);
				Utils.DrawInvBG(spriteBatch, descBackground, NameBoxColor * Alpha);

				int descOffset = (descBackground.Height - (int)(32f * Scale)) / 2;
				var icon = new Rectangle(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * Scale), (int)(32 * Scale));
				spriteBatch.Draw(EventIcon, icon, Color.White);
				Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.Spooky.EventsAndBosses.PandoraBoxBarDisplayName"), new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
			}
		}
	}
}