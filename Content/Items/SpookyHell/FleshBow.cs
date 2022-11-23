using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class FleshBow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seer");
            Tooltip.SetDefault("Shoots fast bloody tears\nHold down for longer to shoot a super charged eye");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 66;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}