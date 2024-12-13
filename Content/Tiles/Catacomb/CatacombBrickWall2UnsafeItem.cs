using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombBrickWall2UnsafeItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<CatacombBrickWall2>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}