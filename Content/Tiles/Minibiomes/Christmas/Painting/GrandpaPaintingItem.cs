using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Painting
{
	public class GrandpaPaintingItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<GrandpaMLGPaintingItem>();
        }

		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<GrandpaPainting>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 25);
		}
	}
}