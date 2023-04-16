using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class FlaskMisc3 : ModItem
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
            .AddIngredient(ItemID.Pumpkin, 35)
            .AddIngredient(ItemID.Hay, 25)
            .AddIngredient(ItemID.RottenEgg, 5)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}