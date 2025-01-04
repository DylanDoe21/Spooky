using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookFishronBow : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 58;
            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 25);
        }
    }
}