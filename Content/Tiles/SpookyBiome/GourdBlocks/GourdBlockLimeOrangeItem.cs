using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdBlockLimeOrangeItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GourdBlockOrangeItem>();
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdBlockLimeOrange>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockLimeOrangeWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<GourdBlockOrangeItem>(), 1)
            .AddIngredient(ModContent.ItemType<GourdBlockLimeItem>(), 1)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockLimeOrangePlatformItem>(), 2)
            .Register();
        }
    }
}