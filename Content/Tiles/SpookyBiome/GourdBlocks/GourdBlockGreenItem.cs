using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdBlockGreenItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GourdBlockLimeItem>();
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdBlockGreen>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockGreenWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockGreenPlatformItem>(), 2)
            .Register();
        }
    }
}