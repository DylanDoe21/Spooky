using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class AnalogueHorrorTape : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 56;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.buyPrice(platinum: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<SpookyPlayer>().BackroomsCorpse = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<GeminiEntertainmentGame>())
            .AddIngredient(ModContent.ItemType<MandelaCatalogueTV>())
            .AddIngredient(ModContent.ItemType<CarnisFlavorEnhancer>())
            .AddIngredient(ModContent.ItemType<BackroomsCorpse>())
            .AddIngredient(ModContent.ItemType<Local58Telescope>())
            .AddIngredient(ModContent.ItemType<MonumentMythosPyramid>())
            .AddIngredient(ItemID.LunarBar, 5)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }
}