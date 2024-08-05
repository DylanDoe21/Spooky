using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.NoseTemple.Furniture;

namespace Spooky.Content.Tiles.NoseTemple
{
    public class NoseTempleFancyBrickGrayItem : ModItem
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
			Item.createTile = ModContent.TileType<NoseTempleFancyBrickGraySafe>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<NoseTempleBrickGrayItem>(), 2)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<NoseTempleFancyWallGrayItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<NoseTemplePlatformGrayItem>(), 2)
            .Register();
        }
    }
}