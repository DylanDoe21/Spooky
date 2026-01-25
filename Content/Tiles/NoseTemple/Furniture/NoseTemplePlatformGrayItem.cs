using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class NoseTemplePlatformGrayItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NoseTemplePlatformGray>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<NoseTempleBrickGrayItem>())
            .Register();

            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<NoseTempleFancyBrickGrayItem>())
            .Register();
        }
	}
}