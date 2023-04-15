using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Linq;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 3;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 4));
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