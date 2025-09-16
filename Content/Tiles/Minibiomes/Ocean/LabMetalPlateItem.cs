using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
    public class LabMetalPlateItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabMetalPlateSafe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LabMetalPlateWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe(2)
            .AddRecipeGroup(RecipeGroupID.IronBar, 1)
            .AddTile(TileID.Furnaces)
            .Register();
        }
    }
}