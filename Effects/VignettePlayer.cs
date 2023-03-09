using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;

namespace Spooky.Effects
{
	class VignettePlayer : ModPlayer
	{
		private bool _lastTickVignetteActive;
		private bool _vignetteActive;
		private Vector2 _targetPosition;
		private float _opacity;
		private float _radius;
		private float _fadeDistance;
		private Color _color;

		public override void ResetEffects()
		{
			_lastTickVignetteActive = _vignetteActive;
			_vignetteActive = false;
		}

		public void SetVignette(float radius, float colorFadeDistance, float opacity) => SetVignette(radius, colorFadeDistance, opacity, Color.Black, Main.screenPosition);

		public void SetVignette(float radius, float colorFadeDistance, float opacity, Color color, Vector2 targetPosition)
		{
			_radius = radius;
			_targetPosition = targetPosition;
			_fadeDistance = colorFadeDistance;
			_color = color;
			_opacity = opacity;
			_vignetteActive = true;
		}

		public override void PostUpdateMiscEffects()
		{
			Spooky.vignetteShader.UseColor(_color);
			Spooky.vignetteShader.UseIntensity(_opacity);
			Spooky.vignetteEffect.Parameters["Radius"].SetValue(_radius);
			Spooky.vignetteEffect.Parameters["FadeDistance"].SetValue(_fadeDistance);
			Player.ManageSpecialBiomeVisuals("Spooky:Vignette", _vignetteActive || _lastTickVignetteActive, _targetPosition);
		}
	}
}
