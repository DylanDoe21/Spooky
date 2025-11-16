using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class SkeletoidCatacomb2Item : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SkeletoidCatacomb2>());
            Item.width = 16;
			Item.height = 16;
		}
	}
}