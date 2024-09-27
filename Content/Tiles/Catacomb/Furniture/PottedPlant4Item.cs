using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
    public class PottedPlant4Item : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<PottedPlant4>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}