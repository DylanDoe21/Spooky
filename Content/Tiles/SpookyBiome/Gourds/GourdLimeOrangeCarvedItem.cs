using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.Gourds
{
    public class GourdLimeOrangeCarvedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdLimeOrangeCarved>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class GourdLimeOrangeCarvedLitItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdLimeOrangeCarvedLit>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}