using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class SpookyBiomeChestItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SpookyBiomeChest>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 5);
		}
	}
}