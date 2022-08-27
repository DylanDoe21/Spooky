using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.BossSummon
{
    public class Fertalizer : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fertalizer");
            Tooltip.SetDefault("A creepy bag of fertalizer and bones\nCan be used at the flower pot at the bottom in the catacombs");
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 1;
        }
		
        public override bool CanUseItem(Player player)
        {
            return false;
        }
    }
}