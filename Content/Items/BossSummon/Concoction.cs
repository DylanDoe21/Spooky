using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.BossSummon
{
    public class Concoction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Acidic Concoction");
            Tooltip.SetDefault("Can be used to corrode the giant egg\nHowever, the creature inside will be unleashed");
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 26;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 1;
        }
		
        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Flask1>())
            .AddIngredient(ModContent.ItemType<Flask2>())
            .AddIngredient(ModContent.ItemType<Flask3>())
            .AddIngredient(ModContent.ItemType<Flask4>())
            .AddIngredient(ModContent.ItemType<CreepyChunk>(), 20)
            .AddIngredient(ItemID.Bone, 12)
            .AddRecipeGroup("SpookyMod:AnyMechBossSoul", 5)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}