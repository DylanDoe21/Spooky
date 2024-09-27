using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class SpookyHellChestItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpookyHellChest>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 5);
		}
	}
}