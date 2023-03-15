using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell.Boss
{
    [LegacyName("OrroboroChunk")]
    public class ArteryPiece : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Serpent Artery");
            Tooltip.SetDefault("'Rich with powerful blood'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 2);
            Item.maxStack = 999;
        }
    }
}