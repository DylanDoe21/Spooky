using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Blooms.Accessory
{
    public class DaylightSavings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 60;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<BloomBuffsPlayer>().DaylightSavings = true;
        }
    }
}