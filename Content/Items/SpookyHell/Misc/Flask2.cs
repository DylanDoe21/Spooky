using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Misc
{
    public class Flask2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mysterious Flask II");
            Tooltip.SetDefault("A nasty and cold organic substance");
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
            .AddIngredient(ItemID.Shiverthorn, 5)
            .AddIngredient(ItemID.GlowingMushroom, 12)
            .AddIngredient(ItemID.RottenChunk, 20)
            .AddIngredient(ItemID.IceBlock, 10)
            .AddIngredient(ItemID.WormFood, 1)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Shiverthorn, 5)
            .AddIngredient(ItemID.GlowingMushroom, 12)
            .AddIngredient(ItemID.Vertebrae, 20)
            .AddIngredient(ItemID.IceBlock, 10)
            .AddIngredient(ItemID.BloodySpine, 1)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}