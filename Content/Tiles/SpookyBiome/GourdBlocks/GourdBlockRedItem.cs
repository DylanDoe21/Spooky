using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.GourdBlocks
{
    public class GourdBlockRedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GourdBlockWhiteItem>();
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdBlockRed>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockRedWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GourdBlockRedPlatformItem>(), 2)
            .Register();
        }
    }
}