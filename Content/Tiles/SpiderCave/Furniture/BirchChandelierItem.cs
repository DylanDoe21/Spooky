using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class BirchChandelierItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BirchChandelier>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<BirchWoodItem>(), 4)
			.AddIngredient(ItemID.Torch, 4)
			.AddIngredient(ItemID.Chain, 1)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}