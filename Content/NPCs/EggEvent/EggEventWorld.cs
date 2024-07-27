using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Biomes;

namespace Spooky.Content.NPCs.EggEvent
{
	public class EggEventWorld : ModSystem
	{
		public static int EventTimeLeft = 0;
		public static int EventTimeLeftUI = 0;
		public static bool EggEventActive;

		public override void OnWorldLoad()
		{
			EventTimeLeft = 0;
			EventTimeLeftUI = 0;
			EggEventActive = false;
		}

		public bool AnyPlayersInBiome()
		{
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];

				int playerInBiomeCount = 0;

				if (player.active && !player.dead && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
				{
					playerInBiomeCount++;
				}

				if (playerInBiomeCount >= 1)
				{
					return true;
				}
			}

			return false;
		}

		public override void PostUpdateEverything()
		{
			//end the event and reset everything if you die, or if you leave the valley of eyes
			if (!AnyPlayersInBiome())
			{
				EventTimeLeft = 0;
				EventTimeLeftUI = 0;
				EggEventActive = false;
			}

			if (!EggEventActive)
			{
				EventTimeLeft = 0;
				EventTimeLeftUI = 0;
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (EggEventActive && Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				int EventIndex = layers.FindIndex(layer => layer is not null && layer.Name.Equals("Vanilla: Inventory"));
				LegacyGameInterfaceLayer NewLayer = new LegacyGameInterfaceLayer("Spooky: Egg Event UI",
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
			if (EggEventActive && Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				const float Scale = 0.875f;
				const float Alpha = 0.65f;
				const int InternalOffset = 6;
				const int OffsetX = 20;
				const int OffsetY = 20;

				Texture2D EventIcon = ModContent.Request<Texture2D>("Spooky/Content/Items/BossSummon/StrangeCyst", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Color NameBoxColor = new Color(113, 35, 206);
				Color ProgressBoxColor = new Color(113, 35, 206);
				Color ProgressBarColor1 = new Color(54, 35, 35);
				Color ProgressBarColor2 = new Color(199, 7, 49);

				int width = (int)(200f * Scale);
				int height = (int)(46f * Scale);

				Rectangle ProgressBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
				Utils.DrawInvBG(spriteBatch, ProgressBackground, ProgressBoxColor * Alpha);

				float divide = 3.6f;

				float timeLeft = EventTimeLeft / 60;
				float timeLeftUI = EventTimeLeftUI / 60;

				TimeSpan time = TimeSpan.FromSeconds(timeLeftUI);
				string actualTime = string.Format("{0:D1}:{1:D2}", time.Minutes, time.Seconds);

				string ProgressText = Language.GetTextValue("Mods.Spooky.UI.EggEvent.EggEventBarProgress") + " " + actualTime;

				Utils.DrawBorderString(spriteBatch, ProgressText, new Vector2(ProgressBackground.Center.X, ProgressBackground.Y), Color.White, Scale, 0.5f, -0.1f);
				Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + ProgressBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

				var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp((timeLeft / divide), 0f, 100f)), TextureAssets.ColorBar.Height());
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
				Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.Spooky.UI.EggEvent.EggEventBarDisplayName"), new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
			}
		}
	}
}