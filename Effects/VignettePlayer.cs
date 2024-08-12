using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Effects
{
    [Autoload(Side = ModSide.Client)]
    class VignettePlayer : ModPlayer
	{
		private bool WasActiveLastTick;
		private bool IsActive;
		private Vector2 TargetPosition;
		private float Opacity;
		private float Radius;
		private float FadeDistance;
		private Color Color;

		public override void ResetEffects()
		{
			WasActiveLastTick = IsActive;
			IsActive = false;
		}

		public void SetVignette(float radius, float colorFadeDistance, float opacity) => SetVignette(radius, colorFadeDistance, opacity, Color.Black, Main.screenPosition);

		public void SetVignette(float radius, float colorFadeDistance, float opacity, Color color, Vector2 targetPosition)
		{
			Radius = radius;
			TargetPosition = targetPosition;
			FadeDistance = colorFadeDistance;
			Color = color;
			Opacity = opacity;
			IsActive = true;
		}

		public override void PostUpdateMiscEffects()
		{
			/*
			if (!IsActive)
			{
    			return;
			}
			*/

			Spooky.vignetteShader.UseColor(Color);
			Spooky.vignetteShader.UseIntensity(Opacity);
			Spooky.vignetteEffect.Parameters["Radius"].SetValue(Radius);
			Spooky.vignetteEffect.Parameters["FadeDistance"].SetValue(FadeDistance);
			Player.ManageSpecialBiomeVisuals("Spooky:Vignette", IsActive || WasActiveLastTick, TargetPosition);
		}
	}
}
