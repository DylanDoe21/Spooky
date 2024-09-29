using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.BossSummon
{
    public class EMFReaderBroke : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("SpookyMod:DemoniteBars", 5)
            .AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
}