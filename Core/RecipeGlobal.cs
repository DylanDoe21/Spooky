using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Generation
{
    public class RecipeGlobal : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.Furnace);
            recipe.AddIngredient(ModContent.ItemType<Tiles.SpookyBiome.SpookyStoneItem>(), 20);
            recipe.AddRecipeGroup(RecipeGroupID.Wood, 4);
            recipe.AddIngredient(ItemID.Torch, 3);
            recipe.Register();
        }
    }
}