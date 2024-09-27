using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.Gourds
{
    public class GourdOrangeCarvedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdOrangeCarved>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class GourdOrangeCarvedLitItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdOrangeCarvedLit>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}