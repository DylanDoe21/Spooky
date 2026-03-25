using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class BigBoneStatueItem : ModItem
	{
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<BigBoneStatue>());
            Item.width = 16;
			Item.height = 16;
		}
	}
}