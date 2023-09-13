using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Generation
{
    public class RecipeGlobal : ModSystem
    {
        public override void AddRecipeGroups()
        {
            //old wood and mossy stone count as a vanilla wood type and stone type respectively
            RecipeGroup wood = RecipeGroup.recipeGroups[RecipeGroupID.Wood];
            wood.ValidItems.Add(ModContent.ItemType<SpookyWoodItem>());
        }

        public override void AddRecipes()
        {

            Recipe furnaceRecipe = Recipe.Create(ItemID.Furnace);
            furnaceRecipe.AddIngredient(ModContent.ItemType<SpookyStoneItem>(), 20);
            furnaceRecipe.AddRecipeGroup(RecipeGroupID.Wood, 4);
            furnaceRecipe.AddIngredient(ItemID.Torch, 3);
            furnaceRecipe.AddTile(TileID.WorkBenches);
            furnaceRecipe.Register();
        }
    }
}