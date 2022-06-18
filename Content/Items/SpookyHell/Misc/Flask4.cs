using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Misc
{
    public class Flask4 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mysterious Flask IV");
            Tooltip.SetDefault("A very corrosive substance");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.maxStack = 999; 
            Item.value = Item.buyPrice(silver: 1); 
            Item.rare = 1;
        }
    }
}