using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class FlaskMisc1 : ModItem
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
            .AddIngredient(ItemID.SoulofNight, 10)
            .AddIngredient(ItemID.CursedFlame, 15)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.SoulofNight, 10)
            .AddIngredient(ItemID.Ichor, 15)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}