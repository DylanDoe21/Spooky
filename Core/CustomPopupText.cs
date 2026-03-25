using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace Spooky.Core
{
	public class CustomPopupText
	{
		public static void SpawnText(Vector2 position, string text, Color color, Vector2 velocity, int duration)
		{
			AdvancedPopupRequest request = default(AdvancedPopupRequest);
			request.Text = text;
			request.DurationInFrames = duration;
			request.Velocity = velocity;
			request.Color = color;
			PopupText.NewText(request, position);
		}
	}
}