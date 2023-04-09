using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyeChestItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 7;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<EyeChest>();
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LivingFleshItem>(), 8)
			.AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}