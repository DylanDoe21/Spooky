using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class FleshAxe : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye Dicer");
            Tooltip.SetDefault("Higher critical strike chance on enemies with low health");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 54;
            Item.height = 50;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}