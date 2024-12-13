using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Content.Items.SpookyHell.Misc
{
    public class SpookyHellKey : ModItem
    {
		public override void SetStaticDefaults()
		{
			ItemID.Sets.UsesCursedByPlanteraTooltip[Item.type] = true;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpookyHellChestItem>();
		}

		public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 28;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Yellow;
        }
    }
}