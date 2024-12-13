using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave.Furniture;

namespace Spooky.Content.Items.SpiderCave.Misc
{
    public class SpiderKey : ModItem
    {
		public override void SetStaticDefaults()
		{
			ItemID.Sets.UsesCursedByPlanteraTooltip[Item.type] = true;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SpiderCaveChestItem>();
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