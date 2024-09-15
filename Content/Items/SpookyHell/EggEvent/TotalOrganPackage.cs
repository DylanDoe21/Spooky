using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class TotalOrganPackage : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 70;
            Item.height = 66;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 50);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GooChompers>())
            .AddIngredient(ModContent.ItemType<VeinChain>())
            .AddIngredient(ModContent.ItemType<PeptoStomach>())
            .AddIngredient(ModContent.ItemType<StonedKidney>())
            .AddIngredient(ModContent.ItemType<SmokerLung>())
            .AddIngredient(ModContent.ItemType<GiantEar>())
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }
}