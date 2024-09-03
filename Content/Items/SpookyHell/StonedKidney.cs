using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell
{
    public class StonedKidney : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 48;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }
    }
}