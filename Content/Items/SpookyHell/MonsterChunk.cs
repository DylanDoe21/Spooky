using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyHell
{
    public class MonsterChunk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Monster Chunk");
            Tooltip.SetDefault("The flesh of a creepy monster");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999; 
            Item.value = Item.buyPrice(silver: 1); 
            Item.rare = 1;
        }
    }
}