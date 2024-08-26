using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
    public class RottenChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1);
            Item.maxStack = 9999;
        }
    }
}