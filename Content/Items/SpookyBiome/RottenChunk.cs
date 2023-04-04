using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome
{
    public class RottenChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rotten Chunk");
            // Tooltip.SetDefault("'It smells foul'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 5);
            Item.maxStack = 999;
        }
    }
}