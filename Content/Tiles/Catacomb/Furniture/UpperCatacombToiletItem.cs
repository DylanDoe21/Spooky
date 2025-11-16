using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class UpperCatacombToiletItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<UpperCatacombToilet>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick1Item>(), 6)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}