using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Water
{
    public class WaterFountainRaveItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WaterFountainRave>());
            Item.width = 16;
			Item.height = 16;
            Item.value = Item.buyPrice(0, 4, 0, 0);
        }
    }
}