using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookFishronTome : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
        }
    }
}