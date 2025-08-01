using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasClockItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasClock>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ChristmasWoodItem>(), 15)
			.AddRecipeGroup(RecipeGroupID.IronBar, 3)
			.AddIngredient(ItemID.Glass, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}