using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Cemetery.Furniture;

namespace Spooky.Content.Items.Cemetery.Misc
{
    public class CemeteryKey : ModItem
    {
		public override void SetStaticDefaults()
		{
			ItemID.Sets.UsesCursedByPlanteraTooltip[Item.type] = true;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CemeteryBiomeChestItem>();
		}

		public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.Yellow;
        }
    }
}