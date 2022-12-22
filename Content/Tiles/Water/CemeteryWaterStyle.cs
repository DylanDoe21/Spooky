using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Water
{
	public class CemeteryWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Spooky/CemeteryWaterfallStyle").Slot;

		public override int GetSplashDust() => 288;

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
			g = 1f;
			b = 1f;
		}

		public override Color BiomeHairColor() => Color.Teal;
	}
}