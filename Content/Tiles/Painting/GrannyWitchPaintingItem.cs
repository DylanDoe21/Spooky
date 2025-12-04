using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Painting
{
	public class GrannyWitchPaintingItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<GrannyWitchPainting>());
            Item.width = 16;
			Item.height = 16;
			Item.value = Item.buyPrice(silver: 25);
		}
	}
}