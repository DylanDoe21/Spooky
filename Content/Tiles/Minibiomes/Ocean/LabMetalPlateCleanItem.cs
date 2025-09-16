using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
    public class LabMetalPlateCleanItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabMetalPlateClean>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LabMetalPlateCleanWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe(2)
            .AddRecipeGroup(RecipeGroupID.IronBar, 1)
            .AddTile(TileID.Furnaces)
            .Register();
        }
    }
}