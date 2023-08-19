using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.BossBags.Accessory
{
    public class MocoNose : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.expert = true;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 12);
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        { 
            player.GetModPlayer<SpookyPlayer>().MocoNose = true;
        }
    }
}