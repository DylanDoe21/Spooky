using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Boss
{
    public class OrroboroChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Parasitic Flesh");
            Tooltip.SetDefault("Ripped right off the serpent");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 2);
            Item.maxStack = 999;
        }
    }
}