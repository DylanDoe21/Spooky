using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Misc
{
    public class Flask1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mysterious Flask I");
            Tooltip.SetDefault("A very corrosive substance");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1); 
            Item.maxStack = 999;
        }
    }
}