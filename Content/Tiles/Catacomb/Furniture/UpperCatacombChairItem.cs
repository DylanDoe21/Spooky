using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class UpperCatacombChairItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<UpperCatacombChair>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick1Item>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}