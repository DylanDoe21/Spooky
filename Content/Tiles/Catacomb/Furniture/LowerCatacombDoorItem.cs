using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class LowerCatacombDoorItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LowerCatacombDoorClosed>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 6)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}