using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class NoseTemplePlatformPurpleItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<NoseTemplePlatformPurple>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<NoseTempleBrickPurpleItem>())
            .Register();

            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<NoseTempleFancyBrickPurpleItem>())
            .Register();
        }
	}
}