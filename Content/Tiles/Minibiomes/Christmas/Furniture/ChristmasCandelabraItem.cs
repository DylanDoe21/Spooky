using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasCandelabraItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasCandelabra>());
            Item.width = 16;
			Item.height = 16;
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LivingFleshItem>(), 5)
			.AddIngredient(ItemID.Torch, 3)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}