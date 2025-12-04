using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Painting
{
	public class SurprisedSkullPaintingItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SurprisedSkullPainting>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 25);
		}
	}
}