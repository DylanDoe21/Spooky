using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
	public class GourdBlockLimeOrangePlatformItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<GourdBlockLimeOrangePlatform>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<GourdBlockLimeOrangeItem>(), 1)
            .Register();
        }
	}
}