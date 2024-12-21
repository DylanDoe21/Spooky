using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpiderCave
{
    public class DampSoilWallItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<DampSoilWallSafe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
            .AddIngredient(ModContent.ItemType<DampSoilItem>())
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}