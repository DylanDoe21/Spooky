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
            CreateRecipe(5)
            .AddIngredient(ItemID.DirtBlock, 5)
			.AddIngredient(ItemID.MudBlock, 5)
			.AddIngredient(ItemID.JungleSpores, 1)
			.Register();

            CreateRecipe(5)
            .AddIngredient(ModContent.ItemType<CemeteryDirtItem>(), 5)
			.AddIngredient(ItemID.MudBlock, 5)
			.AddIngredient(ItemID.JungleSpores, 1)
			.Register();

            CreateRecipe(5)
            .AddIngredient(ModContent.ItemType<DampSoilItem>(), 5)
			.AddIngredient(ItemID.MudBlock, 5)
			.AddIngredient(ItemID.JungleSpores, 1)
			.Register();

            CreateRecipe(5)
            .AddIngredient(ModContent.ItemType<SpookyDirtItem>(), 5)
			.AddIngredient(ItemID.MudBlock, 5)
			.AddIngredient(ItemID.JungleSpores, 1)
			.Register();
        }
    }
}