using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

using Spooky.Content.Items.SpookyHell.Sentient;

namespace Spooky.Content.Items.SpookyHell
{
    public class SentientHeart : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 60;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<SentientRarity>();
        }
    }
}