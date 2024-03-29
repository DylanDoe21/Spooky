using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class Flask1 : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.Quest;
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Daybloom, 5)
            .AddIngredient(ItemID.Blinkroot, 5)
            .AddIngredient(ItemID.Moonglow, 5)
            .AddIngredient(ItemID.PurificationPowder, 10)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}