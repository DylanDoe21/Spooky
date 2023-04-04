using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
    public class SentientHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sentient Heart");
            /* Tooltip.SetDefault("A strange heart ripped from a strong monster, it is still beating on it's own"
            + "\nWhen combined with certain weapons at the cauldron, it will bring them sentience"
            + "\nItems capable of this transformation will be revealed while you have this on you"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 44;
            Item.rare = ModContent.RarityType<SentientRarity>();
        }
    }
}