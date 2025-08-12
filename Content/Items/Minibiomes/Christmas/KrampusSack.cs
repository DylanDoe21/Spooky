using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Minibiomes.Christmas
{
    public class KrampusSack : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 44;
            Item.accessory = true;
            Item.rare = ItemRarityID.Blue;  
            Item.value = Item.buyPrice(gold: 1);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().KrampusSack = true;
        }
    }
}