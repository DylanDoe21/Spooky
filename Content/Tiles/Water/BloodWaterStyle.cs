using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Water
{
	public class BloodWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Spooky/BloodWaterfallStyle").Slot;

		public override int GetSplashDust() => DustID.Blood;

		public override int GetDropletGore() => ModContent.Find<ModGore>("Spooky/BloodWaterDroplet").Type;

		public override void LightColorMultiplier(ref float r, ref float g, ref float b)
		{
			r = 0.98f;
			g = 0.92f;
			b = 0.92f;
		}

		public override Color BiomeHairColor() => Color.Red;
	}
}