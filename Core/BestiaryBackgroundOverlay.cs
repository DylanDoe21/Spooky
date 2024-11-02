using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader;
using Terraria.UI;

namespace Spooky.Core
{
	//utility for drawing a custom background that overlays whatever bestiary background is drawn by default
	//mainly used in spooky mod for enemies that spawn in mod biomes along with vanilla events, such as cemetery + blood moon, spooky forest + rain, ect
	public class BestiaryBackgroundOverlay : IBestiaryInfoElement, IBestiaryBackgroundOverlayAndColorProvider
	{
		string NewOverlayImagePath;

		Color? NewOverlayImageColor;

		public float DisplayPriority { get; set; }

		public UIElement ProvideUIElement(BestiaryUICollectionInfo info)
		{
			return null;
		}
		
		public Color? GetBackgroundOverlayColor()
		{
			return NewOverlayImageColor;
		}

		public Asset<Texture2D> GetBackgroundOverlayImage()
		{
			if (NewOverlayImagePath == null)
			{
				return null;
			}

			return ModContent.Request<Texture2D>(NewOverlayImagePath);
		}

		public BestiaryBackgroundOverlay(string OverlayImagePath = null, Color? OverlayColor = null)
		{
			NewOverlayImagePath = OverlayImagePath;
			NewOverlayImageColor = OverlayColor;
		}
	}
}