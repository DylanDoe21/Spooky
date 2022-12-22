using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Cemetery
{
    public class CemeteryBrickMossItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mossy Crypt Brick");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 7;
			Item.useAnimation = 15;
			Item.useStyle = 1;
			Item.maxStack = 999;
			Item.createTile = ModContent.TileType<CemeteryBrickMoss>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
            .AddIngredient(ModContent.ItemType<CemeteryBrickItem>(), 10)
            .AddIngredient(ModContent.ItemType<CemeteryGrassSeeds>(), 1)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}