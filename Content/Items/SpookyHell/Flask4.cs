using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class Flask4 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mysterious Flask IV");
            Tooltip.SetDefault("A gooey substance, made from the most sticky items");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = 999;
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