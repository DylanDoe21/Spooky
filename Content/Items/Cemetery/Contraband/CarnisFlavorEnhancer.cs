using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class CarnisFlavorEnhancer : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Orange;  
            Item.value = Item.buyPrice(gold: 12);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().CarnisFlavorEnhancer = true;
        }
    }
}