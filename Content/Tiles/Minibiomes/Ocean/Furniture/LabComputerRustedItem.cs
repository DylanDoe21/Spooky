using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
    public class LabComputerRustedItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabComputerRusted>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}