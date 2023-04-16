using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class FlaskMisc4 : ModItem
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
            .AddIngredient(ItemID.SharkFin, 5)
            .AddIngredient(ItemID.Coral, 15)
            .AddIngredient(ItemID.Glowstick, 20)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}