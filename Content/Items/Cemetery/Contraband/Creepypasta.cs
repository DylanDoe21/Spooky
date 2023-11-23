using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class Creepypasta : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 60;
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
            .AddIngredient(ModContent.ItemType<PolybiusArcadeGame>())
            .AddIngredient(ModContent.ItemType<SmileDogPicture>())
            .AddIngredient(ModContent.ItemType<RedMistClarinet>())
            .AddIngredient(ModContent.ItemType<SlendermanPage>())
            .AddIngredient(ModContent.ItemType<RedGodzillaCartridge>())
            .AddIngredient(ModContent.ItemType<HerobrineAltar>())
            .AddIngredient(ItemID.LunarBar, 5)
            .AddTile(TileID.TinkerersWorkbench)
            .Register();
        }
    }
}