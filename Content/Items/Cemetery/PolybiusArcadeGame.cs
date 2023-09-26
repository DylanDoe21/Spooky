using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery
{
    public class PolybiusArcadeGame : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 48;
            Item.accessory = true;
            Item.rare = ItemRarityID.Gray;  
            //Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<SpookyPlayer>().PolybiusArcadeGame = true;
        }
    }
}