using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using Spooky.Content.Items.SpookyHell.Sentient;

namespace Spooky.Content.Items.SpookyHell
{
    public class SentientHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 50;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<SentientRarity>();
        }
    }
}