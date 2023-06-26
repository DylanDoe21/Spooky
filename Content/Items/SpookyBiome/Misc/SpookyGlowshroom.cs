using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome.Misc
{
    public class SpookyGlowshroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
			Item.height = 26;
			Item.maxStack = 9999;
            Item.rare = ItemRarityID.White;
        }
    }
}