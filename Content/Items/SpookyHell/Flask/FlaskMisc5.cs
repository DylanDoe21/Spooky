using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Flask
{
    public class FlaskMisc5 : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sandstorm Flask");
            Tooltip.SetDefault("A mythical substance of rare desert materials"
            + "\nLittle eye may be interested in this");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.Quest;
            Item.value = Item.buyPrice(gold: 1);
            Item.maxStack = 999;
        }
    }
}