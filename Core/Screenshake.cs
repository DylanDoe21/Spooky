using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Core
{
	public class Screenshake : ModSystem
	{
		public static Vector2 Position;
		public static float ShakeIntensity = 0;
		public static float AdditionalFalloff = 0;

		//this is what should be called whenever screenshake needs to be created by setting the screen shake variables to whatever parameters are put in
		public static void ShakeScreenWithIntensity(Vector2 SourcePosition, float ScreenShakeAmount, float FalloffDistance)
		{
			Position = SourcePosition;
			ShakeIntensity = ScreenShakeAmount;
			AdditionalFalloff = FalloffDistance;
		}

		public override void ModifyScreenPosition()
		{
			float ExtraMultiplier = ModContent.GetInstance<SpookyConfig>().ScreenShakeIntensity;

			if (!Main.gameMenu && ExtraMultiplier > 0)
			{
				PunchCameraModifier modifier = new PunchCameraModifier(Position, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), ShakeIntensity * ExtraMultiplier, 1f, 1, 650f + AdditionalFalloff, null);
				Main.instance.CameraModifiers.Add(modifier);

				if (ShakeIntensity * ExtraMultiplier >= 0)
				{
					ShakeIntensity -= 0.2f;
				}
				if (ShakeIntensity * ExtraMultiplier < 0)
				{
					ShakeIntensity = 0;
				}

				if (ShakeIntensity <= 0)
				{
					Position = Vector2.Zero;
					AdditionalFalloff = 0;
				}
			}
			else
			{
				Position = Vector2.Zero;
				ShakeIntensity = 0;
				AdditionalFalloff = 0;
			}
		}
	}
}