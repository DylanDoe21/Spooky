using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome;
using Spooky.Content.Tiles.SpookyBiome.Furniture;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
    public class SpookyGlowshroomYellow : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GlowshroomYellowBlock>());
            Item.width = 18;
			Item.height = 26;
			Item.maxStack = 9999;
            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GlowshroomYellowWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GlowshroomYellowPlatformItem>(), 2)
            .Register();
        }
    }
}