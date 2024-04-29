using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyBiome
{
    public class CreepyCandle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 36;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().MagicCandle = true;
        }
    }
}