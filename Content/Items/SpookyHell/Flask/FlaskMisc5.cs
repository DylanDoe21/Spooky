using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class FlaskMisc5 : ModItem
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
            .AddIngredient(ItemID.SandBlock, 35)
            .AddIngredient(ItemID.FossilOre, 15)
            .AddIngredient(ItemID.AncientBattleArmorMaterial, 1)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}