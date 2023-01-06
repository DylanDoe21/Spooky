using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Misc
{
    public class Flask3 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mysterious Flask III");
            Tooltip.SetDefault("A very warm and particle filled substance");
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