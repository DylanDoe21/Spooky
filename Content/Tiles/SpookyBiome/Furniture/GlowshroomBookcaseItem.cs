using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class GlowshroomBookcaseItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<GlowshroomBookcase>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 20)
			.AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}

	public class GlowshroomYellowBookcaseItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<GlowshroomYellowBookcase>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyGlowshroomYellow>(), 20)
			.AddIngredient(ItemID.Book, 10)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}