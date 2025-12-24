using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class BirchTableItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BirchTable>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<BirchWoodItem>(), 8)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}