using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasBedItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasBed>());
            Item.width = 16;
			Item.height = 16;
		}
		
		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<NoseTempleBrickGrayItem>(), 15)
			.AddIngredient(ItemID.Silk, 5)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}