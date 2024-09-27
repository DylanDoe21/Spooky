using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;
using Spooky.Content.Tiles.Cemetery;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Tiles.Blooms
{
    public class BloomSoilItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BloomSoil>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
            .AddIngredient(ModContent.ItemType<RottenChunk>())
            .AddIngredient(ItemID.DirtBlock, 6)
            .Register();

            CreateRecipe(3)
            .AddIngredient(ModContent.ItemType<RottenChunk>())
            .AddIngredient(ItemID.MudBlock, 6)
            .Register();

            CreateRecipe(3)
            .AddIngredient(ModContent.ItemType<RottenChunk>())
            .AddIngredient(ModContent.ItemType<CemeteryDirtItem>(), 6)
            .Register();

            CreateRecipe(3)
            .AddIngredient(ModContent.ItemType<RottenChunk>())
            .AddIngredient(ModContent.ItemType<DampSoilItem>(), 6)
            .Register();

            CreateRecipe(3)
            .AddIngredient(ModContent.ItemType<RottenChunk>())
            .AddIngredient(ModContent.ItemType<SpookyDirtItem>(), 6)
            .Register();
        }
    }
}