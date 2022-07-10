using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.BossSummon
{
    public class CottonSwab : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cotton Swab");
            Tooltip.SetDefault("A particularly large cotton swab\nDo not shove it up giant noses, or the consequences will be dire");
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 1;
        }
		
        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}