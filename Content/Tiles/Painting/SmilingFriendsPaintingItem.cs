using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Painting
{
	public class SmilingFriendsPaintingItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SmilingFriendsPainting>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 25);
		}
	}
}