using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Water
{
    public class WaterFountainSpookFishronItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<WaterFountainSpookFishronXmasItem>();
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WaterFountainSpookFishron>());
            Item.width = 16;
			Item.height = 16;
            Item.value = Item.buyPrice(0, 4, 0, 0);
        }
    }
}