using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class LowerCatacombClockItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LowerCatacombClock>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 15)
			.AddRecipeGroup(RecipeGroupID.IronBar, 3)
			.AddIngredient(ItemID.Glass, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}