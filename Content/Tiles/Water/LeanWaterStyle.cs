using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Water
{
	public class LeanWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Spooky/LeanWaterfallStyle").Slot;

		public override int GetSplashDust() => 52;

		public override int GetDropletGore() => ModContent.Find<ModGore>("Spooky/CemeteryWaterDroplet").Type;

		public override Asset<Texture2D> GetRainTexture() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/CemeteryRain");
		}
		
		public override byte GetRainVariant() 
		{
			return (byte)Main.rand.Next(3);
		}

		public override void LightColorMultiplier(ref float r, ref float g, ref float b)
		{
			r = 1f;
			g = 0.5f;
			b = 1f;
		}

		public override Color BiomeHairColor() => Color.Purple;
	}
}