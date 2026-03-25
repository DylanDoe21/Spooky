using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
	public class CemeteryLampItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<CemeteryLamp>());
            Item.width = 16;
			Item.height = 16;
		}
	}
}