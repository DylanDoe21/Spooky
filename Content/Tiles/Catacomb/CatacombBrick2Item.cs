using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Catacomb.Furniture;
using Spooky.Content.Tiles.Cemetery;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombBrick2Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<CatacombBrick2Safe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CemeteryStoneItem>())
            .AddIngredient(ItemID.MudBlock)
            .AddTile(TileID.Furnaces)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrickWall2Item>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrickPlatform2Item>(), 2)
            .Register();
        }
    }
}