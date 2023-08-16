using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Biomes;

namespace Spooky.Content.Items.Catacomb
{
    public class SkullAmulet : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;  
            Item.value = Item.buyPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().SkullAmulet = true;
        }
    }
}