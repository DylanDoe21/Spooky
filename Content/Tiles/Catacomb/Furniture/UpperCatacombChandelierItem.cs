using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class UpperCatacombChandelierItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<UpperCatacombChandelier>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick1Item>(), 4)
			.AddIngredient(ItemID.Torch, 4)
			.AddIngredient(ItemID.Chain, 1)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}