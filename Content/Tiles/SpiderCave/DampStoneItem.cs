using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Tiles.SpiderCave
{
    public class DampStoneItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<DampStone>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}