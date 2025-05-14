using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdBlockYellowGreenItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GourdBlockYellowItem>();
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdBlockYellowGreen>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockYellowGreenWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<GourdBlockYellowItem>(), 1)
            .AddIngredient(ModContent.ItemType<GourdBlockGreenItem>(), 1)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockYellowGreenPlatformItem>(), 2)
            .Register();
        }
    }
}