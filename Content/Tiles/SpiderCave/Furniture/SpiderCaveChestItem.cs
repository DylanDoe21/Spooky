using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class SpiderCaveChestItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpiderCaveChest>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 5);
		}
	}
}