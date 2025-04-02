using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Water
{
	public class FishronIceWaterStyle : ModWaterStyle
	{
		public override int ChooseWaterfallStyle() => ModContent.Find<ModWaterfallStyle>("Spooky/FishronIceWaterfallStyle").Slot;

		public override int GetSplashDust() => DustID.ToxicBubble;

		public override int GetDropletGore() => ModContent.Find<ModGore>("Spooky/SpookyWaterDroplet").Type;

		public override Asset<Texture2D> GetRainTexture() 
		{
			return ModContent.Request<Texture2D>("Spooky/Content/Backgrounds/FishronIceRain");
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

		public override Color BiomeHairColor() => Color.White;
	}
}