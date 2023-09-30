using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class BackroomsCorpse : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 52;
            Item.accessory = true;
            Item.rare = ItemRarityID.Gray;
            //Item.value = Item.buyPrice(gold: 20);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            //player.GetModPlayer<SpookyPlayer>().BackroomsCorpse = true;
        }
    }
}