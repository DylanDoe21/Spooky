using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Catacomb.Furniture;
using Spooky.Content.Tiles.Cemetery;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombBrick1Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 10;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<CatacombBrick1Safe>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CemeteryStoneItem>(), 2)
            .AddTile(TileID.Furnaces)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrickWall1Item>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrickPlatform1Item>(), 2)
            .Register();
        }
    }
}