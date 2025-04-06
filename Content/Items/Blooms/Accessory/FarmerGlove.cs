using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Blooms.Accessory
{
    public class FarmerGlove : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 40;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            //player.GetModPlayer<BloomBuffsPlayer>().FarmerGlove = true;
        }
    }
}