using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Biomes;

namespace Spooky.Content.Events
{
	public class EggEventWorld : ModSystem
	{
		public static bool EggEventActive;
		public static float EggEventProgress = 0;
		public static int EggEventTimer = 0;

		public static readonly SoundStyle EventEndSound = new("Spooky/Content/Sounds/SpookyHell/OrroboroAwaken", SoundType.Sound);

		public override void OnWorldLoad()
		{
			EggEventActive = false;
			EggEventProgress = 0;
			EggEventTimer = 0;
		}

		public static ModPacket CreateProgressPacket()
		{
			ModPacket packet = ModContent.GetInstance<Spooky>().GetPacket();
			packet.Write((byte)SpookyMessageType.EggEventData);
			packet.Write(EggEventActive);
			packet.Write(EggEventProgress);
			packet.Write(EggEventTimer);

			return packet;
		}

		public static void SendInfoPacket()
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				return;
			}

			CreateProgressPacket().Send();
		}

		public static void HandlePacket(BinaryReader reader)
		{
			EggEventActive = reader.ReadBoolean();
			EggEventProgress = reader.ReadInt32();
			EggEventTimer = reader.ReadInt32();

			if (Main.netMode == NetmodeID.Server)
			{
				SendInfoPacket(); // Forward packet to rest of clients
			}
		}

		public override void PreUpdateTime()
		{
			if (EggEventActive && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				EggEventTimer++;

				if (EggEventTimer >= 60)
				{
					EggEventProgress += 1f;
					EggEventTimer = 0;
				}
			}

			if (EggEventProgress >= 300f)
			{
				Main.NewText("The mysterious egg has become fragile...", 255, 55, 41);

				EggEventActive = false;
				EggEventProgress = 0f;
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (EggEventActive)
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
			if (EggEventActive && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				const float Scale = 0.875f;
				const float Alpha = 0.5f;
				const int InternalOffset = 6;
				const int OffsetX = 20;
				const int OffsetY = 20;

				Texture2D EventIcon = ModContent.Request<Texture2D>("Spooky/Content/Events/EggEventIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Color NameBoxColor = new Color(113, 35, 206);
				Color ProgressBarColor1 = new Color(54, 35, 35);
				Color ProgressBarColor2 = new Color(199, 7, 49);

				int width = (int)(200f * Scale);
				int height = (int)(46f * Scale);

				Rectangle ProgressBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
				Utils.DrawInvBG(spriteBatch, ProgressBackground, new Color(95, 27, 43, 255) * 0.785f);

				string ProgressText = "Progress: " + Math.Round(EggEventProgress / 3, 0, MidpointRounding.AwayFromZero) + "%";
				Utils.DrawBorderString(spriteBatch, ProgressText, new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + 5), Color.White, Scale, 0.5f, -0.1f);
				Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + ProgressBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

				var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp(EggEventProgress / 3, 0f, 100f)), TextureAssets.ColorBar.Height());
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
				Utils.DrawBorderString(spriteBatch, "Egg Incursion", new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
			}
		}
	}
}