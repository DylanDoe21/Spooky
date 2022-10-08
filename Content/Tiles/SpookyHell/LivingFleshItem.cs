using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class LivingFleshItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Flesh");
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
			Item.createTile = ModContent.TileType<LivingFlesh>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<EyePlatformItem>(), 2)
            .AddTile(TileID.WorkBenches)
            .Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LivingFleshWallItem>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}