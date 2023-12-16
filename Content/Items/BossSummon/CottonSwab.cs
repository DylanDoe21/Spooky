using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.BossSummon
{
    public class CottonSwab : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
		
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.rare = ItemRarityID.White;
            Item.maxStack = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 10)
            .AddIngredient(ItemID.Bone, 12)
            .AddIngredient(ItemID.Cobweb, 25)
            .AddTile(TileID.DemonAltar)
            .Register();
        }
    }
}