using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
    public class ChristmasChainItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasChain>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}