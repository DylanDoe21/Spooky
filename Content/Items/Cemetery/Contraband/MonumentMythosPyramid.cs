using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Cemetery.Contraband
{
    public class MonumentMythosPyramid : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 46;
            Item.accessory = true;
            Item.rare = ItemRarityID.Yellow;  
            Item.value = Item.buyPrice(gold: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpookyPlayer>().MonumentMythosPyramid = true;

            if (player.GetModPlayer<SpookyPlayer>().GizaGlassHits < 3)
            {
                player.statDefense += 30;
            }
        }
    }
}