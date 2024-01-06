using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome
{
    public class OldWoodGreatsword : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 18;
			Item.DamageType = DamageClass.Melee;
			Item.autoReuse = true;
            Item.width = 74;
            Item.height = 74;
            Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
            Item.rare = ItemRarityID.White;
            Item.value = Item.buyPrice(gold: 1);
            Item.UseSound = SoundID.DD2_MonkStaffSwing;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 35)
            .AddRecipeGroup(RecipeGroupID.IronBar, 8)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}