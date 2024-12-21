using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Cemetery
{
    public class CemeteryDirtWallItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<CemeteryDirtWallSafe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
            .AddIngredient(ModContent.ItemType<CemeteryDirtItem>())
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}