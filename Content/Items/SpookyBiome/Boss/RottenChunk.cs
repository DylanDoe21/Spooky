using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Tiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
    public class RottenChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Chunk");
            Tooltip.SetDefault("'It smells foul'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 1);
            Item.maxStack = 999;
        }
    }
}