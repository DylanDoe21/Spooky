using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class LittleBoneStatueItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LittleBoneStatue>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.StoneBlock, 50)
            .AddTile(TileID.HeavyWorkBench)
            .Register();
        }
	}
}