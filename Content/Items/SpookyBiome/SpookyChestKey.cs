using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome
{
    public class SpookyChestKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Wood Key");
            Tooltip.SetDefault("Opens locked chests in the underground spooky forest\n'It's covered in nasty pumpkin guts'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 5);
            Item.maxStack = 1;
        }
    }
}