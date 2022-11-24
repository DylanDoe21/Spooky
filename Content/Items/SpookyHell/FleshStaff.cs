using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class FleshStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Retina Staff");
            Tooltip.SetDefault("Summon eyes that follow your cursor while holding down left click"
            + "\nRelease left click to fling the eyes everywhere"
            + "\nOnly up to 8 eyes can be active at once");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 48;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}