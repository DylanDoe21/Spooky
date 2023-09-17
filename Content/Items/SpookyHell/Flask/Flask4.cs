using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class Flask4 : ModItem
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
            .AddIngredient(ItemID.Fireblossom, 5)
            .AddIngredient(ItemID.Deathweed, 5)
            .AddIngredient(ItemID.Ruby, 12)
            .AddIngredient(ItemID.AshBlock, 35)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}