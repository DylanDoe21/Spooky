using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class FlaskMisc2 : ModItem
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
            .AddIngredient(ItemID.SoulofLight, 10)
            .AddIngredient(ItemID.CrystalShard, 15)
            .AddIngredient(ItemID.UnicornHorn, 3)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}