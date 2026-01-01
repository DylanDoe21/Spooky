using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Water
{
	public class TarWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Spooky/TarWaterfallStyle").Slot;

		public override int GetSplashDust() => 54;

		public override int GetDropletGore() => ModContent.Find<ModGore>("Spooky/TarWaterDroplet").Type;

		public override void LightColorMultiplier(ref float r, ref float g, ref float b)
		{
			r = 0.85f;
			g = 0.85f;
			b = 0.85f;
		}

		public override Color BiomeHairColor() => Color.Black;
	}
}