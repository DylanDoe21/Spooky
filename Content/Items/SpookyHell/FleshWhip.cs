using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class FleshWhip : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye Lasher");
            Tooltip.SetDefault("Crtical hits will sometimes temporarily reduce enemy defense");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}