using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class Flask3 : ModItem
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
            .AddIngredient(ItemID.HoneyBlock, 35)
            .AddIngredient(ItemID.Pumpkin, 35)
            .AddIngredient(ItemID.Sluggy, 5)
            .AddIngredient(ItemID.Cobweb, 15)
            .AddIngredient(ItemID.Abeemination, 1)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}