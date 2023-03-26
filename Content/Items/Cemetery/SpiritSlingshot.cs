using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritSlingshot : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghostly Slingshot");
            Tooltip.SetDefault("Hold down to pull back the slingshot, then release to fling a ghastly orb");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 34;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}