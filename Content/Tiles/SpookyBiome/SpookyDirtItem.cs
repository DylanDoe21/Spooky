using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome.Furniture;

namespace Spooky.Content.Tiles.SpookyBiome
{
    public class SpookyDirtItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SpookyDirt>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyDirtWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}