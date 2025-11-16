using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class UpperCatacombChestItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<UpperCatacombChest>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick1Item>(), 8)
			.AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}