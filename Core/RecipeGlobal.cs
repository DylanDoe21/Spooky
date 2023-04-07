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
            RecipeGroup wood = RecipeGroup.recipeGroups[RecipeGroupID.Wood];
            wood.ValidItems.Add(ModContent.ItemType<SpookyWoodItem>());
        }
    }
}