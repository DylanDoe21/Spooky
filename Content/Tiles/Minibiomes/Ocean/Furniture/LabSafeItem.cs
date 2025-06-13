using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
	public class LabSafeItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LabSafe>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LabMetalPlateItem>(), 8)
			.AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}