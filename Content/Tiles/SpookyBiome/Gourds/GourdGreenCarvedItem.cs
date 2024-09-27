using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyBiome.Gourds
{
    public class GourdGreenCarvedItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdGreenCarved>());
            Item.width = 16;
			Item.height = 16;
        }
    }

    public class GourdGreenCarvedLitItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GourdGreenCarvedLit>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}