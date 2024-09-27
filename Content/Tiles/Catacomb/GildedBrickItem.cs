using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb
{
    public class GildedBrickItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GildedBrickSafe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe(25)
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>(), 25)
            .AddIngredient(ItemID.GoldOre)
            .AddTile(TileID.Furnaces)
            .Register();
        }
    }
}