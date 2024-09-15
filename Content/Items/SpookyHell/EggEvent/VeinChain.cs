using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.EggEvent
{
    public class VeinChain : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 62;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.buyPrice(gold: 30);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
        }
    }
}