using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
    public class SporeMonolithItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SporeMonolith>());
            Item.width = 16;
			Item.height = 16;
            Item.accessory = true;
            Item.vanity = true;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().SporeMonolithEquipped = true;
        }

        public override void UpdateVanity(Player player) 
        {
            UpdateAccessory(player, false);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<MiteMandibles>(), 50)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}