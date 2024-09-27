using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpiderCave
{
    public class RootWoodItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RootWood>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RootWoodWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            /*
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpiderWoodPlatformItem>(), 2)
            .Register();
            */
        }
    }
}