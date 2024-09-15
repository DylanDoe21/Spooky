using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class GiantEar : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 50;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }
    }
}