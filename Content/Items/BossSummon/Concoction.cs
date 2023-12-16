using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.BossSummon
{
    public class Concoction : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }
		
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 38;
            Item.rare = ItemRarityID.White;
            Item.maxStack = 1;
        }
		
        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}