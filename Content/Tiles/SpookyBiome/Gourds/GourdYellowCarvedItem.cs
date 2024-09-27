using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.Gourds
{
    public class GourdYellowCarvedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdYellowCarved>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class GourdYellowCarvedLitItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdYellowCarvedLit>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}