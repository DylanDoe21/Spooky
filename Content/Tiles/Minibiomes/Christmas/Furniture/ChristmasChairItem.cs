using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasChairItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasChair>());
            Item.width = 16;
			Item.height = 16;
		}

        /*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<NoseTempleBrickGrayItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
        */
	}
}