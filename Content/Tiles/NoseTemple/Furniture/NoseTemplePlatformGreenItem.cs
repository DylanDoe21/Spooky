using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class NoseTemplePlatformGreenItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NoseTemplePlatformGreen>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<NoseTempleBrickGreenItem>())
            .Register();

            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<NoseTempleFancyBrickGreenItem>())
            .Register();
        }
	}
}