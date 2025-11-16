using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class LowerCatacombCandleItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LowerCatacombCandle>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 4)
			.AddIngredient(ItemID.Torch, 1)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}