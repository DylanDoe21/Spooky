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
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 62;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<SentientRarity>();
        }
    }
}