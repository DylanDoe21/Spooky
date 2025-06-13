using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
    public class LabScreenRustedItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabScreenRusted>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}