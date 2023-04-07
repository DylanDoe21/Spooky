using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 44;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<SentientRarity>();
        }
    }
}