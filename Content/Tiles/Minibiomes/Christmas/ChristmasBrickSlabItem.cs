using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasBrickSlabItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasBrickSlab>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class ChristmasBrickSlabAltItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ChristmasBrickSlabAlt>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}