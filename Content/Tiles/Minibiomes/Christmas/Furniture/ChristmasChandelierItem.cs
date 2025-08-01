using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasChandelierItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasChandelier>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ChristmasWoodItem>(), 4)
			.AddIngredient(ItemID.Torch, 4)
			.AddIngredient(ItemID.Chain, 1)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}