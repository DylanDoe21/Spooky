using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritScroll : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spiritual Scroll");
            Tooltip.SetDefault("Conjures pumpkin heads that linger around you"
            + "\nAfter a few seconds, they will quickly charge towards your cursor");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 56;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}