using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class RedGodzillaCartridge : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Lime;  
            Item.value = Item.buyPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().RedGodzillaCartridge = true;
        }
    }
}