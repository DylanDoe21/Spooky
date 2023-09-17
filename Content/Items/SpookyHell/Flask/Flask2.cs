using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class Flask2 : ModItem
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
            .AddIngredient(ItemID.Shiverthorn, 5)
            .AddIngredient(ItemID.GlowingMushroom, 12)
            .AddIngredient(ItemID.RottenChunk, 20)
            .AddIngredient(ItemID.IceBlock, 10)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.Shiverthorn, 5)
            .AddIngredient(ItemID.GlowingMushroom, 12)
            .AddIngredient(ItemID.Vertebrae, 20)
            .AddIngredient(ItemID.IceBlock, 10)
            .AddIngredient(ItemID.Bottle, 1)
            .Register();
        }
    }
}