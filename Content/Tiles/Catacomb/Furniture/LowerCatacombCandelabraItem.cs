using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class LowerCatacombCandelabraItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LowerCatacombCandelabra>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 5)
			.AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}