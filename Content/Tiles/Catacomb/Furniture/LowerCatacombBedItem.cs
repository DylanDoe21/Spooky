using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class LowerCatacombBedItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LowerCatacombBed>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 15)
			.AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}