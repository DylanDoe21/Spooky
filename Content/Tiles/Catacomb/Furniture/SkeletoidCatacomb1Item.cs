using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class SkeletoidCatacomb1Item : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SkeletoidCatacomb1>());
            Item.width = 16;
			Item.height = 16;
		}
	}
}