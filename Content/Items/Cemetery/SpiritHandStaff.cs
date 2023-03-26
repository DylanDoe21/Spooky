using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.Cemetery
{
    public class SpiritHandStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rod of Hands");
            Tooltip.SetDefault("Casts spooky hands up from the ground");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 74;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}