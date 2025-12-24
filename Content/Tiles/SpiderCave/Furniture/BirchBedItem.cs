using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class BirchBedItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BirchBed>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<BirchWoodItem>(), 15)
			.AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}