using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.UserInterfaces
{
    public class KrampusResolutionUI
    {
		static float Rotation = 0f;
		static bool Shake = false;

		private static Asset<Texture2D> Texture;
        
        public static void Draw(SpriteBatch spriteBatch)
        {
			Player player = Main.LocalPlayer;

			if (player.GetModPlayer<SpookyPlayer>().KrampusResolutionTimer > 0)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                Texture ??= ModContent.Request<Texture2D>("Spooky/Content/Items/Minibiomes/Christmas/KrampusResolution", AssetRequestMode.ImmediateLoad);

				if (Shake)
				{
					Rotation += 0.1f;
					if (Rotation > 0.2f)
					{
						Shake = false;
					}
				}
				else
				{
					Rotation -= 0.1f;
					if (Rotation < -0.2f)
					{
						Shake = true;
					}
				}

				//draw the main UI box
				spriteBatch.Draw(Texture.Value, player.Center - new Vector2(0f, 65f) - Main.screenPosition, null, Color.White, Rotation, Texture.Size() / 2, 1f, SpriteEffects.None, 0f);

				DisplayTimeText(spriteBatch, player.Center - new Vector2(5f, 65f) - Main.screenPosition, Main.LocalPlayer.GetModPlayer<SpookyPlayer>().KrampusResolutionTimer);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
			}
		}

		public static void DisplayTimeText(SpriteBatch spriteBatch, Vector2 TextTopLeft, int Timer)
		{
			Vector2 scale = new Vector2(0.85f, 0.825f) * MathHelper.Clamp(Main.screenHeight / 1440f, 0.825f, 1f);

			TimeSpan time = TimeSpan.FromSeconds((Timer + 60) / 60);
			string actualTime = ((Timer + 60) / 60).ToString();

			//bloom buff timeleft text
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.DeathText.Value, actualTime, TextTopLeft, Color.Indigo, Color.White, 0f, Vector2.Zero, scale);
		}
    }
}