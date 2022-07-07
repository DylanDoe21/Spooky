using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Generation
{
    public class RecipeGlobal : ModSystem
    {
        public override void AddRecipes()
        {
            Recipe furnaceRecipe = Recipe.Create(ItemID.Furnace);
            furnaceRecipe.AddIngredient(ModContent.ItemType<SpookyStoneItem>(), 20);
            furnaceRecipe.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 4);
            furnaceRecipe.AddIngredient(ItemID.Torch, 3);
            furnaceRecipe.Register();

            Recipe torchRecipe = Recipe.Create(ItemID.Torch);
            torchRecipe.AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 1);
            torchRecipe.AddIngredient(ItemID.Gel, 1);
            torchRecipe.Register();
        }
    }
}