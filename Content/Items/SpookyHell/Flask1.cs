using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class Flask1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mysterious Flask I");
            Tooltip.SetDefault("A smelly substance, made from ground up plants");
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
            .AddIngredient(ItemID.Daybloom, 5)
            .AddIngredient(ItemID.Blinkroot, 5)
            .AddIngredient(ItemID.Moonglow, 5)
            .AddIngredient(ItemID.PurificationPowder, 10)
            .AddIngredient(ItemID.SuspiciousLookingEye, 1)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}