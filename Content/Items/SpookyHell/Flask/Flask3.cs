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
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ItemID.Fireblossom, 5)
            .AddIngredient(ItemID.Deathweed, 5)
            .AddIngredient(ItemID.Ruby, 12)
            .AddIngredient(ItemID.AshBlock, 35)
            .AddIngredient(ItemID.DeerThing, 1)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}