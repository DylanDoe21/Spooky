using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Cemetery.Furniture
{
	public class CemeteryBiomeChestItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<CemeteryBiomeChest>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 5);
		}
	}
}