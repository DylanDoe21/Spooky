using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.Gourds
{
    public class GourdYellowGreenCarvedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdYellowGreenCarved>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class GourdYellowGreenCarvedLitItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdYellowGreenCarvedLit>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}