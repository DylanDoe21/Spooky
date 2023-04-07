using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookyChestKey : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 5);
            Item.maxStack = 1;
        }
    }
}