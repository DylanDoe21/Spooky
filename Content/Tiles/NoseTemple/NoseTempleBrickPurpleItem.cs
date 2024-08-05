using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Tiles.NoseTemple
{
    public class NoseTempleBrickPurpleItem : ModItem
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
			Item.createTile = ModContent.TileType<NoseTempleBrickPurpleSafe>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.GrayBrick)
            .AddTile(ModContent.TileType<Cauldron>())
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<NoseTempleWallPurpleItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}